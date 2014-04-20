Sledge Build Script
-------------------

This script will build Sledge and package it up in a nice way.

To use:

* Install [7-Zip](http://www.7-zip.org/)
* Install [NSIS](http://nsis.sourceforge.net/)
* You also need Powershell, MSBuild 4.0, and Robocopy available.
* In Powershell, run: Set-ExecutionPolicy unrestricted (required to run Powershell scripts)
* Run GenerateInstall.ps1 as an administrator
* The build artifacts are in /Out
    * The results from MSBuild are in /Out/Build
    * The ZIP archive is at /Out/Sledge.Editor.{version}.zip
    * The installer is at /Out/Sledge.Editor.{version}.exe
    * The NSIS script is at /Out/Sledge.Editor.Installer.{version}.nsi
    * The version text file is at /Out/version.txt
    * The complete build log is at /Out/Build.log
* If you wish to disable Powershell scripts again, run: Set-ExecutionPolicy restricted

InsertIcons.exe is complied from: https://github.com/einaregilsson/InsertIcons