ContinuousSourceControl
=======================

Monitor a folder and watch for file changes, keep track of them, outside of the regular source control. So if you make some changes, save, then decide that was not right and delete them, normally you've lost them, but with this they will be in their own little version control container.

To start the application use:
ContinuousSourceControl.ConsoleHost.exe -project=<ProjectName> -path=<Root path to watch>

Uses .gitIgnote file to filter out files to ignore...
