using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Ferrugem.Visuals
{
    // Presentation only. Movement, collision and ownership remain in the network simulation.
    public sealed class SurvivorVisual : MonoBehaviour
    {
        private const string ResourcePath = "Ferrugem/Male_LongSleeve";
        private readonly List<Material> materials = new List<Material>();
        private PlayableGraph graph;
        private AnimationMixerPlayable mixer;
        private AnimationClipPlayable idle;
        private AnimationClipPlayable walk;
        private AnimationClip idleClip;
        private AnimationClip walkClip;
        private float movementSpeed;
        private float blend;
        private bool infected;
        private float threat;
        private Transform modelTransform;

        public static GameObject Create(int ownerId)
        {
            var root = new GameObject($"Survivor Visual {ownerId}");
            var visual = root.AddComponent<SurvivorVisual>();
            visual.Initialize(ownerId);
            return root;
        }

        public static GameObject CreateZombie()
        {
            var root = new GameObject("Infected civilian (provisional)");
            var visual = root.AddComponent<SurvivorVisual>();
            visual.infected = true;
            visual.Initialize(-1);
            return root;
        }
        public void SetThreat(float value) => threat = Mathf.Clamp01(value);
        public void SetMovement(float speed) => movementSpeed = Mathf.Max(0, speed);

        private void Initialize(int ownerId)
        {
            var asset = Resources.Load<GameObject>(ResourcePath);
            if (asset == null)
            {
                Debug.LogError("[Ferrugem] SURVIVOR_ASSET_MISSING " + ResourcePath);
                return;
            }
            var model = Instantiate(asset, transform, false);
            model.name = "Civilian (Quaternius CC0)";
            modelTransform = model.transform;
            foreach (var collider in model.GetComponentsInChildren<Collider>()) Destroy(collider);

            var surface = Resources.Load<Material>("Ferrugem/PrototypeSurface");
            if (surface == null) throw new InvalidOperationException("PrototypeSurface material missing for survivor presentation.");
            var renderers = model.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var originals = renderer.sharedMaterials;
                var replacements = new Material[originals.Length];
                for (var i = 0; i < originals.Length; i++)
                {
                    var part = originals[i] == null ? "Cloth" : originals[i].name;
                    var material = new Material(surface) { name = "Ferrugem " + part };
                    var color = PartColor(part, ownerId);
                    if (infected) color = part.ToLowerInvariant().Contains("skin") ? new Color(0.39f, 0.41f, 0.34f) : Color.Lerp(color, new Color(0.18f, 0.19f, 0.16f), 0.65f);
                    material.SetColor("_BaseColor", color);
                    material.SetFloat("_Smoothness", 0.12f);
                    material.SetFloat("_Metallic", 0);
                    replacements[i] = material;
                    materials.Add(material);
                }
                renderer.sharedMaterials = replacements;
            }

            // Normalize the imported author's units once, before animation starts.
            if (renderers.Length > 0)
            {
                var bounds = renderers[0].bounds;
                foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
                if (bounds.size.y > 0.001f)
                {
                    float scale = 1.75f / bounds.size.y;
                    model.transform.localScale *= scale;
                    model.transform.localPosition = new Vector3(-bounds.center.x * scale, -bounds.min.y * scale, -bounds.center.z * scale);
                }
            }

            var animator = model.GetComponent<Animator>();
            if (animator == null) animator = model.AddComponent<Animator>();
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            foreach (var clip in Resources.LoadAll<AnimationClip>(ResourcePath))
            {
                if (clip.name.EndsWith("Man_Idle", StringComparison.Ordinal)) idleClip = clip;
                if (clip.name.EndsWith("Man_Walk", StringComparison.Ordinal)) walkClip = clip;
            }
            if (idleClip == null || walkClip == null)
            {
                Debug.LogError("[Ferrugem] SURVIVOR_ANIMATION_MISSING idle/walk");
                return;
            }
            graph = PlayableGraph.Create("Survivor Idle Walk");
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            mixer = AnimationMixerPlayable.Create(graph, 2);
            idle = AnimationClipPlayable.Create(graph, idleClip);
            walk = AnimationClipPlayable.Create(graph, walkClip);
            graph.Connect(idle, 0, mixer, 0);
            graph.Connect(walk, 0, mixer, 1);
            mixer.SetInputWeight(0, 1);
            mixer.SetInputWeight(1, 0);
            AnimationPlayableOutput.Create(graph, "Body", animator).SetSourcePlayable(mixer);
            graph.Play();
        }

        private static Color PartColor(string part, int ownerId)
        {
            part = part.ToLowerInvariant();
            if (part.Contains("skin")) return new Color(0.51f, 0.37f, 0.28f);
            if (part.Contains("eye")) return new Color(0.08f, 0.075f, 0.065f);
            if (part.Contains("hair")) return new Color(0.12f, 0.09f, 0.065f);
            if (part.Contains("shoe")) return new Color(0.10f, 0.085f, 0.07f);
            if (part.Contains("pant")) return new Color(0.15f, 0.17f, 0.18f);
            if (part.Contains("sock")) return new Color(0.22f, 0.22f, 0.20f);
            // Identification in the test only; these are not team assignments.
            return (ownerId & 1) == 0 ? new Color(0.31f, 0.32f, 0.23f) : new Color(0.38f, 0.27f, 0.19f);
        }

        private void Update()
        {
            if (!graph.IsValid()) return;
            blend = Mathf.MoveTowards(blend, movementSpeed > 0.1f ? 1 : 0, Time.deltaTime * 8);
            mixer.SetInputWeight(0, 1 - blend);
            mixer.SetInputWeight(1, blend);
            walk.SetSpeed(infected ? Mathf.Clamp(movementSpeed / 1.6f, 0.3f, 0.8f) : Mathf.Clamp(movementSpeed / 1.6f, 0.65f, 2.8f));
            if (infected && modelTransform != null) modelTransform.localRotation = Quaternion.Euler(threat * 14, 0, 0);
            // Explicit wrapping also handles importer changes without holding the last frame.
            if (idle.GetTime() >= idleClip.length && idleClip.length > 0) idle.SetTime(idle.GetTime() % idleClip.length);
            if (walk.GetTime() >= walkClip.length && walkClip.length > 0) walk.SetTime(walk.GetTime() % walkClip.length);
        }

        private void OnDestroy()
        {
            if (graph.IsValid()) graph.Destroy();
            foreach (var material in materials) if (material != null) Destroy(material);
        }
    }
}
