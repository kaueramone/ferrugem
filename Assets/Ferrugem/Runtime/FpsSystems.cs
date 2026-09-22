using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Ferrugem
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateAfter(typeof(GoInGameSystem))]
    public partial class FpsSpawnSystem : SystemBase
    {
        protected override void OnCreate() => RequireForUpdate<FpsPrefab>();
        protected override void OnUpdate()
        {
            var prefab = SystemAPI.GetSingleton<FpsPrefab>().Value;
            using var buffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (id, connection) in SystemAPI.Query<NetworkId>().WithAll<NetworkStreamInGame>()
                         .WithNone<FpsSpawned>().WithEntityAccess())
            {
                var player = buffer.Instantiate(prefab);
                buffer.SetComponent(player, new GhostOwner { NetworkId = id.Value });
                var spawn = FpsArena.Spawn(id.Value);
                buffer.SetComponent(player, LocalTransform.FromPosition(spawn));
                buffer.SetComponent(player, new FpsPlayer { Spawn = spawn, Grounded = 1 });
                buffer.AppendToBuffer(connection, new LinkedEntityGroup { Value = player });
                buffer.AddComponent<FpsSpawned>(connection);
                Debug.Log($"[Ferrugem] FPS_SPAWN owner={id.Value}");
            }
            buffer.Playback(EntityManager);
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class FpsGatherInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var input in SystemAPI.Query<RefRW<FpsInput>>().WithAll<GhostOwnerIsLocal>())
                input.ValueRW = FpsPresentation.ConsumeInput();
        }
    }

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial class FpsMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var dt = SystemAPI.Time.DeltaTime;
            foreach (var (input, transform, player, combat) in SystemAPI.Query<RefRO<FpsInput>, RefRW<LocalTransform>, RefRW<FpsPlayer>, RefRO<CombatState>>()
                         .WithAll<Simulate>())
            {
                if (combat.ValueRO.Life != 0) continue;
                var command = input.ValueRO;
                if (!math.all(math.isfinite(command.Move)) || !math.isfinite(command.Yaw) || !math.isfinite(command.Pitch))
                    continue;
                var motor = player.ValueRO;
                var position = transform.ValueRO.Position;
                FpsMotor.Step(ref motor, ref position, command, dt);
                player.ValueRW = motor;
                transform.ValueRW.Position = position;
                transform.ValueRW.Rotation = quaternion.RotateY(math.radians(command.Yaw % 360));
            }
        }
    }

    // Diagnostic assertions observe authoritative server movement and replicated remote movement.
    // No positions are supplied by the smoke client: it sends the same inputs as a keyboard.
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial class FpsDiagnosticsSystem : SystemBase
    {
        private readonly HashSet<int> moved = new HashSet<int>();
        private readonly HashSet<int> present = new HashSet<int>();
        private readonly HashSet<int> seen = new HashSet<int>();
        private int lastCount = -1;
        private bool smoke;
        protected override void OnCreate() { smoke = Arguments.Has("--fps-smoke"); }
        protected override void OnUpdate()
        {
            if (!smoke) return;
            present.Clear();
            var local = World.IsClient() && SystemAPI.TryGetSingleton<NetworkId>(out var id) ? id.Value : -1;
            int count = 0;
            foreach (var (owner, transform, player) in SystemAPI.Query<RefRO<GhostOwner>, RefRO<LocalTransform>, RefRO<FpsPlayer>>())
            {
                ++count;
                var ownerId = owner.ValueRO.NetworkId;
                if (!present.Add(ownerId)) Debug.LogError($"[Ferrugem] FPS_DUPLICATE owner={ownerId}");
                if (seen.Add(ownerId)) Debug.Log($"[Ferrugem] FPS_OBSERVED world={World.Name} owner={ownerId} local={(ownerId == local ? 1 : 0)}");
                var distance = math.distance(transform.ValueRO.Position, player.ValueRO.Spawn);
                if (distance > 1 && moved.Add(ownerId))
                    Debug.Log($"[Ferrugem] FPS_MOVED world={World.Name} owner={ownerId} local={(ownerId == local ? 1 : 0)} distance={distance:F2}");
                if (math.abs(transform.ValueRO.Position.x) > FpsArena.Limit || math.abs(transform.ValueRO.Position.z) > FpsArena.Limit)
                    Debug.LogError("[Ferrugem] FPS_OUT_OF_BOUNDS");
            }
            foreach (var old in seen)
                if (!present.Contains(old)) Debug.Log($"[Ferrugem] FPS_DESPAWN world={World.Name} owner={old}");
            seen.RemoveWhere(owner => !present.Contains(owner));
            moved.RemoveWhere(owner => !present.Contains(owner));
            if (lastCount != count)
            {
                lastCount = count;
                Debug.Log($"[Ferrugem] FPS_COUNT world={World.Name} count={count}");
            }
        }
    }
}
