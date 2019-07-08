using System;
using System.IO;

namespace Portalia.WindowsService
{
    public static class LogHelpers
    {
        public static void WriteErrorLog(Exception ex)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(
                    $"ERROR: {DateTime.UtcNow.ToString()}: {ex.Source.ToString().Trim()}; {ex.Message.ToString().Trim()}");
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }
        }

        public static void WriteErrorLog(string message)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(
                    $"ERROR: {DateTime.UtcNow.ToString()}: {message}");
                sw.Flush();
                sw.Close();
            }
            catch
            {
                // ignored
            }
        }

        public static void WriteInfoLog(string message)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt", true);
                sw.WriteLine(
                    $"INFO: {DateTime.UtcNow.ToString()}: {message}");
                sw.Flush();
                sw.Close();
            }
            catch
            {
                // ignored
            }
        }
    }
}
