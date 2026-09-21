using System;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Scripting;

namespace Ferrugem
{
    // World creation pattern adapted from Unity's HelloNetcode FrontendBootstrap.
    // See Assets/ThirdParty/HelloNetcode/NOTICE.md and LICENSE.md.
    [Preserve]
    public class FerrugemBootstrap : ClientServerBootstrap
    {
        public static World GameWorld { get; private set; }
        public static bool Server { get; private set; }
        public static NetworkEndpoint Endpoint { get; private set; }
        public static int Protocol { get; private set; }

        public override bool Initialize(string defaultWorldName)
        {
            AutoConnectPort = 0;
            try
            {
                Server = Arguments.Has("--server");
#if UNITY_SERVER
                if (Arguments.Has("--client")) throw new ArgumentException("Dedicated server cannot run as a client.");
                Server = true;
#endif
                if (Arguments.Has("--server") && Arguments.Has("--client"))
                    throw new ArgumentException("Choose --server or --client, not both.");
                var port = Arguments.Integer("--port", 7979, 1, 65535);
                var address = Arguments.Value("--address", "127.0.0.1");
                if (!NetworkEndpoint.TryParse(address, (ushort)port, out var endpoint, NetworkFamily.Ipv4))
                    throw new ArgumentException("--address must be an IPv4 address.");
                if (!Server && address == "0.0.0.0")
                    throw new ArgumentException("Client destination cannot be 0.0.0.0.");
                Endpoint = endpoint;
                Protocol = Arguments.Integer("--protocol", 1, 1, int.MaxValue);
                GameWorld = Server ? CreateServerWorld("FerrugemServer") : CreateClientWorld("FerrugemClient");
                var manager = GameWorld.EntityManager;
                manager.CreateSingleton(new GameProtocolVersion { Version = Protocol });
                if (Server)
                {
                    var tickRate = new ClientServerTickRate { SimulationTickRate = 30, NetworkTickRate = 30 };
                    tickRate.ResolveDefaults();
                    manager.CreateSingleton(tickRate);
                    var request = manager.CreateEntity(typeof(NetworkStreamRequestListen), typeof(NetworkStreamRequestListenResult));
                    manager.SetComponentData(request, new NetworkStreamRequestListen { Endpoint = Endpoint });
                }
                else RequestConnect();
                Application.runInBackground = true;
                Application.targetFrameRate = 60;
                Debug.Log($"[Ferrugem] START role={(Server ? "server" : "client")} endpoint={Endpoint} protocol={Protocol} build={Application.version} world={GameWorld.Name}");
            }
            catch (Exception exception)
            {
                Debug.LogError($"[Ferrugem] START_FAILED {exception.Message}");
                Application.Quit(2);
            }
            return true;
        }

        public static void RequestConnect()
        {
            if (Server || GameWorld == null || !GameWorld.IsCreated) return;
            var manager = GameWorld.EntityManager;
            using var connections = manager.CreateEntityQuery(typeof(NetworkStreamConnection));
            using var requests = manager.CreateEntityQuery(typeof(NetworkStreamRequestConnect));
            if (!connections.IsEmptyIgnoreFilter || !requests.IsEmptyIgnoreFilter) return;
            manager.CreateSingleton(new NetworkStreamRequestConnect { Endpoint = Endpoint });
            Debug.Log($"[Ferrugem] CONNECT_REQUEST endpoint={Endpoint}");
        }

        public static void RequestDisconnect()
        {
            if (Server || GameWorld == null || !GameWorld.IsCreated) return;
            var manager = GameWorld.EntityManager;
            using var connections = manager.CreateEntityQuery(typeof(NetworkStreamConnection));
            using var entities = connections.ToEntityArray(Unity.Collections.Allocator.Temp);
            foreach (var entity in entities)
                if (!manager.HasComponent<NetworkStreamRequestDisconnect>(entity))
                    manager.AddComponent<NetworkStreamRequestDisconnect>(entity);
            Debug.Log("[Ferrugem] DISCONNECT_REQUEST");
        }
    }

    public static class Arguments
    {
        public static bool Has(string key) => Array.IndexOf(Environment.GetCommandLineArgs(), key) >= 0;
        public static string Value(string key, string fallback)
        {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, key);
            if (index < 0) return fallback;
            if (index + 1 >= args.Length || args[index + 1].StartsWith("--"))
                throw new ArgumentException($"Missing value for {key}.");
            return args[index + 1];
        }
        public static int Integer(string key, int fallback, int minimum, int maximum)
        {
            if (!int.TryParse(Value(key, fallback.ToString()), out var value) || value < minimum || value > maximum)
                throw new ArgumentException($"Invalid {key}; expected {minimum}..{maximum}.");
            return value;
        }
    }
}
