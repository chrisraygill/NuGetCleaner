using System;
using System.Collections.Generic;
using CommandLine;

namespace NuGetCleaner
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Options
    {
        [Option("dry-run", HelpText = "Show packages that would be deleted without actually deleting them.")]
        public bool DryRun { get; set; }

        [Option("days", Required = true, HelpText = "Specify cut off for number of days since last package access. \n" + 
            "Packages last used in the number of days specified or greater will be deleted.")]
        public int Days { get; set; }

        [Option("path", HelpText = "Path to global package folder.\n" + 
            "If unspecified, default path is assumed.")]
        public string Path { get; set; }

        //[Option("date", HelpText = "Minimum version, not included in the versions to unlist (i.e. min<x<max). " +
        //    "Defaults to 0.")]
        //public string Date { get; set; }

        //[Option("weeks", HelpText = "Minimum version, not included in the versions to unlist (i.e. min<x<max). " +
        //    "Defaults to 0.")]
        //public string Weeks { get; set; }

        //[Option("months", HelpText = "Minimum version, not included in the versions to unlist (i.e. min<x<max). " +
        //    "Defaults to 0.")]
        //public string Months { get; set; }

        //[Option("package", HelpText = "Minimum version, not included in the versions to unlist (i.e. min<x<max). " +
        //    "Defaults to 0.")]
        //public string Package { get; set; }

        //[Option("version", HelpText = "Minimum version, not included in the versions to unlist (i.e. min<x<max). " +
        //    "Defaults to 0.")]
        //public string Version { get; set; }
    }
}
