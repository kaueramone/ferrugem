# Registro de participação de IA

Este projeto utiliza IA como assistência ao planejamento e ao desenvolvimento. Uma pessoa continua responsável por revisar resultados, licenças, segurança e qualidade antes de distribuir o jogo.

| Data | Trabalho | Participação | Situação |
| --- | --- | --- | --- |
| 21/09/2026 | Lore, arquitetura, plano, infraestrutura e publicação | Documentos preparados com assistência de IA | Propostas iniciais copiadas para `Planejamento`; validação técnica pendente |
| 21/09/2026 | Fundação local do repositório | README, regras Git/LFS e registro de fases preparados com assistência de IA | Arquivos criados; nenhum build executado nesta etapa |
| 21/09/2026 | Programação da fundação multiplayer | IA escreveu/adaptou código C# de bootstrap, seleção de papel, protocolo, monitor de conexão, interface de diagnóstico e automação de builds; revisão técnica independente por outro agente | Compilação Unity aprovada; builds Windows e Linux gerados; teste Windows com dois clientes, reconexão e rejeição de protocolo aprovado. Naquele marco inicial a execução Linux estava pendente; sua validação posterior está registrada abaixo |
| 21/09/2026 | Scripts e documentação de testes | IA escreveu scripts PowerShell de smoke test e exportação limpa, documentou comandos e decisões de versões | Smoke Windows aprovado na origem e no executável compilado de exportação limpa; exportador corrigido para impedir captura de assets transitórios durante build |
| 21/09/2026 | Adaptação de exemplo oficial | Padrões de HelloNetcode adaptados com assistência de IA | Origem e Unity Companion License registradas em `Assets/ThirdParty/HelloNetcode`; isso não transfere autoria do exemplo oficial para o projeto |
| 21/09/2026 | Launcher e validação Linux/WSL | IA criou launcher, extraiu helpers compartilhados de build, implementou cleanup por identidade de processo e revisou o limite de FPS do servidor | Smoke Linux aprovado em 14 verificações com dois clientes Windows; regressão Windows aprovada em 23. Avisos de temporização inicial e quatro alocações persistentes no encerramento continuam em investigação |
| 21/09/2026 | Marco 2A FPS | IA escreveu/adaptou simulação de movimento, criação e remoção dos personagens de rede, câmera FPS, apresentação, importação do modelo e testes automatizados | Windows `-Smoke -Fps` aprovado em 36 verificações e Linux em 27; teste manual confirmado pelo usuário e encerramento limpo em cinco verificações. Servidor Linux passa a ser o padrão dos lançadores. Combate e zombies não foram implementados neste marco |
| 21/09/2026 | Modelo civil e animações de terceiros | IA pesquisou fontes e integrou `Male_LongSleeve` do Animated Men Pack, aplicando materiais sóbrios e escala de apresentação | Modelo e animações são de Quaternius, sob CC0; arquivos originais, licença e procedência preservados em `Assets/ThirdParty/QuaterniusAnimatedMen`. Não são arte gerada por IA pelo Ferrugem |

Até o marco 2A, não foram gerados modelos, texturas ou áudio por IA. A cena utiliza primitivas do Unity, recursos do template URP e o civil de Quaternius; o código de apresentação e as escolhas de materiais tiveram assistência de IA. A revisão independente citada acima também foi feita por IA; não representa aprovação humana do produto. O usuário realizou a ativação da licença Unity, criou o repositório remoto e confirmou os testes de conexão anteriores.

Ao incorporar arte, áudio, texto ou outro conteúdo ao produto, adicionar origem, ferramenta, revisão humana e evidência de licença. Este registro não representa uma declaração submetida à Steam; o questionário aplicável será preenchido na preparação da publicação.

### Marco 2B — testes automáticos aprovados

Em 21/09/2026, IA assistiu a implementação da simulação de combate, regras dos
infectados, HUD e apresentação de arma, carga e barril com primitivas Unity. O
modelo civil existente foi reaproveitado visualmente para o infectado, com cores
dessaturadas e antecipação provisória de ataque. O arquivo FBX original permanece
inalterado; modelo e animações continuam sendo de Quaternius (CC0).

A geometria provisória criada por código com assistência de IA faz parte desta
nova apresentação. Não foram gerados novos arquivos de imagem, textura, áudio ou
FBX por um modelo generativo. Isso atualiza o escopo do registro anterior, sem
atribuir autoria de assets de terceiros à IA ou ao Ferrugem. A versão 0.2.0 passou em 51 verificações de combate Linux (`Logs/LinuxSmoke/20260921-224225-102/result.json`) e 27 de regressão FPS (`Logs/LinuxSmoke/20260921-225105-368/result.json`), ambas com saída 0. O teste de combate inclui cenários controlados e inputs de disparo/recarga reais; o playtest manual do marco 2B continua pendente. O aviso conhecido de quatro alocações persistentes do Unity ao encerrar segue em investigação.

A inspeção visual parcial da sessão `Logs/LinuxSmoke/20260921-225230-064`
confirmou HUD, arma, infectados em movimento, barris e captura inicial do cursor
sem disparo. Logs da sessão normal registraram disparos, morte PvP sem infecção e
respawn. A automação de interface parou ao detectar uso humano; a sessão ficou
aberta ao usuário. Isso não equivale à sua aprovação final do combate nem comprova
infecção causada pela perseguição normal de um zombie.

### Planejamento do mapa brasileiro

IA assistiu a redação do briefing de Santa Brasa e a construção por código de uma
planta vetorial original, renderizada também em PNG. Os arquivos em `Docs/Mapas`
são diagramas de planejamento, não assets do jogo, captura da cena Unity ou render
de um mapa já implementado. A planta adota terreno de 8 × 8 km, cidade de 1 km de
diâmetro e bases a 3 km do centro, conforme o esclarecimento de distância e a
ampliação autorizada pelo usuário.

Os links Synty foram usados apenas como referências; nenhum arquivo dos pacotes
foi utilizado e nenhum asset foi comprado. O usuário definiu arte própria para a
produção. O civil temporário CC0 mantém a autoria de Quaternius e não passa a ser
conteúdo gerado por IA. A apresentação futura ao jogador será em PT-BR; a planta
não modifica o campo técnico executável de 40 × 40 m.
