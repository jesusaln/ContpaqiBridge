# start_bridge_pool.ps1
# Levanta N instancias del bridge en puertos 5000, 5001, 5002.
# Cada instancia es un proceso independiente con su propio _lock, así que procesan
# en paralelo (multi-proceso real, no async).
#
# Requisito: el bridge ya está compilado (dotnet build -c Release).
#
# Uso:
#   .\start_bridge_pool.ps1 -Count 3         # arranca 3 instancias
#   .\start_bridge_pool.ps1 -Count 3 -Stop   # detiene todas las instancias
#
# Endpoints:
#   - http://127.0.0.1:5000  (instancia 0)
#   - http://127.0.0.1:5001  (instancia 1)
#   - http://127.0.0.1:5002  (instancia 2)
#   - http://127.0.0.1:8080  (load balancer Nginx, opcional)

param(
    [int]$Count = 3,
    [switch]$Stop,
    [int]$StartPort = 5000,
    [string]$BridgePath = "C:\Users\JESUS\Desktop\ContpaqiBridge-master\ContpaqiBridge-master"
)

$ErrorActionPreference = "Stop"
$poolName = "ContpaqiBridgePool"

function Stop-Pool {
    Write-Host "[$poolName] Deteniendo todas las instancias..." -ForegroundColor Yellow
    Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object {
        $_.CommandLine -like "*ContpaqiBridge*" -or
        ($_.MainModule.FileName -like "*dotnet*" -and $_.Path -like "*ContpaqiBridge*")
    } | Stop-Process -Force -ErrorAction SilentlyContinue

    Get-CimInstance Win32_Process -Filter "Name = 'ContpaqiBridge.exe'" -ErrorAction SilentlyContinue |
        ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }

    Get-NetTCPConnection -State Listen -ErrorAction SilentlyContinue |
        Where-Object { $_.LocalPort -ge $StartPort -and $_.LocalPort -lt ($StartPort + $Count) } |
        ForEach-Object {
            try { Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue } catch {}
        }
    Write-Host "[$poolName] Listo." -ForegroundColor Green
}

if ($Stop) { Stop-Pool; exit 0 }

Stop-Pool

# Compilar si no existe el binario
$exePath = Join-Path $BridgePath "bin\Release\net6.0\ContpaqiBridge.exe"
if (-not (Test-Path $exePath)) {
    Write-Host "[$poolName] Compilando bridge..." -ForegroundColor Cyan
    Push-Location $BridgePath
    & "C:\Program Files (x86)\dotnet\dotnet.exe" build -c Release -nologo | Out-Null
    Pop-Location
}

if (-not (Test-Path $exePath)) {
    Write-Host "[$poolName] ERROR: binario no encontrado en $exePath" -ForegroundColor Red
    exit 1
}

Write-Host "[$poolName] Iniciando $Count instancias del bridge (puertos $StartPort a $($StartPort + $Count - 1))..." -ForegroundColor Cyan

$instances = @()
for ($i = 0; $i -lt $Count; $i++) {
    $port = $StartPort + $i
    $logPath = "C:\Users\JESUS\AppData\Local\Temp\2\opencode\bridge_pool_$port.log"
    $errPath = "C:\Users\JESUS\AppData\Local\Temp\2\opencode\bridge_pool_$port.err"

    Write-Host "  Instancia $i -> puerto $port (log: $logPath)" -ForegroundColor Gray

    $p = Start-Process -FilePath $exePath `
        -ArgumentList "--urls","http://0.0.0.0:$port" `
        -WorkingDirectory $BridgePath `
        -WindowStyle Normal `
        -RedirectStandardOutput $logPath `
        -RedirectStandardError $errPath `
        -PassThru

    $instances += [PSCustomObject]@{
        Index = $i
        Port = $port
        PID = $p.Id
        LogFile = $logPath
    }
}

Start-Sleep -Seconds 5

Write-Host ""
Write-Host "[$poolName] Estado de las instancias:" -ForegroundColor Cyan
foreach ($inst in $instances) {
    $listen = (Get-NetTCPConnection -LocalPort $inst.Port -State Listen -ErrorAction SilentlyContinue | Measure-Object).Count
    $status = if ($listen -gt 0) { "ONLINE" } else { "OFFLINE" }
    $color = if ($listen -gt 0) { "Green" } else { "Red" }
    Write-Host "  [$($inst.Index)] puerto $($inst.Port) PID $($inst.PID) -> $status" -ForegroundColor $color
}

Write-Host ""
Write-Host "[$poolName] URLs disponibles:" -ForegroundColor Cyan
foreach ($inst in $instances) {
    Write-Host "  http://127.0.0.1:$($inst.Port)  (instancia $($inst.Index))"
}

Write-Host ""
Write-Host "[$poolName] Para detener todas:  .\start_bridge_pool.ps1 -Stop" -ForegroundColor Gray