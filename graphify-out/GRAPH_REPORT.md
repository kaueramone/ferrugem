# Graph Report - Ferrugem  (2026-09-21)

## Corpus Check
- 45 files · ~29,289 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 110 file(s) not represented in the graph (top: .meta 61, .asset 34, .prefab 4)

## Summary
- 873 nodes · 1220 edges · 76 communities (64 shown, 6 thin omitted)
- Extraction: 99% EXTRACTED · 1% INFERRED · 0% AMBIGUOUS · INFERRED: 14 edges (avg confidence: 0.84)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `53a0d1d5`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- manifest.json
- dependencies
- com.unity.entities
- FerrugemBootstrap
- MonoBehaviour
- packages-lock.json
- .PrepareFps
- com.unity.test-framework
- Registro das fases 0 e 1
- dependencies
- .OnUpdate
- com.unity.render-pipelines.core
- ReadmeEditor
- com.unity.sysroot.base
- SystemBase
- Plano de execução por marcos
- Build-Helpers.ps1
- com.unity.modules.physics2d
- CombatVisuals
- Lore e design v0.2
- com.unity.modules.imageconversion
- com.unity.modules.accessibility
- com.unity.modules.unityanalytics
- com.unity.modules.imgui
- Test-NetworkSmoke.ps1
- com.unity.modules.ai
- com.unity.modules.androidjni
- FpsPlayerAuthoring.cs
- com.unity.modules.umbra
- com.unity.modules.jsonserialize
- QuaterniusAnimatedMen/NOTICE.md
- FpsInput
- Run-LinuxServer.sh
- NOTICE.md
- com.unity.burst
- com.unity.nuget.mono-cecil
- com.unity.mathematics
- com.unity.serialization
- REGISTRO-IA.md
- .OnUpdate
- Ferrugem
- com.unity.modules.uielements
- com.unity.ide.visualstudio
- com.unity.scriptablebuildpipeline
- ConnectionMonitorSystem
- FpsArena
- SurvivorModelImporter
- com.unity.collections
- Testes da fundação e do protótipo FPS
- Fase 2 — Protótipo jogável
- Referências visuais e de controle FPS
- FpsPresentation
- com.unity.modules.wind
- .OnUpdate
- Assets do protótipo
- FERRUGEM — O Último Entreposto
- com.unity.modules.subsystems
- com.unity.modules.physics
- com.unity.ext.nunit
- dependencies
- com.unity.transport
- com.unity.modules.assetbundle
- com.unity.modules.audio
- com.unity.modules.unitywebrequest
- com.unity.modules.unitywebrequestassetbundle
- com.unity.modules.unitywebrequestaudio
- ADR-001 — Editor e pacotes da fundação
- com.unity.modules.unitywebrequesttexture
- dependencies
- com.unity.modules.video

## God Nodes (most connected - your core abstractions)
1. `FpsPresentation` - 24 edges
2. `CombatVisuals` - 22 edges
3. `CombatState` - 20 edges
4. `SurvivorVisual` - 19 edges
5. `CombatServerSystem` - 16 edges
6. `ReadmeEditor` - 16 edges
7. `com.unity.modules.jsonserialize` - 14 edges
8. `CombatRules` - 13 edges
9. `com.unity.modules.physics` - 13 edges
10. `Ferrugem` - 12 edges

## Surprising Connections (you probably didn't know these)
- `FpsPresentation` --references--> `CombatState`  [EXTRACTED]
  Assets/Ferrugem/Runtime/FpsPresentation.cs → Assets/Ferrugem/Runtime/CombatActorAuthoring.cs
- `FpsPresentation` --references--> `CombatVisuals`  [EXTRACTED]
  Assets/Ferrugem/Runtime/FpsPresentation.cs → Assets/Ferrugem/Runtime/CombatVisuals.cs
- `FpsPresentation` --references--> `FpsInput`  [EXTRACTED]
  Assets/Ferrugem/Runtime/FpsPresentation.cs → Assets/Ferrugem/Runtime/FpsPlayerAuthoring.cs
- `Format-WslArgument()` --calls--> `Quote-Argument()`  [INFERRED]
  Tools/Start-LinuxTest.ps1 → Tools/Build-Helpers.ps1
- `Start-WindowsClient()` --calls--> `Quote-Argument()`  [INFERRED]
  Tools/Start-LinuxTest.ps1 → Tools/Build-Helpers.ps1

## Import Cycles
- None detected.

## Communities (76 total, 6 thin omitted)

### Community 0 - "manifest.json"
Cohesion: 0.04
Nodes (45): com.unity.entities, com.unity.modules.animation, com.unity.modules.assetbundle, com.unity.modules.audio, com.unity.modules.imageconversion, com.unity.modules.imgui, com.unity.modules.jsonserialize, com.unity.modules.physics (+37 more)

### Community 1 - "dependencies"
Cohesion: 0.04
Nodes (46): dependencies, com.unity.entities, com.unity.ide.visualstudio, com.unity.inputsystem, com.unity.modules.accessibility, com.unity.modules.adaptiveperformance, com.unity.modules.ai, com.unity.modules.androidjni (+38 more)

### Community 2 - "com.unity.entities"
Cohesion: 0.10
Nodes (21): depth, source, url, version, dependencies, depth, source, version (+13 more)

### Community 3 - "FerrugemBootstrap"
Cohesion: 0.11
Nodes (14): Arguments, FerrugemBootstrap, Endpoint, GameWorld, Protocol, Server, RuntimeInitializeOnLoadMethod, FoundationClient (+6 more)

### Community 4 - "MonoBehaviour"
Cohesion: 0.27
Nodes (7): FpsPlayerAuthoring, FpsPlayerBaker, GameObject, FpsPrefabAuthoring, FpsPrefabBaker, Baker, MonoBehaviour

### Community 5 - "packages-lock.json"
Cohesion: 0.05
Nodes (36): com.unity.entities, com.unity.modules.animation, com.unity.modules.assetbundle, com.unity.modules.audio, com.unity.modules.imageconversion, com.unity.modules.imgui, com.unity.modules.jsonserialize, com.unity.modules.physics (+28 more)

### Community 6 - ".PrepareFps"
Cohesion: 0.15
Nodes (12): GameObject, ProjectBuild, BuildTarget, CombatActorAuthoring, Ferrugem.Editor, FpsPlayerAuthoring, FpsPrefabAuthoring, GhostAuthoringComponent (+4 more)

### Community 7 - "com.unity.test-framework"
Cohesion: 0.20
Nodes (11): dependencies, depth, dependencies, depth, source, url, version, source (+3 more)

### Community 8 - "Registro das fases 0 e 1"
Cohesion: 0.20
Nodes (10): Editor instalado e licença resolvida, Fase 0: concluída, Fase 1: conexão concluída, Pendências não fatais e limites do aceite, Política de execução do servidor, Preparado, Registro das fases 0 e 1, Servidor dedicado Linux com clientes Windows (+2 more)

### Community 9 - "dependencies"
Cohesion: 0.13
Nodes (16): dependencies, depth, source, version, dependencies, depth, source, version (+8 more)

### Community 10 - ".OnUpdate"
Cohesion: 0.11
Nodes (24): Entity, BarrelState, ChargeState, CombatActorAuthoring, CombatActorBaker, CombatPrefabs, CombatState, Fresh (+16 more)

### Community 11 - "com.unity.render-pipelines.core"
Cohesion: 0.09
Nodes (25): depth, source, version, dependencies, depth, source, version, dependencies (+17 more)

### Community 12 - "ReadmeEditor"
Cohesion: 0.12
Nodes (14): ReadmeEditor, BodyStyle, ButtonStyle, HeadingStyle, LinkStyle, TitleStyle, Readme, Section (+6 more)

### Community 13 - "com.unity.sysroot.base"
Cohesion: 0.08
Nodes (24): dependencies, depth, source, url, version, dependencies, depth, source (+16 more)

### Community 14 - "SystemBase"
Cohesion: 0.31
Nodes (6): HashSet, FpsDiagnosticsSystem, FpsGatherInputSystem, FpsMovementSystem, FpsSpawnSystem, SystemBase

### Community 15 - "Plano de execução por marcos"
Cohesion: 0.18
Nodes (11): Fase 0 — Descoberta e decisões, Fase 1 — Fundação reproduzível, Fase 2 — Prova dos riscos combinados, Fase 3 — KOTH de 12 jogadores, Fase 4 — Ciclo de logística completo, Fase 5 — Recorte jogável de 12–30 pessoas, Fase 6 — Escala e operação: 30 → 60 → 100, Fase 7 — Conteúdo e aéreos civis (+3 more)

### Community 16 - "Build-Helpers.ps1"
Cohesion: 0.27
Nodes (10): Assert-EditorClosed(), Get-Sha256(), Get-SourceFingerprint(), Get-VerifiedBuild(), Quote-Argument(), Read-SharedLog(), Format-WslArgument(), Invoke-Wsl() (+2 more)

### Community 17 - "com.unity.modules.physics2d"
Cohesion: 0.20
Nodes (10): dependencies, depth, source, version, dependencies, depth, source, version (+2 more)

### Community 18 - "CombatVisuals"
Cohesion: 0.06
Nodes (32): AnimationClip, AnimationClipPlayable, AnimationMixerPlayable, Animator, Collider, Color, Dictionary, Entity (+24 more)

### Community 19 - "Lore e design v0.2"
Cohesion: 0.05
Nodes (36): Armas e veículos, Ciclo de uma vida, Economia de escassez, Lore e design v0.2, Premissa, Primeiro mapa e primeiro recorte, Regras de partida, Três comunidades (+28 more)

### Community 20 - "com.unity.modules.imageconversion"
Cohesion: 0.20
Nodes (10): dependencies, depth, source, version, dependencies, depth, source, version (+2 more)

### Community 21 - "com.unity.modules.accessibility"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.accessibility

### Community 22 - "com.unity.modules.unityanalytics"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.unityanalytics

### Community 23 - "com.unity.modules.imgui"
Cohesion: 0.12
Nodes (17): dependencies, depth, source, version, dependencies, depth, source, version (+9 more)

### Community 25 - "com.unity.modules.ai"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.ai

### Community 26 - "com.unity.modules.androidjni"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.androidjni

### Community 27 - "FpsPlayerAuthoring.cs"
Cohesion: 0.33
Nodes (5): Entity, FpsPrefab, FpsSpawned, NetworkId, NetworkStreamInGame

### Community 29 - "com.unity.modules.umbra"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.umbra

### Community 30 - "com.unity.modules.jsonserialize"
Cohesion: 0.14
Nodes (15): dependencies, depth, source, version, dependencies, depth, source, version (+7 more)

### Community 32 - "FpsInput"
Cohesion: 0.33
Nodes (5): FpsInput, GhostOwnerIsLocal, IInputComponentData, InputEvent, RefRW

### Community 37 - "com.unity.burst"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.burst

### Community 38 - "com.unity.nuget.mono-cecil"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.nuget.mono-cecil

### Community 39 - "com.unity.mathematics"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.mathematics

### Community 40 - "com.unity.serialization"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.serialization

### Community 42 - ".OnUpdate"
Cohesion: 0.38
Nodes (5): FpsPlayer, GhostOwner, LocalTransform, RefRO, Simulate

### Community 44 - "com.unity.modules.uielements"
Cohesion: 0.13
Nodes (15): dependencies, depth, source, url, version, depth, source, version (+7 more)

### Community 45 - "com.unity.ide.visualstudio"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.ide.visualstudio

### Community 46 - "com.unity.scriptablebuildpipeline"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.scriptablebuildpipeline

### Community 47 - "ConnectionMonitorSystem"
Cohesion: 0.40
Nodes (3): ConnectionMonitorSystem, NetworkStreamDriver, NetworkStreamRequestListenResult

### Community 48 - "FpsArena"
Cohesion: 0.40
Nodes (3): float3, FpsArena, float2

### Community 50 - "SurvivorModelImporter"
Cohesion: 0.33
Nodes (3): AssetPostprocessor, SurvivorModelImporter, Ferrugem.Visuals.Editor

### Community 51 - "com.unity.collections"
Cohesion: 0.20
Nodes (10): depth, source, url, version, dependencies, depth, source, version (+2 more)

### Community 52 - "Testes da fundação e do protótipo FPS"
Cohesion: 0.22
Nodes (9): Argumentos implementados, Diagnóstico manual com servidor Windows, Diagnóstico opcional com servidor Windows, Dois cliques para testar, Gerar executáveis, Importação limpa, Playtest do marco 2B — após compilar a versão de combate, Servidor Linux no WSL (+1 more)

### Community 53 - "Fase 2 — Protótipo jogável"
Cohesion: 0.18
Nodes (11): Evidências automáticas do marco 2B, Evidências do marco 2A, Fase 2 — Protótipo jogável, Inspeção visual parcial do marco 2B, Limites de validação do combate, Marco 2A — FPS e presença no mundo, Marco 2B — Combate e infectados (testes automáticos aprovados), Marco 2C — Veículo e destruição (planejado) (+3 more)

### Community 54 - "Referências visuais e de controle FPS"
Cohesion: 0.33
Nodes (6): Aplicação ao mapa e à destruição, Caminho sem compra de assets agora, Direção de arte: referências Synty, Próximo recorte: refinar o FPS antes de ampliar a arte, Referências visuais e de controle FPS, Sensação de controle: POLYGON

### Community 56 - "FpsPresentation"
Cohesion: 0.12
Nodes (17): Collider, Color, Dictionary, Entity, GameObject, HashSet, List, Material (+9 more)

### Community 57 - "com.unity.modules.wind"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.wind

### Community 58 - ".OnUpdate"
Cohesion: 0.21
Nodes (9): Entity, GhostType, NetworkId, NetworkStreamInGame, ConnectionLogged, FpsReadyRpc, GoInGameSystem, IRpcCommand (+1 more)

### Community 59 - "Assets do protótipo"
Cohesion: 0.33
Nodes (6): Apresentação provisória do marco 2B, Assets do protótipo, Candidatos reservados, ainda não importados, Modelo integrado para o marco 2A, Referências adicionais do usuário, Regra de inclusão

### Community 60 - "FERRUGEM — O Último Entreposto"
Cohesion: 0.50
Nodes (4): Decisões de partida, FERRUGEM — O Último Entreposto, Para começar, Regras de trabalho

### Community 62 - "com.unity.modules.subsystems"
Cohesion: 0.20
Nodes (10): dependencies, depth, source, version, dependencies, depth, source, version (+2 more)

### Community 63 - "com.unity.modules.physics"
Cohesion: 0.13
Nodes (15): dependencies, depth, source, version, dependencies, depth, source, version (+7 more)

### Community 64 - "com.unity.ext.nunit"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, version, dependencies, com.unity.ext.nunit

### Community 65 - "dependencies"
Cohesion: 0.29
Nodes (7): dependencies, dependencies, depth, source, url, version, com.unity.profiling.core

### Community 66 - "com.unity.transport"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.transport

### Community 67 - "com.unity.modules.assetbundle"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.assetbundle

### Community 68 - "com.unity.modules.audio"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.audio

### Community 69 - "com.unity.modules.unitywebrequest"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.unitywebrequest

### Community 70 - "com.unity.modules.unitywebrequestassetbundle"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.unitywebrequestassetbundle

### Community 71 - "com.unity.modules.unitywebrequestaudio"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.unitywebrequestaudio

### Community 72 - "ADR-001 — Editor e pacotes da fundação"
Cohesion: 0.50
Nodes (4): ADR-001 — Editor e pacotes da fundação, Consequências, Decisão, Motivo e evidência

### Community 73 - "com.unity.modules.unitywebrequesttexture"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.unitywebrequesttexture

### Community 74 - "dependencies"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.unitywebrequestwww

### Community 75 - "com.unity.modules.video"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.video

## Knowledge Gaps
- **453 isolated node(s):** `Ferrugem.Editor`, `Fresh`, `GameWorld`, `Server`, `Endpoint` (+448 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 545 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **6 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `dependencies` connect `dependencies` to `com.unity.entities`, `packages-lock.json`, `com.unity.test-framework`, `com.unity.render-pipelines.core`, `com.unity.sysroot.base`, `com.unity.modules.physics2d`, `com.unity.modules.imageconversion`, `com.unity.modules.accessibility`, `com.unity.modules.unityanalytics`, `com.unity.modules.imgui`, `com.unity.modules.ai`, `com.unity.modules.androidjni`, `com.unity.modules.umbra`, `com.unity.modules.jsonserialize`, `com.unity.burst`, `com.unity.nuget.mono-cecil`, `com.unity.mathematics`, `com.unity.serialization`, `com.unity.modules.uielements`, `com.unity.ide.visualstudio`, `com.unity.scriptablebuildpipeline`, `com.unity.collections`, `com.unity.modules.wind`, `com.unity.modules.subsystems`, `com.unity.modules.physics`, `com.unity.ext.nunit`, `dependencies`, `com.unity.transport`, `com.unity.modules.assetbundle`, `com.unity.modules.audio`, `com.unity.modules.unitywebrequest`, `com.unity.modules.unitywebrequestassetbundle`, `com.unity.modules.unitywebrequestaudio`, `com.unity.modules.unitywebrequesttexture`, `dependencies`, `com.unity.modules.video`?**
  _High betweenness centrality (0.154) - this node is a cross-community bridge._
- **Why does `CombatVisuals` connect `CombatVisuals` to `FpsPresentation`, `Ferrugem`, `MonoBehaviour`?**
  _High betweenness centrality (0.020) - this node is a cross-community bridge._
- **Why does `FpsPresentation` connect `FpsPresentation` to `FpsInput`, `MonoBehaviour`, `.OnUpdate`, `Ferrugem`, `FpsArena`, `CombatVisuals`?**
  _High betweenness centrality (0.019) - this node is a cross-community bridge._
- **What connects `Ferrugem.Editor`, `Fresh`, `GameWorld` to the rest of the system?**
  _453 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `manifest.json` be split into smaller, more focused modules?**
  _Cohesion score 0.043478260869565216 - nodes in this community are weakly interconnected._
- **Should `dependencies` be split into smaller, more focused modules?**
  _Cohesion score 0.043478260869565216 - nodes in this community are weakly interconnected._
- **Should `com.unity.entities` be split into smaller, more focused modules?**
  _Cohesion score 0.09523809523809523 - nodes in this community are weakly interconnected._