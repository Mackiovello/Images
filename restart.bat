@echo off
staradmin kill all

REM Start Launcher
call "%~dp0..\Launcher\run.bat"

Rem Start RetailSuite
call "%~dp0..\RetailSuite\run.bat"

REM PIM
call "%~dp0..\PIM\run.bat"

REM PIM Images
call "%~dp0run.bat"