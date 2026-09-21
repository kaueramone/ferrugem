using Unity.Entities;
using UnityEngine;

namespace Ferrugem
{
    public class FpsPrefabAuthoring : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        public GameObject ZombiePrefab;
        public GameObject BarrelPrefab;
        public GameObject ChargePrefab;
        class FpsPrefabBaker : Baker<FpsPrefabAuthoring>
        {
            public override void Bake(FpsPrefabAuthoring authoring)
            {
                AddComponent(GetEntity(TransformUsageFlags.None), new FpsPrefab
                { Value = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic) });
                AddComponent(GetEntity(TransformUsageFlags.None), new CombatPrefabs { Zombie = GetEntity(authoring.ZombiePrefab, TransformUsageFlags.Dynamic), Barrel = GetEntity(authoring.BarrelPrefab, TransformUsageFlags.Dynamic), Charge = GetEntity(authoring.ChargePrefab, TransformUsageFlags.Dynamic) });
            }
        }
    }
}
