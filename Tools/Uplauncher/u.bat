copy /Y "Uplauncher.exe" "C:\wamp64\www\releases\uplauncher\"

@set b=1

for /f "delims=" %%i in (C:\wamp64\www\releases\version\VERSION) do set c=%%i
echo %c%

set /a "a=%c%+%b%"

echo %a% > C:\wamp64\www\releases\version\VERSION
