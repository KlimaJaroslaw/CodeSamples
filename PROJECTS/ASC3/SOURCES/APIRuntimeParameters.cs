using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIHandler
{
    internal static class APIRunTimeParameters
    {
        public static bool UseLogger { get; set; } 
        public static string LogPath { get; set; }
        public static bool CopyLogToConsole { get; set; }

        public static void SetParameters()
        {
            APIRunTimeParameters.UseLogger = false;
            APIRunTimeParameters.LogPath = Path.Combine(Environment.CurrentDirectory, @"LOG\", "APILog.txt");
            APIRunTimeParameters.CopyLogToConsole = true;
        }
    }
}
