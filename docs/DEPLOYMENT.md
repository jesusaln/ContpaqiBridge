# Despliegue en producción

Guía paso a paso para poner ContpaqiBridge en producción.

## Opción 1: Windows VPS (recomendado para producción)

Contrata un Windows Server VPS. Proveedores sugeridos (precios en MXN/mes):

| Proveedor | Plan básico | CPU | RAM | Disco |
|---|---|---|---|---|
| Contabo | ~$300 | 4 vCPU | 8 GB | 200 GB SSD |
| Hetzner | ~$350 | 4 vCPU | 8 GB | 80 GB SSD |
| IONOS | ~$400 | 4 vCPU | 8 GB | 160 GB SSD |
| Microsoft Azure | ~$700 | 2 vCPU | 4 GB | 64 GB |
| Unificado | ~$350 | 2 vCPU | 4 GB | 80 GB |

### Setup inicial

1. **Windows Server 2019/2022** (con Escritorio Remoto habilitado).
2. **Instalar .NET 6 SDK x86**:
   ```powershell
   winget install Microsoft.DotNet.SDK.6 --architecture x86
   ```
3. **Instalar SQL Server Express 2019+**:
   ```powershell
   winget install Microsoft.SQLServer2019Express
   ```
   Crear instancia `COMPAC22` durante la instalación.
4. **Instalar CONTPAQi Comercial Premium** (mismo instalador que en tu PC).
5. **Activar CONTPAQi** con tu licencia (o las de tus clientes).

### Desplegar el bridge

```powershell
# Clonar o copiar el código
cd C:\inetpub\sites\contpaqi-bridge

# Compilar en Release
dotnet publish -c Release -r win-x86 --self-contained false -o C:\inetpub\contpaqi-bridge

# Crear servicio de Windows (para auto-inicio)
sc.exe create ContpaqiBridge `
  binPath="C:\inetpub\contpaqi-bridge\ContpaqiBridge.exe" `
  DisplayName="Contpaqi Bridge API" `
  start=auto

sc.exe start ContpaqiBridge
```

### Configurar IIS como reverse proxy (HTTPS + dominio)

1. Instalar ARR (Application Request Routing):
   ```powershell
   # En IIS Manager: Web Platform Installer → ARR
   ```
2. Crear sitio en IIS (`mi-dominio.com`):
   - `C:\inetpub\contpaqi-bridge` como carpeta
   - `http://localhost:5000` como proxy backend
3. Agregar certificado SSL (Let's Encrypt con win-acme).

**Resultado final:** `https://contpaqi.miempresa.com` → proxy → `http://localhost:5000` (bridge).

### Firewall

```powershell
# Solo permitir entrada al proxy HTTPS (443) y RDP (3389)
New-NetFirewallRule -DisplayName "ContpaqiBridge HTTPS" -Direction Inbound -LocalPort 443 -Protocol TCP -Action Allow
New-NetFirewallRule -DisplayName "ContpaqiBridge Direct (debug)" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
```

---

## Opción 2: Tu PC local + túnel (gratis, baja concurrencia)

Si solo necesitas facturar desde tu Laravel y no esperas alto volumen:

### ZeroTier (recomendado, gratis)

1. Instala ZeroTier en tu PC: https://www.zerotier.com/download/
2. Instala ZeroTier en tu VPS Linux.
3. Crea una red en https://my.zerotier.com/.
4. Une ambos dispositivos a esa red.
5. Anota las IPs ZeroTier (ej: `192.168.191.226` para PC, `192.168.191.227` para VPS).
6. En tu Laravel, usa `http://192.168.191.226:5000` como URL del bridge.
7. Asegúrate de que el firewall de Windows permita el puerto 5000:
   ```powershell
   New-NetFirewallRule -DisplayName "ContpaqiBridge" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
   ```

**Ventajas:** Gratis, sin IP pública, encriptado por ZeroTier.

### Cloudflare Tunnel (gratis, permanente)

```powershell
# Instalar cloudflared
winget install Cloudflare.cloudflared

# Login
cloudflared tunnel login

# Crear túnel
cloudflared tunnel create contpaqi-bridge

# Configurar
cloudflared tunnel route dns contpaqi-bridge contpaqi.miempresa.com

# Crear config.yml
@"
tunnel: contpaqi-bridge
credentials-file: C:\Users\TU_USUARIO\.cloudflared\<TUNNEL_ID>.json
ingress:
  - hostname: contpaqi.miempresa.com
    service: http://localhost:5000
  - service: http_status:404
"@ | Out-File "$env:USERPROFILE\.cloudflared\config.yml"

# Correr
cloudflared tunnel run contpaqi-bridge
```

**Ventajas:** HTTPS automático, sin abrir puertos en firewall.

### ngrok (testing temporal, gratis limitado)

```powershell
ngrok http 5000
```

Te da una URL temporal tipo `https://abc-123.ngrok-free.app`.

---

## Opción 3: ngrok + Auto-renew

Para desarrollo local rápido.

---

## Variables de entorno en producción

En `appsettings.json` de producción:

```json
{
  "Contpaqi": {
    "EmpresasPath": "C:\\Compac\\Empresas",
    "DefaultUsuario": "SUPERVISOR",
    "DefaultClave": "PASSWORD_SEGURO",
    "InstanceSql": "localhost\\COMPAC22",
    "SqlUser": "sa",
    "SqlPassword": "SQL_PASSWORD_SEGURO"
  },
  "Bridge": {
    "ApiKey": "CLAVE_ALEATORIA_DE_64_CHARS_MIN"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

**Para generar una API Key segura:**

```powershell
# 64 caracteres aleatorios
[System.Convert]::ToBase64String((1..48 | ForEach-Object { Get-Random -Max 256 })) -replace '[/+=]','x'
```

---

## Hardening de seguridad

1. **Cambiar `ApiKey`** a una cadena larga aleatoria (mínimo 64 chars).
2. **Cambiar contraseñas** de SUPERVISOR y sa SQL.
3. **CORS restrictivo** (editar `Program.cs`):
   ```csharp
   policy.WithOrigins("https://tu-laravel.com")
         .AllowAnyHeader()
         .AllowAnyMethod();
   ```
4. **HTTPS obligatorio** vía reverse proxy.
5. **No exponer RDP** a internet (usar VPN).
6. **Logs centralizados** (Seq, ELK, Application Insights).
7. **Backups** de la carpeta `C:\Compac\Empresas` regularmente.
8. **Monitoreo** (health check cada minuto desde un servicio externo).

---

## Auto-inicio tras reinicio

Si lo corres como servicio de Windows (`sc.exe create`), se inicia solo.

Si lo corres manualmente, crea una tarea programada:

```powershell
$action = New-ScheduledTaskAction -Execute "C:\inetpub\contpaqi-bridge\ContpaqiBridge.exe"
$trigger = New-ScheduledTaskTrigger -AtStartup
$principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -LogonType ServiceAccount -RunLevel Highest
Register-ScheduledTask -TaskName "ContpaqiBridge" -Action $action -Trigger $trigger -Principal $principal
```

---

## Backups

```powershell
# Diario a las 3am
$action = New-ScheduledTaskAction -Execute "robocopy" -Argument '"C:\Compac\Empresas" "D:\Backups\Contpaqi" /MIR /R:3 /W:10 /LOG+:D:\Backups\backup.log'
$trigger = New-ScheduledTaskTrigger -Daily -At "03:00"
Register-ScheduledTask -TaskName "ContpaqiBackup" -Action $action -Trigger $trigger
```

---

## Monitoreo

```powershell
# Verificar salud cada 5 minutos desde el exterior
$action = New-ScheduledTaskAction -Execute "powershell.exe" -Argument "-Command `Invoke-WebRequest -Uri 'https://contpaqi.miempresa.com/api/Status/health' -UseBasicParsing`"
$trigger = New-ScheduledTaskTrigger -RepetitionInterval (New-TimeSpan -Minutes 5) -Once -At (Get-Date)
Register-ScheduledTask -TaskName "ContpaqiHealthCheck" -Action $action -Trigger $trigger
```

Para alertas, integra con un servicio de uptime (UptimeRobot, Healthchecks.io, Better Stack).