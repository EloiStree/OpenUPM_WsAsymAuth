using Nethereum.RPC.AccountSigning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EthMaskTunneling;


public class ForTestingSignerEthMask : IEthMaskCliboardableSigner{
    public IEthMaskPrivateKeyHolderGet m_privateKey;
    

    public ForTestingSignerEthMask()
    {
        m_privateKey = new STRUCT_EthMaskPrivateKey() { m_privateKey = MetaMaskSignUtility.GeneratePrivateKey() };
    }
    public ForTestingSignerEthMask(IEthMaskPrivateKeyHolderGet privateKey)
    {
        m_privateKey = privateKey;
    }
    public ForTestingSignerEthMask(string privateKey)
    {
        m_privateKey = new STRUCT_EthMaskPrivateKey() { m_privateKey = privateKey };
    }

    public void GetClipboardSignedMessage(string message, out string clipboardableSignedMessage)
    {
        string privateKey = m_privateKey.GetPrivateKey();
        MetaMaskSignUtility.GenerateClipboardSignMessage(privateKey, message, out clipboardableSignedMessage);

    }
}

public class ConnectStressTestToServerTunnelingRsaMono : MonoBehaviour
{

    public string m_serverUri = "ws://81.240.94.97:4501";
    public float m_clientAdditionInterval = 1f;
    public List<WebsocketConnectionEthMaskTunneling> m_tunnels = new List<WebsocketConnectionEthMaskTunneling>();


    public bool m_continueAddingClients = true;

    public void Awake()
    {
        StartCoroutine(PushRandomInteger());
    }
    public IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(m_clientAdditionInterval);

            if (m_continueAddingClients) { 
                WebsocketConnectionEthMaskTunneling tunnel = new WebsocketConnectionEthMaskTunneling();
                ForTestingSignerEthMask randomSigner = new ForTestingSignerEthMask();
                tunnel.SetConnectionInfo(m_serverUri,randomSigner);
                m_tunnels.Add(tunnel);
                tunnel.StartConnection();
            }
        }
    }

    public WebsocketConnectionEthMaskTunneling m_lastUsedTunnel;
    public float m_delayBetweenEachPushClient = 0.5f;
    private IEnumerator PushRandomInteger()
    {
        while (true) {
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < m_tunnels.Count; i++)
            {
                yield return new WaitForSeconds(m_delayBetweenEachPushClient);
                byte[] iBytes = BitConverter.GetBytes(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
                m_tunnels[i].EnqueueBinaryMessages(iBytes);
                m_tunnels[i].UpdateRunningStateInfo();
                m_lastUsedTunnel= m_tunnels[i]; 
            }
        }
    }


    public void PushIntegerAtAll(int value) { 
    
        for (int i = 0; i < m_tunnels.Count; i++)
        {
            byte[] iBytes = BitConverter.GetBytes(value);
            m_tunnels[i].EnqueueBinaryMessages(iBytes);
            m_tunnels[i].UpdateRunningStateInfo();
        }
    }


    [ContextMenu("Push Random Same on All")]
    public void PushRandomSameToAll()
    {
        PushIntegerAtAll(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
    }
    [ContextMenu("Push Random Same on All")]
    public void PushRandomToAll()
    {
        for (int i = 0; i < m_tunnels.Count; i++)
        {
            byte[] iBytes = BitConverter.GetBytes(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            m_tunnels[i].EnqueueBinaryMessages(iBytes);
            m_tunnels[i].UpdateRunningStateInfo();
        }
    }


}
