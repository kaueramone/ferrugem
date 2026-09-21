# Testes da fundação

Use PowerShell na pasta `C:\Dev\Ferrugem`. Estes comandos correspondem à implementação da fase 1; o resultado observado e os critérios concluídos ficam em [FASES-0-1.md](FASES-0-1.md). Uma execução com saída 0 não prova conexão multiplayer sem os eventos nos logs.

## Gerar executáveis

Feche o Editor deste projeto antes de executar um build em batchmode. Use exatamente Unity 6000.3.24f1. Ajuste somente o caminho do Editor se a instalação estiver em outra pasta.

```powershell
$editor = 'C:\Program Files\Unity\Hub\Editor\6000.3.24f1\Editor\Unity.exe'
New-Item -ItemType Directory -Path C:\Dev\Ferrugem\Logs -Force | Out-Null
$build = Start-Process -FilePath $editor -ArgumentList @('-batchmode', '-quit', '-projectPath', 'C:\Dev\Ferrugem', '-buildTarget', 'Win64', '-standaloneBuildSubtarget', 'Player', '-executeMethod', 'Ferrugem.Editor.ProjectBuild.WindowsClient', '-logFile', 'C:\Dev\Ferrugem\Logs\build-windows.log') -WindowStyle Hidden -Wait -PassThru
if ($build.ExitCode -ne 0) { throw 'Build Windows falhou; consulte Logs\build-windows.log.' }
Select-String -Path C:\Dev\Ferrugem\Logs\build-windows.log -Pattern 'BUILD_OK|return code'
```

A saída do cliente é `Builds\Windows\Ferrugem.exe`. Para gerar o servidor dedicado Linux com o módulo correspondente instalado:

```powershell
New-Item -ItemType Directory -Path C:\Dev\Ferrugem\Logs -Force | Out-Null
$build = Start-Process -FilePath $editor -ArgumentList @('-batchmode', '-quit', '-projectPath', 'C:\Dev\Ferrugem', '-buildTarget', 'Linux64', '-standaloneBuildSubtarget', 'Server', '-executeMethod', 'Ferrugem.Editor.ProjectBuild.LinuxServer', '-logFile', 'C:\Dev\Ferrugem\Logs\build-linux-server.log') -WindowStyle Hidden -Wait -PassThru
if ($build.ExitCode -ne 0) { throw 'Build Linux falhou; consulte Logs\build-linux-server.log.' }
Select-String -Path C:\Dev\Ferrugem\Logs\build-linux-server.log -Pattern 'BUILD_OK|return code'
```

A saída Linux é `Builds\LinuxServer\FerrugemServer.x86_64`. A pasta inteira do build é necessária. Não inicie os dois builds simultaneamente sobre o mesmo projeto. Aguarde o processo terminar e confirme `BUILD_OK` e encerramento sem erro no log antes de usar os executáveis.

Os comandos usam o caminho sem espaços do projeto. Se mudar para um caminho com espaços, inclua aspas duplas literais no respectivo item de `-ArgumentList` (por exemplo, `'"C:\Meus Projetos\Ferrugem"'`). `-FilePath $editor` já aceita o caminho do Editor com espaços.

## Teste automatizado local Windows

Depois de gerar o cliente Windows:

```powershell
.\Tools\Test-NetworkSmoke.ps1
```

O script usa somente `127.0.0.1`, porta UDP 17979, e executa um processo servidor Windows e dois clientes independentes sem janela. Um cliente desconecta e reconecta no mesmo processo. O teste exige eventos de conexão, desconexão, `CYCLE_COMPLETE` e a contagem de dois clientes no servidor. Também verifica rejeição de protocolo incompatível e de argumentos inválidos. Processos têm tempo limite; o script encerra somente os que iniciou. Se a porta estiver ocupada, use `-Port 17980`.

O resultado é `SMOKE_PASS` ou uma falha explícita. Logs e `result.json` ficam em `Logs\Smoke\<data-hora>`, ignorados pelo Git. Este teste usa o cliente Windows no modo `--server`; **não substitui executar o build dedicado Linux**.

## Teste manual com janela

Em um terminal, inicie o servidor Windows de teste:

```powershell
.\Builds\Windows\Ferrugem.exe -batchmode -nographics --server --address 127.0.0.1 --port 17979 -logFile C:\Dev\Ferrugem\Logs\manual-server.log
```

Em outro terminal, abra o cliente com a interface de diagnóstico:

```powershell
.\Builds\Windows\Ferrugem.exe --client --address 127.0.0.1 --port 17979 -logFile C:\Dev\Ferrugem\Logs\manual-client.log
```

O botão da interface permite desconectar e reconectar. Para um segundo cliente use outro processo e outro caminho de log. Ainda não há personagem controlável, combate ou replicação dos objetos de cenário.

## Argumentos implementados

| Argumento | Padrão / uso |
| --- | --- |
| `--client` / `--server` | Cliente por padrão; o executável dedicado Linux força servidor e rejeita `--client` |
| `--address` | IPv4 `127.0.0.1`; no servidor indica interface de escuta, no cliente indica destino |
| `--port` | 7979; intervalo 1–65535 |
| `--protocol` | 1; deve coincidir nos dois lados |
| `--quit-after` | 0, desativado; segundos para encerrar automaticamente |
| `--cycle-after` | 0, desativado; segundos conectado antes do ciclo de desconexão |
| `--reconnect-delay` | 2; segundos antes da tentativa de reconexão automática |

## Importação limpa

```powershell
.\Tools\Export-CleanProject.ps1 -Destination C:\Dev\Ferrugem-CleanCheck
```

O destino deve ser novo e externo ao projeto. Feche o Editor da origem e aguarde qualquer build terminar; o script recusa exportar enquanto há processo ou lock do Unity, pois preprocessadores alteram temporariamente arquivos em `Assets` e `ProjectSettings`. A exportação inclui os arquivos elegíveis pelo Git, inclusive alterações ainda sem commit, e exclui caches e builds. Abra essa cópia com o Editor fixado ou execute o método de validação/build apontando `-projectPath` para ela. A exportação sozinha não comprova compilação limpa.
