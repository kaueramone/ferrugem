# Registro das fases 0 e 1

Data inicial: 21/09/2026. Estado: fase 0 concluída; fase 1 parcialmente validada em Windows, incluindo compilação e teste de uma exportação limpa. Execução do servidor dedicado Linux ainda pendente.

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

Unity 6000.3.24f1 e Linux Dedicated Server Build Support foram instalados pelo Hub CLI, lado a lado com 6000.2.2f1. A instalação terminou com All Tasks Completed Successfully. As variações Linux server Mono e IL2CPP foram encontradas no Editor. Logs locais em `Logs/Setup` são ignorados pelo Git. O serviço WSL está desabilitado; a execução do servidor Linux permanece pendente.

A primeira execução do Editor em batchmode tentou criar somente o projeto descartável `C:/Dev/Ferrugem-Validation` e encerrou com `No valid Unity Editor license`. Após o usuário ativar sua licença no Hub, a nova execução registrou `Successfully resolved entitlement details`. A ativação foi realizada pelo usuário; não houve aceitação de licença em seu nome pela equipe de desenvolvimento.

O projeto real foi criado a partir do template URP. A importação e a compilação mínima passaram com URP 17.3.0, Entities 1.4.8, Netcode for Entities 1.14.2 e Transport 2.7.4. `ProjectVersion.txt` confirma Unity 6000.3.24f1; manifesto e lock concordam com essas versões. A primeira tentativa com Entities 1.4.4 falhou por `CS0234` em `System.IO.Hashing`; o patch 1.4.8 remove essa dependência. Ver [ADR-001](ADR-001-UNITY-E-PACOTES.md).

Verificação Linux: os recursos Windows Subsystem for Linux e VirtualMachinePlatform estão habilitados, mas `WSLService` está parado e com inicialização desativada. `wsl --status` e `wsl --list --verbose` falharam com `Wsl/0x80070422`; não foi possível confirmar uma distribuição executável. Não foram alterados serviços nem configurações do sistema nesta verificação.

## Fase 0: concluída

Evidência local: `Logs/phase0-repeat.log`, linha 1716, registra `PHASE0_COMPILE_OK editor=6000.3.24f1 netcode=Unity.NetCode`. Linhas 1743–1744 confirmam encerramento batchmode com retorno 0. Esses logs são locais e ignorados pelo Git; a compilação deverá ser repetida ao validar uma cópia limpa na fase 1. Este aceite cobre preparação, importação e compilação mínima; não comprova funcionamento multiplayer.

| Item | Estado |
| --- | --- |
| Cliente Windows / servidor Linux dedicado | Alvos definidos |
| Editor e licença ativa | 6000.3.24f1 instalado; nova execução resolveu os direitos de licença |
| Módulos de build Windows e Linux Dedicated Server | Presentes; ambos os builds gerados. Execução Linux ainda pendente |
| URP / Entities / Netcode for Entities | Versões fixadas no manifesto e lock; importação e compilação mínima aprovadas |
| Exemplo oficial correspondente ao pacote instalado | Adaptações HelloNetcode compiladas; atribuição e Unity Companion License registradas em `Assets/ThirdParty/HelloNetcode` |
| Destruição inicial | Proposta: árvore com estados, parede modular e cratera limitada; implementação em fase posterior |

## Fase 1: critérios ainda abertos

- Executar o servidor Linux sem GPU e conectar dois clientes independentes.

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

### Validação por exportação limpa

A primeira exportação, `C:/Dev/Ferrugem-CleanValidation-20260921`, ocorreu durante um build Linux e capturou referências de preload e um asset temporário criado pelos preprocessadores dos pacotes Input System/Netcode. Essa tentativa foi interrompida e **não conta como aceite**. A pasta foi preservada para diagnóstico.

O exportador passou a recusar origem com Editor aberto ou lock do Unity. O asset temporário `Assets/netcode-build-assets-temp` também foi excluído no `.gitignore`. Após fechar o Editor da origem, uma nova exportação em `C:/Dev/Ferrugem-CleanFinal-20260921` copiou 103 arquivos elegíveis pelo Git, sem `Library`, builds ou assets temporários, e com `ProjectSettings` idêntico à origem estável. Trata-se de uma exportação dos arquivos elegíveis pelo Git, não de um clone de commit.

Essa cópia foi importada e compilada com sucesso: `C:/Dev/Ferrugem-CleanFinal-20260921/Logs/build-clean-windows.log:6163` registra `PHASE0_COMPILE_OK`, a linha 14417 registra `BUILD_OK`, e a linha 14449 confirma encerramento normal em código 0. Todos os arquivos em `Assets`, `Packages` e `ProjectSettings` foram comparados por SHA-256 após o build: nenhuma diferença nem arquivo extra em relação à origem.

O smoke test foi executado novamente usando **o executável dessa cópia limpa**, passando as 23 verificações dos cenários positivo, protocolo incompatível e argumentos inválidos. Evidência: `Logs/Smoke/20260921-185039-854/result.json`. O teste não reutilizou `Library` nem o executável da origem.

Não fechar a fase por existir código ou um build isolado. Registrar caminho dos logs, versão do sistema, comandos usados e resultado observado. Atualizar este documento conforme evidências forem produzidas.

## Política de execução do servidor

Os comandos implementados estão em [COMO-TESTAR.md](COMO-TESTAR.md). Os testes iniciais usam somente loopback, sem exposição à internet. Endereço, porta, protocolo e logs podem ser informados explicitamente. Clientes com versão de protocolo incompatível são rejeitados antes de entrar no jogo. Validação de ações de gameplay será acrescentada quando essas ações existirem.
