# View Keypass Debug Log
$logPath = "$env:APPDATA\Keypass\debug.log"

if (Test-Path $logPath) {
    Write-Host "=== KEYPASS DEBUG LOG ===" -ForegroundColor Green
    Write-Host "Location: $logPath" -ForegroundColor Cyan
    Write-Host ""
    Get-Content $logPath -Wait -Tail 50
} else {
    Write-Host "Log file not found at: $logPath" -ForegroundColor Red
    Write-Host "Run Keypass.exe first to generate logs." -ForegroundColor Yellow
}
