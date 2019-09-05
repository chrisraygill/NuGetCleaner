# Your spec's journey
* Create a new wiki page using the spec template below, to jot your thoughts down.
* Create an [issue](https://github.com/NuGet/Home/issues) to track the feature or link an existing issue to the new wiki page. Engage the community on the feature. Feel free to tweet it from the @NuGet handle or ask the PM to tweet it out.
* Send a mail with a link to the wiki page to the core team alias.
* Campaign offline or in a meeting for the feature :.
* Once it is reviewed and signed off, a Manager or PM will move it to the Reviewed section.

## Issue
TBD

## Problem
There is currently no automated process to delete .npkg files from the Global Package Folder that are no longer needed/relevant. This presents a storage ineffiency that can be potentially significant for customers with very large or very many packages that are no longer in use. Customers who experience this problem currently must find the GPS folder themselves and delete the unwanted files manually.
## Who is the customer?
Any .NET developer that installs packages (i.e. every .NET developer).

## Evidence
At the moment, my evidence is that Karan says he has seen complaints from customers about this issue. Exact evidence TBD.

## Solution

The soltuion will come in a form on a downloadable .NET CLI tool. The first version of this tool will be very simple and follow the steps oulined below:

1. User invokes "clean" command
2. GPS file metadata is checked for most recent read
3. Files with read date > 30 days will be deleted
4. List of deleted files will be printed to console
