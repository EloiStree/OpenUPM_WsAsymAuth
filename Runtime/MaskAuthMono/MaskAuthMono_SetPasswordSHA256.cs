using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class MaskAuthMono_SetPasswordSHA256 : MonoBehaviour
{



    public string m_passwordToHash = "HelloWorld";
    public string m_hashedPassword = "";
    public UnityEvent<string> m_onHashChanged;

    public bool m_useAwake;

    public void Awake()
    {
        SetPasswordFromTextInInspector();
    }
    public void OnValidate()
    {
        SetPasswordFromText(m_passwordToHash);
    }

    public void GetCurrentPassword(out string password)
    {
        password = m_passwordToHash;
    }
    public void GetHashedPassword(out string hashedPassword)
    {
        hashedPassword = m_hashedPassword;
    }
    [ContextMenu("Inspector to set password")]
    public void SetPasswordFromTextInInspector()
    {
        SetPasswordFromText(m_passwordToHash);
    }
    public void SetPasswordFromText(string m_passwordToHash)
    {
        m_passwordToHash = m_passwordToHash.Trim();
        SHA256Encoder.TextToSHA256(m_passwordToHash, out m_hashedPassword);
        m_onHashChanged.Invoke(m_hashedPassword);
    }

    public static void TextToSHA256(string text, out string textHashed)
    {

        // Convert the password string to a byte array
        byte[] passwordBytes = Encoding.UTF8.GetBytes(text);

        // Compute the SHA-256 hash
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);

            // Convert the hash byte array to a hexadecimal string
            StringBuilder hashStringBuilder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                hashStringBuilder.Append(b.ToString("x2")); // Convert each byte to hex format
            }
            textHashed = hashStringBuilder.ToString(); // Return the resulting hash
            return;
        }
    
    }
  
}
