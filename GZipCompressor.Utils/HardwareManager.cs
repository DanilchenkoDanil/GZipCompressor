using System;
using System.Diagnostics;
using System.Management;

namespace GZipCompressor.Utils
{
    public class HardwareManager
    {
        public static uint GetThreadCount() {
            string result = string.Empty;
            string result2 = string.Empty;
            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
            mbsList = mbs.Get();
            foreach (ManagementObject mo in mbsList) {
                result = mo["ThreadCount"].ToString();
            }

            return uint.Parse(result);
        }

        public static long GetAvalibleMemorySize() {
            PerformanceCounter pc = new PerformanceCounter("Memory", "Available Bytes");
            return Convert.ToInt64(pc.NextValue());
        }
    }
}
