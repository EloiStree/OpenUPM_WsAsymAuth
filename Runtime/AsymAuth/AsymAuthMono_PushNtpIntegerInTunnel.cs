using System;
using UnityEngine;
using UnityEngine.Events;
namespace Eloi.WsAsymAuth
{
    public class AsymAuthMono_PushNtpIntegerInTunnel : MonoBehaviour
    {

        private void Reset()
        {
            m_tunnel = GetComponent<WsConnectToAsymServerMono>();
        }

        public WsConnectToAsymServerMono m_tunnel;

        [SerializeField] int m_userIndexDefault = 0;
        public Listener m_unityThreadListener = new Listener();
        [System.Serializable]
        public class Listener
        {
            public int m_defaultIndexIfNone = -1;
            public UnityEvent<int> m_integerReceived;
            public UnityEvent<int, int> m_indexIntegerReceived;
            public UnityEvent<int, int, ulong> m_indexIntegerDateReceived;
        }

       
        public void GetCurrentTimeAsMillisecondsNtp( out long timeInMilliseconds)
        {
            m_tunnel.GetCurrentTimeAsMillisecondsNtp(out timeInMilliseconds);
        }
        

        #region Push NTP Integer

        /// <summary>
        /// Send directly the integer but with a linked dated using currentime + delay offseted to match NTP server
        /// </summary>
        /// <param name="integerValue"></param>
        /// <param name="millisecondsDelay"></param>
        public void PushNtpIntegerInTunnelWithDelayMilliseconds(int integerValue, int millisecondsDelay)
        {
            GetCurrentTimeAsMillisecondsNtp(out long timeInMilliseconds);
            ulong dateSent = (ulong)(timeInMilliseconds + millisecondsDelay);
            m_tunnel.PushIndexIntegerDate(m_userIndexDefault, integerValue, dateSent);
            m_unityThreadListener.m_integerReceived.Invoke(integerValue);
            m_unityThreadListener.m_indexIntegerReceived.Invoke(m_userIndexDefault, integerValue);
            m_unityThreadListener.m_indexIntegerDateReceived.Invoke(m_userIndexDefault, integerValue, dateSent);
          
        }

        /// <summary>
        /// Send directly the integer but with a linked dated using currentime + delay offseted to match NTP server
        /// </summary>
        /// <param name="index"></param>
        /// <param name="integerValue"></param>
        /// <param name="millisecondsDelay"></param>
        public void PushNtpIndexIntegerInTunnelWithDelayMilliseconds(int index, int integerValue, int millisecondsDelay)
        {
            GetCurrentTimeAsMillisecondsNtp(out long timeInMilliseconds);
            m_tunnel.PushIndexIntegerDate(m_userIndexDefault,integerValue,(ulong)( timeInMilliseconds + millisecondsDelay));
        }
        #endregion



        public void PushNtpIntegerInTunnelWith100MsDelay(int integerValue) 
        =>PushNtpIntegerInTunnelWithDelayMilliseconds(integerValue, 100);
        
        public void PushNtpIntegerInTunnel(int integerValue)
        =>PushNtpIntegerInTunnelWithDelayMilliseconds(integerValue, 0);

        public void PushNtpIntegerInTunnelWithDelaySeconds(int integerValue, float secondDelay)
        => PushNtpIntegerInTunnelWithDelayMilliseconds(integerValue, (int)(secondDelay * 1000f));

        public void PushNtpIntegerInTunnelWithDelaySeconds(int integerValue, double secondDelay)
        => PushNtpIntegerInTunnelWithDelayMilliseconds(integerValue, (int)(secondDelay * 1000f));



        public void PushNtpIndexIntegerInTunnelWith100MsDelay(int index, int integerValue)
            => PushNtpIndexIntegerInTunnelWithDelayMilliseconds(index, integerValue, 100);
        
        public void PushNtpIndexIntegerInTunnel(int index, int integerValue)
            => PushNtpIndexIntegerInTunnelWithDelayMilliseconds(index, integerValue, 0);
        
        public void PushNtpIndexIntegerInTunnelWithDelaySeconds(int index, int integerValue, float secondDelay)
            => PushNtpIndexIntegerInTunnelWithDelayMilliseconds(index, integerValue, (int)(secondDelay * 1000f));

        public void PushNtpIndexIntegerInTunnelWithDelaySeconds(int index, int integerValue, double secondDelay)
            => PushNtpIndexIntegerInTunnelWithDelayMilliseconds(index, integerValue, (int)(secondDelay * 1000f));



        public void PushNtpRandom(int minInclusive, int maxInclusive)
        {
            int randomValue = UnityEngine.Random.Range(minInclusive, maxInclusive);
            PushNtpIntegerInTunnel(randomValue);
        }


        [ContextMenu("Push 0")]
        public void PushNtp0() => PushNtpIntegerInTunnelWithDelayMilliseconds(0, 0);

        [ContextMenu("Push 1")]
        public void PushNtp1() => PushNtpIntegerInTunnelWithDelayMilliseconds(1, 0);

        [ContextMenu("Push 42")]
        public void PushNtp42() => PushNtpIntegerInTunnelWithDelayMilliseconds(42, 0);

        [ContextMenu("Push 2501")]
        public void PushNtp2501() => PushNtpIntegerInTunnelWithDelayMilliseconds(2501,0);

        [ContextMenu("Random Integer")]
        public void PushNtpRandomInteger()=> PushNtpRandom(int.MinValue, int.MaxValue);
        [ContextMenu("Random Positive Integer")]
        public void PushNtpRandomPositiveInteger() => PushNtpRandom(0, int.MaxValue);
        [ContextMenu("Random Negative Integer")]
        public void PushNtpRandomNegativeInteger() => PushNtpRandom(int.MinValue,-0);
        [ContextMenu("Random Short")]
        public void PushNtpRandomShort()=> PushNtpRandom(short.MinValue, short.MaxValue);
        [ContextMenu("Random UShort")]
        public void PushNtpRandomUShort() => PushNtpRandom(ushort.MinValue, ushort.MaxValue);
        [ContextMenu("Random Byte")]
        public void PushNtpRandomByte() => PushNtpRandom(byte.MinValue, byte.MaxValue);
        [ContextMenu("Random SByte")]
        public void PushNtpRandomSByte() => PushNtpRandom(sbyte.MinValue, sbyte.MaxValue);
        [ContextMenu("Random From 0 To 1")]
        public void PushNtpRandomFrom0To1() => PushNtpRandom(0, 1);
        [ContextMenu("Random From 0 To 10")]
        public void PushNtpRandomFrom0To10() => PushNtpRandom(0, 10);
        [ContextMenu("Random From 0 To 100")]
        public void PushNtpRandomFrom0To100() => PushNtpRandom(0,100);
    }
}
