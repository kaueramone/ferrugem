param(
    [switch]$Smoke,
    [switch]$Fps,
    [switch]$Combat,
    [switch]$Manual,
    [switch]$ForceBuild,
    [string]$EditorPath,
    [string]$Distribution = 'Ubuntu-24.04',
    [ValidateRange(1024,65535)][int]$Port = 17981
)

$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$processes = [Collections.Generic.List[Diagnostics.Process]]::new()
$checks = [Collections.Generic.List[string]]::new()
$runRoot = Join-Path $projectRoot ('Logs/LinuxSmoke/' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
$launcherMutex = $null; $ownsMutex = $false; $server = $null; $linuxRecord = $null; $linuxWrapper = $null
$wslAddress = $null; $exitCode = 0; $failure = $null
. (Join-Path $PSScriptRoot 'Build-Helpers.ps1')
. (Join-Path $PSScriptRoot 'Assert-FpsSmoke.ps1')
. (Join-Path $PSScriptRoot 'Assert-CombatSmoke.ps1')

function Format-WslArgument([string]$Value) {
    # WSL interpreta switches diretamente; aspas sao necessarias somente em valores com espacos.
    if ($Value -match '\s|"') { return Quote-Argument $Value }
    return $Value
}

function Invoke-Wsl([string[]]$Arguments) {
    $info = [Diagnostics.ProcessStartInfo]::new()
    $info.FileName = 'wsl.exe'
    $info.Arguments = ((@('-d', $Distribution) + $Arguments | ForEach-Object { Format-WslArgument $_ }) -join ' ')
    $info.UseShellExecute = $false; $info.CreateNoWindow = $true
    $info.RedirectStandardOutput = $true; $info.RedirectStandardError = $true
    $process = [Diagnostics.Process]::Start($info)
    $output = $process.StandardOutput.ReadToEndAsync(); $errorOutput = $process.StandardError.ReadToEndAsync()
    try {
        if (-not $process.WaitForExit(120000)) { $process.Kill(); throw 'WSL excedeu 120 segundos.' }
        $process.WaitForExit()
        $text = $output.GetAwaiter().GetResult().Trim()
        $errorText = $errorOutput.GetAwaiter().GetResult().Trim()
        if ($process.ExitCode -ne 0) { throw "WSL falhou: $errorText $text" }
        return $text
    }
    finally { $process.Dispose() }
}

function Assert-LinuxTest([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw $Message }
    $checks.Add($Message); Write-Host "PASS: $Message"
}

function Start-WindowsClient([string]$Name, [string[]]$ExtraArguments) {
    $clientLog = Join-Path $runRoot ($Name + '.log')
    $arguments = @('--client', '--address', $wslAddress, '--port', "$Port", '-logFile', (Quote-Argument $clientLog))
    if ($ExtraArguments -and $ExtraArguments.Count -gt 0) { $arguments += $ExtraArguments }
    if ($Manual -and $Name -ne 'protocol-mismatch') {
        $arguments += @('-screen-fullscreen','0','-screen-width','960','-screen-height','540')
        $process = Start-Process -FilePath $windowsExecutable -ArgumentList $arguments -PassThru
    }
    else { $process = Start-Process -FilePath $windowsExecutable -ArgumentList (@('-batchmode','-nographics') + $arguments) -WindowStyle Hidden -PassThru }
    $processes.Add($process)
    return $process
}

function Wait-LinuxTestLog([string]$Name, [string]$Pattern, [Diagnostics.Process]$Process, [int]$Seconds = 60) {
    $path = Join-Path $runRoot ($Name + '.log')
    $deadline = [DateTime]::UtcNow.AddSeconds($Seconds)
    do {
        if ((Read-SharedLog $path) -match $Pattern) { return }
        if ($Process.HasExited) { throw "$Name encerrou antes do evento esperado. Consulte $path" }
        Start-Sleep -Milliseconds 200
    } while ([DateTime]::UtcNow -lt $deadline)
    throw "Timeout aguardando $Name. Consulte $path"
}

try {
    Import-Module Microsoft.PowerShell.Utility -Scope Global -ErrorAction Stop
    New-Item -ItemType Directory -Path $runRoot -Force | Out-Null
    if ($Manual -and $Smoke) { throw 'Escolha -Manual ou -Smoke.' }
    if ($Manual -and $Fps) { throw 'Use -Smoke -Fps para o teste automatico de movimentacao.' }
    if ($Manual -and $Combat) { throw 'Use -Smoke -Combat para o teste automatico de combate.' }
    if ($Fps -and $Combat) { throw 'Execute -Smoke -Fps e -Smoke -Combat separadamente.' }
    $pathHasher = [Security.Cryptography.SHA256]::Create()
    try { $mutexSuffix = [BitConverter]::ToString($pathHasher.ComputeHash([Text.Encoding]::UTF8.GetBytes($projectRoot.ToLowerInvariant()))).Replace('-', '') }
    finally { $pathHasher.Dispose() }
    $launcherMutex = [Threading.Mutex]::new($false, ('Local\FerrugemLauncher-' + $mutexSuffix))
    try { $ownsMutex = $launcherMutex.WaitOne(0) } catch [Threading.AbandonedMutexException] { $ownsMutex = $true }
    if (-not $ownsMutex) { throw 'Ja existe outro launcher em andamento neste projeto.' }
    $route = Invoke-Wsl @('--exec','/usr/sbin/ip','-4','-o','route','get','1.1.1.1')
    if ($route -notmatch '\bsrc\s+(\d+\.\d+\.\d+\.\d+)') { throw 'Nao foi possivel descobrir o IPv4 do WSL.' }
    $wslAddress = $Matches[1]
    $ip = [Net.IPAddress]::Parse($wslAddress); $bytes = $ip.GetAddressBytes()
    $privateIp = $bytes[0] -eq 10 -or ($bytes[0] -eq 172 -and $bytes[1] -ge 16 -and $bytes[1] -le 31) -or ($bytes[0] -eq 192 -and $bytes[1] -eq 168)
    if (-not $privateIp) { throw "O IP WSL nao e privado: $wslAddress" }
    Write-Host "Distro: $Distribution; destino privado WSL: ${wslAddress}:$Port"
    $windowsExecutable = Get-VerifiedBuild -Platform Windows -EditorPath $EditorPath -ForceBuild:$ForceBuild
    $linuxExecutable = Get-VerifiedBuild -Platform LinuxServer -EditorPath $EditorPath -ForceBuild:$ForceBuild
    $currentSource = Get-SourceFingerprint
    foreach ($platform in @('Windows','LinuxServer')) {
        $stamp = Get-Content -LiteralPath (Join-Path $projectRoot "Builds/$platform/.verified-build.json") -Raw | ConvertFrom-Json
        if ($stamp.source -ne $currentSource) { throw 'Configuracao mudou durante os builds. Execute novamente para validar ambos com a mesma fonte.' }
    }
    $linuxRoot = Invoke-Wsl @('--exec','wslpath','-a','-u',$projectRoot)
    $linuxRun = $linuxRoot + '/' + $runRoot.Substring($projectRoot.Length + 1).Replace('\','/')
    $linuxExePath = $linuxRoot + '/Builds/LinuxServer/FerrugemServer.x86_64'
    $linuxLog = $linuxRun + '/server.log'
    $linuxRecord = $linuxRun + '/server-process.txt'
    $linuxWrapper = $linuxRoot + '/Tools/Run-LinuxServer.sh'
    $duration = if ($Manual) { '1800' } else { '180' }
    $serverArguments = @('-d',$Distribution,'--exec','/bin/bash',$linuxWrapper,$linuxExePath,$wslAddress,"$Port",$linuxLog,$linuxRecord,$duration)
    if ($Fps) { $serverArguments += '--fps-smoke' }
    if ($Combat) { $serverArguments += '--combat-smoke' }
    $server = Start-Process -FilePath 'wsl.exe' -ArgumentList (($serverArguments | ForEach-Object { Format-WslArgument $_ }) -join ' ') -WindowStyle Hidden -RedirectStandardOutput (Join-Path $runRoot 'wsl.stdout.log') -RedirectStandardError (Join-Path $runRoot 'wsl.stderr.log') -PassThru
    $processes.Add($server)
    Wait-LinuxTestLog 'server' ('LISTEN_RESULT state=Succeeded endpoint=' + [regex]::Escape("${wslAddress}:$Port")) $server
    $serverLog = Read-SharedLog (Join-Path $runRoot 'server.log')
    Assert-LinuxTest ($serverLog -match 'START role=server' -and $serverLog -match 'NullGfxDevice|Null Device|GfxDevice: creating device client; threaded=0') 'Servidor Linux dedicado iniciou sem GPU no IP privado WSL.'
    $clientArgs = @()
    if (-not $Manual) { $clientArgs = @('--quit-after','45') }
    if ($Fps) { $clientArgs += '--fps-smoke' }
    if ($Combat) { $clientArgs += '--combat-smoke' }
    $clientA = Start-WindowsClient 'client-a' $clientArgs
    $clientBArgs = @($clientArgs)
    if (-not $Manual) { $clientBArgs += @('--cycle-after','5','--reconnect-delay','2') }
    $clientB = Start-WindowsClient 'client-b' $clientBArgs
    Wait-LinuxTestLog 'client-a' '\[Ferrugem\] CONNECTED world=FerrugemClient' $clientA
    if ($Manual) {
        Wait-LinuxTestLog 'client-b' '\[Ferrugem\] CONNECTED world=FerrugemClient' $clientB
        Wait-LinuxTestLog 'server' 'CONNECTION_COUNT world=FerrugemServer count=2\b' $server
        Assert-LinuxTest (-not $clientA.HasExited -and -not $clientB.HasExited -and -not $server.HasExited) 'Dois clientes Windows conectados ao servidor Linux para jogar.'
    }
    else {
        Wait-LinuxTestLog 'client-b' '\[Ferrugem\] CYCLE_COMPLETE' $clientB
        Wait-LinuxTestLog 'server' 'CONNECTION_COUNT world=FerrugemServer count=2[\s\S]*CONNECTION_COUNT world=FerrugemServer count=2' $server
        $clientBLog = Read-SharedLog (Join-Path $runRoot 'client-b.log')
        Assert-LinuxTest ([regex]::Matches($clientBLog,'\[Ferrugem\] CONNECTED world=FerrugemClient').Count -ge 2) 'Cliente Windows B conectou duas vezes ao servidor Linux.'
        Assert-LinuxTest ($clientBLog -match 'CONNECTION_EVENT[^\r\n]*Disconnected') 'Desconexao real precedeu a reconexao do cliente B.'
        Assert-LinuxTest (-not $server.HasExited) 'Servidor Linux permaneceu ativo durante o ciclo.'
        Assert-LinuxTest ((Read-SharedLog (Join-Path $runRoot 'server.log')) -notmatch 'Exception:|START_FAILED') 'Servidor sem excecoes no caminho positivo.'
        foreach ($name in @('client-a','client-b')) {
            $log = Read-SharedLog (Join-Path $runRoot ($name + '.log'))
            Assert-LinuxTest ($log -match 'START role=client' -and $log -notmatch 'world=FerrugemServer|LISTEN_RESULT|Exception:|START_FAILED|CYCLE_INCOMPLETE') "$name executou somente cliente Windows sem falhas."
        }
        $mismatch = Start-WindowsClient 'protocol-mismatch' @('--protocol','2','--quit-after','12')
        if (-not $mismatch.WaitForExit(30000)) { throw 'Timeout no teste de protocolo incompativel.' }
        $badLog = Read-SharedLog (Join-Path $runRoot 'protocol-mismatch.log')
        $serverLog = Read-SharedLog (Join-Path $runRoot 'server.log')
        Assert-LinuxTest (($badLog + $serverLog) -match 'BadProtocolVersion|bad protocol version') 'Protocolo incompativel rejeitado explicitamente.'
        Assert-LinuxTest ($badLog -notmatch '\[Ferrugem\] CONNECTED' -and $serverLog -notmatch 'CONNECTION_COUNT world=FerrugemServer count=3') 'Cliente incompativel nao entrou no jogo.'
    }
    if ($Manual) {
        Write-Host 'Sessao Linux pronta para jogar. Feche as duas janelas para encerrar. Limite da sessao: 30 minutos.'
        while (-not $clientA.HasExited -or -not $clientB.HasExited) {
            if ($server.HasExited) { throw 'Servidor Linux encerrou durante a sessao manual.' }
            Start-Sleep -Milliseconds 300
        }
    }
    else {
        foreach ($client in @($clientA,$clientB)) {
            if (-not $client.WaitForExit(60000)) { throw 'Timeout encerrando cliente Windows.' }
            $client.WaitForExit()
            Assert-LinuxTest ($client.ExitCode -eq 0) 'Cliente Windows encerrou normalmente.'
        }
    }
    foreach ($name in @('client-a','client-b')) {
        $finalLog = Read-SharedLog (Join-Path $runRoot ($name + '.log'))
        Assert-LinuxTest ($finalLog -notmatch 'Exception:|START_FAILED|CYCLE_INCOMPLETE') "$name sem falhas tardias no log final."
    }
    Assert-LinuxTest (-not $server.HasExited) 'Servidor Linux permaneceu ativo ate o fim do teste.'
    if ($Fps) {
        Assert-FpsSmoke -ServerLog (Read-SharedLog (Join-Path $runRoot 'server.log')) -ClientALog (Read-SharedLog (Join-Path $runRoot 'client-a.log')) -ClientBLog (Read-SharedLog (Join-Path $runRoot 'client-b.log')) -Assert { param($condition, $message) Assert-LinuxTest $condition $message }
    }
    if ($Combat) {
        Assert-CombatSmoke -ServerLog (Read-SharedLog (Join-Path $runRoot 'server.log')) -ClientALog (Read-SharedLog (Join-Path $runRoot 'client-a.log')) -ClientBLog (Read-SharedLog (Join-Path $runRoot 'client-b.log')) -Assert { param($condition, $message) Assert-LinuxTest $condition $message }
    }
}
catch { $failure = $_.Exception.Message; Write-Host ("ERRO: " + $failure) -ForegroundColor Red; $exitCode = 1 }
finally {
    if ($server -and $linuxWrapper -and $linuxRecord) {
        try { $null = Invoke-Wsl @('--exec','/bin/bash',$linuxWrapper,'--stop',$linuxRecord) }
        catch { $failure = "Falha no cleanup Linux: " + $_.Exception.Message; Write-Host $failure; $exitCode = 1 }
    }
    foreach ($process in $processes) {
        if (-not $process.HasExited) { Stop-Process -InputObject $process -Force -ErrorAction SilentlyContinue }
        $process.Dispose()
    }
    if (Test-Path -LiteralPath $runRoot) {
        $warnings = @()
        if ((Read-SharedLog (Join-Path $runRoot 'server.log')) -match 'Leak Detected') { $warnings += 'Unity reportou Leak Detected no encerramento; investigar antes de producao.' }
        [ordered]@{ result = $(if ($exitCode -eq 0) { 'PASS' } else { 'FAIL' }); mode = $(if ($Manual) { 'manual' } else { 'smoke' }); error = $failure; distribution = $Distribution; address = $wslAddress; port = $Port; checks = $checks.ToArray(); warnings = $warnings; logDirectory = $runRoot } | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $runRoot 'result.json') -Encoding UTF8
    }
    if ($ownsMutex) { $launcherMutex.ReleaseMutex() }
    if ($launcherMutex) { $launcherMutex.Dispose() }
}
if ($exitCode -eq 0) {
    if ($Manual) { Write-Host "LINUX_MANUAL_PASS logs=$runRoot" }
    else { Write-Host "LINUX_SMOKE_PASS logs=$runRoot" }
}
exit $exitCode
