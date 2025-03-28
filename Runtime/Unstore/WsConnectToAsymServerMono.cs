using Eloi.WsAsymAuth;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using static Eloi.WsAsymAuth.AsymAuthTunnel;
namespace Eloi.WsAsymAuth
{
    /// <summary>
    /// I am a class that allows a websocket connection to be established with a server supporting a MetaMask clipboard connection.
    /// </summary>
    public class WsConnectToAsymServerMono : MonoBehaviour
    {
        [Header("Set Target")]
        public string m_serverUri = "ws://raspberrypi.local:4615/";

        [Header("Set Crpyto Signer")]
        public AsymSignerMono_AbstractClipboardSigner m_signerReference;
   

        public WebsocketConnectionAsymAuthTunnel m_tunnel;
        public AsymAuthTunnel.TraffictInEvent m_trafficEvent;



        [Header("Auto Start (temporary)")]
        public bool m_autoStart = true;
        public bool m_autoReconnect = true;
        public float m_reconnectDelay = 5;


        public bool IsActiveInHierarchy()
        {
            return gameObject.activeInHierarchy;
        }
        public bool IsNotActiveInHierarchy()
        {
            return !gameObject.activeInHierarchy;
        }

    
        [ContextMenu("Remove Sign Holder")]
        public void RemoveSignHolder()
        {
            m_signerReference = null;
        }

        public void ParsePasswordToSHA256(string password, out string passwordSHA256)
        {
            if (password.Length <= 62)
            {
                Bit4096B58Pkcs1SHA256.
                    TextToSHA256(password, out passwordSHA256);
            }
            else {

                passwordSHA256 = password;
            }

        }


        public void SetSignerToUse(AsymSignerMono_AbstractClipboardSigner privateKeySigner)
        {
            m_signerReference = privateKeySigner;
        }

        public void SetWebsocketUrlWS(string url)
        {
            m_serverUri = url;
        }


        private void OnDestroy()
        {
            m_tunnel.CloseTunnel();
        }
        
        void OnEnable()
        {
            StartClient();

        }

        void StartClient()
        {
            if (m_autoStart)
            {
                TryToLaunchOrRelaunchClient();
            }
            if (m_autoReconnect)
                InvokeRepeating("TryToLaunchOrRelaunchClient", m_reconnectDelay, m_reconnectDelay);
        }

        public void TryToLaunchOrRelaunchClient()
        {

            if (m_tunnel == null)

                LaunchNewConnection();

            else if (!m_tunnel.IsStillRunning())
            {
                m_tunnel.CloseTunnel();
                LaunchNewConnection();
            }
        }




        public AsymAuthTunnel.DebugRunningState m_runningState;
        private void Update()
        {
            m_runningState = m_tunnel.m_runningState;
            m_tunnel.UpdateRunningStateInfo();

        }
        [ContextMenu("Launch relaunch or Connect")]
        public void LaunchRelaunchOrConnect()
        {
            bool isGood = m_tunnel.HasStarted() && m_tunnel.IsStillRunning();
            if (!isGood)
            {
                CloseAndRelanchConnection();
            }
        }
        [ContextMenu("Close and connect")]
        public void CloseAndRelanchConnection()
        {
            CloseCurrentConnection();
            LaunchNewConnection();
        }

        [ContextMenu("Close")]
        public void CloseCurrentConnection()
        {
            if (m_tunnel != null)
                m_tunnel.CloseTunnel();
        }

        private void LaunchNewConnection()
        {
            WebsocketConnectionAsymAuthTunnel c = new WebsocketConnectionAsymAuthTunnel();
            c.SetConnectionInfo(m_serverUri, (IAsymSignerClipboardable)m_signerReference);
            HookTunnelEventToMonoScript(c);
            c.StartConnection();
            m_tunnel = c;
        }


        private void HookTunnelEventToMonoScript(WebsocketConnectionAsymAuthTunnel c)
        {
            c.m_trafficEvent.m_onThreadMessagePushedBinary = OnMessagePushedBinary;
            c.m_trafficEvent.m_onThreadMessagePushedText = OnMessagePushedText;
            c.m_trafficEvent.m_onThreadMessageReceivedBinary = OnMessageReceivedBinary;
            c.m_trafficEvent.m_onThreadMessageReceivedText = OnMessageReceivedText;
            c.m_trafficEvent.m_onConnectionSignedAndValidated = OnSignedAndValidated;
            c.m_trafficEvent.m_onWebsocketConnected = OnConnectionEstablished;
            c.m_trafficEvent.m_onConnectionLost = OnConnectionLost;
            c.m_trafficEvent.m_onIndexLockChanged = OnIndexChanged;
        }

        private void OnIndexChanged(int index)
        {
            if (m_trafficEvent.m_onIndexLockChanged != null)
                m_trafficEvent.m_onIndexLockChanged(index);
        }

        private void OnSignedAndValidated()
        {
            if (m_trafficEvent.m_onConnectionSignedAndValidated != null)
                m_trafficEvent.m_onConnectionSignedAndValidated();
        }

        private void OnConnectionEstablished()
        {
            if (m_trafficEvent.m_onWebsocketConnected != null)
                m_trafficEvent.m_onWebsocketConnected();
        }

        private void OnConnectionLost()
        {
            if (m_trafficEvent.m_onConnectionLost != null)
                m_trafficEvent.m_onConnectionLost();
        }

        private void OnMessageReceivedText(string message)
        {
            if (m_trafficEvent.m_onThreadMessageReceivedText != null)
                m_trafficEvent.m_onThreadMessageReceivedText(message);
        }

        private void OnMessageReceivedBinary(byte[] message)
        {
            if (m_trafficEvent.m_onThreadMessageReceivedBinary != null)
                m_trafficEvent.m_onThreadMessageReceivedBinary(message);
        }

        private void OnMessagePushedText(string message)
        {
            if (m_trafficEvent.m_onThreadMessagePushedText != null)
                m_trafficEvent.m_onThreadMessagePushedText(message);

        }

        private void OnMessagePushedBinary(byte[] message)
        {
            if (m_trafficEvent.m_onThreadMessagePushedBinary != null)
                m_trafficEvent.m_onThreadMessagePushedBinary(message);
        }


        [ContextMenu("Push Hello World")]
        public void PushHelloWorldMessage() => PushMessageText("Hello world");
        [ContextMenu("Push GUID")]
        public void PushGuid() => PushMessageText(Guid.NewGuid().ToString());

        [ContextMenu("Push Date UTC ")]
        public void PushDateTimeNowUTC() => PushMessageText(DateTime.UtcNow.ToString());




        [ContextMenu("Push Random Bytes 0 to 20")]
        public void PushRandomBytesTo20() => PushMessageBytes(GetRandomBytes(0, 20));
        [ContextMenu("Push Random Bytes 0 to 1000")]
        public void PushRandomBytesTo1023() => PushMessageBytes(GetRandomBytes(0, 1000));
        [ContextMenu("Push Random Bytes 1050 to 65000")]
        public void PushRandomBytesTo65500() => PushMessageBytes(GetRandomBytes(1050, 65000));

        private byte[] GetRandomBytes(int min, int max)
        {
            byte[] b = new byte[UnityEngine.Random.Range(min, max)];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)UnityEngine.Random.Range(0, 255);
            }
            return b;
        }

        public void PushMessageText(string textToSend)
        {
            if (IsNotActiveInHierarchy())
                return;
            m_tunnel.EnqueueTextMessages(textToSend);
        }
        public void PushMessageBytes(byte[] bytesToSend)
        {
            if (IsNotActiveInHierarchy())
                return;
            m_tunnel.EnqueueBinaryMessages(bytesToSend);
        }
        public int m_previousInteger = 0;




        public void PushIndexIntegerDate(int index, int value, ulong date) {

            byte[] localBytes = new byte[16];
            BitConverter.GetBytes(index).CopyTo(localBytes, 0);
            BitConverter.GetBytes(value).CopyTo(localBytes, 4);
            BitConverter.GetBytes(date).CopyTo(localBytes, 8);
            m_tunnel.EnqueueBinaryMessages(localBytes);
        }
       
        [ContextMenu("Push random integer LE")]
        public void PushRandomInteger4Bytes()
        {
            PushMessageInteger4BytesLE(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        }
        [ContextMenu("Push random integer LE 0-100")]
        public void PushRandomInteger4BytesFrom0To100()
        {
            PushMessageInteger4BytesLE(UnityEngine.Random.Range(0, 100));
        }

        public void PushMessageInteger4BytesLE(int value)
        {
            if (IsNotActiveInHierarchy())
                return;
            m_previousInteger = value;
            m_tunnel.EnqueueBinaryMessages(BitConverter.GetBytes(value));
        }


        [ContextMenu("Push random integer IID")]
        public void PushRandomIntegerIID()
        {
            PushMessageIntegerIID(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        }
        [ContextMenu("Push random integer IID 0-100")]
        public void PushRandomIntegerIIDFrom0To100()
        {
            PushMessageIntegerIID(UnityEngine.Random.Range(0, 100));
        }


        public void PushMessageIntegerIIDWithSecondsDelay(int value, float delayInSeconds)
        {
            GetCurrentTimeAsMillisecondsNtp(out long timeInMS);
            timeInMS += (long)(delayInSeconds * 1000);
            PushMessageIntegerIID(value, timeInMS);
        }

        public void PushMessageIntegerIID(int value)
        {
            GetCurrentTimeAsMillisecondsNtp(out long timeInMS);
            PushMessageIntegerIID(value, timeInMS);
        }
        public void PushMessageIntegerIID(int value, long tickUtcTimestamp)
        {

            PushMessageIntegerIID(value, (ulong)tickUtcTimestamp);
        }
        public void PushMessageIntegerIID(int value, ulong tickUtcTimestamp)
        {
            if (IsNotActiveInHierarchy())
                return;
            byte[] localBytes = new byte[12];
            m_previousInteger = value;
            BitConverter.GetBytes(value).CopyTo(localBytes, 0);
            BitConverter.GetBytes(tickUtcTimestamp).CopyTo(localBytes, 4);
            m_tunnel.EnqueueBinaryMessages(localBytes);
        }

        public void PushClampedBytesAsIID(byte[] bytes)
        {
            m_tunnel.PushClampedBytesAsIID(bytes);
        }

        public long m_millisecondsOffsetLocalToNtp;

        public void SetNtpOffsetLocalToServerMilliseconds(long offsetInMilliseconds)
        {
            m_millisecondsOffsetLocalToNtp = offsetInMilliseconds;
        }

        public void GetCurrentTimeAsMillisecondsNtp(out long timeInMilliseconds)
        {
            long milliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            milliseconds += m_millisecondsOffsetLocalToNtp ;
            timeInMilliseconds = milliseconds;
        }
        public void GetCurrentTimeAsTickNtp(out long timeinTick)
        {
            long milliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            milliseconds += m_millisecondsOffsetLocalToNtp * 10000;
            timeinTick = milliseconds;
        }

    }
}
