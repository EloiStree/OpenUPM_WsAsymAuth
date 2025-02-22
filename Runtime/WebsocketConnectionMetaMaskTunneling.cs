

using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using UnityEngine;
using static Eloi.WsMetaMaskAuth.MetaMaskTunneling.WebSocketConnectionState;


namespace Eloi.WsMetaMaskAuth
{

    public class MetaMaskTunneling
{
    [System.Serializable]
    public class WebsocketConnectionMetaMaskTunneling
    {
        public IMaskSignerCliboardable m_messageSigner;
        public WebSocketConnectionState m_connection=new();
        public HandshakeConnectionState m_handshake=new();

        [TextArea(0,10)]
        public string m_signatureSample="";

        [Header("Debuggers")]
        public DebugTime m_timeDebug = new();
        public DebugByteCount m_byteCountDebug = new();
        public DebugRunningState m_runningState = new();

        public TraffictInEvent m_trafficEvent = new();
        public TrafficOutQueue m_pushInTunnel = new();
        [Tooltip("If the server allows SHA256 connection.")]
        public string m_passwordSha256;

        //public string pfxText; I AM HERE

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
            if (m_messageSigner != null)
                m_messageSigner.GetClipboardSignedMessage("Hello Tunnel", out m_signatureSample);
        }
        public void SetPasswordSHA256(string sha256SignIn)
        {
            m_passwordSha256= sha256SignIn;
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
        [System.Serializable]
        public class TrafficOutQueue
        {
            /// QUEUE
            public Queue<string> m_toSendToTheServerUTF8 = new Queue<string>();
            public Queue<byte[]> m_toSendToTheServerBytes = new Queue<byte[]>();
        }
        [System.Serializable]

        public class TraffictInEvent
        {
            public delegate void OnMessageReceivedText(string message);
            public delegate void OnMessageReceivedBinary(byte[] message);
            public OnMessageReceivedText m_onThreadMessageReceivedText = null;
            public OnMessageReceivedBinary m_onThreadMessageReceivedBinary = null;
            public delegate void OnMessagePushedText(string message);
            public delegate void OnMessagePushedBinary(byte[] message);
            public OnMessagePushedText m_onThreadMessagePushedText = null;
            public OnMessagePushedBinary m_onThreadMessagePushedBinary = null;

            public Action m_onWebsocketConnected;
            public Action m_onConnectionSignedAndValidated;
            public Action m_onConnectionLost;
            public Action<int> m_onIndexLockChanged;
        }


        [System.Serializable]
        public class StopLoop
        {
            public bool m_needToBeKilled = false;
        }


        [System.Serializable]
        public class WebSocketConnectionState
        {
            public enum LaunchState
            {
                WaitingToBeLaunched, Created, Launched, Creating, TaskCreated, ReadyToBeUsed
            }
            public LaunchState m_launchState = LaunchState.WaitingToBeLaunched;
            public StopLoop m_killSwitch = new StopLoop();
            public Task m_runningThread = null;
            public ClientWebSocket m_websocket = null;
            public Task m_runningListener = null;
            public string m_serverUri = "";
            public string m_runningThreadErrorHappened = "";
            public string m_runningListenerErrorHappened = "";

            public void KillIt()
            {
                if (m_killSwitch != null)
                    m_killSwitch.m_needToBeKilled = true;
                if (m_websocket != null)
                {
                    m_websocket.Abort();
                    m_websocket.Dispose();
                }
                if (m_runningThread != null)
                {

                    if (m_runningThread.IsCompleted)
                        m_runningThread.Dispose();
                }
                if (m_runningListener != null)
                {
                    if (m_runningThread.IsCompleted)
                        m_runningListener.Dispose();
                }
                // Missing some dispose when closed later.
            }

            public bool IsStillRunning()
            {
                return (
                           (m_runningThread != null && !m_runningThread.IsCompleted)
                        && (m_runningListener != null && !m_runningListener.IsCompleted)
                        && m_websocket != null
                        && (
                        m_websocket.State == WebSocketState.Open ||
                        m_websocket.State == WebSocketState.Connecting
                        )
                    );
            }

            public void SetLaunchState(LaunchState launched)
            {
                m_launchState = launched;
            }
        }


        [System.Serializable]
        public class HandshakeConnectionState
        {
            [Tooltip("Wait the server to send a GUID id to sign with SIGN:GUID")]
            public string m_signInMessage = "";
            [Tooltip("The GUID received to signed as bytes and send back")]
            public string m_guidToSigned = "";
            [Tooltip("Signed message ready to be sent back")]
            public string m_signedMessage;
            [Tooltip("Server recevied out signed GUID and validate the connection")]
            public string m_receivedValideHankShake = "";
            [Tooltip("Server identify the connection as integer index (negative: means guest on the server, positive: means a claimed registered index")]
            public string m_receivedIndexLockValidation = "";
            [Tooltip("Is connection with server exists as a websocket?")]
            public bool m_isConnectionValidated = false;
            [Tooltip("Is connected was established and the handshake verified")]
            public bool m_connectionEstablishedAndVerified = false;
            [Tooltip("Is server return us a index integer lock ?")]
            public bool m_receiveGivenIndexLock = false;
            [Tooltip("What is our given index locked by the server for this public rsa key ?")]
            public int m_givenIndexLock = int.MinValue;

            [Tooltip("Connected in the name of this public address")]
            public string m_inNameOfPublicAddress;

            [Tooltip("Detected a signature by letter marque (call coaster in my tool)")]
            public string m_coasterPublicAddress;

            public void ResetToNewHandshake()
            {
                m_signInMessage = "";
                m_guidToSigned = "";
                m_signedMessage = "";
                m_receivedValideHankShake = "";
                m_receivedIndexLockValidation = "";
                m_isConnectionValidated = false;
                m_connectionEstablishedAndVerified = false;
                m_receiveGivenIndexLock = false;
                m_givenIndexLock = 0;
                m_inNameOfPublicAddress = "";
                m_coasterPublicAddress = "";
            }
        }

        [System.Serializable]
        public class DebugRunningState
        {
            [Header("Running State")]
            public bool isStillRunning = false;
            public bool isRunningHandleTask = false;
            public bool isRunningListener = false;
            public bool isWebsocketConnected = false;
            public WebSocketState m_websocketState = WebSocketState.None;
        }
        [System.Serializable]
        public class DebugByteCount
        {

            [Header("Byte count")]
            public long m_receivedBinaryBytesCount = 0;
            public long m_receivedTextBytesCount = 0;
            public long m_sentBinaryBytesCount = 0;
            public long m_sentTextBytesCount = 0;
            //public long m_datetimeNow = 0;
        }

        [System.Serializable]
        public class DebugTime
        {
            [Header("Time")]
            public string m_lastReceivedMessageTextDate = "";
            public string m_lastReceivedMessageBinaryDate = "";
            public string m_lastPushMessageTextDate = "";
            public string m_lastPushMessageBinaryDate = "";

            public void ResetToNew()
            {
                m_lastReceivedMessageTextDate = "";
                m_lastReceivedMessageBinaryDate = "";
                m_lastPushMessageTextDate = "";
                m_lastPushMessageBinaryDate = "";
            }
        }
    }

}
