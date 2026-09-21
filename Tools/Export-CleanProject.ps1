param(
    [Parameter(Mandatory = $true)]
    [string]$Destination
)

$ErrorActionPreference = 'Stop'
$sourceRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$destinationRoot = [System.IO.Path]::GetFullPath($Destination)
$unityLock = Join-Path $sourceRoot 'Temp/UnityLockfile'
if (Test-Path -LiteralPath $unityLock) {
    throw 'Feche o Editor do projeto de origem e aguarde o build encerrar antes de exportar. O lock do Unity ainda existe.'
}
# Alguns preprocessadores de build alteram temporariamente arquivos versionaveis.
# O processo tambem e conferido para cobrir a janela anterior a criacao do lock.
$normalizedSource = $sourceRoot.Replace('\', '/').TrimEnd('/')
$sourceEditors = @(Get-CimInstance Win32_Process -Filter "Name='Unity.exe'" | Where-Object {
    if (-not $_.CommandLine) { return $false }
    $commandLine = $_.CommandLine.Replace('\', '/')
    $commandLine -match ('(?i)-projectPath\s+"?' + [regex]::Escape($normalizedSource) + '(?:"|\s|$)')
})
if ($sourceEditors.Count -gt 0) {
    throw 'O Editor do projeto de origem ainda esta aberto. Aguarde seu encerramento antes de exportar.'
}
$sourcePrefix = $sourceRoot.TrimEnd('\', '/') + [System.IO.Path]::DirectorySeparatorChar
if ($destinationRoot.Equals($sourceRoot, [System.StringComparison]::OrdinalIgnoreCase) -or
    $destinationRoot.StartsWith($sourcePrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw 'Escolha uma pasta fora do projeto de origem.'
}
if (Test-Path -LiteralPath $destinationRoot) {
    throw 'A pasta de destino deve ser nova para garantir uma importacao limpa.'
}

$relativeFiles = @(& git -C $sourceRoot -c core.quotepath=false ls-files --cached --others --exclude-standard)
if ($LASTEXITCODE -ne 0) { throw 'Nao foi possivel listar os arquivos elegiveis pelo Git.' }
$relativeFiles = @($relativeFiles | Sort-Object -Unique)
if ($relativeFiles.Count -eq 0) { throw 'Nenhum arquivo elegivel para exportar.' }

# Caches e artefatos nao podem entrar mesmo se adicionados ao Git por engano.
$excludedRoots = @('.git', 'Library', 'Temp', 'Obj', 'Logs', 'UserSettings', 'Build', 'Builds', 'MemoryCaptures', 'Recordings')
$exportFiles = @()
foreach ($relativeFile in $relativeFiles) {
    $firstSegment = ($relativeFile -split '[/\\]')[0]
    if ($excludedRoots -contains $firstSegment) { continue }
    $sourceFile = [System.IO.Path]::GetFullPath((Join-Path $sourceRoot $relativeFile))
    if (-not $sourceFile.StartsWith($sourcePrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Caminho fora do projeto: $relativeFile"
    }
    if (-not (Test-Path -LiteralPath $sourceFile -PathType Leaf)) { continue }
    $item = Get-Item -LiteralPath $sourceFile
    if ($item.Attributes -band [System.IO.FileAttributes]::ReparsePoint) {
        throw "Link simbolico requer revisao manual: $relativeFile"
    }
    $exportFiles += $relativeFile
}

New-Item -ItemType Directory -Path $destinationRoot | Out-Null
foreach ($relativeFile in $exportFiles) {
    $targetFile = Join-Path $destinationRoot $relativeFile
    $targetDirectory = Split-Path -Parent $targetFile
    if (-not (Test-Path -LiteralPath $targetDirectory)) {
        New-Item -ItemType Directory -Path $targetDirectory -Force | Out-Null
    }
    Copy-Item -LiteralPath (Join-Path $sourceRoot $relativeFile) -Destination $targetFile
}
Write-Output "Exportados $($exportFiles.Count) arquivos para $destinationRoot"
Write-Output 'Esta copia nao contem .git nem caches. Importe e compile com a versao registrada em ProjectSettings/ProjectVersion.txt.'
