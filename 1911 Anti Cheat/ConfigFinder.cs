using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace _1911_Anti_Cheat
{
    public static class ConfigFinder
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out uint processId);

        public static string[] GetConfigFiles(IntPtr hwnd)
        {
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            Process proc = Process.GetProcessById((int)pid);
            string filename = proc.MainModule.FileName.ToString();

            string configDirectory = Path.Combine(Directory.GetParent(filename).ToString(), "dod");
            if (!Directory.Exists(configDirectory))
                return new string[] { };

            return System.IO.Directory.GetFiles(configDirectory, "*.cfg");
        }
    }
}
