@echo off
setlocal
cd /d "%~dp0BonjourMtl"
if exist "bin\Debug\net10.0-desktop\BonjourMtl.exe" (
  start "" "bin\Debug\net10.0-desktop\BonjourMtl.exe" --present
) else (
  dotnet run -f net10.0-desktop -- --present
)
endlocal
