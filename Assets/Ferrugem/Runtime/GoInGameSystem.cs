using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Ferrugem
{
    public struct FpsReadyRpc : IRpcCommand { }
    public struct ConnectionLogged : IComponentData { }

    // Unity HelloNetcode ready-RPC pattern: snapshots start only after the client's ghost prefab loaded.
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation)]
    public partial class GoInGameSystem : SystemBase
    {
        private bool Ready(Entity prefab) => EntityManager.Exists(prefab) && EntityManager.HasComponent<GhostType>(prefab);
        protected override void OnCreate() { RequireForUpdate<NetworkId>(); }
        protected override void OnUpdate()
        {
            using var buffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (id, entity) in SystemAPI.Query<NetworkId>().WithNone<ConnectionLogged>().WithEntityAccess())
            {
                UnityEngine.Debug.Log($"[Ferrugem] CONNECTED world={World.Name} networkId={id.Value}");
                buffer.AddComponent<ConnectionLogged>(entity);
            }
            if (SystemAPI.TryGetSingleton<FpsPrefab>(out var prefab) && EntityManager.Exists(prefab.Value)
                && EntityManager.HasComponent<GhostType>(prefab.Value)
                && SystemAPI.TryGetSingleton<CombatPrefabs>(out var combat) && Ready(combat.Zombie) && Ready(combat.Barrel) && Ready(combat.Charge))
            {
                if (World.IsClient())
                {
                    foreach (var (_, connection) in SystemAPI.Query<NetworkId>().WithNone<NetworkStreamInGame>().WithEntityAccess())
                    {
                        buffer.AddComponent<NetworkStreamInGame>(connection);
                        var request = buffer.CreateEntity();
                        buffer.AddComponent<FpsReadyRpc>(request);
                        buffer.AddComponent(request, new SendRpcCommandRequest { TargetConnection = connection });
                    }
                }
                else
                {
                    foreach (var (request, entity) in SystemAPI.Query<ReceiveRpcCommandRequest>().WithAll<FpsReadyRpc>().WithEntityAccess())
                    {
                        var connection = request.SourceConnection;
                        if (EntityManager.Exists(connection) && EntityManager.HasComponent<NetworkId>(connection)
                            && !EntityManager.HasComponent<NetworkStreamInGame>(connection))
                            buffer.AddComponent<NetworkStreamInGame>(connection);
                        buffer.DestroyEntity(entity);
                    }
                }
            }
            buffer.Playback(EntityManager);
        }
    }
}
