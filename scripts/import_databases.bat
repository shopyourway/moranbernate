@echo off
rem Use this script during Integration tests on CI and local environment

SET ROOTUSER=%1
SET ROOTPASS=%2

if "%ROOTUSER%"=="" (
    echo Usage: import_databases.bat ^<db admin user name^> ^<db admin password^>
    exit 1
) 
if "%ROOTPASS%"=="" (
    echo Usage: import_databases.bat ^<db admin user name^> ^<db admin password^>
    exit 1
) 

set batch_path=%~dp0

set LOCAL_CONNECTION=moranbernate localhost 3306 %ROOTUSER% %ROOTPASS%

echo Restore database locally ... 
call %batch_path%feed_mysql.bat %batch_path%schema.sql mysql localhost %ROOTUSER% %ROOTPASS%
if "%errorlevel%" == "0" goto END

echo Failed to restore database
exit /b %errorlevel%

:END
echo Done
exit /b 0
