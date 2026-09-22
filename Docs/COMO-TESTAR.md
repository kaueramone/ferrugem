# Testes da fundação e do protótipo FPS

Use PowerShell na pasta `C:\Dev\Ferrugem`. Os resultados da conexão ficam em [FASES-0-1.md](FASES-0-1.md); movimentação e presença dos personagens ficam em [FASE-2.md](FASE-2.md). Uma execução com saída 0 não prova conexão multiplayer sem os eventos nos logs.

## Dois cliques para testar

Feche o Unity deste projeto e dê dois cliques em **`Testar-Ferrugem.bat`**, na raiz do repositório. O console permanece aberto enquanto o launcher:

1. Confere a versão do Editor indicada em `ProjectSettings/ProjectVersion.txt`.
2. Verifica o conteúdo de `Assets`, `Packages` e `ProjectSettings`. Na primeira execução, ou quando esses arquivos mudarem, gera os builds atuais do cliente Windows e servidor Linux.
3. Inicia o servidor Linux no Ubuntu-24.04/WSL 2, descobre seu IPv4 privado e espera a confirmação de escuta na porta UDP 17981.
4. Abre duas janelas de cliente, em 960 × 540, conectadas ao mesmo servidor.

Clique na janela para capturar o mouse: WASD movimenta, Shift corre, mouse olha e Esc libera o cursor. Alterne entre as janelas para observar o outro personagem. Use os botões de desconexão/reconexão com o cursor livre. Feche **as duas janelas do jogo** para encerrar o servidor e finalizar o teste. Mantenha o console aberto durante a sessão. O campo tem 40 × 40 m e dois obstáculos. O marco 2B acrescenta clique para disparar, R para recarregar e G para lançar carga; sua validação está registrada separadamente abaixo. Na versão 0.3.0, Ctrl agacha, Espaço pula e botão direito mira; há rampa, escada e passagem baixa. Veículos permanecem pendentes.

O launcher só reutiliza um build quando existe um registro de validação compatível com os arquivos atuais e com os hashes do executável/código compilado. Os registros `.verified-build.json` das pastas de build são criados apenas após compilação bem-sucedida, com código de saída 0; são ignorados pelo Git. A simples existência de `Ferrugem.exe` não é suficiente. A primeira compilação pode demorar; logs da sessão Linux ficam em `Logs/LinuxSmoke/<data-hora>`.

Se o Editor estiver usando o projeto, o launcher pede para fechá-lo. Não encerra o Editor do usuário nem remove locks. Uma porta 17981 já ocupada no servidor também interrompe a abertura do teste. Os processos encerrados automaticamente são somente os iniciados pelo próprio launcher.

Para executar o teste automático completo, sem abrir as janelas do jogo nem pausar o `.bat` no final:

```powershell
.\Testar-Ferrugem.bat -Smoke
```

Para incluir as verificações do personagem FPS e movimento automático, mantendo o servidor Linux:

```powershell
.\Testar-Ferrugem.bat -Smoke -Fps
```

Esse modo verifica movimento observado pelo servidor e pelo outro cliente, identidade
local, remoção na desconexão e ausência de duplicação na reconexão. Não substitui
olhar a imagem e testar os controles manualmente. Sem `-Fps`, o smoke mantém o
contrato de conexão da fase 1. `-Fps` é um complemento do modo automático; o teste
manual já possui os personagens controláveis normalmente.

Para forçar uma compilação mesmo quando o registro está atual, use `-ForceBuild`. Se o Editor foi instalado fora da pasta padrão do Hub, informe `-EditorPath "D:\Unity\6000.3.24f1\Editor\Unity.exe"`; a versão ainda será conferida. Esses parâmetros podem ser combinados com `-Smoke`.

Histórico anterior à mudança de padrão: em 21/09/2026, `Testar-Ferrugem.bat -Smoke` ainda iniciava servidor Windows e passou nas 23 verificações, retornando código 0 (`Logs/Smoke/20260921-191513-863/result.json`). O uso normal agora inicia servidor Linux; não é necessário testar ambos os servidores em toda alteração.

## Servidor Linux no WSL

Com a distribuição **Ubuntu-24.04 no WSL 2** funcionando, execute:

```powershell
.\Testar-Servidor-Linux.bat -Smoke
```

Para testar também a movimentação FPS contra o servidor Linux:

```powershell
.\Testar-Servidor-Linux.bat -Smoke -Fps
```

Esse também é o modo padrão do novo `.bat`, sem argumentos. Ele verifica os builds Windows e Linux com as mesmas rotinas de hash e registro de validação do launcher Windows, recompilando sequencialmente quando necessário. O módulo Linux Dedicated Server do Editor deve estar instalado. O teste descobre o IPv4 privado atual do WSL e conecta os clientes Windows a esse endereço, na porta UDP 17981; não presume encaminhamento UDP por `localhost`.

O teste exige dois clientes Windows conectados ao servidor Linux sem GPU, desconexão e reconexão de um deles, retorno da contagem de dois clientes no servidor e rejeição de protocolo incompatível. Logs e `result.json` ficam em `Logs/LinuxSmoke/<data-hora>`. O resultado pode incluir avisos de encerramento do Unity, separados das verificações de conexão. Consulte o registro das fases para os resultados efetivamente observados.

Para abrir duas janelas e jogar livremente:

```powershell
.\Testar-Servidor-Linux.bat -Manual
```

Esse é o mesmo modo usado pelo duplo clique em `Testar-Ferrugem.bat`. O modo
manual conecta os dois jogadores e espera as janelas fecharem; não desconecta
automaticamente o segundo jogador nem abre um terceiro cliente incompatível.
Esses cenários fazem parte exclusivamente do smoke automático.

Feche as duas janelas ao terminar. A sessão manual tem limite de 30 minutos; o smoke usa servidor com limite de 180 segundos e esperas limitadas em cada etapa. `-Distribution` seleciona explicitamente outra distribuição instalada; `-Port` altera a porta. `-ForceBuild` e `-EditorPath` funcionam como no launcher Windows.

No modo manual, os controles e o mapa são os mesmos do Windows: clique para capturar
o mouse, WASD, Shift e Esc. Verifique se o outro civil se move, se os obstáculos
bloqueiam a passagem e se desconectar/reconectar remove e recria apenas um corpo.

O encerramento atua somente sobre o PID Linux criado pelo teste, conferindo também a identidade temporal do processo para evitar atingir um PID reutilizado. O launcher não encerra a distribuição WSL nem outros servidores. Um launcher já aberto para este projeto impede iniciar outro simultaneamente.

Esse teste usa Linux real dentro do WSL, no mesmo computador dos clientes. Ele não mede latência de internet, capacidade para 100 jogadores, configuração de firewall de uma VPS ou estabilidade de operação prolongada. A homologação de uma VPS continua sendo uma etapa separada.

Validação em 21/09/2026: `Testar-Servidor-Linux.bat -Smoke` passou nas 14 verificações e retornou código 0 em Ubuntu-24.04/WSL 2. Evidência: `Logs/LinuxSmoke/20260921-214016-768/result.json`. O log do servidor ainda contém aviso inicial de `Ran 0 steps` e aviso de quatro alocações persistentes no encerramento (`Leak Detected`); ambos permanecem em investigação e não impediram o teste de conexão. A regressão do launcher Windows também passou nas 23 verificações em `Logs/Smoke/20260921-213747-769/result.json`.

Validação FPS em 21/09/2026: `Testar-Servidor-Linux.bat -Smoke -Fps` passou em
**27 verificações**, com saída 0. Evidência:
`Logs/LinuxSmoke/20260921-221702-121/result.json`. O usuário confirmou o teste manual do marco 2A; encerramento limpo em `Logs/LinuxSmoke/20260921-222757-885/result.json` (cinco verificações). Esse é o alvo normal dos testes seguintes.

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

## Diagnóstico opcional com servidor Windows

O produto usa servidor dedicado Linux. Manter o servidor Windows como recurso
de diagnóstico ajuda a isolar problemas de WSL ou rede, sem torná-lo requisito
de cada teste. Para verificar/recompilar e testar esse caminho explicitamente:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Tools\Start-LocalTest.ps1 -Smoke -Fps
```

Depois de gerar o cliente Windows:

```powershell
.\Tools\Test-NetworkSmoke.ps1
```

O script usa somente `127.0.0.1`, porta UDP 17979, e executa um processo servidor Windows e dois clientes independentes sem janela. Um cliente desconecta e reconecta no mesmo processo. O teste exige eventos de conexão, desconexão, `CYCLE_COMPLETE` e a contagem de dois clientes no servidor. Também verifica rejeição de protocolo incompatível e de argumentos inválidos. Processos têm tempo limite; o script encerra somente os que iniciou. Se a porta estiver ocupada, use `-Port 17980`.

O resultado é `SMOKE_PASS` ou uma falha explícita. Logs e `result.json` ficam em `Logs\Smoke\<data-hora>`, ignorados pelo Git. Este teste usa o cliente Windows no modo `--server`; **não substitui executar o build dedicado Linux**.

## Diagnóstico manual com servidor Windows

Para o teste normal com servidor Linux, use dois cliques em `Testar-Ferrugem.bat`.
Os comandos abaixo são somente a alternativa de diagnóstico Windows.

Em um terminal, inicie o servidor Windows de teste:

```powershell
.\Builds\Windows\Ferrugem.exe -batchmode -nographics --server --address 127.0.0.1 --port 17979 -logFile C:\Dev\Ferrugem\Logs\manual-server.log
```

Em outro terminal, abra o cliente com o campo FPS e a interface de conexão:

```powershell
.\Builds\Windows\Ferrugem.exe --client --address 127.0.0.1 --port 17979 -logFile C:\Dev\Ferrugem\Logs\manual-client.log
```

O botão da interface permite desconectar e reconectar; pressione Esc para liberar o cursor. Para um segundo cliente use outro processo e outro caminho de log. Há personagem FPS e corpo remoto; combate, infectados e destruição dos objetos ainda pertencem aos próximos marcos.

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

## Playtest do marco 2B — após compilar a versão de combate

1. Abra o teste normal pelo `.bat`; aguarde o personagem e a munição aparecerem.
2. Clique fora do HUD para capturar o cursor. O primeiro clique só captura; os
   cliques seguintes disparam. Confira o consumo de munição e use R para recarregar.
3. Teste o infectado a distância: corpo não deve matá-lo, cabeça deve. O indicador
   de acerto vem do servidor e pode demorar conforme a latência.
4. Aproxime-se de um infectado para observar a antecipação vermelha do ataque.
   Um ataque válido deve matar e criar um único infectado no local. Confira o
   mesmo resultado na outra janela e aguarde o respawn humano.
5. Use G e afaste-se da carga; confira a explosão nos dois clientes. O barril de
   teste também permite verificar explosão e seu estado após reconexão.
6. Alterne entre as janelas para conferir dano PvP, morte e respawn. Uma morte
   provocada por outro jogador não deve criar um infectado neste recorte.

Este roteiro é um critério de aceite, não uma declaração de testes já realizados.
O marco 2B passou em 51 verificações automáticas Linux na versão 0.2.0, seguido de 27 de regressão FPS; [evidências e limites](FASE-2.md) estão registrados. A inspeção visual parcial foi registrada, e o playtest completo do combate pelo usuário continua pendente. A versão 0.2.0 não tinha áudio nem mãos; a 0.3.0 acrescenta efeitos e mãos procedurais provisórios, ainda sem arte final.

Teste automático específico de combate contra o servidor Linux:

```powershell
.\Testar-Ferrugem.bat -Smoke -Combat
```

Execute separadamente de `-Smoke -Fps`. O modo de combate envia disparo/recarga
reais sem depender do foco da janela e verifica um cenário de dano/infecção
injetado no servidor. A perseguição normal dos zombies fica fora desse cenário;
os passos manuais acima continuam necessários. Não use `-Combat` no modo manual:
o combate normal já estará disponível nas duas janelas.

## Refinamento 0.3.0

Consulte [controles, áudio, critérios de inspeção e simulação de rede](REFINAMENTO-FPS.md).
O modo específico é `Testar-Ferrugem.bat -Smoke -Motor`. Execute os modos `-Motor`,
`-Fps` e `-Combat` separadamente. Na build final 0.3.0, Motor passou em 60 verificações e Combat em 53 com simulador ativo (50 ms e perda de 2% por direção); FPS passou em 27 sem simulador. Isso não substitui os passos manuais nem a escuta dos efeitos.
