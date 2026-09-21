#!/usr/bin/env bash
set -euo pipefail

if [[ "$1" == "--stop" ]]; then
    record="$2"
    [[ -f "$record" ]] || exit 0
    read -r server_pid server_start < "$record"
    [[ "$server_pid" =~ ^[0-9]+$ && "$server_start" =~ ^[0-9]+$ ]] || exit 2
    same_process() {
        [[ -r "/proc/$server_pid/stat" ]] || return 1
        local current
        current="$(awk '{print $3 " " $22}' "/proc/$server_pid/stat" 2>/dev/null)" || return 1
        [[ "$current" != "Z "* && "${current#* }" == "$server_start" ]]
    }
    if same_process; then
        kill -TERM "$server_pid" 2>/dev/null || true
        for attempt in {1..100}; do
            same_process || exit 0
            sleep 0.1
        done
        if same_process; then kill -KILL "$server_pid" 2>/dev/null || true; fi
        for attempt in {1..20}; do
            same_process || exit 0
            sleep 0.1
        done
        if same_process; then
            printf 'Servidor iniciado pelo teste continua ativo: %s\n' "$server_pid" >&2
            exit 1
        fi
    fi
    exit 0
fi

executable="$1"; address="$2"; port="$3"; log="$4"; record="$5"; duration="$6"
shift 6
chmod u+x "$executable"
printf '%s %s\n' "$$" "$(awk '{print $22}' "/proc/$$/stat")" > "$record"
exec "$executable" -batchmode -nographics --server --address "$address" --port "$port" --quit-after "$duration" -logFile "$log" "$@"
