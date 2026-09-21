using System;
using UnityEditor;

namespace Ferrugem.Visuals.Editor
{
    public sealed class SurvivorModelImporter : AssetPostprocessor
    {
        private const string ModelPath = "Assets/ThirdParty/QuaterniusAnimatedMen/Resources/Ferrugem/Male_LongSleeve.fbx";

        private void OnPreprocessModel()
        {
            if (assetPath != ModelPath) return;
            var importer = (ModelImporter)assetImporter;
            importer.animationType = ModelImporterAnimationType.Generic;
            importer.importAnimation = true;
            importer.importCameras = false;
            importer.importLights = false;
            importer.addCollider = false;
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
        }

        private void OnPreprocessAnimation()
        {
            if (assetPath != ModelPath) return;
            var importer = (ModelImporter)assetImporter;
            var clips = importer.defaultClipAnimations;
            foreach (var clip in clips)
            {
                clip.loopTime = clip.name.EndsWith("Man_Idle", StringComparison.Ordinal)
                    || clip.name.EndsWith("Man_Walk", StringComparison.Ordinal)
                    || clip.name.EndsWith("Man_Run", StringComparison.Ordinal);
                clip.lockRootRotation = true;
                clip.lockRootHeightY = true;
                clip.lockRootPositionXZ = true;
            }
            importer.clipAnimations = clips;
        }
    }
}
