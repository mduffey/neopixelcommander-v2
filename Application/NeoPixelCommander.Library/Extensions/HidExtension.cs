using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HidLibrary.HidDeviceData;

namespace NeoPixelCommander.Library.Extensions
{
    public static class HidExtension
    {
        // Thanks! https://github.com/mikeobrien/HidLibrary/issues/11#issuecomment-255485213
        [DllImport("kernel32.dll", SetLastError = true)]
        static internal extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, [In] ref System.Threading.NativeOverlapped lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        static internal extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        static internal extern bool ReadFile(IntPtr hFile, IntPtr lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, [In] ref System.Threading.NativeOverlapped lpOverlapped);


        public static bool FastWrite(this HidLibrary.HidDevice device, byte[] buffer)
        {
            try
            {
                uint bytesWritten;
                var outputBuffer = new byte[device.Capabilities.InputReportByteLength];
                Array.Copy(buffer, outputBuffer, Math.Min(buffer.Length, device.Capabilities.InputReportByteLength));
                var overlapped = new NativeOverlapped();
                if (WriteFile(device.Handle, outputBuffer, (uint)outputBuffer.Length, out bytesWritten, ref overlapped))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        public static (ReadStatus Status, byte[] Output) FastRead(this HidLibrary.HidDevice device)
        {
            try
            {
                var data = new byte[device.Capabilities.OutputReportByteLength];
                uint bytesRead;
                if (ReadFile(device.Handle, data, (uint)data.Length, out bytesRead, IntPtr.Zero))
                {
                    return (ReadStatus.Success, data);
                }
                else
                {
                    return (ReadStatus.NoDataRead, new byte[0]);
                }
            }
            catch (Exception)
            {
                return (ReadStatus.ReadError, new byte[0]);
            }
        }
    }
}
