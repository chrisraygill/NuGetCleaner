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
`fsutil behavior set disablelastaccess 2`

You may be prompted to reboot afterwards for the settings change to take effect.

## Usage

```bash
nugetcleaner [--dry-run] --days <# of days>
```

`--dry-run` - (Optional) List packages that would be deleted given the specified number of days, but don't actually delete them.

`--days <# of days>` - (Required) Packages last accessed (used in a project build) on the number of days in the past specified or greater are selected for deletion.

#### Example 1: Delete packages that have not been accessed in 60 or more days.

`nugetcleaner --days 60`

#### Example 2: List packages that have not been accessed in 90 or more days, but don't delete them.

`nugetcleaner --dry-run --days 90`
