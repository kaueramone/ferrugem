# Helpers compartilhados; os launchers fornecem projectRoot, runRoot e processes.
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

function Get-VerifiedBuild {
    param([ValidateSet('Windows','LinuxServer')][string]$Platform, [string]$EditorPath, [switch]$ForceBuild)
    $buildRoot = Join-Path $projectRoot ('Builds/' + $Platform)
    if ($Platform -eq 'Windows') {
        $executable = Join-Path $buildRoot 'Ferrugem.exe'
        $assembly = Join-Path $buildRoot 'Ferrugem_Data/Managed/Assembly-CSharp.dll'
        $target = 'Win64'; $subtarget = 'Player'; $method = 'Ferrugem.Editor.ProjectBuild.WindowsClient'
        $successPattern = '\[Ferrugem\] BUILD_OK target=StandaloneWindows64 subtarget=Player'
    }
    else {
        $executable = Join-Path $buildRoot 'FerrugemServer.x86_64'
        $assembly = Join-Path $buildRoot 'FerrugemServer_Data/Managed/Assembly-CSharp.dll'
        $target = 'Linux64'; $subtarget = 'Server'; $method = 'Ferrugem.Editor.ProjectBuild.LinuxServer'
        $successPattern = '\[Ferrugem\] BUILD_OK target=StandaloneLinux64 subtarget=Server'
    }
    $stampPath = Join-Path $buildRoot '.verified-build.json'
    Assert-EditorClosed
    $versionText = Get-Content -LiteralPath (Join-Path $projectRoot 'ProjectSettings/ProjectVersion.txt') -Raw
    if ($versionText -notmatch '(?m)^m_EditorVersion:\s*(\S+)') { throw 'Versao Unity ausente em ProjectVersion.txt.' }
    $editorVersion = $Matches[1]
    if (-not $EditorPath) { $EditorPath = Join-Path $env:ProgramFiles "Unity/Hub/Editor/$editorVersion/Editor/Unity.exe" }
    if (-not (Test-Path -LiteralPath $EditorPath -PathType Leaf)) { throw "Instale Unity $editorVersion pelo Hub ou informe -EditorPath." }
    $installedVersion = (Get-Item -LiteralPath $EditorPath).VersionInfo.ProductVersion
    if ($installedVersion -notmatch ('^' + [regex]::Escape($editorVersion) + '(?:_|$)')) { throw "Editor incorreto. Este projeto exige Unity $editorVersion." }
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
        Write-Host "Compilando $Platform com Unity $editorVersion. A primeira compilacao pode demorar."
        Assert-EditorClosed
        if (Test-Path -LiteralPath $stampPath) { Remove-Item -LiteralPath $stampPath }
        $inputFingerprint = Get-SourceFingerprint @('Assets', 'Packages')
        $buildLog = Join-Path $runRoot ("build-" + $Platform.ToLowerInvariant() + ".log")
        $buildArguments = @('-batchmode', '-quit', '-projectPath', (Quote-Argument $projectRoot), '-buildTarget', $target, '-standaloneBuildSubtarget', $subtarget, '-executeMethod', $method, '-logFile', (Quote-Argument $buildLog))
        $build = Start-Process -FilePath $EditorPath -ArgumentList $buildArguments -WindowStyle Hidden -PassThru
        $processes.Add($build)
        if (-not $build.WaitForExit(1200000)) { throw "Build excedeu 20 minutos. Consulte $buildLog" }
        $build.WaitForExit()
        if ($build.ExitCode -ne 0 -or (Read-SharedLog $buildLog) -notmatch $successPattern) { throw "Build falhou (codigo $($build.ExitCode)). Consulte $buildLog" }
        if (-not (Test-Path -LiteralPath $executable) -or -not (Test-Path -LiteralPath $assembly)) { throw 'Build terminou sem os arquivos esperados.' }
        if ($inputFingerprint -ne (Get-SourceFingerprint @('Assets', 'Packages'))) { throw 'Assets ou pacotes mudaram durante o build. Execute novamente para validar a versao atual.' }
        # ProjectSettings pode ser salvo pelo Unity durante build; registrar seu estado final.
        $stamp = [ordered]@{ schema = 1; editor = $editorVersion; source = (Get-SourceFingerprint); exeHash = (Get-Sha256 $executable); assemblyHash = (Get-Sha256 $assembly); buildLog = $buildLog; verifiedUtc = [DateTime]::UtcNow.ToString('o') }
        $temporaryStamp = $stampPath + '.tmp'
        $stamp | ConvertTo-Json | Set-Content -LiteralPath $temporaryStamp -Encoding UTF8
        Move-Item -LiteralPath $temporaryStamp -Destination $stampPath -Force
    }
    else { Write-Host "Build $Platform atual confirmado; reutilizando compilacao verificada." }
    return $executable
}
