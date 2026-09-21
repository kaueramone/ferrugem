# ADR-001 — Editor e pacotes da fundação

Data: 21/09/2026. Decisão aceita para a compilação mínima da fase 0; execução multiplayer e builds ainda precisam da fase 1.

## Decisão

Fixar Unity **6000.3.24f1** (revisão `4e7b9b5b6244`) e os pacotes abaixo em `Packages/manifest.json`, preservando `Packages/packages-lock.json` no versionamento.

| Pacote | Versão |
| --- | --- |
| Universal Render Pipeline | 17.3.0 |
| Entities | 1.4.8 |
| Netcode for Entities | 1.14.2 |
| Unity Transport | 2.7.4 |

## Motivo e evidência

A tentativa inicial com Entities 1.4.4 falhou com `CS0234`, namespace `System.IO.Hashing` ausente, em `Unity.Scenes.Editor/Build/RemoteContentCatalogBuildUtility.cs:12`. Evidência local: `Logs/create-project.log:1723`, com encerramento em código 1.

O changelog oficial de Entities 1.4.8, datado de 10/07/2026, registra a remoção das dependências das assemblies `System.IO.Hashing` e `System.Runtime.CompilerServices.Unsafe`. Foi usado esse patch oficial, sem editar código de pacote nem adicionar DLLs manualmente. Fontes: [registry oficial de Entities](https://packages.unity.com/com.unity.entities) e `CHANGELOG.md` do pacote instalado em `Library/PackageCache/com.unity.entities@*/`.

Na importação, o migrador do Editor selecionou Netcode 1.14.2 e Transport 2.7.4. O manifesto foi reduzido e a validação repetida para confirmar que as versões persistem. Fontes dos pacotes: [Netcode](https://packages.unity.com/com.unity.netcode), [Transport](https://packages.unity.com/com.unity.transport).

`Logs/phase0-repeat.log:1716` registra `PHASE0_COMPILE_OK`; as linhas 1743–1744 registram encerramento normal com código 0. A primeira validação havia permanecido no encerramento do runtime Mono e foi interrompida; o aceite se baseia na repetição que encerrou normalmente. Manifesto, lock e `ProjectVersion.txt` foram conferidos independentemente.

## Consequências

Não atualizar Editor ou pacotes automaticamente. Uma mudança exige novo registro da motivação, importação e compilação, além dos testes de build/conexão quando estiverem implementados. O exemplo adaptado de HelloNetcode mantém atribuição e licença em `Assets/ThirdParty/HelloNetcode`.

Os logs citados são evidência local ignorada pelo Git. A fase 1 deverá reproduzir a compilação a partir de uma cópia limpa. Este aceite não demonstra a meta de 100 jogadores nem a execução do servidor Linux.
