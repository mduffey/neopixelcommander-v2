using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using HidLibrary;

namespace NeoPixelCommander.Library
{
    public class Communicator
    {
        private static Communicator _communicator = new Communicator();
        public static Communicator Instance => _communicator;

        private HidDevice _teensy;

        public bool Active { get; private set; }

        private Timer _refreshTimer;
        protected Communicator()
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
                    if (_teensy.FastWrite(message))
                        successes++;
                }
                return (messages.Length, successes);
            }
            return (0, 0);
        }
    }
}
