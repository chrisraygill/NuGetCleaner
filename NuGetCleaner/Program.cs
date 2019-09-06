using System;
using System.IO;

namespace NuGetCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.Write("Enter a dir path");
            var dir = Console.ReadLine();
            DirectorySearch(dir);

        }

        public static void DirectorySearch(string dir)
        {
            try
            {
                foreach (string f in Directory.GetFiles(dir))
                {
                    Console.WriteLine(Path.GetFileName(f) + "  ---  " + File.GetLastWriteTime(f));
                }
                foreach (string d in Directory.GetDirectories(dir))
                {
                    Console.WriteLine(Path.GetFileName(d));
                    DirectorySearch(d);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //var lastModified = System.IO.File.GetLastWriteTime("C:\foo.bar");

        //Console.WriteLine(lastModified.ToString("dd/MM/yy HH:mm:ss"));
    }
}
