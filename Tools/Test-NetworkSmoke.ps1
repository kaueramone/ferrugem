param(
    [switch]$Fps,
    [string]$Executable = (Join-Path $PSScriptRoot '../Builds/Windows/Ferrugem.exe'),
    [ValidateRange(1024, 65535)][int]$Port = 17979
)

$ErrorActionPreference = 'Stop'
$projectRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$executablePath = (Resolve-Path -LiteralPath $Executable).Path
$runDirectory = Join-Path $projectRoot ('Logs/Smoke/' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
New-Item -ItemType Directory -Path $runDirectory -Force | Out-Null
$startedProcesses = [System.Collections.Generic.List[System.Diagnostics.Process]]::new()
$checks = [System.Collections.Generic.List[string]]::new()
. (Join-Path $PSScriptRoot 'Assert-FpsSmoke.ps1')

function Assert-Smoke([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw $Message }
    $checks.Add($Message)
    Write-Output "PASS: $Message"
}

function Read-SmokeLog([string]$Name) {
    $logPath = Join-Path $runDirectory "$Name.log"
    if (Test-Path -LiteralPath $logPath) {
        $stream = [System.IO.File]::Open($logPath, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::ReadWrite)
        $reader = [System.IO.StreamReader]::new($stream)
        try { return $reader.ReadToEnd() }
        finally { $reader.Dispose() }
    }
    return ''
}

function Start-SmokePlayer([string]$Name, [string[]]$PlayerArguments) {
    $logPath = Join-Path $runDirectory "$Name.log"
    $arguments = @('-batchmode', '-nographics', '-logFile', ('"' + $logPath + '"')) + $PlayerArguments
    if ($Fps -and $Name -in @('server','client-a','client-b')) { $arguments += '--fps-smoke' }
    $process = Start-Process -FilePath $executablePath -ArgumentList $arguments -WorkingDirectory (Split-Path $executablePath) -WindowStyle Hidden -PassThru
    $startedProcesses.Add($process)
    return $process
}

function Wait-SmokeLog([string]$Name, [string]$Pattern, [System.Diagnostics.Process]$Process, [int]$Seconds = 25) {
    $deadline = [DateTime]::UtcNow.AddSeconds($Seconds)
    do {
        if ((Read-SmokeLog $Name) -match $Pattern) { return }
        if ($Process.HasExited) { throw "$Name encerrou antes do evento esperado: $Pattern. Ver $runDirectory" }
        Start-Sleep -Milliseconds 200
    } while ([DateTime]::UtcNow -lt $deadline)
    throw "Timeout em $Name aguardando $Pattern. Ver $runDirectory"
}

function Wait-SmokeExit([System.Diagnostics.Process]$Process, [int]$Seconds = 40) {
    if (-not $Process.WaitForExit($Seconds * 1000)) { throw "Timeout no processo $($Process.Id). Ver $runDirectory" }
    $Process.WaitForExit()
    return $Process.ExitCode
}

try {
    # Verificacao local previa: nunca interromper um listener que ja existe.
    $existingListener = Get-NetUDPEndpoint -LocalPort $Port -ErrorAction SilentlyContinue
    if ($existingListener) { throw "Porta UDP $Port ja esta ocupada. Escolha outra com -Port." }

    $server = Start-SmokePlayer 'server' @('--server', '--address', '127.0.0.1', '--port', "$Port", '--quit-after', '120')
    Wait-SmokeLog 'server' '\[Ferrugem\] LISTEN_RESULT state=Succeeded' $server
    Assert-Smoke ((Read-SmokeLog 'server') -match 'START role=server endpoint=127\.0\.0\.1:') 'Servidor unico iniciou em loopback.'

    $clientA = Start-SmokePlayer 'client-a' @('--client', '--address', '127.0.0.1', '--port', "$Port", '--quit-after', '30')
    $clientB = Start-SmokePlayer 'client-b' @('--client', '--address', '127.0.0.1', '--port', "$Port", '--cycle-after', '5', '--reconnect-delay', '2', '--quit-after', '30')
    Wait-SmokeLog 'client-a' '\[Ferrugem\] CONNECTED world=FerrugemClient' $clientA
    Wait-SmokeLog 'client-b' '\[Ferrugem\] CONNECTED world=FerrugemClient' $clientB
    Wait-SmokeLog 'server' 'CONNECTION_COUNT world=FerrugemServer count=2' $server
    Wait-SmokeLog 'client-b' '\[Ferrugem\] CYCLE_COMPLETE' $clientB
    Wait-SmokeLog 'server' 'CONNECTION_COUNT world=FerrugemServer count=2[\s\S]*CONNECTION_COUNT world=FerrugemServer count=2' $server
    $cycleLog = Read-SmokeLog 'client-b'
    Assert-Smoke ([regex]::Matches($cycleLog, '\[Ferrugem\] CONNECTED world=FerrugemClient').Count -ge 2) 'Cliente B conectou duas vezes no mesmo processo.'
    Assert-Smoke ($cycleLog -match 'DISCONNECT_REQUEST' -and $cycleLog -match 'CONNECTION_EVENT[^\r\n]*Disconnected') 'Cliente B observou desconexao real antes de reconectar.'
    Assert-Smoke ([regex]::Matches((Read-SmokeLog 'server'), 'CONNECTION_COUNT world=FerrugemServer count=2').Count -ge 2) 'Servidor voltou a registrar dois clientes apos a reconexao.'
    Assert-Smoke (-not $server.HasExited) 'Servidor permaneceu vivo durante a reconexao.'
    Assert-Smoke ((Read-SmokeLog 'server') -notmatch 'Exception:|START_FAILED') 'Servidor sem falha runtime no caminho positivo antes do teste de protocolo.'

    $mismatch = Start-SmokePlayer 'protocol-mismatch' @('--client', '--address', '127.0.0.1', '--port', "$Port", '--protocol', '2', '--quit-after', '12')
    $null = Wait-SmokeExit $mismatch 25
    $mismatchLog = Read-SmokeLog 'protocol-mismatch'
    $protocolEvidence = $mismatchLog + (Read-SmokeLog 'server')
    Assert-Smoke ($protocolEvidence -match 'BadProtocolVersion|bad protocol version') 'Protocolo incompatível foi rejeitado com motivo especifico.'
    Assert-Smoke ($mismatchLog -notmatch '\[Ferrugem\] CONNECTED') 'Cliente incompatível nao entrou no jogo.'
    Assert-Smoke ((Read-SmokeLog 'server') -notmatch 'CONNECTION_COUNT world=FerrugemServer count=3') 'Servidor nao contou cliente incompatível como aprovado.'

    Assert-Smoke ((Wait-SmokeExit $clientA) -eq 0) 'Cliente A encerrou normalmente.'
    Assert-Smoke ((Wait-SmokeExit $clientB) -eq 0) 'Cliente B encerrou apos ciclo completo.'
    foreach ($clientName in @('client-a', 'client-b')) {
        $clientLog = Read-SmokeLog $clientName
        Assert-Smoke ($clientLog -match 'START role=client' -and $clientLog -notmatch 'world=FerrugemServer|LISTEN_RESULT') "$clientName executou somente papel cliente."
        Assert-Smoke ($clientLog -notmatch 'Exception:|CYCLE_INCOMPLETE|START_FAILED') "$clientName sem falha runtime no caminho positivo."
    }

    if ($Fps) {
        Assert-FpsSmoke -ServerLog (Read-SmokeLog 'server') -ClientALog (Read-SmokeLog 'client-a') -ClientBLog (Read-SmokeLog 'client-b') -Assert { param($condition, $message) Assert-Smoke $condition $message }
    }

    $invalidCases = @(
        @{ Name = 'invalid-port'; Arguments = @('--server', '--port', '0') },
        @{ Name = 'invalid-address'; Arguments = @('--server', '--address', 'not-an-ip') },
        @{ Name = 'missing-port'; Arguments = @('--server', '--port') },
        @{ Name = 'conflicting-role'; Arguments = @('--server', '--client') }
    )
    foreach ($case in $invalidCases) {
        $process = Start-SmokePlayer $case.Name $case.Arguments
        $exitCode = Wait-SmokeExit $process 20
        $invalidLog = Read-SmokeLog $case.Name
        Assert-Smoke ($exitCode -eq 2 -and $invalidLog -match 'START_FAILED') "$($case.Name) rejeitado com codigo 2."
        Assert-Smoke ($invalidLog -notmatch '\[Ferrugem\] START role=|LISTEN_RESULT|CONNECT_REQUEST') "$($case.Name) nao iniciou listener nem conexao alternativa."
    }
    [ordered]@{ result = 'PASS'; executable = $executablePath; port = $Port; checks = $checks.ToArray(); logDirectory = $runDirectory } |
        ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $runDirectory 'result.json') -Encoding utf8
    Write-Output "SMOKE_PASS logs=$runDirectory"
}
catch {
    [ordered]@{ result = 'FAIL'; error = $_.Exception.Message; checks = $checks.ToArray(); logDirectory = $runDirectory } |
        ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $runDirectory 'result.json') -Encoding utf8
    throw
}
finally {
    foreach ($process in $startedProcesses) {
        if (-not $process.HasExited) { Stop-Process -InputObject $process -Force -ErrorAction SilentlyContinue }
        $process.Dispose()
    }
}
