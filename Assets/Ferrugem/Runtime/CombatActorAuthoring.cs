using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Ferrugem
{
    public struct CombatState : IComponentData
    {
        [GhostField] public int Health;
        [GhostField] public int Ammo;
        [GhostField] public int Reserve;
        [GhostField] public int Charges;
        [GhostField] public int Life;
        [GhostField] public float RespawnRemaining;
        [GhostField] public float ProtectionRemaining;
        [GhostField] public float ReloadRemaining;
        [GhostField] public int ShotSequence;
        [GhostField] public int LastHit;
        [GhostField] public int DeathSequence;
        public float Cooldown;
        public static CombatState Fresh => new CombatState { Health = 100, Ammo = 6, Reserve = 30, Charges = 2, ProtectionRemaining = 3 };
    }
    public struct ZombieState : IComponentData
    {
        [GhostField] public int Alive;
        [GhostField] public float AttackRemaining;
        public Entity Target;
        public float Recovery;
    }
    public struct BarrelState : IComponentData { [GhostField] public int Alive; }
    public struct ChargeState : IComponentData { [GhostField] public float Fuse; }
    public struct CombatPrefabs : IComponentData { public Entity Zombie; public Entity Barrel; public Entity Charge; }
    public class CombatActorAuthoring : MonoBehaviour
    {
        public int Kind;
        class CombatActorBaker : Baker<CombatActorAuthoring>
        {
            public override void Bake(CombatActorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                if (authoring.Kind == 0) AddComponent(entity, new ZombieState { Alive = 1 });
                else if (authoring.Kind == 1) AddComponent(entity, new BarrelState { Alive = 1 });
                else AddComponent(entity, new ChargeState { Fuse = 1.5f });
            }
        }
    }
}
