//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static MetaMask
//ing;

//public class ConnectArrayToServerTunnelingMono : MonoBehaviour
//{
//    public string m_serverUri = "ws://81.240.94.97:4501";
//    public string[] m_privateKeys= new string[12];
//    public List<WebsocketConnectionMetaMaskTunneling> m_tunnels;
//    public void SetServerURI(string serverUri) { m_serverUri = serverUri; }
//    public bool m_autoStartAtStart=true;
//    public float m_timeBeforeConnection = 0.5f;
//    [ContextMenu("Push Random Value Integer LE to all")]
//    public void PushRandomIIDToAll()
//    {
//        for (int i = 0; i < m_tunnels.Count; i++)
//        {
//            byte[] iBytes = BitConverter.GetBytes(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
//            PushBytesToIndex(i, iBytes);
//        }
//    }
//    [ContextMenu("Push Random Value Integer LE to one")]
//    public void PushRandomIIDToOne()
//    {
//        int index = UnityEngine.Random.Range(0, m_tunnels.Count);
//        PushIntegerToIndex(index, UnityEngine.Random.Range(int.MinValue, int.MaxValue));
//     }

//    public void PushBytesToIndex(int index, byte [] bytes)
//    {
//        if(index >= 0 && index < m_tunnels.Count)
//        m_tunnels[index].EnqueueBinaryMessages(bytes);
//    }
//    public void PushIntegerToIndex(int index, int integer) { 
    
//        byte[] iBytes = BitConverter.GetBytes(integer);
//        PushBytesToIndex(index, iBytes);
//    }

//    [ContextMenu("Join current game IID")]
//    public void JoinCurrentGame()
//    {
//        NotifyWantToBePlayerAll();
//        ConfirmWantToBePlayerAll();
//    }

//    [ContextMenu("Want to be player 123456789")]
//    public void NotifyWantToBePlayerAll()
//    {

//        for(int i = 0; i < m_tunnels.Count; i++)
//        {
//            PushIntegerToIndex(i, 123456789);
//        }
//    }
//    [ContextMenu("Want to be player confirmed 987654321")]
//    public void ConfirmWantToBePlayerAll()
//    {

//        for (int i = 0; i < m_tunnels.Count; i++)
//        {
//            PushIntegerToIndex(i, 987654321);
//        }

//    }



//    [ContextMenu("Force Reconnect All")]
//    public void ForceToReconnectAll() {

//        if (m_tunnels != null) { 
//            for (int i = 0; i < m_tunnels.Count; i++)
//            {
//                try { 
//                if (m_tunnels[i]!=null)
//                        if (m_tunnels[i].HasStarted())
//                            m_tunnels[i].CloseTunnel();
                
//                }catch(Exception e) { Debug.Log("Did not close properly:" + e.StackTrace, this.gameObject); }
//            }
//            m_tunnels.Clear();
//        }

//        if(m_tunnels == null)
//            m_tunnels = new List<WebsocketConnectionMetaMaskTunneling>();

//        m_tunnels.Clear();

//        for (int i = 0; i < m_privateKeys.Length; i++)
//        {
//            WebsocketConnectionMetaMaskTunneling tunnel = new WebsocketConnectionMetaMaskTunneling();

//            m_maskSignerBuilder.BuildSigner(out  Imask);
//            ForTestingSignerEthMask signer = new ForTestingSignerEthMask(this.m_privateKeys[i]);
//            tunnel.SetConnectionInfo(m_serverUri, signer);
//            m_tunnels.Add(tunnel);
//            tunnel.StartConnection();
//        }
//    }

//    public MaskSignerMono_AbstractBuilder m_maskSignerBuilder;
//    public bool m_createNewRsaAtStart = false;
//    public bool m_autoJoinCurrentGame = true;

//    public IEnumerator Start()
//    {


//        if (m_createNewRsaAtStart)
//            GenerateRandomPrivateKey();

//        if (m_autoStartAtStart) { 
//            yield return new WaitForSeconds(m_timeBeforeConnection);
//            ForceToReconnectAll();
//        }
//        yield return new WaitForSeconds(1f);

//        if (m_autoJoinCurrentGame)
//        {
//            NotifyWantToBePlayerAll();
//            ConfirmWantToBePlayerAll();
//        }
//    }

//    private void ConnectAll()
//    {
//        m_tunnels = new List<WebsocketConnectionMetaMaskTunneling>();
//        for (int i = 0; i < m_privateKeys.Length; i++)
//        {
//            WebsocketConnectionMetaMaskTunneling tunnel = new WebsocketConnectionMetaMaskTunneling();
//            ForTestingSignerEthMask signer = new ForTestingSignerEthMask(this.m_privateKeys[i]);
//            tunnel.SetConnectionInfo(m_serverUri, signer);
//            m_tunnels.Add(tunnel);
//            tunnel.StartConnection();
//        }
//    }

//    private void Update()
//    {
//        for (int i = 0; i < m_tunnels.Count; i++)
//        {
//            m_tunnels[i].UpdateRunningStateInfo();
//        }
//    }
//    [ContextMenu("Generate random keys")]
//    public void GenerateRandomPrivateKey() { 
    
//        for(int i = 0; i < m_privateKeys.Length; i++) {
//            m_privateKeys[i] = EthMaskTunnelingUtility.GenerateRandomPrivateKeyXml();
//        }
//    }
//}
