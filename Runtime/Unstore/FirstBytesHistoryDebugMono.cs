using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Eloi.WsMetaMaskAuth
{
    public class FirstBytesHistoryDebugMono : MonoBehaviour
    {

        [TextArea(2, 4)]
        public string m_debugText;
        public int m_maxLenght = 100;
        public List<byte> m_ignoreBytes = new List<byte>();

        private StringBuilder sb = new StringBuilder();
        public void PushBytesIn(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }
            if (m_ignoreBytes.Contains(bytes[0]))
            {
                return;
            }

            sb.Insert(0, ' ');
            sb.Insert(0, bytes[0]);
            while (sb.Length > m_maxLenght)
            {
                sb.Remove(sb.Length - 1, 1);
            }



        }

        public void Update()
        {
            m_debugText = sb.ToString();
        }

    }
}