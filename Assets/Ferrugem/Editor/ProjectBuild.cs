using System;
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

        public static void ValidateFoundation()
        {
            Debug.Log($"[Ferrugem] PHASE0_COMPILE_OK editor={Application.unityVersion} netcode={typeof(Unity.NetCode.ClientServerBootstrap).Assembly.GetName().Name}");
            EditorSettings.serializationMode = SerializationMode.ForceText;
            PlayerSettings.companyName = "Ferrugem Project";
            PlayerSettings.productName = "Ferrugem";
            PlayerSettings.bundleVersion = "0.0.1";
            PlayerSettings.runInBackground = true;
            if (!System.IO.File.Exists(ScenePath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.name = "Foundation Ground";
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = "Connection Test Landmark";
                cube.transform.position = new Vector3(0, 0.5f, 0);
                EditorSceneManager.SaveScene(scene, ScenePath);
            }
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
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
                locationPathName = output, options = BuildOptions.Development
            });
            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception($"Build failed: {report.summary.result}, {report.summary.totalErrors} errors");
            Debug.Log($"[Ferrugem] BUILD_OK target={target} subtarget={subtarget} output={output}");
        }
    }
}
