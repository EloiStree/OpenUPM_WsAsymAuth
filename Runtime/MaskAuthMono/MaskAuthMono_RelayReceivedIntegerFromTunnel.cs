using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Eloi.WsMetaMaskAuth
{
    public class MaskAuthMono_RelayReceivedIntegerFromTunnel : MonoBehaviour
    {

        private void Reset()
        {
            m_tunnel = GetComponent<ConnectToServerTunnelingMetaMaskMono>();
        }
        [System.Serializable]
        public struct STRUCT_IID_INTEGER
        {
            public int m_index;
            public int m_value;
            public ulong m_date;

            public STRUCT_IID_INTEGER(int index, int value, ulong date)
            {
                m_index = index;
                m_value = value;
                m_date = date;
            }
        }
        public ConnectToServerTunnelingMetaMaskMono m_tunnel;
        public Listener m_unityThreadListener = new Listener();
        [System.Serializable]
        public class Listener { 
            public int m_defaultIndexIfNone = -1;
            public STRUCT_IID_INTEGER m_lastReceived;
            public UnityEvent<int> m_integerReceived;
            public UnityEvent<int, int> m_indexIntegerReceived;
            public UnityEvent<int, int, ulong> m_indexIntegerDateReceived;
        }

        public Queue<STRUCT_IID_INTEGER> m_waitingToBeOnUnityThread = new System.Collections.Generic.Queue<STRUCT_IID_INTEGER>();

         void OnEnable()
        {
            m_tunnel.m_trafficEvent.m_onThreadMessageReceivedBinary += OnReceivedBinary;
        }
         void OnDisable()
        {
            m_tunnel.m_trafficEvent.m_onThreadMessageReceivedBinary -= OnReceivedBinary;
        }

        void Update()
        {
            while(m_waitingToBeOnUnityThread.Count > 0)
            {
                STRUCT_IID_INTEGER s = m_waitingToBeOnUnityThread.Dequeue();
                m_unityThreadListener.m_lastReceived = s;
                m_unityThreadListener.m_integerReceived.Invoke(s.m_value);
                m_unityThreadListener.m_indexIntegerReceived.Invoke(s.m_index, s.m_value);
                m_unityThreadListener.m_indexIntegerDateReceived.Invoke(s.m_index, s.m_value, s.m_date);                
            }
        }

        private void OnReceivedBinary(byte[] message)
        {
            if (message != null && this.gameObject.activeInHierarchy )
            {
                if (message.Length == 4)
                {
                    int value = BitConverter.ToInt32(message, 0);
                    m_waitingToBeOnUnityThread.Enqueue(new STRUCT_IID_INTEGER(m_unityThreadListener.m_defaultIndexIfNone, value, 0));
                }
                else if (message.Length == 8)
                {
                    int index = BitConverter.ToInt32(message, 0);
                    int value = BitConverter.ToInt32(message, 4);
                    m_waitingToBeOnUnityThread.Enqueue(new STRUCT_IID_INTEGER(index, value, 0));
                }
                else if (message.Length == 12)
                {
                    int value = BitConverter.ToInt32(message, 0);
                    ulong time = BitConverter.ToUInt64(message, 4);
                    m_waitingToBeOnUnityThread.Enqueue(new STRUCT_IID_INTEGER(m_unityThreadListener.m_defaultIndexIfNone, value, time));
                }
                else if (message.Length == 16)
                {
                    int index = BitConverter.ToInt32(message, 0);
                    int value = BitConverter.ToInt32(message, 4);
                    ulong time = BitConverter.ToUInt64(message, 8);
                    m_waitingToBeOnUnityThread.Enqueue(new STRUCT_IID_INTEGER(index, value, time));
                }
            }
        }
    }
}
