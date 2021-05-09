using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloatToolGUI
{
    static class Logger
    {
        static string LaunchTime = DateTime.Now.ToString().Replace(' ', '_').Replace(':', '-');

        static public void Log(object data)
        {
            using (StreamWriter w = File.AppendText("debug.log"))
            {
                w.WriteLine(data);
            }
        }

        static public void SaveCrashReport()
        {
            Directory.CreateDirectory("crashreports");
            File.Copy("debug.log", @$"crashreports{Path.DirectorySeparatorChar}{LaunchTime}.log", true);
        }

        static public void ClearLogs()
        {
            File.WriteAllText("debug.log", string.Empty);
        }
    }
}
