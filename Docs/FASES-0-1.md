# Registro das fases 0 e 1

Data inicial: 21/09/2026. Estado: fases 0 e 1 de conexão concluídas. Builds Windows/Linux, compilação de exportação limpa e conexão de dois clientes Windows ao servidor Linux no WSL aprovados. Avisos não fatais registrados abaixo continuam pendentes de investigação.

## Preparado

- Repositório novo em `C:\Dev\Ferrugem`, separado da pasta sincronizada pelo OneDrive.
- Git inicializado. Remoto `origin`: `https://github.com/kaueramone/ferrugem.git`; na verificação inicial, fetch e ls-remote confirmaram repositório remoto vazio. Branch desta fundação: `feat/unity-foundation`.
- Git LFS ativado localmente; padrões para arte binária em `.gitattributes`.
- `.gitignore` para caches do Unity, arquivos de IDE, builds e segredos locais.
- Cópia dos seis documentos fundadores em `Docs/Planejamento`.
- Nome FERRUGEM provisório; uso comercial ainda não validado.
- Perfil do desenvolvedor: iniciante em Unity e C#; instruções devem explicar os passos necessários.
- Ferramentas identificadas na verificação da equipe: Unity Hub 3.14, Git LFS 3.7.1 e Visual Studio Build Tools 2022.

## Editor instalado e licença resolvida

Unity 6000.3.24f1 e Linux Dedicated Server Build Support foram instalados pelo Hub CLI, lado a lado com 6000.2.2f1. A instalação terminou com All Tasks Completed Successfully. As variações Linux server Mono e IL2CPP foram encontradas no Editor. Logs locais em `Logs/Setup` são ignorados pelo Git. Após o usuário instalar Ubuntu-24.04 no WSL 2, o agente corrigiu o serviço WSL com autorização para prosseguir com a validação.

A primeira execução do Editor em batchmode tentou criar somente o projeto descartável `C:/Dev/Ferrugem-Validation` e encerrou com `No valid Unity Editor license`. Após o usuário ativar sua licença no Hub, a nova execução registrou `Successfully resolved entitlement details`. A ativação foi realizada pelo usuário; não houve aceitação de licença em seu nome pela equipe de desenvolvimento.

O projeto real foi criado a partir do template URP. A importação e a compilação mínima passaram com URP 17.3.0, Entities 1.4.8, Netcode for Entities 1.14.2 e Transport 2.7.4. `ProjectVersion.txt` confirma Unity 6000.3.24f1; manifesto e lock concordam com essas versões. A primeira tentativa com Entities 1.4.4 falhou por `CS0234` em `System.IO.Hashing`; o patch 1.4.8 remove essa dependência. Ver [ADR-001](ADR-001-UNITY-E-PACOTES.md).

Na verificação Linux inicial, os recursos Windows Subsystem for Linux e VirtualMachinePlatform estavam habilitados, mas `WSLService` estava parado e com inicialização desativada. `wsl --status` e `wsl --list --verbose` falharam com `Wsl/0x80070422`; naquela ocasião não foi possível confirmar uma distribuição executável. Após a instalação de Ubuntu-24.04 pelo usuário e a correção do serviço pelo agente, o servidor iniciou sem GPU e passou nos cenários com dois clientes Windows e reconexão registrados abaixo.

## Fase 0: concluída

Evidência local: `Logs/phase0-repeat.log`, linha 1716, registra `PHASE0_COMPILE_OK editor=6000.3.24f1 netcode=Unity.NetCode`. Linhas 1743–1744 confirmam encerramento batchmode com retorno 0. Esses logs são locais e ignorados pelo Git; a compilação deverá ser repetida ao validar uma cópia limpa na fase 1. Este aceite cobre preparação, importação e compilação mínima; não comprova funcionamento multiplayer.

| Item | Estado |
| --- | --- |
| Cliente Windows / servidor Linux dedicado | Alvos definidos |
| Editor e licença ativa | 6000.3.24f1 instalado; nova execução resolveu os direitos de licença |
| Módulos de build Windows e Linux Dedicated Server | Presentes; ambos os builds gerados e execução Linux com clientes Windows aprovada |
| URP / Entities / Netcode for Entities | Versões fixadas no manifesto e lock; importação e compilação mínima aprovadas |
| Exemplo oficial correspondente ao pacote instalado | Adaptações HelloNetcode compiladas; atribuição e Unity Companion License registradas em `Assets/ThirdParty/HelloNetcode` |
| Destruição inicial | Proposta: árvore com estados, parede modular e cratera limitada; implementação em fase posterior |

## Fase 1: conexão concluída

O build dedicado Linux foi gerado: `Logs/build-linux-server.log:6189` registra `BUILD_OK target=StandaloneLinux64 subtarget=Server`; a linha 6221 confirma código de saída 0. Executável: `Builds/LinuxServer/FerrugemServer.x86_64`. Gerar esse executável no Windows não comprova sua execução em Linux.

Após o build Linux, o alvo do Editor original foi restaurado para Windows/Player e a validação registrou `PHASE0_COMPILE_OK`. Essa execução permaneceu parada em `Cleanup mono` durante o encerramento e foi interrompida após salvar; não é contabilizada como encerramento normal com código 0. Os builds Windows e Linux aprovados têm seus próprios logs de encerramento normal. O lock temporário residual dessa interrupção foi removido somente após confirmar ausência do Editor e acesso exclusivo ao arquivo.

### Verificações Windows concluídas

`Logs/build-windows.log` registra `BUILD_OK target=StandaloneWindows64 subtarget=Player` e encerramento normal. Saída: `Builds/Windows/Ferrugem.exe`.

O teste independente `Tools/Test-NetworkSmoke.ps1` passou em 21/09/2026. Evidência local: `Logs/Smoke/20260921-182654-165/result.json` e logs da mesma pasta. Resultados:

- Um processo servidor em `127.0.0.1:17979` e dois processos clientes distintos, com mundos exclusivos por papel.
- Cliente B desconectou e reconectou no mesmo processo, registrando `CYCLE_COMPLETE`; o servidor voltou à contagem de dois clientes e permaneceu ativo.
- Logs do caminho positivo sem exceções runtime, e clientes encerraram normalmente.
- Cliente com protocolo 2 rejeitado por `BadProtocolVersion`, sem entrar no jogo nem elevar a contagem de aprovados para três.
- Porta zero, IPv4 inválido, valor de porta ausente e papéis conflitantes rejeitados com código 2 antes de iniciar listener ou conexão alternativa.

O processo servidor deste teste usa o executável Windows no modo `--server`, não o build dedicado Linux. A cena é um diagnóstico de conexão, sem gameplay ou replicação dos objetos visuais. Ver [comandos e argumentos](COMO-TESTAR.md).

### Servidor dedicado Linux com clientes Windows

Comando aprovado: `Testar-Servidor-Linux.bat -Smoke`. Evidência: `Logs/LinuxSmoke/20260921-214016-768/result.json`, resultado `PASS`, 14 verificações, código de saída 0. Ambiente: Ubuntu-24.04, WSL 2, IPv4 privado `172.31.82.70`, porta UDP 17981. O endereço foi descoberto pelo teste, não fixado no launcher.

- Servidor Linux executado sem GPU; dois processos cliente Windows aprovados na conexão.
- Cliente B desconectou e reconectou no mesmo processo; o servidor voltou à contagem de dois clientes e permaneceu ativo.
- Protocolo 2 rejeitado explicitamente, sem entrada no jogo ou contagem como terceiro cliente aprovado.
- Clientes encerraram normalmente e seus logs finais não apresentaram exceções; limpeza do processo Linux limitada ao PID e identidade criados pelo teste.

O build Linux desta execução registrou `BUILD_OK` em `build-linuxserver.log:3397` e saída 0 em `:3431`, dentro da pasta do teste. O encerramento do Editor demorou em `Cleanup mono`, mas terminou naturalmente: não foi interrompido nem tratado como sucesso após ser morto.

A revisão alterou o limite de 60 FPS para ser aplicado somente ao cliente; o servidor fica sob controle de ticks do Netcode. A regressão Windows passou nas 23 verificações: `Logs/Smoke/20260921-213747-769/result.json`.

### Pendências não fatais e limites do aceite

Mesmo após restringir o limite de FPS ao cliente, `server.log:79` ainda registra o aviso `Expected server to always update once per frame when in sleep mode. Ran 0 steps`. Sua causa permanece em investigação. No encerramento, `server.log:472` registra `Leak Detected : Persistent allocates 4 individual allocations`; não há evidência suficiente para atribuir a origem dessas alocações ou afirmar ausência de vazamento.

Os avisos não impediram os cenários de conexão aprovados, mas devem ser investigados antes de operação prolongada ou produção. O teste no WSL não representa homologação de VPS, desempenho pela internet ou capacidade para 100 jogadores. A fase de conexão não inclui movimento, combate, zombies, destruição ou economia.

### Validação por exportação limpa

A primeira exportação, `C:/Dev/Ferrugem-CleanValidation-20260921`, ocorreu durante um build Linux e capturou referências de preload e um asset temporário criado pelos preprocessadores dos pacotes Input System/Netcode. Essa tentativa foi interrompida e **não conta como aceite**. A pasta foi preservada para diagnóstico.

O exportador passou a recusar origem com Editor aberto ou lock do Unity. O asset temporário `Assets/netcode-build-assets-temp` também foi excluído no `.gitignore`. Após fechar o Editor da origem, uma nova exportação em `C:/Dev/Ferrugem-CleanFinal-20260921` copiou 103 arquivos elegíveis pelo Git, sem `Library`, builds ou assets temporários, e com `ProjectSettings` idêntico à origem estável. Trata-se de uma exportação dos arquivos elegíveis pelo Git, não de um clone de commit.

Essa cópia foi importada e compilada com sucesso: `C:/Dev/Ferrugem-CleanFinal-20260921/Logs/build-clean-windows.log:6163` registra `PHASE0_COMPILE_OK`, a linha 14417 registra `BUILD_OK`, e a linha 14449 confirma encerramento normal em código 0. Todos os arquivos em `Assets`, `Packages` e `ProjectSettings` foram comparados por SHA-256 após o build: nenhuma diferença nem arquivo extra em relação à origem.

O smoke test foi executado novamente usando **o executável dessa cópia limpa**, passando as 23 verificações dos cenários positivo, protocolo incompatível e argumentos inválidos. Evidência: `Logs/Smoke/20260921-185039-854/result.json`. O teste não reutilizou `Library` nem o executável da origem.

O aceite acima se baseia na execução dos cenários e nos logs, além dos builds. As pendências de operação prolongada e capacidade permanecem registradas, mesmo com a fase de conexão concluída.

## Política de execução do servidor

Os comandos implementados estão em [COMO-TESTAR.md](COMO-TESTAR.md). Os testes locais Windows usam loopback; o teste WSL usa seu IPv4 privado, sem publicação de portas na internet. Endereço, porta, protocolo e logs podem ser informados explicitamente. Clientes com versão de protocolo incompatível são rejeitados antes de entrar no jogo. Validação de ações de gameplay será acrescentada quando essas ações existirem.
