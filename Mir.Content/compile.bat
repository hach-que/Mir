@echo off
cd %~dp0
if exist compiled goto cleanup
goto compile
:cleanup
rem echo Removing existing compiled assets...
rem del /S /Q compiled
:compile
echo Compiling assets...
cd assets
..\..\Protogame\ProtogameAssetTool\bin\Debug\ProtogameAssetTool.exe ^
	-o ..\compiled ^
	-p Windows ^
	-p MacOSX ^
	-p Linux 
cd ..

rem -p MacOSX ^
rem -p Xbox360 ^
rem -p WindowsPhone ^
rem -p iOS ^
rem -p WindowsStoreApp ^
rem -p NativeClient ^
rem -p Ouya ^
rem -p PlayStationMobile ^
rem -p WindowsPhone8 ^
rem -p RaspberryPi ^