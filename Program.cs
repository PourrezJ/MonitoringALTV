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
        private static DateTime _nextUpdate;

        public static void Main()
        {

            Console.WriteLine($"{DateTime.Now} | ResurrectionRP Server Deamon by Djoe_");

            FileIniDataParser fileIniData = new FileIniDataParser();

            if (!File.Exists("Configuration.ini"))
                return;

            Process.GetProcesses().Where(x => x.ProcessName.ToLower().Equals("server")).ToList().ForEach(x => x.Kill());

            IniData parsedData = fileIniData.ReadFile("Configuration.ini");


            string[] reboot = parsedData["GeneralConfiguration"]["reboot"].Split(',');
            string path = parsedData["GeneralConfiguration"]["path"].ToString();
            
            ProcessStartInfo startInfo = new ProcessStartInfo("server.exe");
            startInfo.FileName = "server.exe";
            startInfo.WorkingDirectory = path;
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
                if (Process != null && DateTime.Now > _nextUpdate)
                {
                    foreach (var rebootHours in reboot)
                    {
                        if (DateTime.Now.Hour == Convert.ToInt32(rebootHours) && DateTime.Now.Minute == 0)
                        {
                            Console.WriteLine($"{DateTime.Now} | Reboot de {rebootHours} en cours");
                            Process.Kill();
                            Process.Start();
                            _nextUpdate = DateTime.Now.AddMinutes(1);
                            return;
                        }
                    }
                    
                    if (Process.HasExited)
                    {
                        Console.WriteLine($"{DateTime.Now} | Crash détecter!");
                        Process.Start();
                    }

                    _nextUpdate = DateTime.Now.AddMinutes(1);
                }
            }
        }

    }
}