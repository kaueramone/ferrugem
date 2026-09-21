using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Ferrugem
{
    public struct FpsInput : IInputComponentData
    {
        public float2 Move;
        public float Yaw;
        public float Pitch;
        public byte Sprint;
        public InputEvent Fire;
        public InputEvent Reload;
        public InputEvent Throw;
    }

    public struct FpsPlayer : IComponentData
    {
        [GhostField] public float3 Spawn;
        [GhostField] public float Pitch;
    }

    public struct FpsPrefab : IComponentData { public Entity Value; }
    public struct FpsSpawned : IComponentData { }

    // Shared kinematic arena. All dimensions are metres; movement stays on a flat floor.
    // This intentionally does not yet implement general physics, slopes, jumps or player pushing.
    public static class FpsArena
    {
        public const float Radius = 0.35f;
        public const float Limit = 19.5f;
        public static readonly float2[] Obstacles = { new float2(-4, 0), new float2(4, 0) };
        public static readonly float2 ObstacleHalfSize = new float2(1.5f, 1);
        public static float3 Spawn(int id) => new float3((id % 6 - 2.5f) * 2, 0, -9);

        public static float3 Move(float3 current, float3 delta)
        {
            var next = current;
            next.x = math.clamp(current.x + delta.x, -Limit + Radius, Limit - Radius);
            foreach (var obstacle in Obstacles)
            {
                var half = ObstacleHalfSize + Radius;
                if (math.abs(current.z - obstacle.y) < half.y && math.abs(next.x - obstacle.x) < half.x)
                    next.x = delta.x > 0 ? obstacle.x - half.x : delta.x < 0 ? obstacle.x + half.x : current.x;
            }
            next.z = math.clamp(current.z + delta.z, -Limit + Radius, Limit - Radius);
            foreach (var obstacle in Obstacles)
            {
                var half = ObstacleHalfSize + Radius;
                if (math.abs(next.x - obstacle.x) < half.x && math.abs(next.z - obstacle.y) < half.y)
                    next.z = delta.z > 0 ? obstacle.y - half.y : delta.z < 0 ? obstacle.y + half.y : current.z;
            }
            next.y = 0;
            return next;
        }
    }

    public class FpsPlayerAuthoring : MonoBehaviour
    {
        class FpsPlayerBaker : Baker<FpsPlayerAuthoring>
        {
            public override void Bake(FpsPlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<FpsPlayer>(entity);
                AddComponent<FpsInput>(entity);
                AddComponent(entity, CombatState.Fresh);
            }
        }
    }
}
