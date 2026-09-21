# HelloNetcode attribution

Entity Component System copyright © 2017-2020 Unity Technologies ApS.
Licensed under the Unity Companion License for Unity-dependent projects.
See LICENSE.md and https://unity.com/legal/licenses/unity-companion-license.

The initial FerrugemBootstrap and GoInGameSystem are adaptations of the official
Unity-Technologies/EntityComponentSystemSamples HelloNetcode examples, retrieved
2026-09-21:

- https://github.com/Unity-Technologies/EntityComponentSystemSamples/blob/6786a741ee1f118ed14cecfa02beae8e926937b0/NetcodeSamples/Assets/Samples/HelloNetcode/1_Basics/01_BootstrapAndFrontend/Bootstrap/FrontendBootstrap.cs
- https://github.com/Unity-Technologies/EntityComponentSystemSamples/blob/6786a741ee1f118ed14cecfa02beae8e926937b0/NetcodeSamples/Assets/Samples/HelloNetcode/1_Basics/04_GoInGame/GoInGameSystem.cs

Changes: project namespace, automatic connection without sample-specific authoring
components or system groups, deterministic command-buffer disposal and log labels.
These excerpts are a compatibility spike, not completed gameplay.
