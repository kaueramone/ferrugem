# Graph Report - Ferrugem  (2026-09-22)

## Corpus Check
- 52 files · ~48,600 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 113 file(s) not represented in the graph (top: .meta 64, .asset 34, .prefab 4)

## Summary
- 975 nodes · 1426 edges · 67 communities (56 shown, 5 thin omitted)
- Extraction: 99% EXTRACTED · 1% INFERRED · 0% AMBIGUOUS · INFERRED: 17 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `e3082b71`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- manifest.json
- dependencies
- com.unity.transport
- FerrugemBootstrap
- Refinamento FPS — versão 0.3.0
- packages-lock.json
- .PrepareFps
- com.unity.test-framework
- Registro das fases 0 e 1
- com.unity.modules.particlesystem
- .OnUpdate
- com.unity.render-pipelines.core
- ReadmeEditor
- com.unity.sysroot.base
- dependencies
- Plano de execução por marcos
- Build-Helpers.ps1
- com.unity.modules.physics2d
- FpsPresentation
- Arquitetura técnica proposta
- com.unity.modules.imageconversion
- SurvivorVisual
- ProceduralAudio
- Lore e design v0.2
- Test-NetworkSmoke.ps1
- com.unity.modules.ai
- dependencies
- com.unity.modules.androidjni
- com.unity.modules.umbra
- QuaterniusAnimatedMen/NOTICE.md
- FpsPlayer
- Run-LinuxServer.sh
- NOTICE.md
- com.unity.burst
- com.unity.nuget.mono-cecil
- com.unity.mathematics
- com.unity.collections
- Registro de participação de IA
- com.unity.modules.audio
- com.unity.ide.visualstudio
- com.unity.scriptablebuildpipeline
- com.unity.modules.unitywebrequest
- README.md
- SurvivorModelImporter
- Mapa brasileiro — Santa Brasa, Vale do Sal
- Testes da fundação e do protótipo FPS
- Fase 2 — Protótipo jogável
- Referências visuais e de controle FPS
- com.unity.modules.jsonserialize
- com.unity.modules.wind
- Ferrugem
- Assets do protótipo
- FERRUGEM — O Último Entreposto
- dependencies
- com.unity.modules.imgui
- com.unity.profiling.core
- com.unity.modules.assetbundle
- com.unity.modules.animation
- com.unity.modules.uielements
- com.unity.modules.ui
- ADR-001 — Editor e pacotes da fundação

## God Nodes (most connected - your core abstractions)
1. `FpsPresentation` - 30 edges
2. `CombatVisuals` - 25 edges
3. `SurvivorVisual` - 24 edges
4. `CombatState` - 22 edges
5. `CombatServerSystem` - 16 edges
6. `FpsPlayer` - 16 edges
7. `ReadmeEditor` - 16 edges
8. `Ferrugem` - 15 edges
9. `CombatRules` - 15 edges
10. `com.unity.modules.jsonserialize` - 14 edges

## Surprising Connections (you probably didn't know these)
- `FpsPresentation` --references--> `CombatState`  [EXTRACTED]
  Assets/Ferrugem/Runtime/FpsPresentation.cs → Assets/Ferrugem/Runtime/CombatActorAuthoring.cs
- `FpsPresentation` --references--> `FpsInput`  [EXTRACTED]
  Assets/Ferrugem/Runtime/FpsPresentation.cs → Assets/Ferrugem/Runtime/FpsPlayerAuthoring.cs
- `Format-WslArgument()` --calls--> `Quote-Argument()`  [INFERRED]
  Tools/Start-LinuxTest.ps1 → Tools/Build-Helpers.ps1
- `Start-WindowsClient()` --calls--> `Quote-Argument()`  [INFERRED]
  Tools/Start-LinuxTest.ps1 → Tools/Build-Helpers.ps1
- `Wait-LinuxTestLog()` --calls--> `Read-SharedLog()`  [INFERRED]
  Tools/Start-LinuxTest.ps1 → Tools/Build-Helpers.ps1

## Import Cycles
- None detected.

## Communities (67 total, 5 thin omitted)

### Community 0 - "manifest.json"
Cohesion: 0.04
Nodes (45): com.unity.entities, com.unity.modules.animation, com.unity.modules.assetbundle, com.unity.modules.audio, com.unity.modules.imageconversion, com.unity.modules.imgui, com.unity.modules.jsonserialize, com.unity.modules.physics (+37 more)

### Community 1 - "dependencies"
Cohesion: 0.04
Nodes (46): dependencies, com.unity.entities, com.unity.ide.visualstudio, com.unity.inputsystem, com.unity.modules.accessibility, com.unity.modules.adaptiveperformance, com.unity.modules.ai, com.unity.modules.androidjni (+38 more)

### Community 2 - "com.unity.transport"
Cohesion: 0.17
Nodes (12): dependencies, depth, source, url, version, dependencies, depth, source (+4 more)

### Community 3 - "FerrugemBootstrap"
Cohesion: 0.11
Nodes (14): Arguments, FerrugemBootstrap, Endpoint, GameWorld, Protocol, Server, RuntimeInitializeOnLoadMethod, FoundationClient (+6 more)

### Community 4 - "Refinamento FPS — versão 0.3.0"
Cohesion: 0.20
Nodes (10): Controles e apresentação, Evidências de validação — aceite do refinamento pendente, Facções aprovadas, Inspeção visual parcial da build final, Movimento e campo de teste, Refinamento FPS — versão 0.3.0, Regressões sobre a mesma build final, Rodada final do motor (+2 more)

### Community 5 - "packages-lock.json"
Cohesion: 0.05
Nodes (36): com.unity.entities, com.unity.modules.animation, com.unity.modules.assetbundle, com.unity.modules.audio, com.unity.modules.imageconversion, com.unity.modules.imgui, com.unity.modules.jsonserialize, com.unity.modules.physics (+28 more)

### Community 6 - ".PrepareFps"
Cohesion: 0.14
Nodes (12): GameObject, ProjectBuild, BuildTarget, CombatActorAuthoring, Ferrugem.Editor, FpsPlayerAuthoring, FpsPrefabAuthoring, GhostAuthoringComponent (+4 more)

### Community 7 - "com.unity.test-framework"
Cohesion: 0.20
Nodes (11): dependencies, depth, dependencies, depth, source, url, version, source (+3 more)

### Community 8 - "Registro das fases 0 e 1"
Cohesion: 0.20
Nodes (10): Editor instalado e licença resolvida, Fase 0: concluída, Fase 1: conexão concluída, Pendências não fatais e limites do aceite, Política de execução do servidor, Preparado, Registro das fases 0 e 1, Servidor dedicado Linux com clientes Windows (+2 more)

### Community 9 - "com.unity.modules.particlesystem"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.particlesystem

### Community 10 - ".OnUpdate"
Cohesion: 0.10
Nodes (25): Entity, float3, BarrelState, ChargeState, CombatActorAuthoring, CombatActorBaker, CombatPrefabs, CombatState (+17 more)

### Community 11 - "com.unity.render-pipelines.core"
Cohesion: 0.09
Nodes (25): depth, source, version, dependencies, depth, source, version, dependencies (+17 more)

### Community 12 - "ReadmeEditor"
Cohesion: 0.12
Nodes (14): ReadmeEditor, BodyStyle, ButtonStyle, HeadingStyle, LinkStyle, TitleStyle, Readme, Section (+6 more)

### Community 13 - "com.unity.sysroot.base"
Cohesion: 0.08
Nodes (24): dependencies, depth, source, url, version, dependencies, depth, source (+16 more)

### Community 14 - "dependencies"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.entities

### Community 15 - "Plano de execução por marcos"
Cohesion: 0.18
Nodes (11): Fase 0 — Descoberta e decisões, Fase 1 — Fundação reproduzível, Fase 2 — Prova dos riscos combinados, Fase 3 — KOTH de 12 jogadores, Fase 4 — Ciclo de logística completo, Fase 5 — Recorte jogável de 12–30 pessoas, Fase 6 — Escala e operação: 30 → 60 → 100, Fase 7 — Conteúdo e aéreos civis (+3 more)

### Community 16 - "Build-Helpers.ps1"
Cohesion: 0.27
Nodes (10): Assert-EditorClosed(), Get-Sha256(), Get-SourceFingerprint(), Get-VerifiedBuild(), Quote-Argument(), Read-SharedLog(), Format-WslArgument(), Invoke-Wsl() (+2 more)

### Community 17 - "com.unity.modules.physics2d"
Cohesion: 0.20
Nodes (10): dependencies, depth, source, version, dependencies, depth, source, version (+2 more)

### Community 18 - "FpsPresentation"
Cohesion: 0.06
Nodes (45): Collider, Color, Dictionary, Entity, GameObject, HashSet, List, LocalTransform (+37 more)

### Community 19 - "Arquitetura técnica proposta"
Cohesion: 0.06
Nodes (27): Arquitetura técnica proposta, Destruição com impacto real, Direção e escolha de rede, Metas para medir, Navegação dos infectados, Persistência e exploração de falhas, Responsabilidades, Sincronização (+19 more)

### Community 20 - "com.unity.modules.imageconversion"
Cohesion: 0.13
Nodes (15): dependencies, depth, source, version, dependencies, depth, source, version (+7 more)

### Community 21 - "SurvivorVisual"
Cohesion: 0.11
Nodes (15): AnimationClip, AnimationClipPlayable, AnimationMixerPlayable, Animator, Collider, Color, GameObject, List (+7 more)

### Community 22 - "ProceduralAudio"
Cohesion: 0.11
Nodes (17): FpsPlayerAuthoring, FpsPlayerBaker, GameObject, FpsPrefabAuthoring, FpsPrefabBaker, ProceduralAudio, SoundCue, Blast (+9 more)

### Community 23 - "Lore e design v0.2"
Cohesion: 0.18
Nodes (10): Armas e veículos, Ciclo de uma vida, Direção brasileira e arte própria, Economia de escassez, Lore e design v0.2, Premissa, Primeiro mapa e primeiro recorte, Regras de partida (+2 more)

### Community 25 - "com.unity.modules.ai"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.ai

### Community 26 - "dependencies"
Cohesion: 0.20
Nodes (10): dependencies, depth, source, version, depth, source, version, dependencies (+2 more)

### Community 27 - "com.unity.modules.androidjni"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.androidjni

### Community 29 - "com.unity.modules.umbra"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.umbra

### Community 32 - "FpsPlayer"
Cohesion: 0.06
Nodes (36): float3, FpsMotor, Dictionary, Entity, GhostOwner, HashSet, LocalTransform, NetworkId (+28 more)

### Community 37 - "com.unity.burst"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.burst

### Community 38 - "com.unity.nuget.mono-cecil"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.nuget.mono-cecil

### Community 39 - "com.unity.mathematics"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.mathematics

### Community 40 - "com.unity.collections"
Cohesion: 0.18
Nodes (11): depth, source, url, version, dependencies, depth, source, url (+3 more)

### Community 41 - "Registro de participação de IA"
Cohesion: 0.40
Nodes (4): Marco 2B — testes automáticos aprovados, Planejamento do mapa brasileiro, Refinamento FPS — 22/09/2026, em validação, Registro de participação de IA

### Community 44 - "com.unity.modules.audio"
Cohesion: 0.20
Nodes (10): dependencies, depth, source, version, dependencies, depth, source, version (+2 more)

### Community 45 - "com.unity.ide.visualstudio"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.ide.visualstudio

### Community 46 - "com.unity.scriptablebuildpipeline"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.scriptablebuildpipeline

### Community 47 - "com.unity.modules.unitywebrequest"
Cohesion: 0.18
Nodes (11): dependencies, dependencies, depth, source, version, dependencies, depth, source (+3 more)

### Community 50 - "SurvivorModelImporter"
Cohesion: 0.33
Nodes (3): AssetPostprocessor, SurvivorModelImporter, Ferrugem.Visuals.Editor

### Community 51 - "Mapa brasileiro — Santa Brasa, Vale do Sal"
Cohesion: 0.18
Nodes (11): Arte e linguagem, Cidade central, Decisões confirmadas, Escala técnica e localização pendentes, Geometria adotada para o desenho, Lugar e história, Mapa brasileiro — Santa Brasa, Vale do Sal, Ordem de produção (+3 more)

### Community 52 - "Testes da fundação e do protótipo FPS"
Cohesion: 0.20
Nodes (10): Argumentos implementados, Diagnóstico manual com servidor Windows, Diagnóstico opcional com servidor Windows, Dois cliques para testar, Gerar executáveis, Importação limpa, Playtest do marco 2B — após compilar a versão de combate, Refinamento 0.3.0 (+2 more)

### Community 53 - "Fase 2 — Protótipo jogável"
Cohesion: 0.17
Nodes (12): Evidências automáticas do marco 2B, Evidências do marco 2A, Fase 2 — Protótipo jogável, Inspeção visual parcial do marco 2B, Limites de validação do combate, Marco 2A — FPS e presença no mundo, Marco 2B — Combate e infectados (testes automáticos aprovados), Marco 2C — Veículo e destruição (planejado) (+4 more)

### Community 54 - "Referências visuais e de controle FPS"
Cohesion: 0.29
Nodes (7): Aplicação ao mapa e à destruição, Arte original; referências sem compra, Direção brasileira confirmada, Direção de arte: referências Synty, Próximo recorte: refinar o FPS antes de ampliar a arte, Referências visuais e de controle FPS, Sensação de controle: POLYGON

### Community 56 - "com.unity.modules.jsonserialize"
Cohesion: 0.05
Nodes (45): dependencies, depth, source, version, dependencies, depth, source, version (+37 more)

### Community 57 - "com.unity.modules.wind"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.wind

### Community 58 - "Ferrugem"
Cohesion: 0.10
Nodes (14): ConnectionMonitorSystem, Entity, GhostType, NetworkId, NetworkStreamInGame, ConnectionLogged, FpsReadyRpc, GoInGameSystem (+6 more)

### Community 59 - "Assets do protótipo"
Cohesion: 0.25
Nodes (8): Apresentação provisória do marco 2B, Assets do protótipo, Candidatos reservados, ainda não importados, Decisão de produção: arte original brasileira, Modelo integrado para o marco 2A, Referências adicionais do usuário, Refinamento 0.3.0: mãos e sons originais, Regra de inclusão

### Community 60 - "FERRUGEM — O Último Entreposto"
Cohesion: 0.40
Nodes (5): Decisões de partida, Direção brasileira e mapa, FERRUGEM — O Último Entreposto, Para começar, Regras de trabalho

### Community 62 - "dependencies"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, version, dependencies, com.unity.modules.terrain

### Community 64 - "com.unity.modules.imgui"
Cohesion: 0.12
Nodes (16): dependencies, depth, source, version, dependencies, depth, source, version (+8 more)

### Community 65 - "com.unity.profiling.core"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.profiling.core

### Community 67 - "com.unity.modules.assetbundle"
Cohesion: 0.14
Nodes (15): dependencies, depth, source, version, dependencies, depth, source, version (+7 more)

### Community 68 - "com.unity.modules.animation"
Cohesion: 0.20
Nodes (10): dependencies, depth, source, version, dependencies, depth, source, version (+2 more)

### Community 69 - "com.unity.modules.uielements"
Cohesion: 0.13
Nodes (15): dependencies, depth, source, url, version, depth, source, version (+7 more)

### Community 70 - "com.unity.modules.ui"
Cohesion: 0.18
Nodes (11): dependencies, depth, source, version, dependencies, depth, source, version (+3 more)

### Community 72 - "ADR-001 — Editor e pacotes da fundação"
Cohesion: 0.50
Nodes (4): ADR-001 — Editor e pacotes da fundação, Consequências, Decisão, Motivo e evidência

## Knowledge Gaps
- **487 isolated node(s):** `Ferrugem.Editor`, `Fresh`, `GameWorld`, `Server`, `Endpoint` (+482 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 593 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **5 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `dependencies` connect `dependencies` to `com.unity.transport`, `packages-lock.json`, `com.unity.test-framework`, `com.unity.modules.particlesystem`, `com.unity.render-pipelines.core`, `com.unity.sysroot.base`, `dependencies`, `com.unity.modules.physics2d`, `com.unity.modules.imageconversion`, `com.unity.modules.ai`, `com.unity.modules.androidjni`, `com.unity.modules.umbra`, `com.unity.burst`, `com.unity.nuget.mono-cecil`, `com.unity.mathematics`, `com.unity.collections`, `com.unity.modules.audio`, `com.unity.ide.visualstudio`, `com.unity.scriptablebuildpipeline`, `com.unity.modules.unitywebrequest`, `com.unity.modules.jsonserialize`, `com.unity.modules.wind`, `dependencies`, `com.unity.modules.imgui`, `com.unity.profiling.core`, `com.unity.modules.assetbundle`, `com.unity.modules.animation`, `com.unity.modules.uielements`, `com.unity.modules.ui`?**
  _High betweenness centrality (0.124) - this node is a cross-community bridge._
- **Why does `FpsPresentation` connect `FpsPresentation` to `FpsPlayer`, `Ferrugem`, `.OnUpdate`, `ProceduralAudio`?**
  _High betweenness centrality (0.022) - this node is a cross-community bridge._
- **Why does `CombatVisuals` connect `FpsPresentation` to `Ferrugem`, `ProceduralAudio`?**
  _High betweenness centrality (0.016) - this node is a cross-community bridge._
- **What connects `Ferrugem.Editor`, `Fresh`, `GameWorld` to the rest of the system?**
  _487 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `manifest.json` be split into smaller, more focused modules?**
  _Cohesion score 0.043478260869565216 - nodes in this community are weakly interconnected._
- **Should `dependencies` be split into smaller, more focused modules?**
  _Cohesion score 0.043478260869565216 - nodes in this community are weakly interconnected._
- **Should `FerrugemBootstrap` be split into smaller, more focused modules?**
  _Cohesion score 0.1076923076923077 - nodes in this community are weakly interconnected._