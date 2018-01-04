@echo off

set local_rootpassword=%2
set local_rootuser=%1
set rest_params_c=-C -u%local_rootuser% -p%local_rootpassword% --default-character-set=utf8
set rest_params_d=--user=%local_rootuser% --password=%local_rootpassword% --default-character-set=utf8

set mysqldir=none
set mysqlcmd=mysql %rest_params_c%
set sqldumpcmd=mysqldump %rest_params_d%
set mysqlclient=mysql -C --default-character-set=utf8

call %mysqlcmd% --version 1>> nul 2>&1
if "%errorlevel%" == "0" goto :EOF

REM - looking for mysqld in it's original location
REM - In a 64bit OS, MsBuild is still 32bit, so %PROGRAMFILES% points to C:\Program Files (x86).
for /d %%a in ("%PROGRAMFILES%" "%PROGRAMFILES(x86)%" "c:\program files") do (
	for /f "usebackq delims=" %%b in (`dir "%%~a\MySQL\MySQL Server *" /ad /b /od 2^> nul`) do (
		set mysqldir=%%~a\MySQL\%%b
	)
)
set mysqlcmd="%mysqldir%\bin\mysql.exe" %rest_params_c%
set sqldumpcmd="%mysqldir%\bin\mysqldump.exe" %rest_params_d%
set mysqlclient="%mysqldir%\bin\mysql.exe" -C --default-character-set=utf8

%mysqlcmd% --version 1>> nul 2>&1
if not "%errorlevel%" == "0" (
	echo Everything sucks. We'll all die miserably ^(Shlomo Primak^)
	exit /b 1
)
