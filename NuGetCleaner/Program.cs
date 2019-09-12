using System;
using System.Reflection;
using System.IO;
using System.Text;

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
                    GeneralInfo();
                }
                else if (args[0] == "clean")
                {
                    var setting = CheckDisableLastAccess();

                    if (setting != 2)
                    {
                        Console.WriteLine("\nYour Last Access updates are not currently enabled so this tool will not work");
                        Console.WriteLine("To enable Last Access updates, run powershell as administrator and input:\n");
                        Console.WriteLine("  fsutil behavior set disablelastaccess 2\n");
                        Console.WriteLine("Note: you may be asked to reboot for the settings change to take effect\n");

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
                    Console.WriteLine("\n" + args[0] + " is not a recognized command\n");
                    CommandsSupported();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void GeneralInfo()
        {
            var versionString = Assembly.GetEntryAssembly()
                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                        .InformationalVersion
                        .ToString();

            Console.WriteLine($"\nNuGetCleaner v{versionString}");
            Console.WriteLine("-------------\n");
            Console.WriteLine("Usage: nugetcleaner <command>\n");
            CommandsSupported();
            return;
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

        public static void CommandsSupported()
        {
            Console.WriteLine("Supported Commands:");
            CommandAndDescriptionPrint("clean");
            CommandAndDescriptionPrint("clean <path>");
            Console.WriteLine("");
        }

        public static void CommandAndDescriptionPrint(string comm)
        {
            var commSpace = 18;

            StringBuilder sb = new StringBuilder(" " + comm);

            for(int i = 0; i < commSpace-comm.Length; i++)
            {
                sb.Append(" ");
            }

            sb.Append(CommandDescription(comm));

            Console.WriteLine(sb);
        }

        public static string CommandDescription(string comm)
        {
            switch (comm)
            {
                case "clean":
                    return "Searches default GPF path for packages " +
                            "unused beyond time threshold and deletes them";
                case "clean <path>":
                    return "Searches specified GPF path for packages " +
                            "unused beyond time threshold and deletes them";
                //case "enable":
                //    return "Enables last access timestamp updates" +
                //            "(must be ran as administrator)";
                //case "info":
                //    return "Enables last access timestamp updates" +
                //            "(must be ran as administrator)";
                default:
                    return "";
            }
        }
    }
}