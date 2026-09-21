using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
namespace Ferrugem
{
    public static class CombatSmoke
    {
        public static void Run()
        {
            int count = 0;
            void Check(string name, bool pass) { if (!pass) throw new InvalidOperationException("COMBAT_SUITE_FAIL " + name); count++; Debug.Log($"[Ferrugem] COMBAT_CHECK name={name} pass=1"); }
            var z = new ZombieState { Alive = 1 }; Check("body_immune", !CombatRules.HurtZombie(ref z, false, false) && z.Alive == 1);
            Check("head_kill", CombatRules.HurtZombie(ref z, true, false) && z.Alive == 0);
            z.Alive = 1; Check("explosion_kill", CombatRules.HurtZombie(ref z, false, true) && z.Alive == 0);
            var p = CombatState.Fresh; p.ProtectionRemaining = 0;
            int infections = 0; CombatRules.DamagePlayer(ref p, 100, true, out bool infected); if (infected) infections++; CombatRules.DamagePlayer(ref p, 100, true, out infected); if (infected) infections++;
            Check("infection_once", infections == 1 && p.DeathSequence == 1 && p.Life == 1);
            Check("respawn_human", CombatRules.Respawn(ref p, 4) && p.Health == 100 && p.Life == 0 && p.ProtectionRemaining == 3);
            p.ProtectionRemaining = 0; Check("pvp_no_infection", CombatRules.DamagePlayer(ref p, 100, false, out infected) && !infected);
            Check("cover_blocks", !CombatRules.Clear(new float3(-4, 1.65f, -4), new float3(-4, 1.65f, 4)) && CombatRules.Clear(new float3(0, 1.65f, -4), new float3(0, 1.65f, 4)));
            p = CombatState.Fresh; Check("shot_cooldown", CombatRules.SpendShot(ref p) && !CombatRules.SpendShot(ref p) && p.Ammo == 5);
            p.Ammo = 0; p.Cooldown = 0; Check("empty_clip", !CombatRules.SpendShot(ref p));
            bool begun = CombatRules.BeginReload(ref p); CombatRules.Tick(ref p, 2); Check("reload", begun && p.Ammo == 6 && p.Reserve == 24);
            p = CombatState.Fresh; Check("spawn_protection", !CombatRules.Hurt(ref p, 100) && p.Health == 100);
                        Check("melee_range", CombatRules.MeleeLands(float3.zero, new float3(0, 0, 1)) && !CombatRules.MeleeLands(float3.zero, new float3(0, 0, 1.3f)));
            Check("melee_cover", !CombatRules.MeleeLands(new float3(-4, 0, -1.1f), new float3(-4, 0, -.1f)));
            var ray = CombatRules.Direction(0, 0);
            Check("ray_geometry", CombatRules.RayBox(new float3(0, 1.65f, -5), ray, new float3(-.3f, 0, -.3f), new float3(.3f, 1.85f, .3f), out float hitDistance) && hitDistance > 4 && hitDistance < 5);
            Debug.Log($"[Ferrugem] COMBAT_SUITE_PASS count={count}");
        }
    }
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial class CombatDiagnosticsSystem : SystemBase
    {
        private readonly Dictionary<Entity, CombatState> seen = new Dictionary<Entity, CombatState>();
        private bool smoke; private int zombieCount = -1; private int aliveCount = -1;
        protected override void OnCreate() { smoke = Arguments.Has("--combat-smoke"); }
        protected override void OnUpdate()
        {
            if (!smoke) return;
            int localId = SystemAPI.TryGetSingleton<NetworkId>(out var id) ? id.Value : -1;
            using var query = EntityManager.CreateEntityQuery(typeof(CombatState), typeof(GhostOwner)); using var players = query.ToEntityArray(Allocator.Temp);
            var present = new HashSet<Entity>();
            foreach (var e in players)
            {
                present.Add(e); var c = EntityManager.GetComponentData<CombatState>(e); int owner = EntityManager.GetComponentData<GhostOwner>(e).NetworkId; int local = owner == localId ? 1 : 0;
                if (!seen.TryGetValue(e, out var before)) Debug.Log($"[Ferrugem] COMBAT_OBSERVED owner={owner} local={local} health={c.Health} ammo={c.Ammo}");
                if (c.ShotSequence > before.ShotSequence) Debug.Log($"[Ferrugem] COMBAT_SHOT_OBSERVED owner={owner} local={local} ammo={c.Ammo}");
                if (c.Ammo > before.Ammo && before.ShotSequence > 0) Debug.Log($"[Ferrugem] COMBAT_RELOAD_OBSERVED owner={owner} ammo={c.Ammo}");
                if (c.Life == 1 && before.Life == 0) Debug.Log($"[Ferrugem] COMBAT_DEATH_OBSERVED owner={owner}");
                if (c.Life == 0 && before.Life == 1) Debug.Log($"[Ferrugem] COMBAT_RESPAWN_OBSERVED owner={owner}");
                seen[e] = c;
            }
            foreach (var e in new List<Entity>(seen.Keys)) if (!present.Contains(e)) seen.Remove(e);
            if (localId < 0) { zombieCount = -1; aliveCount = -1; return; }
            using var zombies = EntityManager.CreateEntityQuery(typeof(ZombieState)); using var states = zombies.ToComponentDataArray<ZombieState>(Allocator.Temp); int alive = 0; foreach (var state in states) alive += state.Alive;
            if (states.Length > 0 && states.Length != zombieCount) { Debug.Log($"[Ferrugem] COMBAT_ZOMBIES_OBSERVED count={states.Length}"); if (zombieCount > 0 && states.Length > zombieCount) Debug.Log("[Ferrugem] COMBAT_INFECTION_OBSERVED"); zombieCount = states.Length; }
            int dead = states.Length - alive; if (aliveCount >= 0 && dead > aliveCount) Debug.Log("[Ferrugem] COMBAT_ZOMBIE_DEATH_OBSERVED"); aliveCount = dead;
        }
    }
}
