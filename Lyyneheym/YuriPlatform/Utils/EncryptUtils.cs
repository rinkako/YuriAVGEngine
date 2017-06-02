using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Yuri.PlatformCore;

namespace Yuri.Utils
{
    /// <summary>
    /// 加密解密类
    /// </summary>
    public static class EncryptUtils
    {
        /// <summary>
        /// 对一个字符串做DES加密
        /// </summary>
        /// <param name="data">要加密的数据</param>
        /// <param name="key">私钥</param>
        /// <returns>加密完毕的字符串</returns>
        public static string EncryptString(string data, string key)
        {
            string str = string.Empty;
            if (string.IsNullOrEmpty(data))
            {
                return str;
            }
            MemoryStream ms = new MemoryStream();
            byte[] myKey = Encoding.UTF8.GetBytes(key);
            byte[] myIV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            DES myProvider = new DESCryptoServiceProvider();
            CryptoStream cs = new CryptoStream(ms, myProvider.CreateEncryptor(myKey, myIV), CryptoStreamMode.Write);
            try
            {
                byte[] bs = Encoding.UTF8.GetBytes(data);
                cs.Write(bs, 0, bs.Length);
                cs.FlushFinalBlock();
                str = Convert.ToBase64String(ms.ToArray());
            }
            finally
            {
                cs.Close();
                ms.Close();
            }
            return str;
        }

        /// <summary>
        /// 对一个字符串做DES解密
        /// </summary>
        /// <param name="data">要解密的数据</param>
        /// <param name="key">私钥</param>
        /// <returns>解密完毕的字符串</returns>
        public static string DecryptString(string data, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(data))
                {
                    throw new Exception("data is empty");
                }
                MemoryStream ms = new MemoryStream();
                var myKey = Encoding.UTF8.GetBytes(key);
                byte[] myIV = {0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF};
                DES myProvider = new DESCryptoServiceProvider();
                CryptoStream cs = new CryptoStream(ms, myProvider.CreateDecryptor(myKey, myIV), CryptoStreamMode.Write);
                var bs = Convert.FromBase64String(data);
                cs.Write(bs, 0, bs.Length);
                cs.FlushFinalBlock();
                var str = Encoding.UTF8.GetString(ms.ToArray());
                cs.Close();
                ms.Close();
                return str;
            }
            catch (Exception ex)
            {
                LogUtils.LogLine("Game Key check failed." + ex, "YuriEncryptor", LogLevel.Error);
                Director.GetInstance().GetMainRender().Shutdown();
            }
            return String.Empty;
        }
    }
}