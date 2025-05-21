using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUIClient
{
    public static class Constants
    {
        // URL of REST service

        public static string LocalhostUrl = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
        public static string Scheme = "http"; // or https
        public static string Port = "5000";
        public static string RestUrl = $"{Scheme}://{LocalhostUrl}:{Port}/{{0}}"; 

    }
}