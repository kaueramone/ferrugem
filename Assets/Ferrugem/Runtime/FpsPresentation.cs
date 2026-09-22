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
    public class FpsPresentation : MonoBehaviour
    {
        public static FpsInput CurrentInput;
        public static bool HasPlayer { get; private set; }
        public static CombatState LocalCombat { get; private set; }
        public static float HitFeedbackUntil { get; private set; }
        public static int ConfirmedHit { get; private set; }
        public static bool IsAiming { get; private set; }
        private Entity localPlayer;
        private int shotSequence;
        private CombatVisuals combatVisuals;
        private Camera view;
        private float yaw, pitch, eyeHeight = 1.65f, aimBlend;
        private Vector2 lookDelta;
        private bool smoke, combatSmoke, motorSmoke;
        private float smokePlayerAt = -1;
        private bool smokeFired, smokeReloaded, smokeJumped;
        private readonly Dictionary<Entity, PlayerVisual> bodies = new Dictionary<Entity, PlayerVisual>();
        private readonly HashSet<Entity> visible = new HashSet<Entity>();
        private readonly List<Entity> removed = new List<Entity>();
        private Vector3 lastLocalPosition;
        private float localStepDistance;
        private bool localReloading;
        private sealed class PlayerVisual
        {
            public GameObject Body;
            public int Shot;
            public bool Reloading;
            public float StepDistance;
            public Vector3 LastPosition;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Create()
        {
            if (!FerrugemBootstrap.Server) new GameObject("FPS Presentation").AddComponent<FpsPresentation>();
        }

        private void Awake()
        {
            smoke = Arguments.Has("--fps-smoke");
            combatSmoke = Arguments.Has("--combat-smoke");
            motorSmoke = Arguments.Has("--motor-smoke");
            foreach (var camera in FindObjectsByType<Camera>(FindObjectsSortMode.None)) camera.enabled = false;
            foreach (var listener in FindObjectsByType<AudioListener>(FindObjectsSortMode.None)) listener.enabled = false;
            view = new GameObject("First Person Camera", typeof(Camera), typeof(AudioListener)).GetComponent<Camera>();
            view.fieldOfView = 80;
            view.nearClipPlane = 0.035f;
            view.farClipPlane = 150;
            view.transform.position = new Vector3(0, 1.65f, -9);
            view.backgroundColor = new Color(0.36f, 0.39f, 0.4f);
            view.clearFlags = CameraClearFlags.SolidColor;
            gameObject.AddComponent<ProceduralAudio>();
            CreateArena();
            combatVisuals = gameObject.AddComponent<CombatVisuals>();
            combatVisuals.CreateWeapon(view.transform);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            var wasCaptured = Cursor.lockState == CursorLockMode.Locked;
            if (!smoke && !combatSmoke && !motorSmoke)
            {
                if (!Application.isFocused || (keyboard != null && keyboard.escapeKey.wasPressedThisFrame))
                { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
                else if (mouse != null && mouse.leftButton.wasPressedThisFrame && HasPlayer
                         && !FoundationClient.HudRect.Contains(new Vector2(mouse.position.ReadValue().x, Screen.height - mouse.position.ReadValue().y)))
                { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
            }
            // Keep queued event counters until GhostInputSystem consumes them, even without a simulation tick this frame.
            if ((!smoke && !combatSmoke && !motorSmoke && (!Application.isFocused || Cursor.lockState != CursorLockMode.Locked)) || LocalCombat.Life != 0)
                CurrentInput.Fire = CurrentInput.Reload = CurrentInput.Throw = CurrentInput.Jump = default;
            CurrentInput.Move = default;
            CurrentInput.Sprint = CurrentInput.Crouch = CurrentInput.Aim = 0;
            lookDelta = Vector2.zero;
            if (combatSmoke || motorSmoke)
            {
                if (!HasPlayer) { smokePlayerAt = -1; smokeFired = smokeReloaded = smokeJumped = false; }
                else
                {
                    if (smokePlayerAt < 0) smokePlayerAt = Time.realtimeSinceStartup;
                    var elapsed = Time.realtimeSinceStartup - smokePlayerAt;
                    if (combatSmoke)
                    {
                        if (elapsed >= 2 && !smokeFired) { CurrentInput.Fire.Set(); smokeFired = true; }
                        if (elapsed >= 4 && !smokeReloaded) { CurrentInput.Reload.Set(); smokeReloaded = true; }
                    }
                    else
                    {
                        CurrentInput.Move = new float2(0, elapsed >= 1 && elapsed < 9 ? 1 : 0);
                        CurrentInput.Sprint = (byte)(elapsed >= 1 && elapsed < 2 ? 1 : 0);
                        if (elapsed >= 2 && !smokeJumped) { CurrentInput.Jump.Set(); smokeJumped = true; }
                        CurrentInput.Crouch = (byte)(elapsed >= 3.5f && elapsed < 5 ? 1 : 0);
                        CurrentInput.Aim = (byte)(elapsed >= 6 && elapsed < 8 ? 1 : 0);
                    }
                }
            }
            else if (smoke)
                CurrentInput.Move = new float2(0, ((int)(Time.realtimeSinceStartup / 4) % 2 == 0) ? 1 : -1);
            else if (Cursor.lockState == CursorLockMode.Locked && Application.isFocused && LocalCombat.Life == 0)
            {
                if (mouse != null)
                {
                    // Capture can warp the pointer: do not fire or use its delta on that first frame.
                    if (wasCaptured)
                    {
                        if (mouse.leftButton.wasPressedThisFrame) CurrentInput.Fire.Set();
                        lookDelta = mouse.delta.ReadValue();
                    }
                    CurrentInput.Aim = (byte)(mouse.rightButton.isPressed ? 1 : 0);
                    var sensitivity = Mathf.Lerp(0.12f, 0.066f, aimBlend);
                    yaw = (yaw + lookDelta.x * sensitivity) % 360;
                    pitch = Mathf.Clamp(pitch - lookDelta.y * sensitivity, -85, 85);
                }
                if (keyboard != null)
                {
                    CurrentInput.Move = new float2((keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0),
                        (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0));
                    CurrentInput.Sprint = (byte)(keyboard.leftShiftKey.isPressed ? 1 : 0);
                    CurrentInput.Crouch = (byte)(keyboard.leftCtrlKey.isPressed ? 1 : 0);
                    if (keyboard.spaceKey.wasPressedThisFrame) CurrentInput.Jump.Set();
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
            CurrentInput.Fire = CurrentInput.Reload = CurrentInput.Throw = CurrentInput.Jump = default;
            return input;
        }

        private void LateUpdate()
        {
            var world = FerrugemBootstrap.GameWorld;
            if (world == null || !world.IsCreated) return;
            var manager = world.EntityManager;
            using var ids = manager.CreateEntityQuery(typeof(NetworkId));
            var localId = ids.CalculateEntityCount() == 1 ? ids.GetSingleton<NetworkId>().Value : -1;
            using var players = manager.CreateEntityQuery(typeof(FpsPlayer), typeof(GhostOwner), typeof(LocalTransform), typeof(CombatState));
            using var entities = players.ToEntityArray(Allocator.Temp);
            visible.Clear(); HasPlayer = false;
            foreach (var entity in entities)
            {
                var owner = manager.GetComponentData<GhostOwner>(entity).NetworkId;
                var pose = manager.GetComponentData<LocalTransform>(entity);
                var motor = manager.GetComponentData<FpsPlayer>(entity);
                var combat = manager.GetComponentData<CombatState>(entity);
                var position = (Vector3)pose.Position;
                if (owner == localId)
                {
                    HasPlayer = true; LocalCombat = combat;
                    if (localPlayer != entity)
                    {
                        localPlayer = entity; shotSequence = combat.ShotSequence;
                        lastLocalPosition = position; localStepDistance = 0; localReloading = combat.ReloadRemaining > 0;
                        eyeHeight = FpsMotor.Eye(motor.Crouched != 0);
                    }
                    if (combat.ShotSequence != shotSequence)
                    {
                        shotSequence = combat.ShotSequence; combatVisuals.ConfirmedShot();
                        ProceduralAudio.Play(SoundCue.Shot, position, true);
                        ConfirmedHit = combat.LastHit; HitFeedbackUntil = Time.unscaledTime + 0.35f;
                        if (combat.LastHit != 0) combatVisuals.ConfirmedImpact(combat.LastHitPosition);
                    }
                    if (combat.ReloadRemaining > 0 && !localReloading) ProceduralAudio.Play(SoundCue.Reload, position, true);
                    localReloading = combat.ReloadRemaining > 0;
                    UpdateSteps(position, ref lastLocalPosition, ref localStepDistance, motor, combat, true);
                    IsAiming = motor.Aiming != 0 && combat.Life == 0 && combat.ReloadRemaining <= 0;
                    aimBlend = Mathf.MoveTowards(aimBlend, IsAiming ? 1 : 0, Time.deltaTime * 7);
                    var targetEye = combat.Life == 0 ? FpsMotor.Eye(motor.Crouched != 0) : 0.4f;
                    eyeHeight = Mathf.Lerp(eyeHeight, targetEye, 1 - Mathf.Exp(-Time.deltaTime * 16));
                    view.fieldOfView = Mathf.Lerp(80, 58, aimBlend);
                    view.transform.SetPositionAndRotation(position + Vector3.up * eyeHeight, Quaternion.Euler(pitch, yaw, 0));
                    combatVisuals.SetWeaponState(combat, aimBlend, lookDelta, math.length(motor.Velocity.xz), motor.Grounded != 0);
                    continue;
                }
                if (combat.Life != 0) continue;
                visible.Add(entity);
                if (!bodies.TryGetValue(entity, out var visual))
                {
                    visual = new PlayerVisual { Body = SurvivorVisual.Create(owner), Shot = combat.ShotSequence,
                        Reloading = combat.ReloadRemaining > 0, LastPosition = position };
                    bodies.Add(entity, visual);
                }
                if (combat.ShotSequence != visual.Shot)
                {
                    visual.Shot = combat.ShotSequence;
                    ProceduralAudio.Play(SoundCue.Shot, position + Vector3.up * FpsMotor.Eye(motor.Crouched != 0));
                    if (combat.LastHit != 0) combatVisuals.ConfirmedImpact(combat.LastHitPosition);
                }
                if (combat.ReloadRemaining > 0 && !visual.Reloading) ProceduralAudio.Play(SoundCue.Reload, position);
                visual.Reloading = combat.ReloadRemaining > 0;
                UpdateSteps(position, ref visual.LastPosition, ref visual.StepDistance, motor, combat, false);
                visual.Body.transform.SetPositionAndRotation(pose.Position, pose.Rotation);
                var body = visual.Body.GetComponent<SurvivorVisual>();
                body.SetMovement(math.length(motor.Velocity.xz)); body.SetCrouched(motor.Crouched != 0);
            }
            removed.Clear();
            foreach (var pair in bodies) if (!visible.Contains(pair.Key)) { Destroy(pair.Value.Body); removed.Add(pair.Key); }
            foreach (var entity in removed) bodies.Remove(entity);
            if (!HasPlayer) { combatVisuals.HideWeapon(); LocalCombat = default; IsAiming = false; Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        }

        private static void UpdateSteps(Vector3 position, ref Vector3 previous, ref float distance, FpsPlayer motor, CombatState combat, bool local)
        {
            var travel = Vector3.Distance(position, previous); previous = position;
            if (travel > 1 || motor.Grounded == 0 || combat.Life != 0) { distance = 0; return; }
            distance += travel;
            if (distance > (motor.Crouched != 0 ? 1.1f : 1.6f))
            { distance = 0; ProceduralAudio.Play(SoundCue.Step, position, local); }
        }

        private static void CreateArena()
        {
            var ground = Surface("Concrete", new Color(0.31f, 0.30f, 0.27f));
            var rust = Surface("Weathered steel", new Color(0.28f, 0.20f, 0.15f));
            var wall = Surface("Boundary concrete", new Color(0.39f, 0.38f, 0.34f));
            Block("Test ground", new Vector3(0, -0.15f, 0), new Vector3(40, 0.3f, 40), ground);
            foreach (var solid in FpsArena.Solids)
                Block(solid.Name, (solid.Min + solid.Max) * 0.5f, solid.Max - solid.Min, rust);
            CreateRamp(wall);
            Block("North boundary", new Vector3(0, 1, 20), new Vector3(41, 2, 1), wall);
            Block("South boundary", new Vector3(0, 1, -20), new Vector3(41, 2, 1), wall);
            Block("East boundary", new Vector3(20, 1, 0), new Vector3(1, 2, 40), wall);
            Block("West boundary", new Vector3(-20, 1, 0), new Vector3(1, 2, 40), wall);
            RenderSettings.ambientLight = new Color(0.55f, 0.57f, 0.60f);
        }
        private static void CreateRamp(Material material)
        {
            var min = FpsArena.RampMinXZ; var max = FpsArena.RampMaxXZ; var height = FpsArena.RampHeight;
            var mesh = new Mesh { name = "Original test ramp" };
            mesh.vertices = new[] { new Vector3(min.x, 0, min.y), new Vector3(max.x, 0, min.y),
                new Vector3(min.x, 0, max.y), new Vector3(max.x, 0, max.y), new Vector3(min.x, height, max.y), new Vector3(max.x, height, max.y) };
            mesh.triangles = new[] { 0,4,1, 1,4,5, 0,2,4, 1,5,3, 2,3,5, 2,5,4, 0,1,2, 1,3,2 };
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            var ramp = new GameObject("Rampa de teste", typeof(MeshFilter), typeof(MeshRenderer));
            ramp.GetComponent<MeshFilter>().sharedMesh = mesh; ramp.GetComponent<MeshRenderer>().sharedMaterial = material;
        }
        private static Material Surface(string title, Color color)
        {
            var material = new Material(Resources.Load<Material>("Ferrugem/PrototypeSurface")) { name = title }; material.color = color; return material;
        }
        private static void Block(string title, Vector3 position, Vector3 scale, Material material)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube); block.name = title;
            block.transform.position = position; block.transform.localScale = scale;
            block.GetComponent<Renderer>().sharedMaterial = material; Destroy(block.GetComponent<Collider>());
        }
        private void OnDestroy()
        {
            CurrentInput = default; HasPlayer = false; IsAiming = false;
            Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
        }
    }
}
