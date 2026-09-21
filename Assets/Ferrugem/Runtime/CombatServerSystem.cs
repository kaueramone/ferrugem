using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Ferrugem
{
    // All damage/AI runs only on the server. Clients send buttons and view angles, never targets or damage.
    // Geometric hit volumes match the flat prototype arena; no rewind/lag compensation yet.
    public static class CombatRules
    {
        public const float Eye = 1.65f;
        public static float3 Direction(float yaw, float pitch) => math.rotate(quaternion.EulerXYZ(math.radians(pitch), math.radians(yaw % 360), 0), new float3(0, 0, 1));
        public static bool RayBox(float3 origin, float3 direction, float3 min, float3 max, out float distance)
        {
            float near = 0, far = 60;
            for (int i = 0; i < 3; i++)
            {
                if (math.abs(direction[i]) < 0.00001f) { if (origin[i] < min[i] || origin[i] > max[i]) { distance = 0; return false; } }
                else
                {
                    float a = (min[i] - origin[i]) / direction[i], b = (max[i] - origin[i]) / direction[i];
                    near = math.max(near, math.min(a, b)); far = math.min(far, math.max(a, b));
                    if (near > far) { distance = 0; return false; }
                }
            }
            distance = near; return true;
        }
        public static float CoverDistance(float3 origin, float3 direction)
        {
            float closest = 60;
            foreach (var obstacle in FpsArena.Obstacles)
                if (RayBox(origin, direction, new float3(obstacle.x - 1.5f, 0, obstacle.y - 1), new float3(obstacle.x + 1.5f, 2.4f, obstacle.y + 1), out var d)) closest = math.min(closest, d);
            return closest;
        }
        public static bool Clear(float3 from, float3 to)
        { var delta = to - from; var length = math.length(delta); return length < .001f || CoverDistance(from, delta / length) >= length; }
        public static bool BeginReload(ref CombatState state)
        {
            if (state.Life != 0 || state.ReloadRemaining > 0 || state.Ammo >= 6 || state.Reserve <= 0) return false;
            state.ReloadRemaining = 2; return true;
        }
        public static void Tick(ref CombatState state, float dt)
        {
            state.Cooldown = math.max(0, state.Cooldown - dt);
            state.ProtectionRemaining = math.max(0, state.ProtectionRemaining - dt);
            if (state.ReloadRemaining > 0 && state.Life == 0)
            {
                state.ReloadRemaining = math.max(0, state.ReloadRemaining - dt);
                if (state.ReloadRemaining == 0) { int transfer = math.min(6 - state.Ammo, state.Reserve); state.Ammo += transfer; state.Reserve -= transfer; }
            }
        }
        public static bool SpendShot(ref CombatState state)
        {
            if (state.Life != 0 || state.ReloadRemaining > 0 || state.Cooldown > 0 || state.Ammo <= 0) return false;
            state.Ammo--; state.Cooldown = .4f; state.ShotSequence++; state.LastHit = 0; state.ProtectionRemaining = 0; return true;
        }
        public static bool Hurt(ref CombatState state, int damage)
        {
            if (state.Life != 0 || state.ProtectionRemaining > 0) return false;
            state.Health = math.max(0, state.Health - damage);
            if (state.Health > 0) return false;
            state.Life = 1; state.DeathSequence++; state.RespawnRemaining = 4; state.ReloadRemaining = 0; return true;
        }
                public static bool DamagePlayer(ref CombatState state, int damage, bool zombieMelee, out bool infection)
        { bool died = Hurt(ref state, damage); infection = died && zombieMelee; return died; }
        public static bool MeleeLands(float3 zombie, float3 target)
        { return math.distance(zombie, target) <= 1.25f && Clear(zombie + new float3(0, 1, 0), target + new float3(0, 1, 0)); }
        public static bool HurtZombie(ref ZombieState state, bool head, bool explosion)
        { if (state.Alive == 0 || (!head && !explosion)) return false; state.Alive = 0; state.AttackRemaining = 0; return true; }
        public static bool Respawn(ref CombatState state, float dt)
        {
            if (state.Life == 0) return false;
            state.RespawnRemaining = math.max(0, state.RespawnRemaining - dt);
            if (state.RespawnRemaining > 0) return false;
            var deaths = state.DeathSequence; var shots = state.ShotSequence;
            state = CombatState.Fresh; state.DeathSequence = deaths; state.ShotSequence = shots; return true;
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(FpsMovementSystem))]
    public partial class CombatServerSystem : SystemBase
    {
        private bool initialized;
        private bool smoke;
        private bool passive;
        private CombatPrefabs prefabs;
        private float smokeTime;
        private bool liveMutation;
        protected override void OnCreate() { RequireForUpdate<CombatPrefabs>(); smoke = Arguments.Has("--combat-smoke"); passive = smoke || Arguments.Has("--fps-smoke"); }
        protected override void OnUpdate()
        {
            prefabs = SystemAPI.GetSingleton<CombatPrefabs>();
            if (!EntityManager.Exists(prefabs.Zombie) || !EntityManager.HasComponent<GhostType>(prefabs.Zombie)) return;
            if (!initialized)
            {
                initialized = true;
                if (smoke) CombatSmoke.Run();
                foreach (var p in new[] { new float3(-9, 0, 9), new float3(0, 0, 12), new float3(9, 0, 9) }) SpawnZombie(p);
                Spawn(prefabs.Barrel, new float3(-9, 0, 4)); Spawn(prefabs.Barrel, new float3(9, 0, 4));
            }
            float dt = SystemAPI.Time.DeltaTime;
            using var playerQuery = EntityManager.CreateEntityQuery(typeof(CombatState), typeof(FpsInput), typeof(FpsPlayer), typeof(LocalTransform));
            using var players = playerQuery.ToEntityArray(Allocator.Temp);
            foreach (var player in players)
            {
                var state = EntityManager.GetComponentData<CombatState>(player);
                bool reloading = state.ReloadRemaining > 0;
                CombatRules.Tick(ref state, dt);
                if (CombatRules.Respawn(ref state, dt))
                {
                    var t = EntityManager.GetComponentData<LocalTransform>(player); t.Position = EntityManager.GetComponentData<FpsPlayer>(player).Spawn; EntityManager.SetComponentData(player, t);
                    Debug.Log($"[Ferrugem] COMBAT_RESPAWN owner={Owner(player)}");
                }
                if (reloading && state.ReloadRemaining == 0 && state.Life == 0) Debug.Log($"[Ferrugem] COMBAT_RELOAD owner={Owner(player)} ammo={state.Ammo}");
                var input = EntityManager.GetComponentData<FpsInput>(player);
                if (input.Reload.IsSet) CombatRules.BeginReload(ref state);
                if (state.Life == 0 && math.isfinite(input.Yaw) && math.isfinite(input.Pitch) && math.abs(input.Pitch) <= 85)
                {
                    var position = EntityManager.GetComponentData<LocalTransform>(player).Position;
                    var origin = position + new float3(0, CombatRules.Eye, 0);
                    var direction = CombatRules.Direction(input.Yaw, input.Pitch);
                    if (input.Fire.IsSet && CombatRules.SpendShot(ref state))
                    {
                        // Store first so explosion/self-damage cannot be overwritten by this local copy.
                        EntityManager.SetComponentData(player, state);
                        int hit = Shoot(player, origin, direction);
                        state = EntityManager.GetComponentData<CombatState>(player); state.LastHit = hit;
                        Debug.Log($"[Ferrugem] COMBAT_SHOT owner={Owner(player)} ammo={state.Ammo}");
                    }
                    if (input.Throw.IsSet && state.Charges > 0 && state.Life == 0)
                    {
                        state.Charges--; state.ProtectionRemaining = 0;
                        var horizontal = math.normalizesafe(new float3(direction.x, 0, direction.z), new float3(0, 0, 1));
                        float range = math.min(6, math.max(0, CombatRules.CoverDistance(origin, horizontal) - .4f));
                        var destination = position + horizontal * range; destination.xz = math.clamp(destination.xz, new float2(-18.5f), new float2(18.5f)); destination.y = .15f;
                        Spawn(prefabs.Charge, destination);
                    }
                }
                EntityManager.SetComponentData(player, state);
            }
                        if (smoke && players.Length == 2 && !liveMutation)
            {
                smokeTime += dt;
                if (smokeTime > 15)
                {
                    liveMutation = true;
                    using var zq = EntityManager.CreateEntityQuery(typeof(ZombieState));
                    using var zs = zq.ToEntityArray(Allocator.Temp);
                    // Passive smoke keeps these initial zombies far from players and barrels.
                    Explode(EntityManager.GetComponentData<LocalTransform>(zs[0]).Position + new float3(0, .8f, 0));
                    HurtPlayer(players[0], 100, true); HurtPlayer(players[0], 100, true);
                    if (zq.CalculateEntityCount() != zs.Length + 1) throw new InvalidOperationException("COMBAT_SUITE_FAIL live_infection_count");
                    Debug.Log("[Ferrugem] COMBAT_LIVE_MUTATION infection=1 repeatedDamage=guarded zombieDeath=1");
                }
            }
            TickCharges(dt);
            if (!passive) TickZombies(players, dt);
        }
        private int Owner(Entity entity) => EntityManager.GetComponentData<GhostOwner>(entity).NetworkId;
        private Entity Spawn(Entity prefab, float3 position) { var entity = EntityManager.Instantiate(prefab); EntityManager.SetComponentData(entity, LocalTransform.FromPosition(position)); return entity; }
        private void SpawnZombie(float3 position) { Spawn(prefabs.Zombie, position); }
        private int Shoot(Entity shooter, float3 origin, float3 direction)
        {
            float closest = CombatRules.CoverDistance(origin, direction); Entity target = Entity.Null; bool head = false;
            using var query = EntityManager.CreateEntityQuery(typeof(LocalTransform));
            using var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var e in entities)
            {
                if (e == shooter) continue;
                bool human = EntityManager.HasComponent<CombatState>(e), zombie = EntityManager.HasComponent<ZombieState>(e), barrel = EntityManager.HasComponent<BarrelState>(e);
                if (!human && !zombie && !barrel) continue;
                if (human && EntityManager.GetComponentData<CombatState>(e).Life != 0 || zombie && EntityManager.GetComponentData<ZombieState>(e).Alive == 0 || barrel && EntityManager.GetComponentData<BarrelState>(e).Alive == 0) continue;
                var p = EntityManager.GetComponentData<LocalTransform>(e).Position;
                var min = p + new float3(-.3f, 0, -.3f); var max = p + new float3(.3f, barrel ? 1.1f : 1.85f, .3f);
                if (CombatRules.RayBox(origin, direction, min, max, out var distance) && distance < closest)
                { closest = distance; target = e; head = !barrel && (origin + direction * distance).y >= p.y + 1.45f; }
            }
            if (target == Entity.Null) return 0;
            if (EntityManager.HasComponent<ZombieState>(target))
            { var z = EntityManager.GetComponentData<ZombieState>(target); CombatRules.HurtZombie(ref z, head, false); EntityManager.SetComponentData(target, z); }
            else if (EntityManager.HasComponent<CombatState>(target)) HurtPlayer(target, head ? 100 : 34, false);
            else DetonateBarrel(target);
            return head ? 2 : 1;
        }
        private void HurtPlayer(Entity target, int damage, bool infection)
        {
            var state = EntityManager.GetComponentData<CombatState>(target);
            if (CombatRules.DamagePlayer(ref state, damage, infection, out bool spawnInfection))
            {
                EntityManager.SetComponentData(target, state); // death guard precedes creating infection ghost.
                Debug.Log($"[Ferrugem] COMBAT_DEATH owner={Owner(target)} infection={(infection ? 1 : 0)}");
                if (spawnInfection) { SpawnZombie(EntityManager.GetComponentData<LocalTransform>(target).Position); Debug.Log($"[Ferrugem] COMBAT_INFECTION owner={Owner(target)} death={state.DeathSequence}"); }
            }
            EntityManager.SetComponentData(target, state);
        }
        private void DetonateBarrel(Entity barrel)
        {
            var state = EntityManager.GetComponentData<BarrelState>(barrel); if (state.Alive == 0) return;
            state.Alive = 0; EntityManager.SetComponentData(barrel, state);
            Explode(EntityManager.GetComponentData<LocalTransform>(barrel).Position + new float3(0, .7f, 0));
        }
        private void Explode(float3 origin)
        {
            using var query = EntityManager.CreateEntityQuery(typeof(LocalTransform)); using var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var e in entities)
            {
                var p = EntityManager.GetComponentData<LocalTransform>(e).Position + new float3(0, .8f, 0);
                if (math.distance(origin, p) > 4 || !CombatRules.Clear(origin, p)) continue;
                if (EntityManager.HasComponent<CombatState>(e)) HurtPlayer(e, 100, false);
                else if (EntityManager.HasComponent<ZombieState>(e)) { var z = EntityManager.GetComponentData<ZombieState>(e); CombatRules.HurtZombie(ref z, false, true); EntityManager.SetComponentData(e, z); }
                else if (EntityManager.HasComponent<BarrelState>(e)) DetonateBarrel(e);
            }
        }
        private void TickCharges(float dt)
        {
            using var query = EntityManager.CreateEntityQuery(typeof(ChargeState), typeof(LocalTransform)); using var charges = query.ToEntityArray(Allocator.Temp);
            foreach (var e in charges)
            {
                var c = EntityManager.GetComponentData<ChargeState>(e); float before = c.Fuse; c.Fuse -= dt; EntityManager.SetComponentData(e, c);
                if (before > 0 && c.Fuse <= 0) Explode(EntityManager.GetComponentData<LocalTransform>(e).Position);
                if (c.Fuse < -.5f) EntityManager.DestroyEntity(e);
            }
        }
        private void TickZombies(NativeArray<Entity> players, float dt)
        {
            using var query = EntityManager.CreateEntityQuery(typeof(ZombieState), typeof(LocalTransform)); using var zombies = query.ToEntityArray(Allocator.Temp);
            foreach (var e in zombies)
            {
                var z = EntityManager.GetComponentData<ZombieState>(e); if (z.Alive == 0) continue;
                var t = EntityManager.GetComponentData<LocalTransform>(e); z.Recovery = math.max(0, z.Recovery - dt);
                Entity nearest = Entity.Null; float best = 400;
                foreach (var p in players)
                {
                    var state = EntityManager.GetComponentData<CombatState>(p); if (state.Life != 0 || state.ProtectionRemaining > 0) continue;
                    float d = math.distancesq(t.Position, EntityManager.GetComponentData<LocalTransform>(p).Position);
                    if (d < best) { best = d; nearest = p; }
                }
                if (z.AttackRemaining > 0)
                {
                    z.AttackRemaining -= dt;
                    if (z.AttackRemaining <= 0)
                    {
                        if (EntityManager.Exists(z.Target))
                        {
                            var targetPos = EntityManager.GetComponentData<LocalTransform>(z.Target).Position;
                            if (CombatRules.MeleeLands(t.Position, targetPos)) HurtPlayer(z.Target, 100, true);
                        }
                        z.AttackRemaining = 0; z.Recovery = 1;
                    }
                }
                else if (nearest != Entity.Null)
                {
                    var destination = EntityManager.GetComponentData<LocalTransform>(nearest).Position;
                    if (best <= 1.1f * 1.1f && z.Recovery == 0 && CombatRules.Clear(t.Position + new float3(0, 1, 0), destination + new float3(0, 1, 0))) { z.Target = nearest; z.AttackRemaining = .8f; }
                    else if (best > 1.1f * 1.1f)
                    {
                        var direction = Steer(t.Position, destination); t.Position = FpsArena.Move(t.Position, direction * (dt * .9f));
                        if (math.lengthsq(direction) > .001f) t.Rotation = quaternion.LookRotationSafe(direction, math.up());
                    }
                }
                EntityManager.SetComponentData(e, z); EntityManager.SetComponentData(e, t);
            }
        }
        // Visibility graph over inflated obstacle corners: small fixed arena, no NavMesh or Unity Physics dependency.
        private static float3 Steer(float3 from, float3 to)
        {
            if (WalkClear(from, to)) return math.normalizesafe(to - from);
            var points = new float3[10]; points[0] = from; points[1] = to; int index = 2;
            foreach (var o in FpsArena.Obstacles) foreach (int x in new[] { -1, 1 }) foreach (int z in new[] { -1, 1 }) points[index++] = new float3(o.x + x * 1.9f, 0, o.y + z * 1.4f);
            var cost = new float[10]; var previous = new int[10]; var visited = new bool[10]; for (int i = 0; i < 10; i++) { cost[i] = float.MaxValue; previous[i] = -1; } cost[0] = 0;
            for (int step = 0; step < 10; step++)
            {
                int current = -1; for (int i = 0; i < 10; i++) if (!visited[i] && (current < 0 || cost[i] < cost[current])) current = i;
                if (current < 0 || cost[current] == float.MaxValue) break; visited[current] = true; if (current == 1) break;
                for (int j = 0; j < 10; j++) if (!visited[j] && WalkClear(points[current], points[j])) { float candidate = cost[current] + math.distance(points[current], points[j]); if (candidate < cost[j]) { cost[j] = candidate; previous[j] = current; } }
            }
            int next = 1; if (previous[next] < 0) return float3.zero; while (previous[next] > 0) next = previous[next]; return math.normalizesafe(points[next] - from);
        }
        private static bool WalkClear(float3 from, float3 to)
        {
            var delta = to - from; float length = math.length(delta); if (length < .001f) return true;
            foreach (var o in FpsArena.Obstacles) if (CombatRules.RayBox(from, delta / length, new float3(o.x - 1.86f, -1, o.y - 1.36f), new float3(o.x + 1.86f, 2, o.y + 1.36f), out float d) && d < length) return false;
            return true;
        }
    }
}
