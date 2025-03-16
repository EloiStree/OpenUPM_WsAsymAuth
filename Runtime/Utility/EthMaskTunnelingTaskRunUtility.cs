
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Eloi.WsMetaMaskAuth.MetaMaskTunneling;
using static Eloi.WsMetaMaskAuth.MetaMaskTunneling.WebSocketConnectionState;
namespace Eloi.WsMetaMaskAuth
{
    public class EthMaskTunnelingTaskRunUtility
    {
        public string m_pfxTextFileContent;
        public string m_pfxPassword;
        public static void StartRunnningTunnel(WebsocketConnectionMetaMaskTunneling tunnel)
        {
            if (tunnel.HasStarted())
                throw new Exception("Already started");
            Task t = Task.Run(() => ConnectAndRun(tunnel));
            tunnel.m_connection.m_runningThread = t;
        }

        private static string m_dateFormat = "yyyy-MM-dd HH:mm:ss.fff";
        public static string  GetDateNowAsStringForDebug() { 
            return DateTime.UtcNow.ToString(m_dateFormat);
        }

        public static async Task ConnectAndRun(WebsocketConnectionMetaMaskTunneling tunnel)
        {
            tunnel.m_connection.SetLaunchState(LaunchState.Launched);

            using (ClientWebSocket ws = new ClientWebSocket())
            {
                HandshakeConnectionState handshake = tunnel.m_handshake;
                tunnel.m_connection.m_websocket = ws;
                tunnel.m_handshake.ResetToNewHandshake();

                
                try
                {
                    string serverUri = tunnel.m_connection.m_serverUri;
                    //if (m_)
                        await ws.ConnectAsync(new Uri(serverUri), CancellationToken.None);
                    //else { 
                    //    X509Certificate2 cert = new X509Certificate2()

                    //    ws.Options.ClientCertificates.Add()
                    //}

                    tunnel.m_connection.m_runningListener = Task.Run(() => ReceiveMessages(tunnel));
                    tunnel.m_connection.SetLaunchState(LaunchState.TaskCreated);
                    bool firstOpenState = false;
                    while (ws.State == WebSocketState.Open)
                    {
                        if (firstOpenState == false)
                        {
                            firstOpenState = true;
                            tunnel.m_connection.SetLaunchState(LaunchState.ReadyToBeUsed);
                            tunnel.NotifyAsWebsocketInOpenState();
                        }
                        if (tunnel.IsInMustBeKillMode())
                            throw new Exception("Force close");


                        Queue<string> queueText = tunnel.m_pushInTunnel.m_toSendToTheServerUTF8;
                        while (queueText.Count > 0)
                        {
                            tunnel.m_timeDebug.m_lastPushMessageTextDate = GetDateNowAsStringForDebug();
                            if (tunnel.m_trafficEvent.m_onThreadMessagePushedText != null)
                                tunnel.m_trafficEvent.m_onThreadMessagePushedText(queueText.Peek());
                            byte[] messageBytes = Encoding.UTF8.GetBytes(queueText.Dequeue());
                            tunnel.m_byteCountDebug.m_sentTextBytesCount += messageBytes.Length;
                            await ws.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        Queue<byte[]> queueBytes = tunnel.m_pushInTunnel.m_toSendToTheServerBytes;
                        while (queueBytes.Count > 0)
                        {
                            tunnel.m_timeDebug.m_lastPushMessageBinaryDate = GetDateNowAsStringForDebug();
                            if (tunnel.m_trafficEvent.m_onThreadMessagePushedBinary != null)
                                tunnel.m_trafficEvent.m_onThreadMessagePushedBinary(queueBytes.Peek());
                            byte[] messageBytes = queueBytes.Dequeue();
                            tunnel.m_byteCountDebug.m_sentBinaryBytesCount += messageBytes.Length;
                            await ws.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Binary, true, CancellationToken.None);
                        }
                        await Task.Delay(1);
                    }
                }
                catch (Exception ex)
                {
                    tunnel.m_connection.m_runningThreadErrorHappened = ex.Message + "\n\n" + ex.StackTrace;
                    //Console.WriteLine($"WebSocket error: {ex.Message}");
                }

            }
            tunnel.NotifyAsTunnelEnded();

        }



        public static async Task ReceiveMessages(WebsocketConnectionMetaMaskTunneling tunnel)
        {
            ClientWebSocket webSocket = tunnel.m_connection.m_websocket;
            HandshakeConnectionState handshake = tunnel.m_handshake;

            byte[] buffer = new byte[8096];
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    if (tunnel.IsInMustBeKillMode())
                        throw new Exception("Force close");
                    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        tunnel.m_byteCountDebug.m_receivedTextBytesCount += result.Count;
                        tunnel.m_timeDebug.m_lastReceivedMessageTextDate = GetDateNowAsStringForDebug();
                        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        if (tunnel.m_trafficEvent.m_onThreadMessageReceivedText != null)
                            tunnel.m_trafficEvent.m_onThreadMessageReceivedText(receivedMessage);

                        if (!handshake.m_connectionEstablishedAndVerified)
                        {
                            if (receivedMessage.ToUpper().StartsWith("SIGN:"))
                            {
                                if (tunnel.m_messageSigner == null)
                                {
                                    tunnel.m_passwordSha256 = tunnel.m_passwordSha256.Trim();
                                    if (!string.IsNullOrEmpty(tunnel.m_passwordSha256)) {

                                        if (tunnel.m_passwordSha256.Length != 64) { 
                                            Bit4096B58Pkcs1SHA256.TextToSHA256(tunnel.m_passwordSha256, out string sha256);
                                            tunnel.m_passwordSha256 = sha256;
                                        }

                                        await webSocket.SendAsync(new ArraySegment<byte>(
                                            Encoding.UTF8.GetBytes("SHA256:" + tunnel.m_passwordSha256)),
                                            WebSocketMessageType.Text, true, CancellationToken.None);
                                    }
                                }
                                else { 
                                    string guid = receivedMessage.Replace("SIGN:", "");
                                    handshake.m_isConnectionValidated = true;
                                    handshake.m_signInMessage = receivedMessage;
                                    handshake.m_guidToSigned = guid;
                                    tunnel.m_messageSigner.GetClipboardSignedMessage(guid, out string signedGuid);
                                    handshake.m_signedMessage = signedGuid;
                                    byte[] signatureBytes = Encoding.UTF8.GetBytes(signedGuid);
                                    await webSocket.SendAsync(new ArraySegment<byte>(signatureBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                                   
                                }

                            }
                            else if (receivedMessage.ToUpper().StartsWith("HELLO "))
                            {
                                string[] splitPiece = receivedMessage.Split(" ");
                                // UnityEngine.Debug.Log($"RR Received: {receivedMessage}");
                                if (splitPiece.Length >= 2)
                                {
                                    if (int.TryParse(splitPiece[1], out int index))
                                    {
                                        handshake.m_receiveGivenIndexLock = true;
                                        handshake.m_givenIndexLock = index;

                                    }
                                    else
                                    {

                                        handshake.m_receiveGivenIndexLock = false;
                                        handshake.m_givenIndexLock = 0;
                                    }

                                    if (tunnel.m_trafficEvent.m_onIndexLockChanged != null)
                                        tunnel.m_trafficEvent.m_onIndexLockChanged(handshake.m_givenIndexLock);
                                }
                                if (splitPiece.Length >= 3)
                                {
                                    handshake.m_inNameOfPublicAddress = splitPiece[2];
                                }
                                if (splitPiece.Length >= 4)
                                {
                                    handshake.m_coasterPublicAddress = splitPiece[3];
                                }

                                handshake.m_receivedValideHankShake = receivedMessage;
                                handshake.m_connectionEstablishedAndVerified = true;
                                tunnel.NotifyAsConnectedAndVerified();
                            }

                        }

                    }
                    else if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        tunnel.m_byteCountDebug.m_receivedBinaryBytesCount += result.Count;
                        tunnel.m_timeDebug.m_lastReceivedMessageBinaryDate = GetDateNowAsStringForDebug();
                        byte[] receivedMessage = new byte[result.Count];
                        Array.Copy(buffer, receivedMessage, result.Count);
                        if (tunnel.m_trafficEvent.m_onThreadMessageReceivedBinary != null)
                            tunnel.m_trafficEvent.m_onThreadMessageReceivedBinary(receivedMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                tunnel.m_connection.m_runningListenerErrorHappened = ex.Message + "\n\n" + ex.StackTrace;
                UnityEngine.Debug.Log($"WebSocket error: {tunnel.m_connection.m_runningListenerErrorHappened}");

            }
        }
    }
}