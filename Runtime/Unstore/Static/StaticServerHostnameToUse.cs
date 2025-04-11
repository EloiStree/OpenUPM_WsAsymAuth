using System.IO;
using UnityEngine;

public class StaticServerHostnameToUse : MonoBehaviour
{

    public static string m_hostnameToUse= "raspberrypi.local";
    public static void SetHostnameAsRaspberryPi() => SetHostenameToUse("raspberrypi.local", true);
    public static void SetHostnameAsApintDefaultServer() => SetHostenameToUse("apint.ddns.net", true);
    public static void SetHostnameAsApintAsLocalhost() => SetHostenameToUse("127.0.0.1", true);

    public static void GetHostenameToUse(out string hostname)
    {
        hostname = m_hostnameToUse;
    }
    public static string GetHostenameToUse()
    {
        return m_hostnameToUse;
    }

    public static void SetHostenameToUse(string hostname, bool andSaveItAsFile=true)
    {
        m_hostnameToUse = hostname;
        if (andSaveItAsFile)
        {
            SaveHostnameAsFile();
        }

    }
    public static string GetHostnameStoringFilePath()
    {
        return Application.persistentDataPath+"/ServerHostname/Hostname.txt";
    }

    static StaticServerHostnameToUse()
    {
        LoadHostnameSavedOrCreateDefault();
    }

    public static void SaveHostnameAsFile()
    {
        string path = GetHostnameStoringFilePath();
        string directory = Path.GetDirectoryName(path);
        if ( !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);

        }
        File.WriteAllText(path, m_hostnameToUse);
    }

    public static void LoadHostnameSavedOrCreateDefault()
    {
        string path = GetHostnameStoringFilePath();
        if (File.Exists(path))
        {
            string hostname = File.ReadAllText(path);
            m_hostnameToUse = hostname;
        }
        else
        {
            // Create the directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            // Write the default hostname to the file
            File.WriteAllText(path, m_hostnameToUse);

        }
    }
}
