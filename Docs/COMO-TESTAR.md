# Testes da fundação

Use PowerShell na pasta `C:\Dev\Ferrugem`. Estes comandos correspondem à implementação da fase 1; o resultado observado e os critérios concluídos ficam em [FASES-0-1.md](FASES-0-1.md). Uma execução com saída 0 não prova conexão multiplayer sem os eventos nos logs.

## Dois cliques para testar

Feche o Unity deste projeto e dê dois cliques em **`Testar-Ferrugem.bat`**, na raiz do repositório. O console permanece aberto enquanto o launcher:

1. Confere a versão do Editor indicada em `ProjectSettings/ProjectVersion.txt`.
2. Verifica o conteúdo de `Assets`, `Packages` e `ProjectSettings`. Na primeira execução, ou quando esses arquivos mudarem, gera o build Windows atual.
3. Inicia um servidor oculto em `127.0.0.1:17979` e espera a confirmação de escuta.
4. Abre duas janelas de cliente, em 960 × 540, conectadas ao mesmo servidor.

Use os botões de desconexão/reconexão da cena. Feche **as duas janelas do jogo** para encerrar o servidor e finalizar o teste. Mantenha o console aberto durante a sessão. Este executável ainda contém apenas diagnóstico de conexão, sem personagem controlável ou combate.

O launcher só reutiliza um build quando existe um registro de validação compatível com os arquivos atuais e com os hashes do executável/código compilado. O registro `Builds/Windows/.verified-build.json` é criado apenas após compilação bem-sucedida, com código de saída 0; ele é ignorado pelo Git. A simples existência de `Ferrugem.exe` não é suficiente. A primeira compilação pode demorar; logs ficam em `Logs/Launcher/<data-hora>`.

Se o Editor estiver usando o projeto, o launcher pede para fechá-lo. Não encerra o Editor do usuário nem remove locks. Uma porta 17979 já ocupada também interrompe a abertura do teste. Os processos encerrados automaticamente são somente os iniciados pelo próprio launcher.

Para executar o teste automático completo, sem abrir as janelas do jogo nem pausar o `.bat` no final:

```powershell
.\Testar-Ferrugem.bat -Smoke
```

Para forçar uma compilação mesmo quando o registro está atual, use `-ForceBuild`. Se o Editor foi instalado fora da pasta padrão do Hub, informe `-EditorPath "D:\Unity\6000.3.24f1\Editor\Unity.exe"`; a versão ainda será conferida. Esses parâmetros podem ser combinados com `-Smoke`.

Validação em 21/09/2026: `Testar-Ferrugem.bat -Smoke` compilou o cliente com encerramento normal do Unity e passou nas 23 verificações, retornando código 0. Evidência local: `Logs/Smoke/20260921-191513-863/result.json`. O launcher usa SHA-256 do .NET para funcionar também no Windows PowerShell 5.1.

## Servidor Linux no WSL

Com a distribuição **Ubuntu-24.04 no WSL 2** funcionando, execute:

```powershell
.\Testar-Servidor-Linux.bat -Smoke
```

Esse também é o modo padrão do novo `.bat`, sem argumentos. Ele verifica os builds Windows e Linux com as mesmas rotinas de hash e registro de validação do launcher Windows, recompilando sequencialmente quando necessário. O módulo Linux Dedicated Server do Editor deve estar instalado. O teste descobre o IPv4 privado atual do WSL e conecta os clientes Windows a esse endereço, na porta UDP 17981; não presume encaminhamento UDP por `localhost`.

O teste exige dois clientes Windows conectados ao servidor Linux sem GPU, desconexão e reconexão de um deles, retorno da contagem de dois clientes no servidor e rejeição de protocolo incompatível. Logs e `result.json` ficam em `Logs/LinuxSmoke/<data-hora>`. O resultado pode incluir avisos de encerramento do Unity, separados das verificações de conexão. Consulte o registro das fases para os resultados efetivamente observados.

Para abrir duas janelas após validar os mesmos cenários:

```powershell
.\Testar-Servidor-Linux.bat -Manual
```

Feche as duas janelas ao terminar. A sessão manual tem limite de 30 minutos; o smoke usa servidor com limite de 180 segundos e esperas limitadas em cada etapa. `-Distribution` seleciona explicitamente outra distribuição instalada; `-Port` altera a porta. `-ForceBuild` e `-EditorPath` funcionam como no launcher Windows.

O encerramento atua somente sobre o PID Linux criado pelo teste, conferindo também a identidade temporal do processo para evitar atingir um PID reutilizado. O launcher não encerra a distribuição WSL nem outros servidores. Um launcher já aberto para este projeto impede iniciar outro simultaneamente.

Esse teste usa Linux real dentro do WSL, no mesmo computador dos clientes. Ele não mede latência de internet, capacidade para 100 jogadores, configuração de firewall de uma VPS ou estabilidade de operação prolongada. A homologação de uma VPS continua sendo uma etapa separada.

Validação em 21/09/2026: `Testar-Servidor-Linux.bat -Smoke` passou nas 14 verificações e retornou código 0 em Ubuntu-24.04/WSL 2. Evidência: `Logs/LinuxSmoke/20260921-214016-768/result.json`. O log do servidor ainda contém aviso inicial de `Ran 0 steps` e aviso de quatro alocações persistentes no encerramento (`Leak Detected`); ambos permanecem em investigação e não impediram o teste de conexão. A regressão do launcher Windows também passou nas 23 verificações em `Logs/Smoke/20260921-213747-769/result.json`.

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
