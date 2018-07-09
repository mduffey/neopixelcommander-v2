using NeoPixelCommander.Library.Messages;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Media;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace NeoPixelCommander.Library.ColorManagers
{
    public class ScreenSamplingManager
    {
        private readonly Timer _timer;
        private readonly PackageHandler _packageHandler;
        
        private readonly Wiring _wiring;
        private bool _stopping;
        private readonly int[] _horizontalSegments;
        private readonly int[] _verticalSegments;


        public ScreenSamplingManager(PackageHandler packageHandler)
        {
            _packageHandler = packageHandler;
            _timer = new Timer();
            _timer.Elapsed += (sender, e) => ProcessFrame();
            _timer.AutoReset = false;
            _wiring = new Wiring();
            // Adding two because we'll be giving the verticalSegments the corners, so the top segments need to avoid that space.
            _horizontalSegments = CreateSegmentsArray(LEDs.Counts[Strip.Top] + 1, _wiring.Width);
            _verticalSegments = CreateSegmentsArray(LEDs.Counts[Strip.Left], _wiring.Height);
        }

        public void Start(int interval)
        {
            _stopping = false;
            _timer.Interval = interval;
            _timer.Start();
        }

        public void Stop()
        {
            _stopping = true;
        }

        private void ProcessFrame()
        {
            try
            {
                (var changes, var screenResource) = GetNextFrame();
                try
                {
                    if (changes)
                    {
                        ProcessScreen(screenResource);
                    }
                }
                finally
                {
                    screenResource.Dispose();
                    _wiring.OutputDuplication.ReleaseFrame();
                }

            }
            // On exception, if it's an expectable type (the output needs to be reacquired, or we ran into the timeout), 
            // then we just give up on this pass and try again later.
            catch (SharpDXException e)
            {
                if (e.ResultCode.Code != SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
                {
                    if (e.ResultCode.Code == SharpDX.DXGI.ResultCode.AccessLost.Result.Code)
                    {
                        _wiring.ReacquireDuplication();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            finally
            {
                if (!_stopping)
                {
                    _timer.Start();
                }
            }
        }

        private void ProcessScreen(SharpDX.DXGI.Resource screenResource)
        {
            try
            {
                using (var screenTexture2D = screenResource.QueryInterface<Texture2D>())
                    _wiring.Device.ImmediateContext.CopyResource(screenTexture2D, _wiring.Texture);
                var mapSource = _wiring.Device.ImmediateContext.MapSubresource(_wiring.Texture, 0, MapMode.Read, MapFlags.None);
                var bytes = new byte[mapSource.RowPitch];
                Marshal.Copy(mapSource.DataPointer, bytes, 0, mapSource.RowPitch);

                var topPtr = mapSource.DataPointer;
                var sidePtr = mapSource.DataPointer;
                topPtr = IntPtr.Add(topPtr, _horizontalSegments[1]);
                var topArray = new int[LEDs.Counts[Strip.Top], 3]; // Adding two to rip out the corners, which are lit up by the side strips.
                var sideArray = new int[LEDs.Counts[Strip.Left], 3];
                for (var verticalI = 0; verticalI < 100; verticalI++)
                {
                    var arrayPos = 1;
                    var linePtr = topPtr;
                    for (var i = _horizontalSegments[1]; i <= _horizontalSegments[_horizontalSegments.Length - 1]; i += 4) // 1 for the declaration and -3 for the check to skip the corners
                    {
                        
                        if (i > _horizontalSegments[arrayPos + 1])
                        {
                            arrayPos++;
                        }
                        topArray[arrayPos - 1, 0] += Marshal.ReadByte(linePtr, 2);
                        topArray[arrayPos - 1, 1] += Marshal.ReadByte(linePtr, 1);
                        topArray[arrayPos - 1, 2] += Marshal.ReadByte(linePtr);
                        linePtr = IntPtr.Add(linePtr, 4);
                    }
                    topPtr = IntPtr.Add(topPtr, mapSource.RowPitch);
                }
                for (var i = 0; i < _wiring.Height; i++)
                {
                    var arrayPos = 0;
                    if (arrayPos < _verticalSegments.Length && i > _verticalSegments[arrayPos + 1])
                    {
                        arrayPos++;
                    }
                    var linePtr = sidePtr;
                    for (var horizontalI = 0; horizontalI < 100; horizontalI += 4)
                    {
                        sideArray[arrayPos, 0] += Marshal.ReadByte(sidePtr, 2);
                        sideArray[arrayPos, 1] += Marshal.ReadByte(sidePtr, 1);
                        sideArray[arrayPos, 2] += Marshal.ReadByte(sidePtr);
                        linePtr = IntPtr.Add(linePtr, 4);
                    }
                    sidePtr = IntPtr.Add(sidePtr, mapSource.RowPitch);
                }
                var messages = new List<RangeMessage>();
                for (int i = 0; i < LEDs.Counts[Strip.Top]; i++)
                {
                    messages.Add(new RangeMessage(Strip.Top, (byte)i, new Color
                    {
                        R = (byte)(topArray[i, 0] / ((_horizontalSegments[i + 2] - _horizontalSegments[i + 1]) / 4) * 100),
                        G = (byte)(topArray[i, 1] / ((_horizontalSegments[i + 2] - _horizontalSegments[i + 1]) / 4) * 100),
                        B = (byte)(topArray[i, 2] / ((_horizontalSegments[i + 2] - _horizontalSegments[i + 1]) / 4) * 100),
                    }));
                }
                for (int i = 0; i < LEDs.Counts[Strip.Left]; i++)
                {
                    messages.Add(new RangeMessage(Strip.Left, (byte)i, new Color
                    {
                        R = (byte)(sideArray[i, 0] / ((_verticalSegments[i + 1] - _verticalSegments[i]) / 4) * 100),
                        G = (byte)(sideArray[i, 1] / ((_verticalSegments[i + 1] - _verticalSegments[i]) / 4) * 100),
                        B = (byte)(sideArray[i, 2] / ((_verticalSegments[i + 1] - _verticalSegments[i]) / 4) * 100),
                    }));
                }
                _packageHandler.SendRange(messages);
            }
            finally
            {
                _wiring.Device.ImmediateContext.UnmapSubresource(_wiring.Texture, 0);
            }
        }
        
        
        private (bool Changes, SharpDX.DXGI.Resource ScreenResource) GetNextFrame()
        {
            _wiring.OutputDuplication.AcquireNextFrame(Convert.ToInt32(_timer.Interval), out OutputDuplicateFrameInformation duplicateFrameInformation, out SharpDX.DXGI.Resource screenResource);
            return (duplicateFrameInformation.AccumulatedFrames > 0, screenResource);
        }

        /// <summary>
        /// Holds the extra-weird core objects for later disposal.
        /// </summary>
        private class Wiring
        {
            private readonly Factory1 _factory;
            private readonly Adapter1 _adapter;
            private readonly Output _output;
            private readonly Output1 _output1;

            public Wiring()
            {
                _factory = new Factory1();
                // Only works as long as you're only using one graphics card. So, TODO to change this someday.
                _adapter = _factory.GetAdapter1(0);
                // Create device from Adapter
                Device = new Device(_adapter);
                // Works only with a single monitor. I'm only using one monitor, so I'm ignoring this. But, TODO.
                _output = _adapter.GetOutput(0);
                _output1 = _output.QueryInterface<Output1>();
                Height = _output.Description.DesktopBounds.Bottom - _output.Description.DesktopBounds.Top;
                Width = _output.Description.DesktopBounds.Right - _output.Description.DesktopBounds.Left;
                Texture = new Texture2D(Device, new Texture2DDescription
                {
                    CpuAccessFlags = CpuAccessFlags.Read,
                    BindFlags = BindFlags.None,
                    Format = Format.B8G8R8A8_UNorm,
                    Width = Width,
                    Height = Height,
                    OptionFlags = ResourceOptionFlags.None,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = { Count = 1, Quality = 0 },
                    Usage = ResourceUsage.Staging
                });
                OutputDuplication = _output1.DuplicateOutput(Device);
            }

            ~Wiring()
            {
                Texture.Dispose();
                OutputDuplication.Dispose();
                _output1.Dispose();
                _output.Dispose();
                Device.Dispose();
                _adapter.Dispose();
                _factory.Dispose();
            }

            public Device Device { get; }
            public OutputDuplication OutputDuplication { get; private set; }
            public int Height { get; }
            public int Width { get; }
            public Texture2D Texture { get; }

            public void ReacquireDuplication()
            {
                OutputDuplication = _output1.DuplicateOutput(Device);
            }
        }

        private int[] CreateSegmentsArray(int segments, int size)
        {

            var segment = size / (double)segments;
            var segmentsArray = new int[segments + 1];
            for (int i = 0; i < segmentsArray.Length; i++)
            {
                segmentsArray[i] = Convert.ToInt32(segment * i) * 4;
            }
            segmentsArray[segmentsArray.Length - 1] = size * 4;
            return segmentsArray;
        }
    }
}
