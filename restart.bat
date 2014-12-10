@echo off
staradmin kill all

REM Start Launcher
call "%~dp0..\Launcher\run.bat"

Rem Procurement
call "%~dp0..Procurement\run.bat"

REM Products
call "%~dp0..\Products\run.bat"

REM Image
call "%~dp0run.bat"