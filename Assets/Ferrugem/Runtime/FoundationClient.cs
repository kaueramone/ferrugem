using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Ferrugem
{
    // Temporary connection diagnostics; no gameplay or account system.
    public class FoundationClient : MonoBehaviour
    {
        private float quitAfter;
        private float cycleAfter;
        private float reconnectDelay;
        private float connectedAt = -1;
        private float disconnectedAt = -1;
        private bool cycleStarted;
        private bool reconnectRequested;
        private bool cycleCompleted;
        private bool connected;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Create() => new GameObject("Connection Diagnostics").AddComponent<FoundationClient>();

        private void Awake()
        {
            try
            {
                quitAfter = Arguments.Integer("--quit-after", 0, 0, 86400);
                cycleAfter = Arguments.Integer("--cycle-after", 0, 0, 3600);
                reconnectDelay = Arguments.Integer("--reconnect-delay", 2, 1, 3600);
            }
            catch (Exception exception) { Debug.LogError(exception.Message); Application.Quit(2); }
        }

        private void Update()
        {
            var now = Time.realtimeSinceStartup;
            if (quitAfter > 0 && now >= quitAfter)
            {
                if (cycleAfter > 0 && !cycleCompleted)
                {
                    Debug.LogError("[Ferrugem] CYCLE_INCOMPLETE");
                    Application.Quit(4);
                }
                else Application.Quit();
                return;
            }
            var world = FerrugemBootstrap.GameWorld;
            if (world == null || !world.IsCreated || FerrugemBootstrap.Server) return;
            using var query = world.EntityManager.CreateEntityQuery(typeof(NetworkId));
            connected = !query.IsEmptyIgnoreFilter;
            if (reconnectRequested && connected && !cycleCompleted)
            {
                cycleCompleted = true;
                Debug.Log("[Ferrugem] CYCLE_COMPLETE connected-disconnected-reconnected");
            }
            if (connected && connectedAt < 0) connectedAt = now;
            if (cycleAfter > 0 && connectedAt >= 0 && !cycleStarted && now - connectedAt >= cycleAfter)
            {
                cycleStarted = true;
                FerrugemBootstrap.RequestDisconnect();
            }
            if (cycleStarted && !connected && disconnectedAt < 0) disconnectedAt = now;
            if (disconnectedAt >= 0 && !reconnectRequested && now - disconnectedAt >= reconnectDelay)
            {
                reconnectRequested = true;
                FerrugemBootstrap.RequestConnect();
            }
        }

        private void OnGUI()
        {
            if (FerrugemBootstrap.Server) return;
            GUILayout.BeginArea(new Rect(20, 20, 420, 210), GUI.skin.box);
            GUILayout.Label("FERRUGEM — teste de conexão");
            GUILayout.Label($"Servidor: {FerrugemBootstrap.Endpoint} | Protocolo: {FerrugemBootstrap.Protocol}");
            GUILayout.Label(connected ? "Conectado" : "Desconectado / conectando");
            if (GUILayout.Button(connected ? "Desconectar" : "Reconectar"))
            {
                if (connected) FerrugemBootstrap.RequestDisconnect();
                else FerrugemBootstrap.RequestConnect();
            }
            GUILayout.Label("Fundação técnica. Ainda não há gameplay.");
            GUILayout.EndArea();
        }
    }
}
