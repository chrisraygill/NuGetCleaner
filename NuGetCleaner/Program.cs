using System;
using System.Reflection;
using System.IO;
using CommandLine;
using System.ComponentModel;

namespace NuGetCleaner
{
    class Program
    {

        public static void Main(string[] args)
        {
            if (CheckDisableLastAccess() != 2)
            {
                Console.Write("\nYour Last Access updates are not currently enabled so this tool will not work. \n" +
                    "To enable Last Access updates, run powershell as administrator and input:\n\n" +
                    "\tfsutil behavior set disablelastaccess 2\n\n" +
                    "You may be asked to reboot for the settings change to take effect\n\n" +
                    "Note: Last usage time of packages will only be tracked once the setting is enabled.\n" +
                    "As such, last package usage will only be determined from the enable date onward.\n");
                return;
            }

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => Execute(options));
        }

        public static void Execute(Options options)
        {

            string Path;

            if (options.Path == null)
            {
                Path = "C:\\Users\\" + Environment.UserName + "\\.nuget\\packages";
            }
            else
            {
                Path = options.Path;
            }

            int Days = options.Days;

            if (options.DryRun)
            {
                SearchAndPrint(Path, Days);
            }
            else {
                SearchAndDestroy(Path, Days);
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

        public static void SearchAndDestroy(string Path, int Days)
        {
            try
            {
                Console.WriteLine("Deleted Packages: ");

                foreach (string pkg in Directory.GetDirectories(Path))
                {
                    foreach (string pkgVersion in Directory.GetDirectories(pkg))
                    {
                        var dirAge = DateTime.Now - RecursiveFindLAT(pkgVersion, DateTime.MinValue);

                        if (dirAge.TotalDays >= Days)
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

        public static void SearchAndDestroyDR(string Path, int Days)
        {
            try
            {
                Console.WriteLine("Deleted Packages: ");

                foreach (string pkg in Directory.GetDirectories(Path))
                {
                    foreach (string pkgVersion in Directory.GetDirectories(pkg))
                    {
                        var dirAge = DateTime.Now - RecursiveFindLAT(pkgVersion, DateTime.MinValue);

                        if (dirAge.TotalDays >= Days)
                        {
                            Console.WriteLine(pkgVersion + " --- Last Access: " + RecursiveFindLAT(pkgVersion, DateTime.MinValue));
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

        public static void SearchAndPrint(string Path, int Days)
        {
            try
            {
                Console.WriteLine("Deleted Packages: ");

                foreach (string pkg in Directory.GetDirectories(Path))
                {
                    foreach (string pkgVersion in Directory.GetDirectories(pkg))
                    {

                        var dirAge = DateTime.Now - RecursiveFindLAT(pkgVersion, DateTime.MinValue);

                        if (dirAge.TotalDays >= Days)
                        {
                            Console.WriteLine(pkgVersion.Substring(Path.Length + 1) + " --- Last Access: " + RecursiveFindLAT(pkgVersion, DateTime.MinValue));
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