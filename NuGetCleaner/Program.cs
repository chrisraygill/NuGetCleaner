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
                if (args.Length == 0)
                {
                    var versionString = Assembly.GetEntryAssembly()
                                            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                            .InformationalVersion
                                            .ToString();

                    Console.WriteLine($"NuGetCleaner v{versionString}");
                    Console.WriteLine("-------------");
                    Console.WriteLine("\nUsage:");
                    Console.WriteLine("  NuGetCleaner <message>");
                    return;
                }

                if (args[0] == "clean")
                {
                    var setting = CheckDisableLastAccess();

                    if (setting != 2)
                    {
                        Console.WriteLine("Your Last Access updates are not currently enabled so this tool will not work");
                        Console.WriteLine("To enable Last Access updates, run powershell as administrator and type:");
                        Console.WriteLine("  fsutil behavior set disablelastaccess 2");
                        Console.WriteLine("note: enabling this setting may incur some performance overhead");
                        return;
                    }

                    var dir = "C:\\Users\\" + Environment.UserName + "\\.nuget\\packages";

                    if (args.Length > 1)
                    {
                        dir = args[1];
                    }

                    Console.Write("Enter max package age (Days): ");
                    var maxDays = Convert.ToInt32(Console.ReadLine());

                    SearchAndDestroy(dir, maxDays);
                    //SearchAndPrint(dir, maxDays);
                }
                else
                {
                    Console.Write(args[0] + " is not a recognized command");
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

        public static void SearchAndDestroy(string dir, int maxDays)
        {
            try
            {
                var it = 1;

                Console.WriteLine(DateTime.Now);

                Console.WriteLine("Deleted Packages: ");

                foreach (string pkg in Directory.GetDirectories(dir))
                {
                    foreach (string pkgVersion in Directory.GetDirectories(pkg))
                    {
                        var dirAge = DateTime.Now - RecursiveFindLAT(pkgVersion, DateTime.MinValue);

                        if (dirAge.TotalDays >= maxDays)
                        {
                            Console.WriteLine(pkgVersion);
                            RecursiveDelete(pkgVersion);

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

        public static void SearchAndPrint(string dir, int maxDays)
        {
            try
            {
                Console.WriteLine(DateTime.Now);

                Console.WriteLine("Deleted Packages: ");

                foreach (string pkg in Directory.GetDirectories(dir))
                {
                    foreach (string pkgVersion in Directory.GetDirectories(pkg))
                    {

                        var dirAge = DateTime.Now - RecursiveFindLAT(pkgVersion, DateTime.MinValue);

                        if (dirAge.TotalDays >= maxDays)
                        {
                            Console.WriteLine(pkgVersion + " --- " + RecursiveFindLAT(pkgVersion, DateTime.MinValue) + " --- Deleted");
                        }
                        else
                        {
                            Console.WriteLine(pkgVersion + " --- " + RecursiveFindLAT(pkgVersion, DateTime.MinValue));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void RecursiveDelete(string dir)
        {
            foreach (string subdir in Directory.GetDirectories(dir))
            {
                RecursiveDelete(subdir);
            }
            foreach (string subdir in Directory.GetDirectories(dir))
            {
                Directory.Delete(subdir);
            }
            foreach (string f in Directory.GetFiles(dir))
            {
                File.Delete(f);
            }
            Directory.Delete(dir);
        }

        public static DateTime RecursiveFindLAT(string dir, DateTime dt)
        {

            foreach (string subdir in Directory.GetDirectories(dir))
            {
                dt = RecursiveFindLAT(subdir, dt);
            }
            foreach (string f in Directory.GetFiles(dir))
            {
                if (dt < File.GetLastAccessTime(f))
                {
                    dt = File.GetLastAccessTime(f);
                }
            }

            return dt;
        }
    }
}
