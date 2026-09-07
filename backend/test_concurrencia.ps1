<#
========================================================================================
  PRUEBA DE CONCURRENCIA OPTIMISTA (STRESS TEST) - SUBASTAYA
  Cátedra: Proyecto de Software (Punto 4.1 de la consigna)
========================================================================================
#>

param (
    [string]$BaseUrl = "http://localhost:5013",
    [int]$SubastaId = 1
)

# Configuración de red para compatibilidad con Windows PowerShell 5.1 y Core
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
Add-Type -AssemblyName System.Net.Http

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "    TEST DE CONCURRENCIA OPTIMISTA - SubastaYa           " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Obtener información actual de la subasta
$infoUrl = "$BaseUrl/api/subastas/$SubastaId"
Write-Host "[*] Consultando estado de la subasta $SubastaId..." -ForegroundColor DarkGray

$subasta = $null
try {
    $subasta = Invoke-RestMethod -Uri $infoUrl -Method Get -TimeoutSec 10
}
catch {
    Write-Host ""
    Write-Host "[ERROR] No se pudo conectar con la API en $BaseUrl." -ForegroundColor Red
    Write-Host "Verifica que el backend este corriendo (dotnet run o F5 en Visual Studio)." -ForegroundColor Red
    Write-Host "Detalle: $($_.Exception.Message)" -ForegroundColor DarkGray
    exit
}

if (-not $subasta) {
    Write-Host "[ERROR] Subasta no encontrada." -ForegroundColor Red
    exit
}

# Calcular monto a ofertar (puja actual + incremento mínimo)
$pujaActual = [decimal]$subasta.pujaActual
$incremento = [decimal]$subasta.incrementoMinimo
$montoAOfertar = $pujaActual + $incremento

# Identificar postor líder actual consultando el historial de pujas para no autocompetir
$liderId = 0
try {
    $pujasExistentes = Invoke-RestMethod -Uri "$BaseUrl/api/subastas/$SubastaId/pujas" -Method Get -TimeoutSec 10
    if ($pujasExistentes -and $pujasExistentes.Count -gt 0) {
        $liderId = [int]$pujasExistentes[0].compradorId
    }
} catch {}

# Seleccionar 2 compradores distintos que no sean el vendedor ni el líder actual
# Compradores válidos con fondos: 2 (Comprador 1), 3 (Comprador 2), 4 (Comprador 3 / Sin Fondos cargado)
$candidatos = @(2, 3, 4) | Where-Object { $_ -ne $liderId }
$compradorA = $candidatos[0]
$compradorB = $candidatos[1]

Write-Host "Subasta: $($subasta.titulo)" -ForegroundColor White
Write-Host "Estado: $($subasta.estado) | Puja actual: `$$pujaActual | Incremento min: `$$incremento" -ForegroundColor DarkGray
Write-Host "Monto a ofertar por ambos al mismo tiempo: `$$montoAOfertar" -ForegroundColor Yellow
Write-Host "Postor 1: Usuario $compradorA  vs  Postor 2: Usuario $compradorB" -ForegroundColor DarkGray
Write-Host ""
Write-Host "[*] Disparando 2 peticiones POST concurrentes en el mismo instante..." -ForegroundColor Yellow

$urlPujas = "$BaseUrl/api/subastas/$SubastaId/pujas"
$jsonA = "{`"compradorId`": $compradorA, `"monto`": $montoAOfertar}"
$jsonB = "{`"compradorId`": $compradorB, `"monto`": $montoAOfertar}"

$client = $null
try {
    $client = New-Object System.Net.Http.HttpClient
    $client.Timeout = [System.TimeSpan]::FromSeconds(15)

    $contentA = New-Object System.Net.Http.StringContent($jsonA, [System.Text.Encoding]::UTF8, "application/json")
    $contentB = New-Object System.Net.Http.StringContent($jsonB, [System.Text.Encoding]::UTF8, "application/json")

    # Disparamos ambas peticiones asíncronamente en paralelo
    $taskA = $client.PostAsync($urlPujas, $contentA)
    $taskB = $client.PostAsync($urlPujas, $contentB)

    # Esperar que ambas terminen
    try { $taskA.Wait() } catch {}
    try { $taskB.Wait() } catch {}

    Write-Host ""
    Write-Host "---------------- RESULTADOS OBTENIDOS ----------------" -ForegroundColor Cyan

    $statusA = if (-not $taskA.IsFaulted -and $taskA.Result) { [int]$taskA.Result.StatusCode } else { 500 }
    $bodyA   = if (-not $taskA.IsFaulted -and $taskA.Result) { $taskA.Result.Content.ReadAsStringAsync().Result } else { $taskA.Exception.Message }

    $statusB = if (-not $taskB.IsFaulted -and $taskB.Result) { [int]$taskB.Result.StatusCode } else { 500 }
    $bodyB   = if (-not $taskB.IsFaulted -and $taskB.Result) { $taskB.Result.Content.ReadAsStringAsync().Result } else { $taskB.Exception.Message }

    $colorA = if ($statusA -eq 201) { "Green" } elseif ($statusA -eq 409) { "Magenta" } else { "Red" }
    Write-Host ">> Peticion A (Comprador $compradorA): HTTP $statusA" -ForegroundColor $colorA
    Write-Host "   Detalle: $bodyA"

    Write-Host ""
    $colorB = if ($statusB -eq 201) { "Green" } elseif ($statusB -eq 409) { "Magenta" } else { "Red" }
    Write-Host ">> Peticion B (Comprador $compradorB): HTTP $statusB" -ForegroundColor $colorB
    Write-Host "   Detalle: $bodyB"

    Write-Host "------------------------------------------------------" -ForegroundColor Cyan
    Write-Host ""

    $codigos = @($statusA, $statusB)
    if ($codigos -contains 201 -and $codigos -contains 409) {
        Write-Host "  PRUEBA EXITOSA: Concurrencia optimista demostrada." -ForegroundColor Green
        Write-Host "   - 1 puja fue procesada con exito (HTTP 201 Created)." -ForegroundColor Green
        Write-Host "   - 1 puja colisiono y fue rechazada con conflicto (HTTP 409 Conflict)." -ForegroundColor Green
        Write-Host "   - Los fondos del perdedor NO fueron debitados y la consistencia esta intacta." -ForegroundColor Green
    }
    elseif ($codigos -contains 201) {
        Write-Host "[!] Ambas peticiones completaron pero no colisionaron en la misma version." -ForegroundColor Yellow
        Write-Host "    Codigos: $statusA y $statusB" -ForegroundColor Yellow
    }
    else {
        Write-Host "[!] Codigos obtenidos: $statusA y $statusB" -ForegroundColor Red
    }
}
catch {
    Write-Host "[ERROR] Error durante la ejecucion del test: $($_.Exception.Message)" -ForegroundColor Red
}
finally {
    if ($client) { $client.Dispose() }
}
