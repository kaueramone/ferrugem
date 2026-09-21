param(
    [switch]$Smoke,
    [switch]$Fps,
    [switch]$ForceBuild,
    [string]$EditorPath
)

$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$processes = [Collections.Generic.List[Diagnostics.Process]]::new()
$launcherMutex = $null
$ownsMutex = $false
$exitCode = 0

. (Join-Path $PSScriptRoot "Build-Helpers.ps1")

try {
    if ($Fps -and -not $Smoke) { throw 'Use -Smoke -Fps para o teste automatico de movimentacao.' }
    # Carregar funcoes do Utility no escopo do script antes das chamadas aninhadas.
    Import-Module Microsoft.PowerShell.Utility -Scope Global -ErrorAction Stop
    $pathHasher = [Security.Cryptography.SHA256]::Create()
    try { $mutexSuffix = [BitConverter]::ToString($pathHasher.ComputeHash([Text.Encoding]::UTF8.GetBytes($projectRoot.ToLowerInvariant()))).Replace('-', '') }
    finally { $pathHasher.Dispose() }
    $launcherMutex = [Threading.Mutex]::new($false, ('Local\FerrugemLauncher-' + $mutexSuffix))
    try { $ownsMutex = $launcherMutex.WaitOne(0) } catch [Threading.AbandonedMutexException] { $ownsMutex = $true }
    if (-not $ownsMutex) { throw 'Ja existe um teste em andamento para esta pasta.' }
    $runRoot = Join-Path $projectRoot ('Logs/Launcher/' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
    New-Item -ItemType Directory -Path $runRoot -Force | Out-Null
    $executable = Get-VerifiedBuild -Platform Windows -EditorPath $EditorPath -ForceBuild:$ForceBuild
    if ($Smoke) {
        & (Join-Path $PSScriptRoot 'Test-NetworkSmoke.ps1') -Executable $executable -Fps:$Fps
    }
    else {
        if (Get-NetUDPEndpoint -LocalPort 17979 -ErrorAction SilentlyContinue) { throw 'A porta UDP 17979 esta ocupada. Feche o teste anterior antes de iniciar outro.' }
        $serverLog = Join-Path $runRoot 'server.log'
        $server = Start-Process -FilePath $executable -ArgumentList @('-batchmode', '-nographics', '--server', '--address', '127.0.0.1', '--port', '17979', '-logFile', (Quote-Argument $serverLog)) -WindowStyle Hidden -PassThru
        $processes.Add($server)
        $deadline = [DateTime]::UtcNow.AddSeconds(40)
        do {
            if ($server.HasExited) { throw "Servidor encerrou antes de iniciar. Consulte $serverLog" }
            $listening = (Read-SharedLog $serverLog) -match '\[Ferrugem\] LISTEN_RESULT state=Succeeded endpoint=127\.0\.0\.1:17979'
            if (-not $listening) { Start-Sleep -Milliseconds 200 }
        } while (-not $listening -and [DateTime]::UtcNow -lt $deadline)
        if (-not $listening) { throw "Servidor nao iniciou em 40 segundos. Consulte $serverLog" }
        $clients = @()
        foreach ($number in 1..2) {
            $clientLog = Join-Path $runRoot "client-$number.log"
            $client = Start-Process -FilePath $executable -ArgumentList @('--client', '--address', '127.0.0.1', '--port', '17979', '-screen-fullscreen', '0', '-screen-width', '960', '-screen-height', '540', '-logFile', (Quote-Argument $clientLog)) -PassThru
            $processes.Add($client)
            $clients += $client
        }
        $connectDeadline = [DateTime]::UtcNow.AddSeconds(60)
        do {
            if ($server.HasExited) { throw "Servidor encerrou durante a conexao. Consulte $serverLog" }
            $allConnected = $true
            for ($index = 0; $index -lt $clients.Count; $index++) {
                $clientLog = Join-Path $runRoot ("client-{0}.log" -f ($index + 1))
                if ($clients[$index].HasExited) { throw "Cliente encerrou antes de conectar. Consulte $clientLog" }
                if ((Read-SharedLog $clientLog) -notmatch '\[Ferrugem\] CONNECTED world=FerrugemClient') { $allConnected = $false }
            }
            if (-not $allConnected) { Start-Sleep -Milliseconds 200 }
        } while (-not $allConnected -and [DateTime]::UtcNow -lt $connectDeadline)
        if (-not $allConnected) { throw "Os dois clientes nao conectaram em 60 segundos. Consulte $runRoot" }
        Write-Host "Teste aberto em duas janelas. Feche ambas para encerrar o servidor. Logs: $runRoot"
        while (@($clients | Where-Object { -not $_.HasExited }).Count -gt 0) {
            if ($server.HasExited) { throw "Servidor encerrou durante o teste. Consulte $serverLog" }
            Start-Sleep -Milliseconds 300
        }
    }
}
catch { Write-Host ("ERRO: " + $_.Exception.Message) -ForegroundColor Red; $exitCode = 1 }
finally {
    foreach ($process in $processes) {
        if (-not $process.HasExited) { Stop-Process -InputObject $process -Force -ErrorAction SilentlyContinue }
        $process.Dispose()
    }
    if ($ownsMutex) { $launcherMutex.ReleaseMutex() }
    if ($launcherMutex) { $launcherMutex.Dispose() }
}
exit $exitCode
