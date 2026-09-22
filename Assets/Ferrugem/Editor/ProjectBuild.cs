using System;
using System.IO;
using Unity.NetCode;
using Unity.Scenes;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ferrugem.Editor
{
    public static class ProjectBuild
    {
        private const string ScenePath = "Assets/Scenes/Foundation.unity";
        private const string SubScenePath = "Assets/Scenes/FpsEntities.unity";
        private const string PrefabPath = "Assets/Ferrugem/Prefabs/FpsPlayer.prefab";

        public static void ValidateMotor() { ValidateFoundation(); FpsMotorSmoke.Run(); }

        public static void ValidateFoundation()
        {
            Debug.Log($"[Ferrugem] PHASE0_COMPILE_OK editor={Application.unityVersion} netcode={typeof(ClientServerBootstrap).Assembly.GetName().Name}");
            EditorSettings.serializationMode = SerializationMode.ForceText;
            PlayerSettings.companyName = "Ferrugem Project";
            PlayerSettings.productName = "Ferrugem";
            PlayerSettings.bundleVersion = "0.3.0";
            PlayerSettings.runInBackground = true;
            PrepareFps();
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
        }

        // Run once before launcher hashing. Generated assets are versioned and subsequent builds leave them unchanged.
        public static void PrepareFps()
        {
            const string surfacePath = "Assets/Ferrugem/Resources/Ferrugem/PrototypeSurface.mat";
            if (!File.Exists(surfacePath))
            {
                Directory.CreateDirectory("Assets/Ferrugem/Resources/Ferrugem");
                AssetDatabase.Refresh();
                AssetDatabase.CreateAsset(new Material(Shader.Find("Universal Render Pipeline/Lit")), surfacePath);
            }
            if (File.Exists(PrefabPath) && File.Exists(SubScenePath) && File.Exists("Assets/Ferrugem/Prefabs/CombatCharge.prefab"))
            {
                Debug.Log("[Ferrugem] FPS_ASSETS_READY existing=1");
                return;
            }
            Directory.CreateDirectory("Assets/Ferrugem/Prefabs");
            AssetDatabase.Refresh();
            var authoring = new GameObject("FpsPlayer");
            authoring.AddComponent<FpsPlayerAuthoring>();
            var ghost = authoring.AddComponent<GhostAuthoringComponent>();
            ghost.HasOwner = true;
            ghost.SupportAutoCommandTarget = true;
            ghost.DefaultGhostMode = GhostMode.OwnerPredicted;
            ghost.SupportedGhostModes = GhostModeMask.All;
            var prefab = PrefabUtility.SaveAsPrefabAsset(authoring, PrefabPath);
            UnityEngine.Object.DestroyImmediate(authoring);

            var entities = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var reference = new GameObject("FPS prefab reference").AddComponent<FpsPrefabAuthoring>();
            reference.PlayerPrefab = prefab;
            reference.ZombiePrefab = CreateCombatPrefab("CombatZombie", 0);
            reference.BarrelPrefab = CreateCombatPrefab("CombatBarrel", 1);
            reference.ChargePrefab = CreateCombatPrefab("CombatCharge", 2);
            EditorSceneManager.SaveScene(entities, SubScenePath);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var sun = new GameObject("Overcast daylight").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.2f;
            sun.transform.rotation = Quaternion.Euler(50, -30, 0);
            var subscene = new GameObject("FPS Entities").AddComponent<SubScene>();
            subscene.SceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(SubScenePath);
            subscene.AutoLoadScene = true;
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("[Ferrugem] FPS_ASSETS_READY existing=0");
        }

                private static GameObject CreateCombatPrefab(string name, int kind)
        {
            var actor = new GameObject(name);
            actor.AddComponent<CombatActorAuthoring>().Kind = kind;
            var ghost = actor.AddComponent<GhostAuthoringComponent>();
            ghost.DefaultGhostMode = GhostMode.Interpolated;
            ghost.SupportedGhostModes = GhostModeMask.Interpolated;
            var prefab = PrefabUtility.SaveAsPrefabAsset(actor, "Assets/Ferrugem/Prefabs/" + name + ".prefab");
            UnityEngine.Object.DestroyImmediate(actor);
            return prefab;
        }
        public static void WindowsClient() => Build(BuildTarget.StandaloneWindows64,
            StandaloneBuildSubtarget.Player, "Builds/Windows/Ferrugem.exe");
        public static void LinuxServer() => Build(BuildTarget.StandaloneLinux64,
            StandaloneBuildSubtarget.Server, "Builds/LinuxServer/FerrugemServer.x86_64");

        private static void Build(BuildTarget target, StandaloneBuildSubtarget subtarget, string output)
        {
            ValidateFoundation();
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { ScenePath }, target = target, subtarget = (int)subtarget,
                locationPathName = output, options = BuildOptions.Development,
                // Netcode 1.14 NetworkSimulatorSettings/DefaultDriverConstructor require this
                // explicit symbol; DEVELOPMENT_BUILD alone does not include their simulator.
                extraScriptingDefines = new[] { "NETCODE_DEBUG" }
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception($"Build failed: {report.summary.result}, {report.summary.totalErrors} errors");
            Debug.Log($"[Ferrugem] BUILD_OK target={target} subtarget={subtarget} output={output}");
        }
    }
}
