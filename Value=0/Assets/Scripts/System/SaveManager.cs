using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

[Serializable]
public struct SaveData
{
    public int chapter, stage;

    public SaveData(int chapter, int stage)
    {
        this.chapter = chapter;
        this.stage = stage;
    }
}

public static class SaveManager
{
    #region =====Fields=====

    private static readonly string Path = System.IO.Path.Combine(Application.persistentDataPath, "save.dat");

    private static readonly string _aesKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(
        "q9GJSlPC5eRpbJx8e1dQFX4yLx1zyV7I".PadRight(32, '0')));
    private static readonly string _aesIV = Convert.ToBase64String(Encoding.UTF8.GetBytes(
        "4iGQ7mBP7GZSYqhI".PadRight(16, '0')));

    #endregion

    #region =====Methods=====

    public static bool Save()
    {
        try
        {
            SaveData dat = new(3, 1);
            string json = JsonUtility.ToJson(dat, true);
            string enc = Encrypt(json);

            using StreamWriter sw = new(Path);
            sw.Write(enc);

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }

    public static bool Load()
    {
        try
        {
            using StreamReader sr = new(Path);
            string cipher = sr.ReadToEnd();
            string plain = Decrypt(cipher);
            SaveData dat = JsonUtility.FromJson<SaveData>(plain);

            Debug.Log("Load Complete.");
            Debug.Log("Chapter: " + dat.chapter);
            Debug.Log("Stage: " + dat.stage);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return false;
        }
    }

    private static string Encrypt(string plain)
    {
        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Key = Convert.FromBase64String(_aesKey);
        aes.IV = Convert.FromBase64String(_aesIV);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using (MemoryStream ms = new())
        {
            using (CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write))
            {
                using (StreamWriter sw = new(cs))
                {
                    sw.Write(plain);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    private static string Decrypt(string cipher)
    {
        using Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Key = Convert.FromBase64String(_aesKey);
        aes.IV = Convert.FromBase64String(_aesIV);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using (MemoryStream ms = new(Convert.FromBase64String(cipher)))
        {
            using (CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader sr = new(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }

    #endregion
}