using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Ferrugem
{
    // Adapted from Unity's HelloNetcode GoInGameSystem; see third-party notice.
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation)]
    public partial class GoInGameSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<NetworkId>(),
                ComponentType.Exclude<NetworkStreamInGame>()));
        }

        protected override void OnUpdate()
        {
            using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (id, entity) in SystemAPI.Query<NetworkId>()
                         .WithNone<NetworkStreamInGame>().WithEntityAccess())
            {
                UnityEngine.Debug.Log($"[Ferrugem] CONNECTED world={World.Name} networkId={id.Value}");
                commandBuffer.AddComponent<NetworkStreamInGame>(entity);
            }
            commandBuffer.Playback(EntityManager);
        }
    }
}
