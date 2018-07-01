using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using HidLibrary;

namespace ColorControl
{
    public class Communicator
    {
        private HidDevice _teensy;

        public bool Active => _teensy != null && _teensy.IsConnected && _teensy.IsOpen;

        private Timer _refreshTimer;
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
                        return device;
                    }
                }
                i++;
            }
            return null;
        }

        private void StartReconnect()
        {
            _refreshTimer.Start();
        }

        public bool SendMessage(byte[] message)
        {
            return Active && _teensy.Write(message, 20);
        }

        public (int Attempts, int Successes) SendMessages(params byte[][] messages)
        {
            if (Active)
            {
                int successes = 0;
                foreach (var message in messages)
                {
                    if (_teensy.Write(message, 20))
                        successes++;
                }
                return (messages.Length, successes);
            }
            return (0, 0);
        }
    }
}
