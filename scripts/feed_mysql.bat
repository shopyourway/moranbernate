@echo off

SET ROOTUSER=%4
SET ROOTPASS=%5

rem Usage: feed_mysql.bat dump_file target_schema target_host
setlocal
call "%~dp0setup_env.bat" %ROOTUSER% %ROOTPASS%

set target_host=localhost
if not x%3x==xx set target_host=%3

echo Exec "%~1" on schema "%2" on host "%target_host%"
rem Use 'for' here in order to check first parameter (sql file) for no zero value
for %%a in ("%~1") do if %%~za neq 0 %mysqlcmd% %~2 -h%target_host% --max_allowed_packet=150M < "%~1"
