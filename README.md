# FERRUGEM — O Último Entreposto

Nome provisório. Jogo multiplayer PvPvE de três grupos de sobreviventes, captura de zona central e transporte de recursos. Planejado em Unity com participação de IA no desenvolvimento e revisão humana.

**Estado: fases 0 e 1 de conexão concluídas; marco 2A FPS validado; marco 2B de combate com testes automáticos aprovados.** O campo de testes agora possui movimentação em primeira pessoa e personagem civil visível para o outro cliente. O teste FPS com servidor Linux passou em 27 verificações; o teste anterior de diagnóstico com servidor Windows passou em 36. O usuário confirmou o teste manual do FPS. A versão 0.2.0 passou em 51 verificações de combate e 27 de regressão FPS contra o servidor Linux; uma inspeção visual parcial confirmou HUD, arma, infectados e barris. O playtest completo de combate pelo usuário continua pendente. Veículos, destruição, capacidade para 100 jogadores e hospedagem em VPS continuam pendentes. Consulte os [marcos da fase 2](Docs/FASE-2.md) para escopo e evidências.

## Para começar

- Para testar, feche o Unity e dê dois cliques em **`Testar-Ferrugem.bat`**. O launcher verifica os arquivos do projeto, compila quando necessário e abre dois clientes Windows conectados ao servidor dedicado Linux no WSL. Use WASD, Shift e mouse; Esc libera o cursor. No marco 2B, clique dispara, R recarrega e G lança uma carga explosiva. Feche as duas janelas para encerrar o teste. É necessário Ubuntu-24.04 funcionando no WSL 2.
- **`Testar-Ferrugem.bat -Smoke -Fps`** executa os testes automáticos FPS no mesmo servidor Linux. O launcher **`Testar-Servidor-Linux.bat`** continua disponível; seu padrão é automático e `-Manual` abre as janelas. Servidor Windows fica apenas como diagnóstico opcional. Consulte os [comandos e resultados](Docs/COMO-TESTAR.md).
- A pasta de trabalho é `C:\Dev\Ferrugem`, fora do OneDrive para evitar sincronização dos arquivos temporários do Unity.
- O [planejamento fundador](Docs/Planejamento/README.md) explica o jogo. O [registro das fases 0 e 1](Docs/FASES-0-1.md) separa decisões e validações pendentes.
- [Como testar](Docs/COMO-TESTAR.md) contém os comandos de build, teste automatizado e conexão manual.
- [Assets do protótipo](Docs/ASSETS-PROTOTIPO.md) registra o modelo civil gratuito, autoria, licença e direção low poly sóbria.
- O Editor deste projeto é **Unity 6000.3.24f1**. A licença foi resolvida pelo Editor após a ativação feita pelo usuário. Não abra outra instância sobre esta pasta durante a importação ou compilação automatizada.
- `Assets` guarda cenas e código; `Packages` fixa dependências; `ProjectSettings` guarda configurações. `Library` é um cache local, recriado pelo Editor.
- Git registra alterações locais. Git LFS armazena versões de arquivos de arte grandes; ele está ativado somente neste repositório. O remoto `origin` aponta para [kaueramone/ferrugem](https://github.com/kaueramone/ferrugem), e a branch desta fundação é `feat/unity-foundation`.

## Decisões de partida

Cliente Windows e servidor dedicado Linux. O projeto foi criado a partir do template URP do Unity 6000.3.24f1, e o módulo Linux Dedicated Server está instalado. URP 17.3.0, Entities 1.4.8, Netcode for Entities 1.14.2 e Transport 2.7.4 passaram pela importação, compilação e teste de conexão. A [decisão de versões](Docs/ADR-001-UNITY-E-PACOTES.md) registra a escolha dos pacotes; o [registro das fases](Docs/FASES-0-1.md) contém resultados e pendências.

O primeiro aceite técnico foi demonstrado com executáveis independentes: servidor dedicado Linux e dois clientes Windows capazes de conectar, desconectar e reconectar.

## Regras de trabalho

- Não atualizar Editor ou pacotes automaticamente. Registrar a mudança e repetir builds antes de adotar a nova versão.
- Manter `ProjectVersion.txt`, `manifest.json`, `packages-lock.json`, configurações e arquivos `.meta` no Git assim que forem criados. Serializar cenas e prefabs como texto.
- O servidor decide estado de jogo. Clientes enviam intenções; o servidor valida permissões, valores, frequência e sequência dos comandos. Dano, recursos, equipes e placar não podem ser aceitos como verdade enviada pelo cliente.
- Não guardar senhas, tokens ou chaves no projeto. Não expor um servidor à internet antes de concluir os testes locais e documentar a configuração.
- Registrar conteúdo gerado por IA, origem e licença dos assets utilizados. Ver [registro de participação de IA](Docs/REGISTRO-IA.md).

Os documentos em `Docs/Planejamento` são a cópia da proposta original de 21/09/2026. Seus números são hipóteses e serão revistos com medições.
