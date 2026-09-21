using System.Collections.Generic;
using Ferrugem.Visuals;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Ferrugem
{
    // Disposable client visuals. Nothing here decides a hit, death, explosion or damage.
    public sealed class CombatVisuals : MonoBehaviour
    {
        private sealed class Visual
        {
            public GameObject Root;
            public GameObject Warning;
            public bool WasAlive = true;
            public bool Exploded;
        }
        private readonly Dictionary<Entity, Visual> visuals = new Dictionary<Entity, Visual>();
        private readonly HashSet<Entity> present = new HashSet<Entity>();
        private readonly List<Entity> removed = new List<Entity>();
        private readonly List<Material> materials = new List<Material>();
        private Transform weapon;
        private GameObject flash;
        private float shotAt = -10;
        private bool reloading;
        private Material iron;
        private Material warning;
        private Material rust;
        private Material soot;
        private Material fire;

        public void CreateWeapon(Transform cameraTransform)
        {
            iron = Material("Worn iron", new Color(0.16f, 0.17f, 0.17f));
            warning = Material("Attack warning", new Color(0.65f, 0.12f, 0.04f));
            rust = Material("Rusted barrel", new Color(0.30f, 0.21f, 0.12f));
            soot = Material("Burned metal", new Color(0.09f, 0.085f, 0.08f));
            fire = Material("Flash", new Color(1, 0.66f, 0.19f));
            weapon = new GameObject("Provisional civilian revolver").transform;
            weapon.SetParent(cameraTransform, false);
            var grip = Material("Wood grip", new Color(0.25f, 0.15f, 0.09f));
            Part(weapon, "Grip", PrimitiveType.Cube, new Vector3(0, -0.09f, 0), new Vector3(0.055f, 0.14f, 0.065f), grip);
            Part(weapon, "Frame", PrimitiveType.Cube, Vector3.zero, new Vector3(0.055f, 0.065f, 0.18f), iron);
            var cylinder = Part(weapon, "Cylinder", PrimitiveType.Cylinder, new Vector3(0, 0.015f, 0.015f), new Vector3(0.085f, 0.05f, 0.085f), iron);
            cylinder.transform.localRotation = Quaternion.Euler(90, 0, 0);
            Part(weapon, "Barrel", PrimitiveType.Cube, new Vector3(0, 0.03f, 0.15f), new Vector3(0.035f, 0.035f, 0.20f), iron);
            Part(weapon, "Sight", PrimitiveType.Cube, new Vector3(0, 0.055f, 0.23f), new Vector3(0.008f, 0.02f, 0.02f), iron);
            flash = Part(weapon, "Muzzle flash", PrimitiveType.Sphere, new Vector3(0, 0.03f, 0.27f), Vector3.one * 0.055f, fire);
            flash.SetActive(false);
            weapon.gameObject.SetActive(false);
        }

        public void ConfirmedShot() => shotAt = Time.unscaledTime;
        public void HideWeapon() { if (weapon != null) weapon.gameObject.SetActive(false); }
        public void SetWeaponState(CombatState state)
        {
            weapon.gameObject.SetActive(state.Life == 0);
            reloading = state.ReloadRemaining > 0;
        }

        private void LateUpdate()
        {
            if (weapon == null) return;
            var recoil = Mathf.Max(0, 1 - (Time.unscaledTime - shotAt) / 0.16f);
            weapon.localPosition = new Vector3(0.23f, reloading ? -0.4f : -0.22f, 0.48f - recoil * 0.035f);
            weapon.localRotation = Quaternion.Euler(-recoil * 8, -3, reloading ? -25 : 0);
            flash.SetActive(Time.unscaledTime - shotAt < 0.06f);
            var world = FerrugemBootstrap.GameWorld;
            if (world == null || !world.IsCreated) return;
            var manager = world.EntityManager;
            present.Clear();
            using (var query = manager.CreateEntityQuery(typeof(ZombieState), typeof(LocalTransform)))
            using (var entities = query.ToEntityArray(Allocator.Temp))
                foreach (var entity in entities)
                {
                    var state = manager.GetComponentData<ZombieState>(entity);
                    if (state.Alive == 0) continue;
                    var pose = manager.GetComponentData<LocalTransform>(entity);
                    present.Add(entity);
                    if (!visuals.TryGetValue(entity, out var visual))
                    {
                        visual = new Visual { Root = SurvivorVisual.CreateZombie() };
                        visual.Root.transform.position = pose.Position;
                        visual.Warning = Part(visual.Root.transform, "Attack windup", PrimitiveType.Cylinder,
                            new Vector3(0, 0.025f, 0), new Vector3(0.9f, 0.015f, 0.9f), warning);
                        visuals.Add(entity, visual);
                    }
                    var speed = Vector3.Distance(visual.Root.transform.position, pose.Position) / Mathf.Max(Time.deltaTime, 0.001f);
                    visual.Root.transform.SetPositionAndRotation(pose.Position, pose.Rotation);
                    var body = visual.Root.GetComponent<SurvivorVisual>();
                    body.SetMovement(speed);
                    body.SetThreat(state.AttackRemaining > 0 ? 1 : 0);
                    visual.Warning.SetActive(state.AttackRemaining > 0);
                    if (state.AttackRemaining > 0)
                        visual.Warning.transform.localScale = new Vector3(0.9f + Mathf.Sin(Time.time * 18) * 0.08f, 0.015f, 0.9f);
                }
            using (var query = manager.CreateEntityQuery(typeof(BarrelState), typeof(LocalTransform)))
            using (var entities = query.ToEntityArray(Allocator.Temp))
                foreach (var entity in entities)
                {
                    var state = manager.GetComponentData<BarrelState>(entity);
                    var pose = manager.GetComponentData<LocalTransform>(entity);
                    present.Add(entity);
                    if (!visuals.TryGetValue(entity, out var visual))
                    {
                        var root = new GameObject("Explosive fuel barrel");
                        Part(root.transform, "Drum", PrimitiveType.Cylinder, Vector3.up * 0.55f, new Vector3(0.65f, 0.55f, 0.65f), rust);
                        Part(root.transform, "Fuel band", PrimitiveType.Cylinder, Vector3.up * 0.7f, new Vector3(0.66f, 0.06f, 0.66f), warning);
                        visual = new Visual { Root = root, WasAlive = state.Alive != 0 };
                        visuals.Add(entity, visual);
                    }
                    visual.Root.transform.SetPositionAndRotation(pose.Position, pose.Rotation);
                    if (state.Alive == 0)
                    {
                        if (visual.WasAlive) Blast(pose.Position);
                        visual.WasAlive = false;
                        visual.Root.transform.localScale = new Vector3(1, 0.25f, 1);
                        foreach (var renderer in visual.Root.GetComponentsInChildren<Renderer>()) renderer.sharedMaterial = soot;
                    }
                }
            using (var query = manager.CreateEntityQuery(typeof(ChargeState), typeof(LocalTransform)))
            using (var entities = query.ToEntityArray(Allocator.Temp))
                foreach (var entity in entities)
                {
                    var state = manager.GetComponentData<ChargeState>(entity);
                    var pose = manager.GetComponentData<LocalTransform>(entity);
                    present.Add(entity);
                    if (!visuals.TryGetValue(entity, out var visual))
                    {
                        visual = new Visual { Root = new GameObject("Improvised explosive charge"), Exploded = state.Fuse <= 0 };
                        Part(visual.Root.transform, "Package", PrimitiveType.Cube, Vector3.up * 0.12f, new Vector3(0.24f, 0.2f, 0.14f), rust);
                        visual.Warning = Part(visual.Root.transform, "Fuse", PrimitiveType.Sphere, Vector3.up * 0.24f, Vector3.one * 0.045f, fire);
                        visuals.Add(entity, visual);
                    }
                    visual.Root.transform.position = pose.Position;
                    if (state.Fuse <= 0)
                    {
                        if (!visual.Exploded) Blast(pose.Position);
                        visual.Exploded = true;
                        visual.Root.SetActive(false);
                    }
                }
            removed.Clear();
            foreach (var pair in visuals)
                if (!present.Contains(pair.Key)) { Destroy(pair.Value.Root); removed.Add(pair.Key); }
            foreach (var entity in removed) visuals.Remove(entity);
        }

        private void Blast(Vector3 position)
        {
            var burst = Part(null, "Confirmed explosion", PrimitiveType.Sphere, position + Vector3.up * 0.8f, Vector3.one * 2.0f, fire);
            Destroy(burst, 0.15f);
        }
        private Material Material(string name, Color color)
        {
            var material = new Material(Resources.Load<Material>("Ferrugem/PrototypeSurface")) { name = name, color = color };
            materials.Add(material);
            return material;
        }
        private static GameObject Part(Transform parent, string name, PrimitiveType shape, Vector3 position, Vector3 scale, Material material)
        {
            var part = GameObject.CreatePrimitive(shape);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localScale = scale;
            part.GetComponent<Renderer>().sharedMaterial = material;
            Destroy(part.GetComponent<Collider>());
            return part;
        }
        private void OnDestroy()
        {
            foreach (var visual in visuals.Values) if (visual.Root != null) Destroy(visual.Root);
            foreach (var material in materials) Destroy(material);
        }
    }
}
