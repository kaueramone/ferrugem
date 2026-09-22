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
        public byte Crouch;
        public byte Aim;
        public InputEvent Jump;
        public InputEvent Fire;
        public InputEvent Reload;
        public InputEvent Throw;
    }
    public struct FpsPlayer : IComponentData
    {
        [GhostField] public float3 Spawn;
        [GhostField] public float Pitch;
        [GhostField] public float3 Velocity;
        [GhostField] public byte Grounded;
        [GhostField] public byte Crouched;
        [GhostField] public byte Aiming;
    }
    public struct FpsPrefab : IComponentData { public Entity Value; }
    public struct FpsSpawned : IComponentData { }
    public struct ArenaSolid
    {
        public string Name;
        public float3 Min, Max;
        public ArenaSolid(string name, float3 min, float3 max) { Name = name; Min = min; Max = max; }
    }
    // Analytic prototype geometry, shared by motor, bullets, AI and the rendered arena.
    public static class FpsArena
    {
        public const float Radius = .35f, Limit = 19.5f;
        public static readonly float2[] Obstacles = { new float2(-4, 0), new float2(4, 0) };
        public static readonly float2 ObstacleHalfSize = new float2(1.5f, 1);
        public static readonly float2 RampMinXZ = new float2(10, -6), RampMaxXZ = new float2(14, -2);
        public const float RampHeight = 1.2f;
        public static float RampSlopeDegrees => math.degrees(math.atan(RampHeight / (RampMaxXZ.y - RampMinXZ.y)));
        public static readonly ArenaSolid[] Solids = {
            new ArenaSolid("Cobertura oeste", new float3(-5.5f,0,-1), new float3(-2.5f,2.4f,1)),
            new ArenaSolid("Cobertura leste", new float3(2.5f,0,-1), new float3(5.5f,2.4f,1)),
            new ArenaSolid("Degrau 1",new float3(-14,0,-6),new float3(-10,.25f,-5)),
            new ArenaSolid("Degrau 2",new float3(-14,0,-5),new float3(-10,.5f,-4)),
            new ArenaSolid("Degrau 3",new float3(-14,0,-4),new float3(-10,.75f,-3)),
            new ArenaSolid("Degrau 4",new float3(-14,0,-3),new float3(-10,1,-2)),
            new ArenaSolid("Patamar dos degraus",new float3(-14,0,-2),new float3(-10,1,0)),
            new ArenaSolid("Patamar da rampa",new float3(10,0,-2),new float3(14,1.2f,0)),
            new ArenaSolid("Teto baixo",new float3(-2,1.25f,4),new float3(2,1.55f,8))
        };
        public static float3 Spawn(int id) => new float3((id % 6 - 2.5f) * 2, 0, -9);
        public static bool Inside(float2 p, float2 min, float2 max, float radius = 0) => math.all(p > min - radius) && math.all(p < max + radius);
        public static float RampSurface(float z) => math.clamp((z - RampMinXZ.y) / (RampMaxXZ.y - RampMinXZ.y), 0, 1) * RampHeight;
        public static float Ground(float3 p, float maximum, float radius = Radius)
        {
            float ground = 0;
            foreach (var solid in Solids)
                if (solid.Max.y <= maximum + .002f && Inside(p.xz, solid.Min.xz, solid.Max.xz, radius)) ground = math.max(ground, solid.Max.y);
            if (Inside(p.xz, RampMinXZ, RampMaxXZ, radius))
            {
                float ramp = RampSurface(p.z); if (FpsMotor.WalkableSlope(RampSlopeDegrees) && ramp <= maximum + .002f) ground = math.max(ground, ramp);
            }
            return ground;
        }
        public static bool Fits(float3 p, float height)
        {
            foreach (var solid in Solids)
                if (Inside(p.xz, solid.Min.xz, solid.Max.xz, Radius) && p.y < solid.Max.y - .002f && p.y + height > solid.Min.y + .002f) return false;
            return !Inside(p.xz, RampMinXZ, RampMaxXZ, Radius) || p.y >= RampSurface(p.z) - .002f;
        }
        public static float3 Horizontal(float3 current, float3 delta, float height, bool grounded)
        {
            var next = current;
            for (int axis = 0; axis < 2; axis++)
            {
                int index = axis == 0 ? 0 : 2; var candidate = next;
                candidate[index] = math.clamp(candidate[index] + delta[index], -Limit + Radius, Limit - Radius);
                if (grounded)
                {
                    float support = Ground(candidate, next.y + FpsMotor.StepHeight);
                    if (support > candidate.y) candidate.y = support;
                }
                if (Fits(candidate, height)) next = candidate;
            }
            return next;
        }
        public static float Ceiling(float3 p, float headBefore, float headAfter)
        {
            float ceiling = headAfter;
            foreach (var solid in Solids)
                if (Inside(p.xz, solid.Min.xz, solid.Max.xz, Radius) && solid.Min.y >= headBefore - .002f && solid.Min.y < ceiling) ceiling = solid.Min.y;
            return ceiling;
        }
        // Ground-following AI does not jump or crouch and cannot pass the low tunnel.
        public static float3 Move(float3 current, float3 delta)
        {
            var result = Horizontal(current, delta, FpsMotor.StandingHeight, true);
            result.y = Ground(result, result.y + FpsMotor.StepHeight); return result;
        }
    }
    public class FpsPlayerAuthoring : MonoBehaviour
    {
        class FpsPlayerBaker : Baker<FpsPlayerAuthoring>
        {
            public override void Bake(FpsPlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FpsPlayer { Grounded = 1 });
                AddComponent<FpsInput>(entity);
                AddComponent(entity, CombatState.Fresh);
            }
        }
    }
}
