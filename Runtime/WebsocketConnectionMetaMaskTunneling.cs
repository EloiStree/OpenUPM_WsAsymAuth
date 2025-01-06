using System;
using System.Net.WebSockets;
using UnityEngine;
using static MetaMaskTunneling.WebSocketConnectionState;

public partial class MetaMaskTunneling
{
    [System.Serializable]
    public class WebsocketConnectionMetaMaskTunneling
    {
        public IMaskSignerCliboardable m_messageSigner;
        public WebSocketConnectionState m_connection=new();
        public HandshakeConnectionState m_handshake=new ();

        [TextArea(0,10)]
        public string m_signatureSample="";

        [Header("Debuggers")]
        public DebugTime m_timeDebug = new();
        public DebugByteCount m_byteCountDebug = new();
        public DebugRunningState m_runningState = new();

        public TraffictInEvent m_trafficEvent = new();
        public TrafficOutQueue m_pushInTunnel = new();


         ~WebsocketConnectionMetaMaskTunneling()
        {
            CloseTunnel();
        }

        public bool HasServerAnsweredToHelloHandshake()
        {
            return
                m_handshake!=null 
                && m_handshake.m_signInMessage!=null 
                && m_handshake.m_signInMessage.Length>0;       
        }

        public void CloseTunnel()
        {
            m_connection.KillIt();
        }

        public bool IsStillRunning()
        {
            return m_connection.IsStillRunning();
        }

        public void UpdateRunningStateInfo()
        {
            m_runningState.isRunningHandleTask = m_connection.m_runningThread != null && !m_connection.m_runningThread.IsCompleted;
            m_runningState.isRunningListener = m_connection.m_runningListener != null && !m_connection.m_runningListener.IsCompleted;
            m_runningState.isWebsocketConnected = m_connection.m_websocket != null;
            if (m_connection.m_websocket != null)
                m_runningState.m_websocketState = m_connection.m_websocket.State;
            else m_runningState.m_websocketState = WebSocketState.None;
            m_runningState.isStillRunning = m_connection.IsStillRunning();
        }

        public void SetConnectionInfo(string serverURI, IMaskSignerCliboardable signer)
        {
            m_connection.m_serverUri= serverURI;
            m_messageSigner = signer;
            m_messageSigner.GetClipboardSignedMessage("Hello Tunnel", out m_signatureSample);
        }

        public void StartConnection()
        {
            if (!HasStarted()) { 
                EthMaskTunnelingTaskRunUtility.StartRunnningTunnel(this);
            }

        }
       

        public bool HasStarted()
        {
            LaunchState state = m_connection.m_launchState;
            return state != LaunchState.WaitingToBeLaunched;
        }
        public bool IsReadyToBeUsed() {

            LaunchState state = m_connection.m_launchState;
            return state == LaunchState.ReadyToBeUsed;
        }

        public void EnqueueTextMessages(string textToSend)
        {
            m_pushInTunnel.m_toSendToTheServerUTF8.Enqueue(textToSend);
            
        }

        public void EnqueueBinaryMessages(byte[] bytesToSend)
        {
            m_pushInTunnel.m_toSendToTheServerBytes.Enqueue(bytesToSend);
        }

        

        public bool IsInMustBeKillMode()
        {
            return m_connection.m_killSwitch.m_needToBeKilled;
        }

        public bool IsConnectedAndHandShakeVerified()
        {
            return m_handshake.m_connectionEstablishedAndVerified;
        }

        public void NotifyAsTunnelEnded()
        {
            if (m_trafficEvent.m_onConnectionLost != null)
            m_trafficEvent.m_onConnectionLost.Invoke();
        }

        public void NotifyAsConnectedAndVerified()
        {
            if (m_trafficEvent.m_onConnectionSignedAndValidated != null)
            m_trafficEvent.m_onConnectionSignedAndValidated.Invoke();
        }

        public void NotifyAsWebsocketInOpenState()
        {
            if (m_trafficEvent.m_onWebsocketConnected != null)
            m_trafficEvent.m_onWebsocketConnected.Invoke();
        }

        public void EnqueueInteger(int integerValue)
        {
            byte[] iBytes = BitConverter.GetBytes(integerValue);
            EnqueueBinaryMessages(iBytes);
        }

        public void PushClampedBytesAsIID(byte[] bytes)
        {
            if (bytes == null) return;

            if (bytes.Length == 4 || bytes.Length == 8 || bytes.Length == 12 || bytes.Length == 16) { 
                EnqueueBinaryMessages(bytes);
            }
            else if (bytes.Length > 16)
            {
                byte[] b = new byte[16];
                Array.Copy(bytes, 0, b, 0, 16);
                EnqueueBinaryMessages(b);
            }
        }

        public bool HasIndexLock() {
            return m_handshake.m_receiveGivenIndexLock;
        }
        public int GetIndexLock()
        {
            return m_handshake.m_givenIndexLock;
        }

        public bool IsDisconnected()
        {
            return !m_runningState.isWebsocketConnected;
        }

        public bool IsConnected()
        {
            return m_runningState.isWebsocketConnected;
        }

        public string GetPublicAddress()
        {
            return m_handshake.m_inNameOfPublicAddress;
        }
        public bool HasCoasterAddress()
        {
            return string.IsNullOrEmpty(GetCoasterPublicAddress());
        }
        public string GetCoasterPublicAddress()
        {
            return m_handshake.m_coasterPublicAddress;
        }
    }
}
