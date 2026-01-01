Write-Host "Cerrando instancias previas del Bridge..." -ForegroundColor Cyan
Get-Process "ContpaqiBridge" -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "Compilando e Iniciando Contpaqi Bridge..." -ForegroundColor Green
dotnet run --urls "http://0.0.0.0:5000"
