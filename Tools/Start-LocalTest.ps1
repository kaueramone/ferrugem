param(
    [switch]$Smoke,
    [switch]$ForceBuild,
    [string]$EditorPath
)

$ErrorActionPreference = 'Stop'
$projectRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$buildRoot = Join-Path $projectRoot 'Builds/Windows'
$executable = Join-Path $buildRoot 'Ferrugem.exe'
$assembly = Join-Path $buildRoot 'Ferrugem_Data/Managed/Assembly-CSharp.dll'
$stampPath = Join-Path $buildRoot '.verified-build.json'
$processes = [Collections.Generic.List[Diagnostics.Process]]::new()
$launcherMutex = $null
$ownsMutex = $false
$exitCode = 0

function Quote-Argument([string]$Value) {
    if ($Value.Contains('"')) { throw 'Argumento com aspas nao suportado.' }
    return '"' + $Value + '"'
}

function Get-Sha256([string]$Path) {
    $hasher = [Security.Cryptography.SHA256]::Create()
    $stream = $null
    try {
        $stream = [IO.File]::OpenRead($Path)
        return ([BitConverter]::ToString($hasher.ComputeHash($stream))).Replace('-', '')
    }
    finally {
        if ($stream) { $stream.Dispose() }
        $hasher.Dispose()
    }
}

function Get-SourceFingerprint([string[]]$Folders = @('Assets', 'Packages', 'ProjectSettings')) {
    $lines = [Collections.Generic.List[string]]::new()
    foreach ($folder in $Folders) {
        foreach ($file in (Get-ChildItem -LiteralPath (Join-Path $projectRoot $folder) -File -Recurse | Sort-Object FullName)) {
            $relative = $file.FullName.Substring($projectRoot.Length + 1).Replace('\', '/')
            if ($relative -match '^Assets/netcode-build-assets-temp(?:/|\.meta$)' -or
                $relative -match '^Assets/Resources/PerformanceTestRun(?:Info|Settings)\.json(?:\.meta)?$') { continue }
            $lines.Add($relative + ':' + (Get-Sha256 $file.FullName))
        }
    }
    $hasher = [Security.Cryptography.SHA256]::Create()
    try { return ([BitConverter]::ToString($hasher.ComputeHash([Text.Encoding]::UTF8.GetBytes(($lines -join "`n"))))).Replace('-', '') }
    finally { $hasher.Dispose() }
}

function Assert-EditorClosed {
    $normalizedRoot = $projectRoot.Replace('\', '/').TrimEnd('/')
    $active = @(Get-CimInstance Win32_Process -Filter "Name='Unity.exe'" | Where-Object {
        $_.CommandLine -and $_.CommandLine.Replace('\', '/') -match ('(?i)-projectPath\s+"?' + [regex]::Escape($normalizedRoot) + '(?:"|\s|$)')
    })
    if ($active.Count -gt 0) { throw 'Feche o Unity deste projeto e aguarde o build atual terminar.' }
    $lockPath = Join-Path $projectRoot 'Temp/UnityLockfile'
    if (Test-Path -LiteralPath $lockPath) {
        try {
            $lock = [IO.File]::Open($lockPath, [IO.FileMode]::Open, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
            $lock.Dispose()
        }
        catch { throw 'O projeto esta em uso pelo Unity. Feche o Editor antes de testar.' }
        # Um lock residual desbloqueado e deixado intacto; o Unity gerencia seu arquivo.
    }
}

function Read-SharedLog([string]$Path) {
    if (-not (Test-Path -LiteralPath $Path)) { return '' }
    $stream = [IO.File]::Open($Path, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::ReadWrite)
    $reader = [IO.StreamReader]::new($stream)
    try { return $reader.ReadToEnd() } finally { $reader.Dispose() }
}

try {
    # Carregar funcoes do Utility no escopo do script antes das chamadas aninhadas.
    Import-Module Microsoft.PowerShell.Utility -Scope Global -ErrorAction Stop
    $pathHasher = [Security.Cryptography.SHA256]::Create()
    try { $mutexSuffix = [BitConverter]::ToString($pathHasher.ComputeHash([Text.Encoding]::UTF8.GetBytes($projectRoot.ToLowerInvariant()))).Replace('-', '') }
    finally { $pathHasher.Dispose() }
    $launcherMutex = [Threading.Mutex]::new($false, ('Local\FerrugemLauncher-' + $mutexSuffix))
    try { $ownsMutex = $launcherMutex.WaitOne(0) } catch [Threading.AbandonedMutexException] { $ownsMutex = $true }
    if (-not $ownsMutex) { throw 'Ja existe um teste em andamento para esta pasta.' }
    Assert-EditorClosed
    $versionText = Get-Content -LiteralPath (Join-Path $projectRoot 'ProjectSettings/ProjectVersion.txt') -Raw
    if ($versionText -notmatch '(?m)^m_EditorVersion:\s*(\S+)') { throw 'Versao Unity ausente em ProjectVersion.txt.' }
    $editorVersion = $Matches[1]
    if (-not $EditorPath) { $EditorPath = Join-Path $env:ProgramFiles "Unity/Hub/Editor/$editorVersion/Editor/Unity.exe" }
    if (-not (Test-Path -LiteralPath $EditorPath -PathType Leaf)) { throw "Instale Unity $editorVersion pelo Hub ou informe -EditorPath." }
    $installedVersion = (Get-Item -LiteralPath $EditorPath).VersionInfo.ProductVersion
    if ($installedVersion -notmatch ('^' + [regex]::Escape($editorVersion) + '(?:_|$)')) { throw "Editor incorreto. Este projeto exige Unity $editorVersion." }
    $runRoot = Join-Path $projectRoot ('Logs/Launcher/' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff'))
    New-Item -ItemType Directory -Path $runRoot -Force | Out-Null
    $fingerprint = Get-SourceFingerprint
    $verified = $false
    if (-not $ForceBuild -and (Test-Path -LiteralPath $stampPath) -and (Test-Path -LiteralPath $executable) -and (Test-Path -LiteralPath $assembly)) {
        try {
            $stamp = Get-Content -LiteralPath $stampPath -Raw | ConvertFrom-Json
            $verified = $stamp.schema -eq 1 -and $stamp.editor -eq $editorVersion -and $stamp.source -eq $fingerprint -and
                $stamp.exeHash -eq (Get-Sha256 $executable) -and $stamp.assemblyHash -eq (Get-Sha256 $assembly)
        } catch { $verified = $false }
    }
    if (-not $verified) {
        Write-Host "Compilando Windows com Unity $editorVersion. A primeira compilacao pode demorar."
        Assert-EditorClosed
        if (Test-Path -LiteralPath $stampPath) { Remove-Item -LiteralPath $stampPath }
        $inputFingerprint = Get-SourceFingerprint @('Assets', 'Packages')
        $buildLog = Join-Path $runRoot 'build-windows.log'
        $buildArguments = @('-batchmode', '-quit', '-projectPath', (Quote-Argument $projectRoot), '-buildTarget', 'Win64', '-standaloneBuildSubtarget', 'Player', '-executeMethod', 'Ferrugem.Editor.ProjectBuild.WindowsClient', '-logFile', (Quote-Argument $buildLog))
        $build = Start-Process -FilePath $EditorPath -ArgumentList $buildArguments -WindowStyle Hidden -PassThru
        $processes.Add($build)
        if (-not $build.WaitForExit(1200000)) { throw "Build excedeu 20 minutos. Consulte $buildLog" }
        $build.WaitForExit()
        if ($build.ExitCode -ne 0 -or (Read-SharedLog $buildLog) -notmatch '\[Ferrugem\] BUILD_OK target=StandaloneWindows64 subtarget=Player') { throw "Build falhou (codigo $($build.ExitCode)). Consulte $buildLog" }
        if (-not (Test-Path -LiteralPath $executable) -or -not (Test-Path -LiteralPath $assembly)) { throw 'Build terminou sem os arquivos esperados.' }
        if ($inputFingerprint -ne (Get-SourceFingerprint @('Assets', 'Packages'))) { throw 'Assets ou pacotes mudaram durante o build. Execute novamente para validar a versao atual.' }
        # ProjectSettings pode ser salvo pelo Unity durante build; registrar seu estado final.
        $stamp = [ordered]@{ schema = 1; editor = $editorVersion; source = (Get-SourceFingerprint); exeHash = (Get-Sha256 $executable); assemblyHash = (Get-Sha256 $assembly); buildLog = $buildLog; verifiedUtc = [DateTime]::UtcNow.ToString('o') }
        $temporaryStamp = $stampPath + '.tmp'
        $stamp | ConvertTo-Json | Set-Content -LiteralPath $temporaryStamp -Encoding UTF8
        Move-Item -LiteralPath $temporaryStamp -Destination $stampPath -Force
    }
    else { Write-Host 'Build Windows atual confirmado; reutilizando compilacao verificada.' }
    if ($Smoke) {
        & (Join-Path $PSScriptRoot 'Test-NetworkSmoke.ps1') -Executable $executable
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
