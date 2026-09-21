# Graph Report - Ferrugem  (2026-09-21)

## Corpus Check
- 29 files · ~17,351 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 76 file(s) not represented in the graph (top: .meta 34, .asset 34, (none) 2)

## Summary
- 637 nodes · 760 edges · 46 communities (40 shown, 3 thin omitted)
- Extraction: 100% EXTRACTED · 0% INFERRED · 0% AMBIGUOUS · INFERRED: 3 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `8c04e214`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- manifest.json
- dependencies
- com.unity.modules.animation
- FerrugemBootstrap
- com.unity.modules.imgui
- packages-lock.json
- com.unity.modules.unitywebrequest
- com.unity.test-framework
- Testes da fundação
- com.unity.modules.jsonserialize
- com.unity.collections
- com.unity.render-pipelines.core
- ReadmeEditor
- com.unity.sysroot.base
- com.unity.modules.ui
- Plano de execução por marcos
- Build-Helpers.ps1
- com.unity.modules.physics2d
- .Build
- Lore e design v0.1
- com.unity.ext.nunit
- dependencies
- com.unity.inputsystem
- com.unity.profiling.core
- Test-NetworkSmoke.ps1
- com.unity.modules.ai
- com.unity.modules.androidjni
- com.unity.modules.particlesystem
- com.unity.modules.umbra
- com.unity.modules.wind
- dependencies
- com.unity.ide.visualstudio
- Run-LinuxServer.sh
- NOTICE.md
- com.unity.burst
- com.unity.nuget.mono-cecil
- com.unity.mathematics
- com.unity.serialization
- com.unity.transport
- dependencies
- com.unity.scriptablebuildpipeline
- com.unity.modules.physics
- com.unity.modules.uielements

## God Nodes (most connected - your core abstractions)
1. `ReadmeEditor` - 16 edges
2. `com.unity.modules.jsonserialize` - 14 edges
3. `com.unity.modules.physics` - 13 edges
4. `com.unity.modules.unitywebrequest` - 12 edges
5. `FerrugemBootstrap` - 11 edges
6. `com.unity.burst` - 11 edges
7. `com.unity.mathematics` - 11 edges
8. `Plano de execução por marcos` - 11 edges
9. `com.unity.collections` - 10 edges
10. `com.unity.modules.audio` - 10 edges

## Surprising Connections (you probably didn't know these)
- `Format-WslArgument()` --calls--> `Quote-Argument()`  [INFERRED]
  Tools/Start-LinuxTest.ps1 → Tools/Build-Helpers.ps1
- `Start-WindowsClient()` --calls--> `Quote-Argument()`  [INFERRED]
  Tools/Start-LinuxTest.ps1 → Tools/Build-Helpers.ps1
- `Wait-LinuxTestLog()` --calls--> `Read-SharedLog()`  [INFERRED]
  Tools/Start-LinuxTest.ps1 → Tools/Build-Helpers.ps1

## Import Cycles
- None detected.

## Communities (46 total, 3 thin omitted)

### Community 0 - "manifest.json"
Cohesion: 0.04
Nodes (45): com.unity.entities, com.unity.modules.animation, com.unity.modules.assetbundle, com.unity.modules.audio, com.unity.modules.imageconversion, com.unity.modules.imgui, com.unity.modules.jsonserialize, com.unity.modules.physics (+37 more)

### Community 1 - "dependencies"
Cohesion: 0.04
Nodes (46): dependencies, com.unity.entities, com.unity.ide.visualstudio, com.unity.inputsystem, com.unity.modules.accessibility, com.unity.modules.adaptiveperformance, com.unity.modules.ai, com.unity.modules.androidjni (+38 more)

### Community 2 - "com.unity.modules.animation"
Cohesion: 0.12
Nodes (16): dependencies, depth, source, version, dependencies, depth, source, version (+8 more)

### Community 3 - "FerrugemBootstrap"
Cohesion: 0.07
Nodes (21): ConnectionMonitorSystem, Arguments, FerrugemBootstrap, Endpoint, GameWorld, Protocol, Server, FoundationClient (+13 more)

### Community 4 - "com.unity.modules.imgui"
Cohesion: 0.20
Nodes (10): dependencies, depth, source, version, dependencies, depth, source, version (+2 more)

### Community 5 - "packages-lock.json"
Cohesion: 0.05
Nodes (36): com.unity.entities, com.unity.modules.animation, com.unity.modules.assetbundle, com.unity.modules.audio, com.unity.modules.imageconversion, com.unity.modules.imgui, com.unity.modules.jsonserialize, com.unity.modules.physics (+28 more)

### Community 6 - "com.unity.modules.unitywebrequest"
Cohesion: 0.05
Nodes (45): dependencies, depth, source, version, dependencies, depth, source, version (+37 more)

### Community 7 - "com.unity.test-framework"
Cohesion: 0.20
Nodes (11): dependencies, depth, dependencies, depth, source, url, version, source (+3 more)

### Community 8 - "Testes da fundação"
Cohesion: 0.07
Nodes (27): ADR-001 — Editor e pacotes da fundação, Consequências, Decisão, Motivo e evidência, Argumentos implementados, Dois cliques para testar, Gerar executáveis, Importação limpa (+19 more)

### Community 9 - "com.unity.modules.jsonserialize"
Cohesion: 0.07
Nodes (30): dependencies, depth, source, version, dependencies, depth, source, version (+22 more)

### Community 10 - "com.unity.collections"
Cohesion: 0.18
Nodes (11): depth, source, url, version, dependencies, depth, source, version (+3 more)

### Community 11 - "com.unity.render-pipelines.core"
Cohesion: 0.09
Nodes (25): depth, source, version, dependencies, depth, source, version, dependencies (+17 more)

### Community 12 - "ReadmeEditor"
Cohesion: 0.12
Nodes (14): ReadmeEditor, BodyStyle, ButtonStyle, HeadingStyle, LinkStyle, TitleStyle, Readme, Section (+6 more)

### Community 13 - "com.unity.sysroot.base"
Cohesion: 0.08
Nodes (24): dependencies, depth, source, url, version, dependencies, depth, source (+16 more)

### Community 14 - "com.unity.modules.ui"
Cohesion: 0.20
Nodes (10): dependencies, depth, source, version, dependencies, depth, source, version (+2 more)

### Community 15 - "Plano de execução por marcos"
Cohesion: 0.18
Nodes (11): Fase 0 — Descoberta e decisões, Fase 1 — Fundação reproduzível, Fase 2 — Prova dos riscos combinados, Fase 3 — KOTH de 12 jogadores, Fase 4 — Ciclo de logística completo, Fase 5 — Recorte jogável de 12–30 pessoas, Fase 6 — Escala e operação: 30 → 60 → 100, Fase 7 — Conteúdo e aéreos civis (+3 more)

### Community 16 - "Build-Helpers.ps1"
Cohesion: 0.27
Nodes (10): Assert-EditorClosed(), Get-Sha256(), Get-SourceFingerprint(), Get-VerifiedBuild(), Quote-Argument(), Read-SharedLog(), Format-WslArgument(), Invoke-Wsl() (+2 more)

### Community 17 - "com.unity.modules.physics2d"
Cohesion: 0.20
Nodes (10): dependencies, depth, source, version, dependencies, depth, source, version (+2 more)

### Community 18 - ".Build"
Cohesion: 0.31
Nodes (4): ProjectBuild, BuildTarget, Ferrugem.Editor, StandaloneBuildSubtarget

### Community 19 - "Lore e design v0.1"
Cohesion: 0.05
Nodes (36): Armas e veículos, Ciclo de uma vida, Economia de escassez, Lore e design v0.1, Premissa, Primeiro mapa e primeiro recorte, Regras de partida, Três comunidades (+28 more)

### Community 20 - "com.unity.ext.nunit"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, version, dependencies, com.unity.ext.nunit

### Community 21 - "dependencies"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, version, dependencies, com.unity.modules.accessibility

### Community 22 - "com.unity.inputsystem"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.inputsystem

### Community 23 - "com.unity.profiling.core"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.profiling.core

### Community 25 - "com.unity.modules.ai"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.ai

### Community 26 - "com.unity.modules.androidjni"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.androidjni

### Community 27 - "com.unity.modules.particlesystem"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.particlesystem

### Community 29 - "com.unity.modules.umbra"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.umbra

### Community 30 - "com.unity.modules.wind"
Cohesion: 0.40
Nodes (5): dependencies, depth, source, version, com.unity.modules.wind

### Community 31 - "dependencies"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, version, dependencies, com.unity.modules.hierarchycore

### Community 32 - "com.unity.ide.visualstudio"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.ide.visualstudio

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

### Community 41 - "com.unity.transport"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.transport

### Community 43 - "dependencies"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.entities

### Community 44 - "com.unity.scriptablebuildpipeline"
Cohesion: 0.33
Nodes (6): dependencies, depth, source, url, version, com.unity.scriptablebuildpipeline

### Community 48 - "com.unity.modules.physics"
Cohesion: 0.10
Nodes (20): dependencies, depth, source, version, dependencies, depth, source, version (+12 more)

### Community 59 - "com.unity.modules.uielements"
Cohesion: 0.22
Nodes (9): depth, source, version, dependencies, depth, source, version, com.unity.modules.uielements (+1 more)

## Knowledge Gaps
- **426 isolated node(s):** `Ferrugem.Editor`, `GameWorld`, `Server`, `Endpoint`, `Protocol` (+421 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 449 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **3 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `dependencies` connect `dependencies` to `com.unity.modules.animation`, `com.unity.modules.imgui`, `packages-lock.json`, `com.unity.modules.unitywebrequest`, `com.unity.test-framework`, `com.unity.modules.jsonserialize`, `com.unity.collections`, `com.unity.render-pipelines.core`, `com.unity.sysroot.base`, `com.unity.modules.ui`, `com.unity.modules.physics2d`, `com.unity.ext.nunit`, `com.unity.inputsystem`, `com.unity.profiling.core`, `com.unity.modules.ai`, `com.unity.modules.androidjni`, `com.unity.modules.particlesystem`, `com.unity.modules.umbra`, `com.unity.modules.wind`, `dependencies`, `com.unity.ide.visualstudio`, `com.unity.burst`, `com.unity.nuget.mono-cecil`, `com.unity.mathematics`, `com.unity.serialization`, `com.unity.transport`, `dependencies`, `com.unity.scriptablebuildpipeline`, `com.unity.modules.physics`, `com.unity.modules.uielements`?**
  _High betweenness centrality (0.290) - this node is a cross-community bridge._
- **Why does `dependencies` connect `dependencies` to `manifest.json`?**
  _High betweenness centrality (0.015) - this node is a cross-community bridge._
- **Why does `com.unity.modules.jsonserialize` connect `com.unity.modules.jsonserialize` to `com.unity.burst`, `com.unity.test-framework`, `com.unity.collections`, `com.unity.ext.nunit`, `dependencies`, `dependencies`?**
  _High betweenness centrality (0.012) - this node is a cross-community bridge._
- **What connects `Ferrugem.Editor`, `GameWorld`, `Server` to the rest of the system?**
  _426 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `manifest.json` be split into smaller, more focused modules?**
  _Cohesion score 0.043478260869565216 - nodes in this community are weakly interconnected._
- **Should `dependencies` be split into smaller, more focused modules?**
  _Cohesion score 0.043478260869565216 - nodes in this community are weakly interconnected._
- **Should `com.unity.modules.animation` be split into smaller, more focused modules?**
  _Cohesion score 0.125 - nodes in this community are weakly interconnected._