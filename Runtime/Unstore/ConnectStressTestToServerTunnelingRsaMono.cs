
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Eloi.WsAsymAuth.AsymAuthTunnel;
namespace Eloi.WsAsymAuth
{





    public class ConnectStressTestToServerTunnelingRsaMono : MonoBehaviour
    {

        public AsymSignerMono_AbstractBuilder m_singerBuilder;
        public string m_serverUri = "ws://81.240.94.97:4501";
        public float m_clientAdditionInterval = 1f;
        public List<WebsocketConnectionAsymAuthTunnel> m_tunnels = new List<WebsocketConnectionAsymAuthTunnel>();


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

                if (m_continueAddingClients)
                {
                    WebsocketConnectionAsymAuthTunnel tunnel = new WebsocketConnectionAsymAuthTunnel();
                    m_singerBuilder.BuildSigner(out IAsymSignerClipboardable randomSigner);
                    tunnel.SetConnectionInfo(m_serverUri, randomSigner);
                    m_tunnels.Add(tunnel);
                    tunnel.StartConnection();
                }
            }
        }

        public WebsocketConnectionAsymAuthTunnel m_lastUsedTunnel;
        public float m_delayBetweenEachPushClient = 0.5f;
        private IEnumerator PushRandomInteger()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                for (int i = 0; i < m_tunnels.Count; i++)
                {
                    yield return new WaitForSeconds(m_delayBetweenEachPushClient);
                    byte[] iBytes = BitConverter.GetBytes(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
                    m_tunnels[i].EnqueueBinaryMessages(iBytes);
                    m_tunnels[i].UpdateRunningStateInfo();
                    m_lastUsedTunnel = m_tunnels[i];
                }
            }
        }


        public void PushIntegerAtAll(int value)
        {

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
}