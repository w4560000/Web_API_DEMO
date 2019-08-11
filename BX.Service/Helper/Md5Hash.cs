using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BX.Service
{
    /// <summary>
    /// Md5 Hash
    /// </summary>
    public static class Md5Hash
    {
        /// <summary>
        /// 取得MD5加密過的密碼
        /// </summary>
        /// <param name="PassWord">密碼</param>
        /// <returns>加密過的密碼</returns>
        public static string GetMd5Hash(string input)
        {
            StringBuilder sBuilder = new StringBuilder();
            using (MD5 md5Hasher = MD5.Create())
            {
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
            }
            return sBuilder.ToString();
        }
    }
}
