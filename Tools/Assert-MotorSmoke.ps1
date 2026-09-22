# Regras locais do motor e eventos observados em ghosts reais sao verificados separadamente.
function Assert-MotorSmoke {
    param([string]$ServerLog, [string]$ClientALog, [string]$ClientBLog, [scriptblock]$Assert)
    $allLogs = $ServerLog + $ClientALog + $ClientBLog
    & $Assert ($allLogs -notmatch 'Exception:|MOTOR_SUITE_FAIL|MOTOR_INVALID_POSITION|MOTOR_CHECK[^\r\n]*pass=0|FPS_DUPLICATE|FPS_OUT_OF_BOUNDS|SURVIVOR_ASSET_MISSING|SURVIVOR_ANIMATION_MISSING') 'Motor: nenhum erro runtime ou estado invalido nos tres processos.'
    $requiredChecks = @('acceleration','deceleration','sprint_speed','aim_speed','jump','no_double_jump','land','standing_blocked','tunnel_standing_blocked','crouch_clearance','ceiling_collision','steps','step_too_high','ramp','fall','slope_limit','ramp_ray','crouch_head_height','no_wall_climb')
    foreach ($name in $requiredChecks) {
        & $Assert ([regex]::Matches($ServerLog, ('MOTOR_CHECK name=' + $name + ' pass=1\b')).Count -eq 1) "Motor: fixture do servidor validou $name uma vez."
    }
    $suite = [regex]::Matches($ServerLog, 'MOTOR_SUITE_PASS count=(\d+)\b')
    & $Assert ($suite.Count -eq 1 -and [int]$suite[0].Groups[1].Value -eq $requiredChecks.Count) 'Motor: servidor concluiu as 19 fixtures de movimento e colisao.'
    $ownerPattern = 'MOTOR_OBSERVED world=FerrugemClient owner=(\d+) local=1 event=jump\b'
    $jumpsA = [regex]::Matches($ClientALog, $ownerPattern)
    $jumpsB = [regex]::Matches($ClientBLog, $ownerPattern)
    & $Assert ($jumpsA.Count -ge 1 -and $jumpsB.Count -ge 2) 'Motor: A saltou e B saltou antes e depois de reconectar.'
    $ownerA = $jumpsA[0].Groups[1].Value
    $ownerB = $jumpsB[$jumpsB.Count - 1].Groups[1].Value
    & $Assert ($ownerA -ne $ownerB) 'Motor: A e B possuem identidades distintas.'
    foreach ($event in @('move','jump','land','crouch','aim')) {
        foreach ($owner in @($ownerA,$ownerB)) {
            & $Assert ($ServerLog -match ('MOTOR_OBSERVED world=FerrugemServer owner=' + $owner + ' local=\d event=' + $event + '\b')) "Motor: servidor confirmou $event do jogador $owner."
        }
        & $Assert ($ClientALog -match ('MOTOR_OBSERVED world=FerrugemClient owner=' + $ownerB + ' local=0 event=' + $event + '\b')) "Motor: A recebeu $event remoto de B."
        & $Assert ($ClientBLog -match ('MOTOR_OBSERVED world=FerrugemClient owner=' + $ownerA + ' local=0 event=' + $event + '\b')) "Motor: B recebeu $event remoto de A."
    }
    & $Assert ($ClientBLog -match 'CONNECTION_EVENT[^\r\n]*Disconnected[\s\S]*MOTOR_OBSERVED world=FerrugemClient owner=\d+ local=1 event=jump\b') 'Motor: novo salto local foi observado apos desconexao e reconexao reais.'
}
