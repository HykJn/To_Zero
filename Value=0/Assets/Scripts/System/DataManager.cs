using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static GlobalDefines;

[Serializable]
public struct DataSet
{
    //TODO: Define data for saving.
    public int chapter;
    public int clearedStage;

    public DataSet(int chapter, int clearedStage)
    {
        this.chapter = chapter;
        this.clearedStage = clearedStage;
    }
}

public static class DataManager
{
    private static readonly string Path = Application.persistentDataPath + "/data.dat";
    private static readonly Aes Aes = Aes.Create();
    private const string AES_KEY = "LyUtReJMrfsdEAWzSVRe25UkXMNCncap";
    private const string AES_IV = "eV9YSfyQk7eiOecq";

    public static void Save()
    {
        //Create Data set
        //TODO: Improve setting data logic
        DataSet data = new(0, 0);

        //Save data
        using StreamWriter sw = new(Path);
        sw.Write(Encrypt(JsonUtility.ToJson(data)));
        sw.Flush();
        sw.Close();
    }

    public static void Load()
    {
        using StreamReader sr = new(Path);
        DataSet data = JsonUtility.FromJson<DataSet>(Decrypt(sr.ReadToEnd()));
        sr.Close();

        //TODO: Implement handling for loading data
        Debug.Log("Load Complete");
        Debug.Log("Chapter: " + data.chapter);
        Debug.Log("Cleared Stage: " + data.clearedStage);
    }

    private static string Encrypt(string plainText)
    {
        ICryptoTransform encryptor = Aes.CreateEncryptor();
        byte[] buffer = Encoding.UTF8.GetBytes(plainText);

        byte[] encrypted = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
        return Convert.ToBase64String(encrypted);
    }

    private static string Decrypt(string encryptedText)
    {
        ICryptoTransform decryptor = Aes.CreateDecryptor();
        byte[] buffer = Convert.FromBase64String(encryptedText);

        byte[] decrypted = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(decrypted);
    }

    static DataManager()
    {
        Aes.Key = Encoding.UTF8.GetBytes(AES_KEY);
        Aes.IV = Encoding.UTF8.GetBytes(AES_IV);
        Aes.Mode = CipherMode.CBC;
        Aes.Padding = PaddingMode.PKCS7;
    }
}