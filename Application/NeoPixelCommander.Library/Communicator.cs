using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;
using HidLibrary;
using NeoPixelCommander.Library.Extensions;
using NeoPixelCommander.Library.Messages;

namespace NeoPixelCommander.Library
{
    public class Communicator
    {
        private readonly Timer _refreshTimer;
        private readonly Timer _statusTimer;
        private readonly IUpdateStatus _updateStatus;
        private HidDevice _teensy;


        public Communicator(IUpdateStatus updateStatus)
        {
            _updateStatus = updateStatus;
            _teensy = GetDevice();
            _refreshTimer = new Timer(200);
            _refreshTimer.Elapsed += (sender, e) =>
            {
                if (_teensy == null || _updateStatus.Availability == Availability.Disabled || _updateStatus.Availability == Availability.Disconnected)
                {
                    _teensy = GetDevice();
                }
            };
            _statusTimer = new Timer(2000);
            _statusTimer.Elapsed += (sender, e) => { GetStatus(); };
            _refreshTimer.AutoReset = true;
            _statusTimer.AutoReset = true;
            _refreshTimer.Start();
            _statusTimer.Start();
        }

        public bool SendMessage(byte[] message)
        {
            return _updateStatus.Availability == Availability.Online
                   // Use the standard one when there are any issues to debug. Otherwise FastWrite uses about 1/20th the processing power.
                   //&& _teensy.Write(message);
                   && _teensy.FastWrite(message);
        }

        public void GetStatus()
        {
            if (_teensy != null)
            {
                // Even if the device has disabled LED updates, we want to be able to request a status check.
                if (_updateStatus.Availability != Availability.Disconnected)
                {
                    var message = new byte[64];
                    message[0] = 0;
                    message[1] = (byte) MessageType.Status;
                    _teensy.FastWrite(message);
                }

                var response = _teensy.FastRead();
                if (response.Status == HidDeviceData.ReadStatus.Success && response.Output[1] == (byte) MessageType.Status)
                {
                    if (Enum.IsDefined(typeof(Availability), (int) response.Output[2]))
                    {
                        _updateStatus.Availability = (Availability) response.Output[2];
                    }

                    if (Enum.IsDefined(typeof(LogLevel), (int) response.Output[3]))
                    {
                        _updateStatus.LogLevel = (LogLevel) response.Output[3];
                    }
                }
                else
                {
                    _updateStatus.Availability = Availability.Disconnected;
                }
            }
            else
            {
                _updateStatus.Availability = Availability.Disconnected;
            }
        }

        private HidDevice GetDevice()
        {
            var devices = HidDevices.Enumerate().ToList();
            var i = 0;
            while (i < devices.Count)
            {
                var device = devices[i];
                byte[] bytes;
                device.ReadProduct(out bytes);
                if (!bytes.All(b => b == 0))
                {
                    var name = Encoding.Unicode.GetString(bytes).Trim(new[] { '\0' });
                    if (name.ToLower().Contains("teensy"))
                    {
                        device.OpenDevice();
                        device.MonitorDeviceEvents = true;
                        _updateStatus.Availability = Availability.Disabled;
                        return device;
                    }
                }
                i++;
            }
            return null;
        }
    }
}
