# Distingue regras exercitadas por fixtures do servidor e replicacao real nos clientes.
function Assert-CombatSmoke {
    param([string]$ServerLog, [string]$ClientALog, [string]$ClientBLog, [scriptblock]$Assert)
    $allLogs = $ServerLog + $ClientALog + $ClientBLog
    & $Assert ($allLogs -notmatch 'Exception:|COMBAT_SUITE_FAIL|COMBAT_CHECK[^\r\n]*pass=0|SURVIVOR_ASSET_MISSING|SURVIVOR_ANIMATION_MISSING') 'Combate: nenhum erro runtime ou falha declarada nos cenarios e recursos visuais.'
    $requiredChecks = @('body_immune','head_kill','explosion_kill','infection_once','respawn_human','pvp_no_infection','cover_blocks','shot_cooldown','empty_clip','reload','spawn_protection','melee_range','melee_cover','ray_geometry')
    foreach ($name in $requiredChecks) {
        & $Assert ([regex]::Matches($ServerLog, ('COMBAT_CHECK name=' + $name + ' pass=1\b')).Count -eq 1) "Combate: fixture do servidor validou $name uma vez."
    }
    $suite = [regex]::Matches($ServerLog, 'COMBAT_SUITE_PASS count=(\d+)\b')
    & $Assert ($suite.Count -eq 1 -and [int]$suite[0].Groups[1].Value -ge $requiredChecks.Count) 'Combate: servidor concluiu a suite completa de regras.'
    foreach ($entry in @(@{ Name='A'; Log=$ClientALog }, @{ Name='B'; Log=$ClientBLog })) {
        & $Assert ($entry.Log -match 'COMBAT_OBSERVED owner=\d+ local=1 health=100 ammo=6\b') "Combate: cliente $($entry.Name) recebeu saude e municao iniciais do jogador local."
        & $Assert ($entry.Log -match 'COMBAT_OBSERVED owner=\d+ local=0 health=100 ammo=6\b') "Combate: cliente $($entry.Name) recebeu estado de jogador remoto."
        & $Assert ($entry.Log -match 'COMBAT_ZOMBIES_OBSERVED count=[1-9]\d*\b') "Combate: cliente $($entry.Name) recebeu zombies vivos do servidor."
    }
    & $Assert ([regex]::Matches($ClientBLog, 'COMBAT_OBSERVED owner=\d+ local=1\b').Count -ge 2 -and $ClientBLog -match 'CONNECTION_EVENT[^\r\n]*Disconnected[\s\S]*COMBAT_OBSERVED owner=\d+ local=1\b') 'Combate: B voltou a receber estado local apos reconectar.'
    & $Assert ([regex]::Matches($ClientBLog, 'COMBAT_ZOMBIES_OBSERVED count=[1-9]\d*\b').Count -ge 2 -and $ClientBLog -match 'CONNECTION_EVENT[^\r\n]*Disconnected[\s\S]*COMBAT_ZOMBIES_OBSERVED count=[1-9]\d*\b') 'Combate: B recebeu zombies novamente ao entrar na partida em andamento.'
    $localA = [regex]::Match($ClientALog, 'COMBAT_OBSERVED owner=(\d+) local=1\b').Groups[1].Value
    $localB = [regex]::Matches($ClientBLog, 'COMBAT_OBSERVED owner=(\d+) local=1\b')
    foreach ($owner in @($localA, $localB[$localB.Count - 1].Groups[1].Value)) {
        & $Assert ($ServerLog -match ('COMBAT_SHOT owner=' + $owner + ' ammo=5\b')) "Combate: servidor processou disparo real enviado pelo jogador $owner."
    }
    & $Assert ([regex]::Matches($ClientBLog, 'COMBAT_SHOT_OBSERVED owner=\d+ local=1 ammo=5\b').Count -ge 2) 'Combate: B recebeu consumo de municao antes e depois da reconexao.'
    & $Assert ($ClientALog -match 'COMBAT_SHOT_OBSERVED owner=\d+ local=0 ammo=5\b') 'Combate: A recebeu consumo de municao causado por disparo remoto.'
    & $Assert ($ServerLog -match ('COMBAT_RELOAD owner=' + $localA + ' ammo=6\b')) 'Combate: servidor concluiu recarga solicitada pelo cliente A.'
    & $Assert ($ClientALog -match ('COMBAT_RELOAD_OBSERVED owner=' + $localA + ' ammo=6\b')) 'Combate: cliente A recebeu municao restaurada apos recarga real.'
    & $Assert ([regex]::Matches($ServerLog, 'COMBAT_LIVE_MUTATION infection=1 repeatedDamage=guarded zombieDeath=1\b').Count -eq 1) 'Combate: cenario injetado no servidor alterou ghosts reais uma vez.'
    $infections = [regex]::Matches($ServerLog, 'COMBAT_INFECTION owner=(\d+) death=(\d+)\b')
    & $Assert ($infections.Count -eq 1) 'Combate: dano infectante repetido gerou exatamente uma infeccao no servidor.'
    $infectedOwner = $infections[0].Groups[1].Value
    & $Assert ([regex]::Matches($ServerLog, ('COMBAT_DEATH owner=' + $infectedOwner + ' infection=1\b')).Count -eq 1 -and [regex]::Matches($ServerLog, ('COMBAT_RESPAWN owner=' + $infectedOwner + '\b')).Count -eq 1) 'Combate: jogador infectado morreu e reapareceu humano exatamente uma vez.'
    foreach ($entry in @(@{ Name='A'; Log=$ClientALog }, @{ Name='B'; Log=$ClientBLog })) {
        & $Assert ($entry.Log -match 'COMBAT_ZOMBIE_DEATH_OBSERVED\b' -and $entry.Log -match 'COMBAT_INFECTION_OBSERVED\b') "Combate: cliente $($entry.Name) recebeu zombie morto e nova infeccao replicados."
        & $Assert ($entry.Log -match ('COMBAT_DEATH_OBSERVED owner=' + $infectedOwner + '\b[\s\S]*COMBAT_RESPAWN_OBSERVED owner=' + $infectedOwner + '\b')) "Combate: cliente $($entry.Name) recebeu morte seguida de respawn do mesmo jogador."
    }
}
