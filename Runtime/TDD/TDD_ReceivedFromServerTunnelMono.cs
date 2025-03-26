using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Eloi.WsAsymAuth
{
    public class TDD_ReceivedFromServerTunnelMono : MonoBehaviour
    {
        public string m_text = "";
        public List<string> m_logReceivedText = new List<string>();
        public int m_maxLogSize = 10;
        public byte[] m_binary;

        public int m_intIndex = 0;
        public int m_intValue = 0;
        public ulong m_utcDateMilliseconds = 0;
        public ulong m_receivedTime = 0;
        public ulong m_sendToReceivedLag = 0;
        public long m_offsetInMilliseconds = 0;


        public void SetNtpOffsetInMilliseconds(long ntpOffsetInMilliseconds) => m_offsetInMilliseconds = ntpOffsetInMilliseconds;
        public void SetNtpOffsetInMilliseconds(int ntpOffsetInMilliseconds) => m_offsetInMilliseconds = ntpOffsetInMilliseconds;




        public delegate void OnIndexIntegerDateFound(int index, int value, ulong utcDateMilliseconds, ulong receivedTime);
        public OnIndexIntegerDateFound m_onIndexIntegerDateFound;

        public void PushText(string receivedText)
        {
            m_text = receivedText;
            m_logReceivedText.Insert(0, receivedText);
            while (m_logReceivedText.Count > m_maxLogSize)
                m_logReceivedText.RemoveAt(m_logReceivedText.Count - 1);
        }
        public void PushBytes(byte[] receivedBytes)
        {
            m_binary = receivedBytes;
            if (m_binary.Length == 16)
            {
                int index = BitConverter.ToInt32(receivedBytes, 0);
                int value = BitConverter.ToInt32(receivedBytes, 4);
                ulong date = BitConverter.ToUInt64(receivedBytes, 8);
                GetCurrentTimeAsMillisecondsNtp(out long receivedTime);
                m_receivedTime = (ulong)receivedTime;

                m_intIndex = index;
                m_intValue = value;
                m_utcDateMilliseconds = date;
                m_sendToReceivedLag = m_receivedTime - m_utcDateMilliseconds;

                if (m_onIndexIntegerDateFound != null)
                {
                    m_onIndexIntegerDateFound(index, value, date, m_receivedTime);
                }
            }
        }
        public void GetCurrentTimeAsMillisecondsNtp(out long timeInMilliseconds)
        {
            long milliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            milliseconds += m_offsetInMilliseconds * 10000;
            timeInMilliseconds = milliseconds;
        }

    }
}