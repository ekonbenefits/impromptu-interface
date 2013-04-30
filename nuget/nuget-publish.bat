@echo off
IF %2.==. GOTO WrongArgs
 
set SYMBOLSOURCE="http://nuget.gw.symbolsource.org/Public/NuGet"
 
..\.nuget\nuget.exe push %1.nupkg %2
..\.nuget\nuget.exe push symbols\%1.nupkg %2 -source %SYMBOLSOURCE%
 
GOTO:EOF
:WrongArgs
ECHO "publish-nuget <pkgname> <apikey>"