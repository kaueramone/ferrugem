using System;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Ferrugem
{
    // Connection diagnostics retained alongside the FPS prototype.
    public class FoundationClient : MonoBehaviour
    {
        public static Rect HudRect => new Rect(16, 16, 440, 238);
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
            GUILayout.BeginArea(HudRect, GUI.skin.box);
            GUILayout.Label("FERRUGEM — combate e infectados");
            GUILayout.Label($"Servidor: {FerrugemBootstrap.Endpoint} | Protocolo: {FerrugemBootstrap.Protocol}");
            GUILayout.Label(connected ? "Conectado" : "Desconectado / conectando");
            if (Cursor.lockState != CursorLockMode.Locked && GUILayout.Button(connected ? "Desconectar" : "Reconectar"))
            {
                if (connected) FerrugemBootstrap.RequestDisconnect();
                else FerrugemBootstrap.RequestConnect();
            }
            GUILayout.Label(FpsPresentation.HasPlayer ? "WASD: mover | Shift: correr | Mouse: olhar" : "Aguardando personagem do servidor...");
            GUILayout.Label("Clique: capturar / atirar | Esc: liberar mouse");
            GUILayout.Label("R: recarregar | G: lançar carga explosiva");
            if (FpsPresentation.HasPlayer)
            {
                var combat = FpsPresentation.LocalCombat;
                GUILayout.Label($"Vida {combat.Health} | Munição {combat.Ammo}/6 | Reserva {combat.Reserve} | Cargas {combat.Charges}");
                if (combat.ReloadRemaining > 0) GUILayout.Label($"Recarregando: {combat.ReloadRemaining:F1}s");
                else if (combat.ProtectionRemaining > 0) GUILayout.Label($"Proteção de respawn: {combat.ProtectionRemaining:F1}s");
                else GUILayout.Label("Infectados: cabeça ou explosão. Ataque é fatal.");
            }
            GUILayout.EndArea();
            if (!FpsPresentation.HasPlayer) return;
            if (FpsPresentation.LocalCombat.Life != 0)
            {
                GUI.Box(new Rect(Screen.width / 2f - 210, Screen.height / 2f - 45, 420, 90),
                    $"VOCÊ MORREU\nRespawn em {Mathf.CeilToInt(FpsPresentation.LocalCombat.RespawnRemaining)}s");
            }
            else
            {
                GUI.Label(new Rect(Screen.width / 2f - 4, Screen.height / 2f - 10, 20, 20), "+");
                if (Time.unscaledTime < FpsPresentation.HitFeedbackUntil && FpsPresentation.ConfirmedHit != 0)
                {
                    GUI.color = FpsPresentation.ConfirmedHit == 2 ? new Color(1, 0.72f, 0.35f) : Color.white;
                    GUI.Label(new Rect(Screen.width / 2f - 28, Screen.height / 2f + 20, 150, 25),
                        FpsPresentation.ConfirmedHit == 2 ? "CABEÇA" : "CORPO");
                    GUI.color = Color.white;
                }
            }
        }
    }
}
