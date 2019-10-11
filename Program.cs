using IniParser;
using System.IO;
using IniParser.Model;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace IniFileParser.Example
{
    public class MainProgram
    {
        private static Process Process;
        private static string _path;

        public static void Main()
        {

            Console.WriteLine($"{DateTime.Now} | ResurrectionRP Server Deamon by Djoe_");

            FileIniDataParser fileIniData = new FileIniDataParser();

            if (!File.Exists("Configuration.ini"))
                return;

            Process.GetProcesses().Where(x => x.ProcessName.ToLower().Equals("server")).ToList().ForEach(x => x.Kill());

            IniData parsedData = fileIniData.ReadFile("Configuration.ini");

            string[] reboot = parsedData["GeneralConfiguration"]["reboot"].Split(',');
            _path = parsedData["GeneralConfiguration"]["path"].ToString();
            
            ProcessStartInfo startInfo = new ProcessStartInfo("server.exe");
            startInfo.FileName = "server.exe";
            startInfo.WorkingDirectory = _path;
            startInfo.WindowStyle = ProcessWindowStyle.Normal;

            try
            {
                Process = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            while (true)
            {
                if (Process != null)
                {
                    foreach (var rebootHours in reboot)
                    {
                        if (DateTime.Now.Hour == Convert.ToInt32(rebootHours) && DateTime.Now.Minute == 0)
                        {
                            Console.WriteLine($"{DateTime.Now} | Reboot de {rebootHours} en cours");
                            Process.Kill();
                            SaveLog();
                            Process.Start();
                            Thread.Sleep(1000);
                            return;
                        }
                    }
                    
                    if (Process.HasExited)
                    {
                        Console.WriteLine($"{DateTime.Now} | Crash détecté!");
                        SaveLog();
                        Process.Start();
                    }

                    Thread.Sleep(1000);
                }
            }
        }

        private static void SaveLog()
        {
            string logFile = $@"{_path}\server.log";

            while (IsFileLocked(logFile))
                Thread.Sleep(100);

            Directory.CreateDirectory($@"{_path}\logs");
            File.Move(logFile, $@"{_path}\logs\{DateTime.Now.ToString("yyyyMMdd_HHmmss")}_server.log");
        }

        private static bool IsFileLocked(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }
    }
}