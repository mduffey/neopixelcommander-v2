using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using HidLibrary;
using NeoPixelCommander.Library.Extensions;

namespace NeoPixelCommander.Library
{
    public class Communicator
    {
        private HidDevice _teensy;

        public EventHandler ActiveChangedEventHandler;

        public Communicator()
        {
            _teensy = GetDevice();
            _refreshTimer = new Timer(200);
            _refreshTimer.Elapsed += (sender, e) =>
            {
                _teensy = GetDevice();
                if (Active)
                    _refreshTimer.Stop();
            };
            if (!Active)
            {
                StartReconnect();
            }
        }

        private bool _active;
        public bool Active
        {
            get => _active;
            private set
            {
                if (_active != value)
                {
                    _active = value;
                    ActiveChangedEventHandler?.Invoke(this, new EventArgs());
                }
            }
        }

        public bool SendMessage(byte[] message)
        {
            return Active
                // Use the standard one when there are any issues to debug. Otherwise FastWrite uses about 1/20th the processing power.
                //&& _teensy.Write(message);
                && _teensy.FastWrite(message);
        }

        private Timer _refreshTimer;
        
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
                        Active = true;
                        return device;
                    }
                }
                i++;
            }
            return null;
        }

        private void StartReconnect()
        {
            Active = false;
            _refreshTimer.Start();
        }
    }
}
