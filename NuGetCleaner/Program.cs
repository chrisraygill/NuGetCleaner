using System;
using System.ComponentModel;
using System.IO;

namespace NuGetCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args[0] == "clean")
                {
                    int setting = CheckDisableLastAccess();

                    if (setting != 2)
                    {
                        Console.Write("Last access updates are not currently enabled. Please follow instructions at ... to enable them.");
                        System.Environment.Exit(0);
                    }

                    Console.Write("Enter a dir path: ");
                    var dir = Console.ReadLine();
                    Console.Write("Enter max package age (min.): ");
                    var maxAge = Convert.ToInt32(Console.ReadLine());
                    DirectorySearch(dir, maxAge);
                }
                else
                {
                    Console.Write(args[0] + "is not a recognized command");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static int CheckDisableLastAccess()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = "CMD.exe";
            startInfo.Arguments = "/c fsutil behavior query disablelastaccess";
            process.StartInfo = startInfo;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] outputArray = output.Split("");
            int setting = Convert.ToInt32(outputArray[2]);

            return setting;
        }

        public static void DirectorySearch(string dir, int maxAge)
        {
            try
            {
                foreach (string f in Directory.GetFiles(dir))
                {
                    var fileAge = DateTime.Now - File.GetLastAccessTime(f);

                    if(fileAge.TotalMinutes > maxAge)
                    {
                        Console.WriteLine(Path.GetFileName(f));
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
