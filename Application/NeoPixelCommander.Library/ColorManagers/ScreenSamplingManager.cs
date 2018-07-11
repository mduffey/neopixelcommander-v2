using NeoPixelCommander.Library.Messages;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace NeoPixelCommander.Library.ColorManagers
{
    public class ScreenSamplingManager
    {
        private readonly PackageHandler _packageHandler;
        private readonly Wiring _wiring;
        private readonly int[] _horizontalSegments;
        private readonly int[] _verticalSegments;
        private Task _task;
        private bool _running;

        public ScreenSamplingManager(PackageHandler packageHandler)
        {
            _packageHandler = packageHandler;
            _wiring = new Wiring();
            // Adding one because we'll be giving the verticalSegments the corners, so the top segments need to skip the leftmost space.
            _horizontalSegments = CreateSegmentsArray(LEDs.Counts[Strip.Top] + 1, _wiring.Width);
            _verticalSegments = CreateSegmentsArray(LEDs.Counts[Strip.Left], _wiring.Height);
        }

        public int Interval { get; set; }
        public int Depth { get; set; }

        public void Start()
        {
            _running = true;
            _task = Task.Run(() => ProcessFrame());
        }

        public void Stop()
        {
            _running = false;
        }

        private void ProcessFrame()
        {
            while (_running)
            {
                var depth = Depth;
                try
                {
                    (var changes, var screenResource) = GetNextFrame();
                    try
                    {
                        if (changes)
                        {
                            ProcessScreen(screenResource, depth);
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
                Thread.Sleep(Interval);
            }
        }

        private void ProcessScreen(SharpDX.DXGI.Resource screenResource, int depth)
        {
            try
            {
                using (var screenTexture2D = screenResource.QueryInterface<Texture2D>())
                    _wiring.Device.ImmediateContext.CopyResource(screenTexture2D, _wiring.Texture);
                var mapSource = _wiring.Device.ImmediateContext.MapSubresource(_wiring.Texture, 0, MapMode.Read, MapFlags.None);
                
                var sidePtr = mapSource.DataPointer;
                var tasks = new Task<int[,]>[4] 
                {
                    ProcessHorizontal(mapSource.DataPointer, mapSource.RowPitch, depth),
                    ProcessHorizontal(IntPtr.Add(mapSource.DataPointer, mapSource.RowPitch * (_wiring.Height - depth)), mapSource.RowPitch, depth),
                    ProcessVertical(mapSource.DataPointer, mapSource.RowPitch, depth),
                    ProcessVertical(IntPtr.Add(mapSource.DataPointer, mapSource.RowPitch - depth * 4), mapSource.RowPitch, depth)
                };
                Task.WaitAll(tasks);
                
                var messages = new List<RangeMessage>();
                messages.AddRange(BuildHorizontalMessages(tasks[0].Result, Strip.Top));
                messages.AddRange(BuildHorizontalMessages(tasks[1].Result, Strip.Bottom));
                messages.AddRange(BuildVerticalMessages(tasks[2].Result, Strip.Left));
                messages.AddRange(BuildVerticalMessages(tasks[3].Result, Strip.Right));
                _packageHandler.SendRange(messages);
            }
            finally
            {
                _wiring.Device.ImmediateContext.UnmapSubresource(_wiring.Texture, 0);
            }
        }


        private (bool Changes, SharpDX.DXGI.Resource ScreenResource) GetNextFrame()
        {
            _wiring.OutputDuplication.AcquireNextFrame(200, out OutputDuplicateFrameInformation duplicateFrameInformation, out SharpDX.DXGI.Resource screenResource);
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
                segmentsArray[i] = Convert.ToInt32(segment * i);
            }
            segmentsArray[segmentsArray.Length - 1] = size;
            return segmentsArray;
        }

        private Task<int[,]> ProcessHorizontal(IntPtr ptr, int rowPitch, int depth)
        {
            return Task.Run(() =>
            {
                var topPtr = IntPtr.Add(ptr, _horizontalSegments[1] * 4); // Skipping the first segment, which is part of the corner that'll be lit up by the side strips.
                var array = new int[LEDs.Counts[Strip.Top], 4];
                for (var verticalI = 0; verticalI < depth; verticalI++)
                {
                    var arrayPos = 1;
                    var linePtr = topPtr;
                    for (var i = _horizontalSegments[1] * 4; i <= _horizontalSegments[_horizontalSegments.Length - 1] * 4; i += 4)
                    {
                        if (i > _horizontalSegments[arrayPos + 1] * 4)
                        {
                            arrayPos++;
                        }
                        linePtr = LoadArrayAndAdvancePointer(array, linePtr, arrayPos - 1);
                    }
                    topPtr = IntPtr.Add(topPtr, rowPitch);
                }
                return array;
            });
        }

        private Task<int[,]> ProcessVertical(IntPtr ptr, int rowPitch, int depth)
        {
            return Task.Run(() =>
            {
                var sidePtr = ptr;
                var sideArray = new int[LEDs.Counts[Strip.Left], 4];
                var sideArrayPos = 0;
                for (var i = 0; i < _wiring.Height; i++)
                {
                    if (sideArrayPos < _verticalSegments.Length && i > _verticalSegments[sideArrayPos + 1])
                    {
                        sideArrayPos++;
                    }
                    var linePtr = sidePtr;
                    for (var horizontalI = 0; horizontalI < depth; horizontalI += 4)
                    {
                        linePtr = LoadArrayAndAdvancePointer(sideArray, linePtr, sideArrayPos);
                    }
                    sidePtr = IntPtr.Add(sidePtr, rowPitch);
                }
                return sideArray;
            });
        }

        private IntPtr LoadArrayAndAdvancePointer(int[,] array, IntPtr ptr, int pos)
        {
            array[pos, 0] += Marshal.ReadByte(ptr, 2);
            array[pos, 1] += Marshal.ReadByte(ptr, 1);
            array[pos, 2] += Marshal.ReadByte(ptr);
            array[pos, 3]++;
            return IntPtr.Add(ptr, 4);
        }

        private IEnumerable<RangeMessage> BuildHorizontalMessages(int[,] array, Strip strip)
        {
            for(int i = 0; i < LEDs.Counts[strip]; i++)
            {
                yield return new RangeMessage(strip, (byte)i, new Color
                {
                    R = (byte)(array[i, 0] / array[i, 3]),
                    G = (byte)(array[i, 1] / array[i, 3]),
                    B = (byte)(array[i, 2] / array[i, 3]),
                });
            }
        }

        private IEnumerable<RangeMessage> BuildVerticalMessages(int[,] array, Strip strip)
        {
            for (int i = 0; i < LEDs.Counts[strip]; i++)
            {
                yield return new RangeMessage(strip, (byte)i, new Color
                {
                    R = (byte)(array[LEDs.Counts[strip] - i - 1, 0] / array[i, 3]),
                    G = (byte)(array[LEDs.Counts[strip] - i - 1, 1] / array[i, 3]),
                    B = (byte)(array[LEDs.Counts[strip] - i - 1, 2] / array[i, 3]),
                });
            }
        }
    }
}
