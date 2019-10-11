using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BX.Service
{
    /// <summary>
    /// 列舉擴充方法
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 取得 enum 描敘
        /// </summary>
        /// <param name="code">結果代碼</param>
        /// <returns>描述文字</returns>
        public static string GetEnumDescription(this Enum code)
        {
            DescriptionAttribute attribute = code.GetType()
                 .GetField(code.ToString())
                 .GetCustomAttributes(typeof(DescriptionAttribute), false)
                 .SingleOrDefault() as DescriptionAttribute;

            return attribute == null ? code.ToString() : attribute.Description;
        }

        /// <summary>
        /// 經由 EmailInfoAttribute 取得顯示名稱
        /// </summary>
        /// <param name="enumVal">The enum value</param>
        /// <param name="prop">公開屬性名稱</param>
        /// <returns>顯示名稱</returns>
        public static string GetEmailValue(this Enum enumVal, string prop = "Href")
        {
            // 取得 enumVal 物件的類型
            Type type = enumVal.GetType();
            EmailInfoAttribute displayAttr = enumVal.GetEnumAttributeOfType<EmailInfoAttribute>();

            // FlagsAttribute 自訂屬性陣列有值
            if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
            {
                // 取得 Enum 常數值陣列
                Array flags = type.GetEnumValues();

                // 設定回傳字串
                StringBuilder sb = new StringBuilder();

                // 設定分隔字串
                string sep = string.Empty;

                // 將 flags 陣列轉換成 enum 型別陣列跑 foreach
                foreach (Enum flag in flags.Cast<Enum>())
                {
                    // enumVal 包含 flag 且都不為0
                    if (enumVal.HasFlag(flag) && ((int)(object)enumVal) > 0 && ((int)(object)flag) != 0)
                    {
                        // 抓 EmailInfoAttribute 的公用值
                        displayAttr = flag.GetEnumAttributeOfType<EmailInfoAttribute>();

                        sb.Append(sep);
                        sb.Append(displayAttr == null ? flag.ToString() : displayAttr.GetDisplay(prop));

                        sep = ", ";
                    }
                }

                return sb.ToString();
            }

            // 如果標籤有連結到 EmailInfoAttribute 則回傳抓取名稱，反之回傳 Enum 的型別
            return displayAttr is null ? enumVal.ToString() : displayAttr.GetDisplay(prop);
        }

        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        public static T GetEnumAttributeOfType<T>(this Enum enumVal) where T : Attribute
        {
            // 取得 enumVal 物件的類型
            Type type = enumVal.GetType();

            // 搜尋 enumVal 公用成員
            MemberInfo[] memInfo = type.GetMember(enumVal.ToString());

            if (memInfo.Length > 0)
            {
                object[] attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
                return (T)attributes.FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// 取得 EmailInfoAttribute 公開屬性內容，預設Href
        /// </summary>
        /// <param name="attr">EmailInfoAttribute 公開屬性內容</param>
        /// <param name="propName">公開屬性名稱，預設Href</param>
        /// <returns>The value that is used for display in the UI</returns>
        private static string GetDisplay(this EmailInfoAttribute attr, string propName = "Href")
        {
            string result = string.Empty;

            // 取得 enumVal 物件的類型
            Type tp = attr.GetType();

            // 設定公用屬性
            PropertyInfo prop = tp.GetProperty(propName);

            // 公用屬性不為 NULL 則回傳對應值
            if (prop != null)
            {
                result = (string)prop.GetValue(attr);
            }

            return result;
        }
    }
}
