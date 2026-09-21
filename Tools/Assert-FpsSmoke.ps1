# Assertions compartilhadas para o mesmo contrato de gameplay em Windows e Linux.
function Assert-FpsSmoke {
    param([string]$ServerLog, [string]$ClientALog, [string]$ClientBLog, [scriptblock]$Assert)
    $allLogs = $ServerLog + $ClientALog + $ClientBLog
    & $Assert ($allLogs -notmatch 'FPS_DUPLICATE|FPS_OUT_OF_BOUNDS') 'FPS: nenhum jogador duplicado ou fora dos limites.'
    & $Assert (($ClientALog + $ClientBLog) -notmatch 'SURVIVOR_ASSET_MISSING|SURVIVOR_ANIMATION_MISSING') 'FPS: clientes carregaram modelo e animacoes sem recursos ausentes.'
    $localPattern = 'FPS_OBSERVED world=FerrugemClient owner=(\d+) local=1\b'
    $ownersA = @([regex]::Matches($ClientALog, $localPattern) | ForEach-Object { $_.Groups[1].Value })
    $ownersB = @([regex]::Matches($ClientBLog, $localPattern) | ForEach-Object { $_.Groups[1].Value })
    & $Assert ($ownersA.Count -eq 1 -and $ownersB.Count -eq 2) 'FPS: A teve um jogador local e B recebeu outro jogador apos reconectar.'
    $ownerA = $ownersA[0]; $ownerB = $ownersB[0]; $ownerReconnected = $ownersB[1]
    & $Assert ($ownerA -ne $ownerB -and $ownerA -ne $ownerReconnected) 'FPS: A e B mantiveram identidades distintas nas duas conexoes de B.'
    # Netcode pode reciclar NetworkId: contar os ciclos, nao exigir outro numero.
    $expectedOwners = @($ownerA,$ownerB,$ownerReconnected)
    foreach ($owner in ($expectedOwners | Select-Object -Unique)) {
        $expectedCount = @($expectedOwners | Where-Object { $_ -eq $owner }).Count
        & $Assert ([regex]::Matches($ServerLog, ('FPS_SPAWN owner=' + $owner + '\b')).Count -eq $expectedCount) "FPS: servidor criou jogador $owner somente nos ciclos esperados ($expectedCount)."
        & $Assert ([regex]::Matches($ServerLog, ('FPS_MOVED world=FerrugemServer owner=' + $owner + '\b[^\r\n]*distance=[1-9]\d*(?:[.,]\d+)?\b')).Count -eq $expectedCount) "FPS: servidor confirmou movimento maior que um metro por ciclo do jogador $owner."
    }
    & $Assert ($ClientBLog -match ('FPS_MOVED world=FerrugemClient owner=' + $ownerA + ' local=0\b')) 'FPS: B observou movimento remoto de A.'
    foreach ($owner in (@($ownerB,$ownerReconnected) | Select-Object -Unique)) {
        $expectedCount = @(@($ownerB,$ownerReconnected) | Where-Object { $_ -eq $owner }).Count
        & $Assert ([regex]::Matches($ClientALog, ('FPS_MOVED world=FerrugemClient owner=' + $owner + ' local=0\b')).Count -eq $expectedCount) "FPS: A observou movimento remoto de B em todos os ciclos da identidade $owner."
    }
    & $Assert ($ServerLog -match ('FPS_DESPAWN world=FerrugemServer owner=' + $ownerB + '\b') -and $ClientALog -match ('FPS_DESPAWN world=FerrugemClient owner=' + $ownerB + '\b')) 'FPS: jogador antigo de B removido no servidor e no cliente A.'
    & $Assert ($ServerLog -match ('FPS_SPAWN owner=' + $ownerB + '\b[\s\S]*FPS_DESPAWN world=FerrugemServer owner=' + $ownerB + '\b[\s\S]*FPS_SPAWN owner=' + $ownerReconnected + '\b')) 'FPS: servidor removeu jogador antigo antes de criar o jogador da reconexao.'
    $serverCounts = @([regex]::Matches($ServerLog, 'FPS_COUNT world=FerrugemServer count=(\d+)') | ForEach-Object { [int]$_.Groups[1].Value })
    & $Assert ($serverCounts.Count -gt 0 -and @($serverCounts | Where-Object { $_ -gt 2 }).Count -eq 0 -and @($serverCounts | Where-Object { $_ -eq 2 }).Count -ge 2) 'FPS: servidor voltou a dois jogadores sem ultrapassar dois durante reconexao.'
}
