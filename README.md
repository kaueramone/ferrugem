# FERRUGEM — O Último Entreposto

Nome provisório. Jogo multiplayer PvPvE de três grupos de sobreviventes, captura de zona central e transporte de recursos. Planejado em Unity com participação de IA no desenvolvimento e revisão humana.

**Estado desta fundação: fases 0 e 1 de conexão concluídas.** Dois clientes Windows conectaram ao servidor dedicado Linux no WSL, com reconexão e rejeição de protocolo incompatível validadas. A compilação limpa e o teste local Windows também passaram. Há avisos de temporização e alocação no log Linux a investigar; capacidade para 100 jogadores, gameplay e hospedagem em VPS ainda não foram demonstrados.

## Para começar

- Para testar no Windows, feche o Unity e dê dois cliques em **`Testar-Ferrugem.bat`**. O launcher verifica os arquivos do projeto, compila quando necessário e abre dois clientes com um servidor local. Feche as duas janelas para encerrar o teste. Ainda é uma cena de diagnóstico de conexão, sem gameplay.
- Para testar os clientes Windows contra o servidor dedicado Linux no WSL, use **`Testar-Servidor-Linux.bat`**. O modo padrão é automático; `-Manual` abre duas janelas. É necessário Ubuntu-24.04 funcionando no WSL 2. Consulte os [comandos e resultados](Docs/COMO-TESTAR.md).
- A pasta de trabalho é `C:\Dev\Ferrugem`, fora do OneDrive para evitar sincronização dos arquivos temporários do Unity.
- O [planejamento fundador](Docs/Planejamento/README.md) explica o jogo. O [registro das fases 0 e 1](Docs/FASES-0-1.md) separa decisões e validações pendentes.
- [Como testar](Docs/COMO-TESTAR.md) contém os comandos de build, teste automatizado e conexão manual.
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
