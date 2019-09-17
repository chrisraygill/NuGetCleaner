# NuGetCleaner

## Description

The Global Packages Folder is where all packages installed through the NuGet package manager tool on Visual Studios are stored. When packages are no longer in use becuase a user has updated their packages for a project or has moved on to other projects, those unused packages remain in the GPF and continue to take up storage. It is not uncommon for the folder to have several Gigabytes of unnecessary package data after long-term use.

`nugetcleaner` is the solution! It is a global .NET CLI tool that deletes packages that haven't been used to build a project in a number of days specified by the user.

## Installation

`nugetcleaner` is available as a .NET Core Global Tool:

```bash
dotnet tool install --global NuGetCleaner --version 1.0.7
```
The latest version can also be downloaded directly from NuGet.org at:
https://www.nuget.org/packages/NuGetCleaner/

## Prerequisites

`nugetcleaner` makes use of last access timestamp updates which are disabled by default in Windows 7, 8, and 10. In order to use the tool, last access updates must be enabled.

In order to enable last access updates, run Windows PowerShell or Command Prompt in administrator mode and enter:
```bash
fsutil behavior set disablelastaccess 2
```

You may be prompted to reboot afterwards for the settings change to take effect.

It should be noted that last access timestamps will only begin to update after the settings change. Therefore, the package last access date will only be tracked from the enable date onward. 

Last access updates were disabled by default starting in Windows Vista in order to increase performance. However, there seems to be little evidence pointing to noticeable performance decreases in more recent Windows OS version when enabling last access updates. However, if you would like to disable last access updates (revert to default) following successfull use of 'nugetcleaner', then run Windows PowerShell or Command Prompt in administrator mode and enter:
```bash
fsutil behavior set disablelastaccess 3
```

Again, you may be prompted to reboot afterwards for the settings change to take effect.

## How It Works

`nugetcleaner` works by checking the last access date of all the packages in the global package folder and deleting ones with last access timestamps that exceed the age in days specified by the user. To be clear, it will check each version of a package and delete only the versions that exceed the age. For example, if there are multiple versions in the "newtonsoft.json" folder and only the most recent version has been used within the time constraint, then only the older version will be deleted. The "newtonsoft.json" parent directory will remain with the lastest version. However, if none of the versions were used within the time constraint, then all versions along with the parent folder will be deleted. At the end of the cleaning process, no empty folders will remain.

### Use Warning: 
If the cleaning process is activated immediately after last access updates enabled, before ongoing projects have been updated or built (thus accessing the relevant packages), then deletion will be determined solely by package installation date as last access timestamps wouldn't have had time to update. If this is an undesirable outcome, then it is highly recommended that the user waits until ongoing/relevant projects have been updated or built with last access timestamps enabled before using 'nugetcleaner' to clean out the global package folder. If you're interested in seeing the potential outcome of the cleaning process before going through with it, enable `--dry-run` to see a preview of the packages that would be deleted.

## Usage

```bash
nugetcleaner [--dry-run] --days <# of days>
```

`--dry-run` - (Optional) List packages that would be deleted given the specified number of days, but don't actually delete them.

`--days <# of days>` - (Required) Packages last accessed (used in a project build) on the number of days in the past specified or greater are selected for deletion.

#### Example 1: Delete packages that have not been accessed in 60 or more days.

```bash
nugetcleaner --days 60
```

#### Example 2: List packages that have not been accessed in 90 or more days, but don't delete them.

```bash
nugetcleaner --dry-run --days 90
```
