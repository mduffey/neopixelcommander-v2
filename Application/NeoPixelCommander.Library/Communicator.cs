using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;
using NeoPixelCommander.Library.Messages;

namespace NeoPixelCommander.Library
{
    public class Communicator
    {
        [DllImport("rawhid.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "#2")]
        internal static extern int rawhid_open(int max, int vid, int pid, int usage_page, int usage);

        [DllImport("rawhid.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "#3")]
        internal static extern int rawhid_recv(int num, IntPtr buffer, int length, int timeout);

        [DllImport("rawhid.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "#4")]
        internal static extern int rawhid_send(int num, IntPtr buffer, int length, int timeout);

        [DllImport("rawhid.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "#1")]
        internal static extern void rawhid_close(int num);

        private readonly Timer _refreshTimer;
        private readonly Timer _statusTimer;
        private readonly IUpdateStatus _updateStatus;
        private int _availableDevices;
        

        public Communicator(IUpdateStatus updateStatus)
        {
            _updateStatus = updateStatus;
            _availableDevices = rawhid_open(5, 0x16C0, 0x0486, 0xFFAB, 0x0200);
            _refreshTimer = new Timer(500);
            _refreshTimer.Elapsed += (sender, e) =>
            {
                _availableDevices = rawhid_open(1, 0x16C0, 0x0486, 0xFFAB, 0x0200);
            };
            _statusTimer = new Timer(2000);
            _statusTimer.Elapsed += (sender, e) => { GetStatus(); };
            _refreshTimer.AutoReset = false;
            _statusTimer.AutoReset = false;
            if (_availableDevices < 0)
            {
                _refreshTimer.Start();
            }

            _statusTimer.Start();
        }

        public bool SendMessage(byte[] message)
        {
            if (_availableDevices <= 0) return false;
            var sendPtr = Marshal.AllocHGlobal(message.Length);
            Marshal.Copy(message, 0, sendPtr, message.Length);
            try
            {
                var result = rawhid_send(0, sendPtr, message.Length, 100);
                ParseResultAndExecute(result);
                return result == message.Length;
            }
            finally
            {
                Marshal.FreeHGlobal(sendPtr);
            }
        }

        public void GetStatus()
        {
            if (_availableDevices <= 0)
            {
                if (!_refreshTimer.Enabled)
                {
                    _refreshTimer.Start();
                }

                return;
            }
            var sendBuffer = new byte[64];
            var sendPtr = Marshal.AllocHGlobal(sendBuffer.Length);
            var receiveBuffer = new byte[64];
            var receivePtr = Marshal.AllocHGlobal(receiveBuffer.Length);
            try
            {
                for (var i = 0; i < sendBuffer.Length; i++)
                {
                    sendBuffer[i] = (byte) MessageType.Status;
                }
                sendBuffer[1] = (byte) MessageType.Status;
                Marshal.Copy(sendPtr, sendBuffer, 0, sendBuffer.Length);
                var result = rawhid_send(0, sendPtr, sendBuffer.Length, 100);
                ParseResultAndExecute(result, () =>
                {
                    result = rawhid_recv(0, receivePtr, 64, 150);
                    Marshal.Copy(receivePtr, receiveBuffer, 0, receiveBuffer.Length);
                    ParseResultAndExecute(result, () =>
                    {
                        if (receiveBuffer[0] == (byte) MessageType.Status)
                        {
                            if (Enum.IsDefined(typeof(Availability), (int) receiveBuffer[1]))
                            {
                                _updateStatus.Availability = (Availability) receiveBuffer[1];
                            }

                            if (Enum.IsDefined(typeof(LogLevel), (int) receiveBuffer[2]))
                            {
                                _updateStatus.LogLevel = (LogLevel) receiveBuffer[2];
                            }
                        }
                    });
                });
            }
            catch
            {
                int test = 9;
            }
            finally
            {
                Marshal.FreeHGlobal(sendPtr);
                Marshal.FreeHGlobal(receivePtr);
                _statusTimer.Start();
            }
        }

        private void ParseResultAndExecute(int result, Action successfulAction = null)
        {
            if (result < 0)
            {
                Reset();
            }

            if (result > 0)
            {
                successfulAction?.Invoke();
            }
        }

        private void Reset()
        {
            _updateStatus.Availability = Availability.Disconnected;
            rawhid_close(0);
            _availableDevices = 0;
            _refreshTimer.Start();
        }
    }
}
