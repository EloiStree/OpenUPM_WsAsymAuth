using System;
using UnityEngine;
using UnityEngine.Events;

namespace Eloi.WsAsymAuth { 
public class StaticServerHostnameToUseMono:MonoBehaviour
{
    [SerializeField] string m_hostnameToUseDebugField="";
    public UnityEvent<string> m_onHostnameSet= new UnityEvent<string>();


    [ContextMenu("Set Raspberrypi.local Hostname")]
    public void SetHostnameAsRaspberryPi() =>
        StaticServerHostnameToUse.SetHostnameAsRaspberryPi();
    [ContextMenu("Set Apint.ddns.net Hostname")]
    public void SetHostnameAsApintDefaultServer() =>
        StaticServerHostnameToUse.SetHostnameAsApintDefaultServer();

    [ContextMenu("Set 127.0.0.1 Hostname")]
    public void SetHostnameAsApintAsLocalhost() =>
        StaticServerHostnameToUse.SetHostnameAsApintAsLocalhost();


    public void SetHostname(string hostname)
    {
        StaticServerHostnameToUse.SetHostenameToUse(hostname);
    }

    private void Awake()
    {
        StaticServerHostnameToUse.AddOnSetListener(OnHostnameSet);
        StaticServerHostnameToUse.GetHostenameToUse(out m_hostnameToUseDebugField );
            OnHostnameSetWithCurrent();
    }

        private void OnHostnameSetWithCurrent()
        {
            OnHostnameSet(m_hostnameToUseDebugField);
        }

        private void OnEnable()
    {
        StaticServerHostnameToUse.GetHostenameToUse(out m_hostnameToUseDebugField);
            OnHostnameSetWithCurrent();
        }

    private void OnDisable()
    {
        StaticServerHostnameToUse.GetHostenameToUse(out m_hostnameToUseDebugField);
            OnHostnameSetWithCurrent();
        }
        private void OnDestroy() { 
        
        StaticServerHostnameToUse.RemoveOnSetListener(OnHostnameSet);
        }
    private void OnHostnameSet(string hostname)
    {
        m_hostnameToUseDebugField = hostname;
            m_onHostnameSet?.Invoke(m_hostnameToUseDebugField);
        }
}

}