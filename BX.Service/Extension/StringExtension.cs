using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BX.Service
{
    /// <summary>
    /// 字串擴充方法
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 產生驗證碼(4碼數字)
        /// </summary>
        /// <returns>回傳亂數碼</returns>
        public static string GeneratorAuthCode()
        {

            return string.Format("{0:0000}", (new Random()).Next(10000));
        }

        /// <summary>
        /// 讀取檔案轉字串
        /// </summary>
        /// <param name="path">檔案相對路徑</param>
        /// <returns>文字內容</returns>
        public static string ReadToString(string path)
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, path);
            string result = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

            return result;
        }

        /// <summary>
        /// 大量替換掉字串中的變數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string ReplaceString<T>(string input , T model)
        {
            Dictionary<string, string> dic = model.GetType().GetProperties().ToDictionary
                                                (prop => prop.Name, prop => prop.GetValue(model) == null ? 
                                                                                string.Empty : 
                                                                                prop.GetValue(model).ToString());
            int firstIndex = 0;
            int secondIndex = 0;
            string replace = "";

            do
            {
                firstIndex = input.IndexOf("$");
                secondIndex = input.IndexOf("$", firstIndex + 1);
                replace = input.Substring(firstIndex, secondIndex - firstIndex + 1);

                input = input.Replace(replace, dic[replace.Trim('$')]);

            } while (!input.IndexOf("$").Equals(-1));

            return input;
        }

        /// <summary>
        /// JSON字串轉物件
        /// </summary>
        /// <typeparam name="T">物件類型</typeparam>
        /// <param name="s">JSON字串</param>
        /// <returns>物件</returns>
        public static T ToTypedObject<T>(this string s)
        {
            // 去掉字串的前後空白、換行、tab 符號
            s = s?.Trim('\r', '\n', '\t', ' ');
            if (s.IsNullOrEmpty() || !Regex.IsMatch(s, @"^(\[|\{)(.|\n)*(\]|\})$", RegexOptions.Compiled))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(s);
        }

        /// <summary>
        /// 物件轉JSON字串
        /// </summary>
        /// <param name="target">物件</param>
        /// <param name="isCamelCase">JSON鍵值是否為小駝峰，預設true</param>
        /// <returns>JSON字串</returns>
        public static string ToJson(this object target, bool isCamelCase = true)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings()
            {
                ContractResolver = isCamelCase ? new CamelCasePropertyNamesContractResolver() : new DefaultContractResolver()
            };

            return JsonConvert.SerializeObject(target, Newtonsoft.Json.Formatting.None, setting);
        }
    }
}
