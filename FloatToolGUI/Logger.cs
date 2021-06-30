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
        static public bool IsFileReady(string filename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        static public void Log(object data)
        {
            try
            {
                using (StreamWriter w = File.AppendText("debug.log"))
                {
                    w.WriteLine(data);
                }
            }
            catch (IOException ioex)
            {
                Console.WriteLine(ioex.Message);
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
