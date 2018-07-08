using System;
using System.Collections.Generic;
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
        private readonly ReaderWriterLockSlim _lock;
        private readonly Timer _refreshTimer;
        private readonly Timer _statusTimer;
        private readonly IUpdateStatus _updateStatus;
        private HidDevice _teensy;
        
        
        public Communicator(IUpdateStatus updateStatus)
        {
            _lock = new ReaderWriterLockSlim();
            _updateStatus = updateStatus;
            _teensy = GetDevice();
            _refreshTimer = new Timer(200);
            _refreshTimer.Elapsed += (sender, e) =>
            {
                _teensy = GetDevice();
                if (_updateStatus.Availability != Availability.Disconnected)
                {
                    _refreshTimer.Stop();
                    GetStatus();
                    _statusTimer.Start();
                }
            };
            _statusTimer = new Timer(2000);
            _statusTimer.Elapsed += (sender, e) =>
            {
                GetStatus();
            };
            if (_updateStatus.Availability == Availability.Disconnected)
            {
                StartReconnect();
            }
            else
            {
                GetStatus();
                _statusTimer.Start();
            }
        }

        public bool SendMessage(byte[] message)
        {
            return _updateStatus.Availability == Availability.Online
                // Use the standard one when there are any issues to debug. Otherwise FastWrite uses about 1/20th the processing power.
                //&& _teensy.Write(message);
                && Send(message);
        }

        private bool Send(byte[] message)
        {
            _lock.EnterReadLock();
            try
            {
                return _teensy.FastWrite(message);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void GetStatus()
        {
            // Even if the device has disabled LED updates, we want to be able to request a status check.
            if (_updateStatus.Availability != Availability.Disconnected)
            {
                var message = new byte[64];
                message[0] = 0;
                message[1] = (byte)MessageType.Status;
                Send(message);
            }
            _lock.EnterWriteLock();
            try
            {
                var response = _teensy.FastRead();
                if (response.Status == HidDeviceData.ReadStatus.Success && response.Output[1] == (byte)MessageType.Status)
                {
                    if (Enum.IsDefined(typeof(Availability), (int)response.Output[2]))
                    {
                        _updateStatus.Availability = (Availability)response.Output[2];
                    }
                    if (Enum.IsDefined(typeof(LogLevel), (int)response.Output[3]))
                    {
                        _updateStatus.LogLevel = (LogLevel)response.Output[3];
                    }
                }
                else
                {
                    _updateStatus.Availability = Availability.Unknown;
                }
            }
            finally
            {
                _lock.ExitWriteLock();
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
                        device.Removed += StartReconnect;
                        _updateStatus.Availability = Availability.Disabled;
                        return device;
                    }
                }
                i++;
            }
            return null;
        }

        private void StartReconnect()
        {
            _statusTimer.Stop();
            _refreshTimer.Start();
        }
    }
}
