@ECHO OFF

IF "%CONFIGURATION%"=="" SET CONFIGURATION=Debug

star --resourcedir="%~dp0src\Images\wwwroot" "%~dp0src/Images/bin/%CONFIGURATION%/Images.exe"