using System;
using System.Reflection;
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
                    Console.Write("Enter a dir path: ");
                    var dir = Console.ReadLine();
                    if (CheckDisableLastAccess() != 2)
                    {
                        Console.Write("Last access updates are not currently enabled. Please follow instructions at ... to enable them.");
                    }

                    Console.Write("Enter max package age (seconds): ");
                    var maxDays = Convert.ToInt32(Console.ReadLine());
                    DirectorySearch(dir, maxDays);
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

            string[] outputArray = output.Split(" ");
            int setting = Convert.ToInt32(outputArray[2]);

            return setting;
        }

        public static void DirectorySearch(string dir, int maxDays)
        {
            try
            {

                Console.WriteLine("Deleted Packages: ");

                foreach (string pkg in Directory.GetDirectories(dir))
                {
                    foreach (string pkgVersion in Directory.GetDirectories(pkg))
                    {
                        var dirAge = DateTime.Now - Directory.GetLastAccessTime(pkgVersion);

                        if (dirAge.TotalSeconds > maxDays)
                        {
                            Console.WriteLine(Path.GetFileName(pkgVersion));
                            Directory.Delete(pkgVersion);

                            if (Directory.GetDirectories(pkg).Length == 0)
                            {
                                Directory.Delete(pkg);
                            }
                        }
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
