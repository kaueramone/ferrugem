# FERRUGEM — O Último Entreposto

Nome provisório. Jogo multiplayer PvPvE de três grupos de sobreviventes, captura de zona central e transporte de recursos. Planejado em Unity com participação de IA no desenvolvimento e revisão humana.

**Estado desta fundação: fase 0 concluída; fase 1 parcialmente validada.** Os builds Windows e Linux foram gerados. O build Windows passou pelo teste de dois clientes e reconexão, inclusive após compilação de uma exportação limpa. Falta executar o servidor dedicado Linux com os clientes Windows. A capacidade de 100 jogadores é uma meta futura, ainda não demonstrada.

## Para começar

- A pasta de trabalho é `C:\Dev\Ferrugem`, fora do OneDrive para evitar sincronização dos arquivos temporários do Unity.
- O [planejamento fundador](Docs/Planejamento/README.md) explica o jogo. O [registro das fases 0 e 1](Docs/FASES-0-1.md) separa decisões e validações pendentes.
- [Como testar](Docs/COMO-TESTAR.md) contém os comandos de build, teste automatizado e conexão manual.
- O Editor deste projeto é **Unity 6000.3.24f1**. A licença foi resolvida pelo Editor após a ativação feita pelo usuário. Não abra outra instância sobre esta pasta durante a importação ou compilação automatizada.
- `Assets` guarda cenas e código; `Packages` fixa dependências; `ProjectSettings` guarda configurações. `Library` é um cache local, recriado pelo Editor.
- Git registra alterações locais. Git LFS armazena versões de arquivos de arte grandes; ele está ativado somente neste repositório. O remoto `origin` aponta para [kaueramone/ferrugem](https://github.com/kaueramone/ferrugem), e a branch desta fundação é `feat/unity-foundation`.

## Decisões de partida

Cliente Windows e servidor dedicado Linux. O projeto foi criado a partir do template URP do Unity 6000.3.24f1, e o módulo Linux Dedicated Server está instalado. URP 17.3.0, Entities 1.4.8, Netcode for Entities 1.14.2 e Transport 2.7.4 passaram pela importação e compilação do exemplo mínimo. A [decisão de versões](Docs/ADR-001-UNITY-E-PACOTES.md) registra a evidência e a correção necessária. Builds e execução multiplayer pertencem à fase 1, ainda aberta.

O primeiro aceite técnico exige cliente compilado, servidor independente e dois clientes capazes de conectar, desconectar e reconectar. Uma cena aberta no Editor não comprova esse aceite.

## Regras de trabalho

- Não atualizar Editor ou pacotes automaticamente. Registrar a mudança e repetir builds antes de adotar a nova versão.
- Manter `ProjectVersion.txt`, `manifest.json`, `packages-lock.json`, configurações e arquivos `.meta` no Git assim que forem criados. Serializar cenas e prefabs como texto.
- O servidor decide estado de jogo. Clientes enviam intenções; o servidor valida permissões, valores, frequência e sequência dos comandos. Dano, recursos, equipes e placar não podem ser aceitos como verdade enviada pelo cliente.
- Não guardar senhas, tokens ou chaves no projeto. Não expor um servidor à internet antes de concluir os testes locais e documentar a configuração.
- Registrar conteúdo gerado por IA, origem e licença dos assets utilizados. Ver [registro de participação de IA](Docs/REGISTRO-IA.md).

Os documentos em `Docs/Planejamento` são a cópia da proposta original de 21/09/2026. Seus números são hipóteses e serão revistos com medições.
