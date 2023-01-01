using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;
using NeoPixelCommander.Library.Messages;

namespace NeoPixelCommander.Library
{
    public class Communicator
    {
        private readonly IUpdateStatus _updateStatus;
        private SerialPort _serialPort = null;
        private readonly Timer _resetConnectionTimer;
        private ConcurrentQueue<byte[]> _messageQueue = new ConcurrentQueue<byte[]>();
        private readonly Task _pumpTask;
        private readonly CancellationTokenSource _pumpTaskCancellation = new CancellationTokenSource();
        public Communicator(IUpdateStatus updateStatus)
        {
            _updateStatus = updateStatus;
            _resetConnectionTimer = new Timer(500) { AutoReset = false };
            _resetConnectionTimer.Elapsed += (sender, args) =>
            {
                if (_serialPort?.IsOpen == true)
                {
                    return;
                }
                _updateStatus.Availability = Availability.Disconnected;
                CheckConnection();
            };
            CheckConnection();
            _pumpTask = Task.Run(Execute);
        }

        ~Communicator()
        {
            _pumpTaskCancellation.Cancel();
            Task.WaitAll(_pumpTask);
        }

        private void CheckConnection()
        {
            foreach (var portName in SerialPort.GetPortNames())
            {
                try
                {
                    var port = new SerialPort(portName, 1200000) {ReadTimeout = 5000, WriteTimeout = 5000};
                    port.Open();
                    port.Write(new byte[] {(int)MessageType.Check, 0, 0, 0, 0}, 0, 5);
                    var response = port.ReadLine();

                    if (response == "YES")
                    {
                        _serialPort = port;
                        _updateStatus.Availability = Availability.Online;
                        break;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"Exception: {e}");
                }

            }
        }

        public void Reset()
        {
            _messageQueue = new ConcurrentQueue<byte[]>();
        }

        public void SendMessage(byte[] message)
        {
            _messageQueue.Enqueue(message);
        }

        public void GetStatus()
        {
            _updateStatus.Availability = _serialPort?.IsOpen == true ? Availability.Online : Availability.Disconnected;
        }


        private void Execute()
        {
            while (!_pumpTaskCancellation.IsCancellationRequested)
            {
                if (!_serialPort?.IsOpen == true)
                {
                    _updateStatus.Availability = Availability.Disconnected;
                    _resetConnectionTimer.Start();
                }
                try
                {
                    if (_serialPort?.IsOpen == true && _serialPort.BytesToWrite == 0)
                    {
                        var success = _messageQueue.TryDequeue(out var message);
                        

                        if (success)
                        {
                            Trace.WriteLine($"Sending message of length {message.Length:N0}");
                            _serialPort.Write(message, 0, message.Length);
                        }
                        else
                        {
                            //Trace.WriteLine("Nothing to send");
                        }
                    }
                    else
                    {
                        Trace.WriteLine("Serial port processing existing messages.");
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"Error: {e}");
                }
            }
        }
    }
}
