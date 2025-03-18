using UnityEngine;
namespace Eloi.WsMetaMaskAuth
{
    public class MaskAuthMono_SetServersOnlineOffline : MonoBehaviour {

        public MaskAuthMono_SetServersToSyncToRelay m_setServer;
        public ConnectionType m_connectionType = ConnectionType.ONLINE_IF_INTERNET_THEN_LAN;
        public TargetServer m_lanServer = new TargetServer("LAN LOCAL PI", "raspberrypi.local","ws://raspberrypi.local:4615/");
        public TargetServer m_onlineServer = new TargetServer("ONLINE PI", "apint.ddns.net", "ws://apint.ddns.net:4615/");
        public bool m_setAtAwake = true;
        public bool m_isInternetReachable = false;
        public TargetServer m_selected;
        public enum ConnectionType { 
        
            LAN,
            ONLINE,
            ONLINE_IF_INTERNET_THEN_LAN
        }

        [System.Serializable]
        public class TargetServer {
            public string m_description="Server LAN";
            public string m_ntpServer= "raspberrypi.local";
            public string m_websocketServer= "ws://raspberrypi.local:4615";
            public TargetServer() { }
            public TargetServer(string description, string ntpServer, string websocketServer)
            {
                m_description = description;
                m_ntpServer = ntpServer;
                m_websocketServer = websocketServer;
            }
        }

        private void Awake()
        {
            if (m_setAtAwake)
            {
                RefreshWithInspectorValue();
            }
        }

        public void SetInModeLAN() {

            m_connectionType = ConnectionType.LAN;
            RefreshWithInspectorValue();
        }
        public void SetInModeOnline()
        {
            m_connectionType = ConnectionType.ONLINE;
            RefreshWithInspectorValue();
        }
        public void SetInModeOnlineIfInternetThenLAN()
        {
            m_connectionType = ConnectionType.ONLINE_IF_INTERNET_THEN_LAN;
            RefreshWithInspectorValue();
        }
        public void OverrideLAN(string ntpServer, string websocketServer)
        {
            m_lanServer.m_ntpServer = ntpServer;
            m_lanServer.m_websocketServer = websocketServer;
            RefreshWithInspectorValue();
        }
        public void OverrideOnline(string ntpServer, string websocketServer)
        {
            m_onlineServer.m_ntpServer = ntpServer;
            m_onlineServer.m_websocketServer = websocketServer;
            RefreshWithInspectorValue();
        }

        [ContextMenu("Refresh with inspector value")]
        public void RefreshWithInspectorValue()
        {
            m_isInternetReachable = Application.internetReachability != NetworkReachability.NotReachable;
            if (m_connectionType == ConnectionType.LAN)
            {
                m_setServer.SetServers(m_lanServer.m_ntpServer, m_lanServer.m_websocketServer);
                m_selected = m_lanServer;
            }
            else if (ConnectionType.ONLINE == m_connectionType)
            {
                m_setServer.SetServers(m_onlineServer.m_ntpServer, m_onlineServer.m_websocketServer);
                m_selected = m_onlineServer;
            }
            else if (ConnectionType.ONLINE_IF_INTERNET_THEN_LAN == m_connectionType)
            {
                if (!m_isInternetReachable)
                {
                    m_setServer.SetServers(m_lanServer.m_ntpServer, m_lanServer.m_websocketServer);
                    m_selected = m_lanServer;
                }
                else
                {
                    m_setServer.SetServers(m_onlineServer.m_ntpServer, m_onlineServer.m_websocketServer);
                    m_selected = m_onlineServer;
                }
            }
            
        }
    }
}
