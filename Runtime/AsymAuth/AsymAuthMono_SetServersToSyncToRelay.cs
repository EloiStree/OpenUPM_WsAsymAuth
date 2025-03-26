using UnityEngine;
using UnityEngine.Events;
namespace Eloi.WsAsymAuth
{
    public class AsymAuthMono_SetServersToSyncToRelay : MonoBehaviour
    {
        public string m_networkTimeProtocolServer = "apint.ddns.net";
        public string m_websocketServer = "ws://apint.ddns.net:4615/";

        public UnityEvent<string> m_onNetworkTimeProtocolUpdated;
        public UnityEvent<string> m_onWebsocketServerUpdated;

        public bool m_relayAtAwake = true;

        private void Awake()
        {
            if (m_relayAtAwake)
            {
                SetServers(m_networkTimeProtocolServer, m_websocketServer);
            }
        }

        [ContextMenu("RaspberryPi.Local")]
        public void SetWithRaspberryPiLocal()
        {
            SetNetworkTimeProtocol("raspberrypi.local");
            SetWebSocketServer("ws://raspberrypi.local:4615/");
        }
        [ContextMenu("RaspberryPi5.Local")]
        public void SetWithRaspberryPi5Local()
            => SetWithRaspberryPiLocal(5);

        [ContextMenu("RaspberryPi4.Local")]
        public void SetWithRaspberryPi4Local()
                    => SetWithRaspberryPiLocal(4);


        public void SetWithRaspberryPiLocal(int i)
        {
            SetNetworkTimeProtocol($"raspberrypi{i}.local");
            SetWebSocketServer($"ws://raspberrypi{i}.local:4615/");
        }

        [ContextMenu("Server APInt IO")]
        public void SetWithAPIntIO()
        {
            SetNetworkTimeProtocol("apint.ddns.net");
            SetWebSocketServer("ws://apint.ddns.net:4615/");
        }
        [ContextMenu("Server Home APInt IO")]
        public void SetWithAPIntIOHome()
        {
            SetNetworkTimeProtocol("apint-home.ddns.net");
            SetWebSocketServer("ws://apint-home.ddns.net:4615/");
        }
        [ContextMenu("Server Gaming APInt IO")]
        public void SetWithAPIntIOGaming()
        {
            SetNetworkTimeProtocol("apint-gaming.ddns.net");
            SetWebSocketServer("ws://apint-gaming.ddns.net:4615/");
        }

        public void SetNetworkTimeProtocol(string server)
        {
            m_networkTimeProtocolServer = server;
            m_onNetworkTimeProtocolUpdated.Invoke(m_networkTimeProtocolServer);
        }
        public void SetWebSocketServer(string server) {
            m_websocketServer = server;
            m_onWebsocketServerUpdated.Invoke(m_websocketServer);
        }

        public void SetServers(string ntpServer, string websocketServer)
        {
            SetNetworkTimeProtocol(ntpServer);
            SetWebSocketServer(websocketServer);
        }

    }
}
