@echo off

if "%1"=="" (
    echo Usage: generate_connection_strings.bat ^<db user name^> ^<db password^>
    exit 1
) 
if "%2"=="" (
    echo Usage: generate_connection_strings.bat ^<db user name^> ^<db password^>
    exit 1
) 

set SCRIPT_PATH=%~dp0
set USER=moranbernate
set MACHINE=localhost
set SQLUSER=%1
set SQLPASS=%2
set PROJECTS_PATH=%SCRIPT_PATH%\..\src

set PROJECTS=OhioBox.Moranbernate.Tests

for %%A in (%PROJECTS%) do (
	del /f %PROJECTS_PATH%\%%A\conf.ConnectionStrings.config
	echo preparing %PROJECTS_PATH%\%%A\conf.ConnectionStrings.config for %USER%%MACHINE% ...
	for /f "usebackq tokens=1,3,5,7,9 delims=#" %%B in (%SCRIPT_PATH%\conf.ConnectionStrings.config.template) do (
		if "%%C" == "" (
			echo %%B >> %PROJECTS_PATH%\%%A\conf.ConnectionStrings.config
		) else (
			echo %%B%MACHINE%%%C%USER%%%D%SQLUSER%%%E%SQLPASS%%%F >> %PROJECTS_PATH%\%%A\conf.ConnectionStrings.config
		)
	)	
)