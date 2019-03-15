﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 11.2 密碼相關管理程式碼
/// </summary>
public static class PasswordUtility
{
    /// <summary>
    /// 11-11 檢查密碼長度
    /// 1. 最少 8 碼
    /// 2. 最少有 1 個大寫或小寫英文
    /// 3. 最少包含 1 個數字
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    public static bool PasswordLength(string password)
    {
        if (password.Length < 8)
        {
            return false;
        }
        else
        {
            // 自訂密碼規則
            if (0 - Convert.ToInt32(Regex.IsMatch(password, "[a-z]")) -             // 小寫
                    Convert.ToInt32(Regex.IsMatch(password, "[A-Z]")) -             // 大寫
                    Convert.ToInt32(Regex.IsMatch(password, "\\d")) -               // 數字
                    Convert.ToInt32(Regex.IsMatch(password, ".{10,}")) <= -2)       // 任意字元(除了換行符號)外重複 10 次以上, 即長度為 10 以上
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 11-17 AES 對稱加密演算法 － 加密
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="Key"></param>
    /// <param name="IV"></param>
    /// <returns></returns>
    public static string AESEncryptor(string plainText, byte[] Key, byte[] IV)
    {
        byte[] data = ASCIIEncoding.ASCII.GetBytes(plainText);
        AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
        string encryptedString = Convert.ToBase64String(aes.CreateEncryptor(Key, IV).TransformFinalBlock(data, 0, data.Length));
        return encryptedString;
    }
    /// <summary>
    /// 11-17 AES 對稱加密演算法 － 解密
    /// </summary>
    /// <param name="encryptedString"></param>
    /// <param name="Key"></param>
    /// <param name="IV"></param>
    /// <returns></returns>
    public static string AESDecryptor(string encryptedString, byte[] Key, byte[] IV)
    {
        byte[] data = Convert.FromBase64String(encryptedString);
        AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
        string decryptedString = ASCIIEncoding.ASCII.GetString(aes.CreateDecryptor(Key, IV).TransformFinalBlock(data, 0, data.Length));
        return decryptedString;
    }

    /// <summary>
    /// 11-19 SHA256 雜湊演算法
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string SHA256Encryptor(string plainText)
    {
        byte[] data = ASCIIEncoding.ASCII.GetBytes(plainText);
        SHA256 sha256 = new SHA256CryptoServiceProvider();
        byte[] result = sha256.ComputeHash(data);       //計算雜湊值

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// 11-19 SHA512 雜湊演算法
    /// </summary>
    /// <param name="plainText"></param>
    public static string SHA512Encryptor(string plainText)
    {
        byte[] data = ASCIIEncoding.ASCII.GetBytes(plainText);
        SHA512 sha512 = new SHA512CryptoServiceProvider();
        byte[] result = sha512.ComputeHash(data);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// 11-20 雜湊密碼 (SHA512 為例，利用與 GUID 的字串連接進行加密)
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string GuidwithPassword(Guid guid, string plainText)
    {
        byte[] data = ASCIIEncoding.ASCII.GetBytes(plainText + guid.ToString());
        byte[] result;
        SHA512Managed sha = new SHA512Managed();
        result = sha.ComputeHash(data);
        return Convert.ToBase64String(result);
    }
}
