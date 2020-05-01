echo off

echo [93mRestarting IIS ...[0m
iisreset || goto :error

echo [93mRemoving previous application ...[0m
del /q "c:\inetpub\wwwroot\MovieLibrary\*"
for /d %%x in (c:\inetpub\wwwroot\MovieLibrary\*) do (
    rd /s /q "%%x" || goto :error
)

rem Checking whether application folder is empty (del does not return an error if some files are not deleted)
for /F %%i in ('dir /b /a "c:\inetpub\wwwroot\MovieLibrary\*"') do (
    echo [91mFailed to clean application folder[0m
    goto :error
)

echo [93mUnpacking "MovieLibrary.zip" ...[0m
powershell.exe -nologo -noprofile -command ^
    "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::ExtractToDirectory('C:\inetpub\temp\deploy\MovieLibrary.zip', 'c:\inetpub\wwwroot\MovieLibrary\'); if ( $? -eq $false ) { exit 1 } }" ^
    || goto :error

echo [93mDeleting "MovieLibrary.zip" ...[0m
del "C:\inetpub\temp\deploy\MovieLibrary.zip"

echo [93mRestarting IIS ...[0m
iisreset || goto :error

echo [92mLocal deployment completed successfully![0m
goto :EOF

:error

set exit_code=%errorlevel%
if "%exit_code%"=="0" (
    set exit_code=1
)

echo [91mLocal deployment has FAILED with the error #%exit_code%[0m
exit /b %exit_code%
