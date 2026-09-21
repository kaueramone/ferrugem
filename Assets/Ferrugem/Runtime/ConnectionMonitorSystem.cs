using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Ferrugem
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(NetworkReceiveSystemGroup))]
    public partial class ConnectionMonitorSystem : SystemBase
    {
        private int previousCount = -1;
        protected override void OnCreate() => RequireForUpdate<NetworkStreamDriver>();
        protected override void OnUpdate()
        {
            foreach (var connectionEvent in SystemAPI.GetSingleton<NetworkStreamDriver>().ConnectionEventsForTick)
                Debug.Log($"[Ferrugem] CONNECTION_EVENT world={World.Name} {connectionEvent.ToFixedString()}");
            using var connections = EntityManager.CreateEntityQuery(typeof(NetworkId));
            var count = connections.CalculateEntityCount();
            if (count != previousCount)
            {
                Debug.Log($"[Ferrugem] CONNECTION_COUNT world={World.Name} count={count}");
                previousCount = count;
            }
            using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (result, entity) in SystemAPI.Query<NetworkStreamRequestListenResult>().WithEntityAccess())
            {
                if (result.RequestState == NetworkStreamRequestListenResult.State.Pending) continue;
                Debug.Log($"[Ferrugem] LISTEN_RESULT state={result.RequestState} endpoint={result.Endpoint}");
                commandBuffer.RemoveComponent<NetworkStreamRequestListenResult>(entity);
                if (result.RequestState != NetworkStreamRequestListenResult.State.Succeeded) Application.Quit(3);
            }
            commandBuffer.Playback(EntityManager);
        }
    }
}
