using System.Collections.Generic;
using Ferrugem.Visuals;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ferrugem
{
    // Presentation only. All authoritative movement runs in FpsMovementSystem.
    public class FpsPresentation : MonoBehaviour
    {
        public static FpsInput CurrentInput;
        public static bool HasPlayer { get; private set; }
        public static CombatState LocalCombat { get; private set; }
        public static float HitFeedbackUntil { get; private set; }
        public static int ConfirmedHit { get; private set; }
        private Entity localPlayer;
        private int shotSequence;
        private CombatVisuals combatVisuals;
        private Camera view;
        private float yaw;
        private float pitch;
        private bool smoke;
        private bool combatSmoke;
        private float smokePlayerAt = -1;
        private bool smokeFired;
        private bool smokeReloaded;
        private readonly Dictionary<Entity, GameObject> bodies = new Dictionary<Entity, GameObject>();
        private readonly HashSet<Entity> visible = new HashSet<Entity>();
        private readonly List<Entity> removed = new List<Entity>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Create()
        {
            if (!FerrugemBootstrap.Server) new GameObject("FPS Presentation").AddComponent<FpsPresentation>();
        }

        private void Awake()
        {
            smoke = Arguments.Has("--fps-smoke");
            combatSmoke = Arguments.Has("--combat-smoke");
            foreach (var camera in FindObjectsByType<Camera>(FindObjectsSortMode.None)) camera.enabled = false;
            view = new GameObject("First Person Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
            view.fieldOfView = 80;
            view.nearClipPlane = 0.05f;
            view.farClipPlane = 150;
            view.transform.position = new Vector3(0, 1.65f, -9);
            view.backgroundColor = new Color(0.36f, 0.39f, 0.4f);
            view.clearFlags = CameraClearFlags.SolidColor;
            CreateArena();
            combatVisuals = gameObject.AddComponent<CombatVisuals>();
            combatVisuals.CreateWeapon(view.transform);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            var wasCaptured = Cursor.lockState == CursorLockMode.Locked;
            if (!smoke && !combatSmoke)
            {
                if (!Application.isFocused || (keyboard != null && keyboard.escapeKey.wasPressedThisFrame))
                { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
                else if (mouse != null && mouse.leftButton.wasPressedThisFrame && HasPlayer
                         && !FoundationClient.HudRect.Contains(new Vector2(mouse.position.ReadValue().x, Screen.height - mouse.position.ReadValue().y)))
                { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
            }
            CurrentInput.Move = default;
            CurrentInput.Sprint = 0;
            if (combatSmoke)
            {
                if (!HasPlayer) { smokePlayerAt = -1; smokeFired = false; smokeReloaded = false; }
                else
                {
                    if (smokePlayerAt < 0) smokePlayerAt = Time.realtimeSinceStartup;
                    var elapsed = Time.realtimeSinceStartup - smokePlayerAt;
                    if (elapsed >= 2 && !smokeFired) { CurrentInput.Fire.Set(); smokeFired = true; }
                    if (elapsed >= 4 && !smokeReloaded) { CurrentInput.Reload.Set(); smokeReloaded = true; }
                }
            }
            else if (smoke)
            {
                CurrentInput.Move = new float2(0, ((int)(Time.realtimeSinceStartup / 4) % 2 == 0) ? 1 : -1);
            }
            else if (Cursor.lockState == CursorLockMode.Locked && Application.isFocused && LocalCombat.Life == 0)
            {
                if (mouse != null)
                {
                    if (wasCaptured && mouse.leftButton.wasPressedThisFrame) CurrentInput.Fire.Set();
                    var delta = mouse.delta.ReadValue();
                    yaw = (yaw + delta.x * 0.12f) % 360;
                    pitch = Mathf.Clamp(pitch - delta.y * 0.12f, -85, 85);
                }
                if (keyboard != null)
                {
                    CurrentInput.Move = new float2((keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0),
                        (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0));
                    CurrentInput.Sprint = (byte)(keyboard.leftShiftKey.isPressed ? 1 : 0);
                    if (keyboard.rKey.wasPressedThisFrame) CurrentInput.Reload.Set();
                    if (keyboard.gKey.wasPressedThisFrame) CurrentInput.Throw.Set();
                }
            }
            CurrentInput.Yaw = yaw;
            CurrentInput.Pitch = pitch;
        }

        public static FpsInput ConsumeInput()
        {
            var input = CurrentInput;
            CurrentInput.Fire = default;
            CurrentInput.Reload = default;
            CurrentInput.Throw = default;
            return input;
        }

        private void LateUpdate()
        {
            var world = FerrugemBootstrap.GameWorld;
            if (world == null || !world.IsCreated) return;
            var manager = world.EntityManager;
            using var ids = manager.CreateEntityQuery(typeof(NetworkId));
            var localId = ids.CalculateEntityCount() == 1 ? ids.GetSingleton<NetworkId>().Value : -1;
            using var players = manager.CreateEntityQuery(typeof(FpsPlayer), typeof(GhostOwner), typeof(LocalTransform));
            using var entities = players.ToEntityArray(Allocator.Temp);
            visible.Clear();
            HasPlayer = false;
            foreach (var entity in entities)
            {
                var owner = manager.GetComponentData<GhostOwner>(entity).NetworkId;
                var transformData = manager.GetComponentData<LocalTransform>(entity);
                if (owner == localId)
                {
                    HasPlayer = true;
                    LocalCombat = manager.GetComponentData<CombatState>(entity);
                    if (localPlayer != entity) { localPlayer = entity; shotSequence = LocalCombat.ShotSequence; }
                    if (LocalCombat.ShotSequence != shotSequence)
                    {
                        shotSequence = LocalCombat.ShotSequence;
                        combatVisuals.ConfirmedShot();
                        ConfirmedHit = LocalCombat.LastHit;
                        HitFeedbackUntil = Time.unscaledTime + 0.35f;
                    }
                    combatVisuals.SetWeaponState(LocalCombat);
                    view.transform.SetPositionAndRotation((Vector3)transformData.Position + Vector3.up * (LocalCombat.Life == 0 ? 1.65f : 0.4f),
                        Quaternion.Euler(pitch, yaw, 0));
                    continue;
                }
                if (manager.GetComponentData<CombatState>(entity).Life != 0) continue;
                visible.Add(entity);
                if (!bodies.TryGetValue(entity, out var body))
                {
                    body = SurvivorVisual.Create(owner);
                    bodies.Add(entity, body);
                    body.transform.position = transformData.Position;
                }
                var speed = Vector3.Distance(body.transform.position, (Vector3)transformData.Position) / Mathf.Max(Time.deltaTime, 0.001f);
                body.transform.SetPositionAndRotation(transformData.Position, transformData.Rotation);
                body.GetComponent<SurvivorVisual>()?.SetMovement(speed);
            }
            removed.Clear();
            foreach (var pair in bodies)
                if (!visible.Contains(pair.Key)) { Destroy(pair.Value); removed.Add(pair.Key); }
            foreach (var entity in removed) bodies.Remove(entity);
            if (!HasPlayer) { combatVisuals.HideWeapon(); LocalCombat = default; Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        }

        private static void CreateArena()
        {
            var ground = Surface("Concrete", new Color(0.31f, 0.30f, 0.27f));
            var rust = Surface("Weathered steel", new Color(0.28f, 0.20f, 0.15f));
            var wall = Surface("Boundary concrete", new Color(0.39f, 0.38f, 0.34f));
            Block("Test ground", new Vector3(0, -0.15f, 0), new Vector3(40, 0.3f, 40), ground);
            foreach (var obstacle in FpsArena.Obstacles)
                Block("Movement obstacle", new Vector3(obstacle.x, 1.2f, obstacle.y), new Vector3(3, 2.4f, 2), rust);
            Block("North boundary", new Vector3(0, 1, 20), new Vector3(41, 2, 1), wall);
            Block("South boundary", new Vector3(0, 1, -20), new Vector3(41, 2, 1), wall);
            Block("East boundary", new Vector3(20, 1, 0), new Vector3(1, 2, 40), wall);
            Block("West boundary", new Vector3(-20, 1, 0), new Vector3(1, 2, 40), wall);
            RenderSettings.ambientLight = new Color(0.55f, 0.57f, 0.60f);
        }

        private static Material Surface(string title, Color color)
        {
            var material = new Material(Resources.Load<Material>("Ferrugem/PrototypeSurface")) { name = title };
            material.color = color;
            return material;
        }
        private static void Block(string title, Vector3 position, Vector3 scale, Material material)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = title;
            block.transform.position = position;
            block.transform.localScale = scale;
            block.GetComponent<Renderer>().sharedMaterial = material;
            Destroy(block.GetComponent<Collider>()); // ECS shared arena is the sole movement collision authority.
        }
        private void OnDestroy()
        {
            CurrentInput = default;
            HasPlayer = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
