using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIHandler
{
    public class APILogger
    {
        static bool copyToConsole = APIRunTimeParameters.CopyLogToConsole;
        public static void Write(string message, string description = "", bool error = false, string customType = "")
        {
            if (!APIRunTimeParameters.UseLogger) { return; }

            string path = APIRunTimeParameters.LogPath;
            StringBuilder information = new StringBuilder();

            information.Append($"{DateTime.Now} | ");
            if (customType == "")
            {
                if (error)
                {
                    information.Append("ERROR | ");
                }
                else
                {
                    information.Append("INFO  | ");
                }
            }
            else
            {
                information.Append($"{customType} | ");
            }
            information.Append(message);
            if (description != "")
            {
                information.Append($" : {description}");
            }
            information.Append("\n");

            File.AppendAllText(path, information.ToString());
            if (copyToConsole) Console.WriteLine(information.ToString());
        }
    }
}
