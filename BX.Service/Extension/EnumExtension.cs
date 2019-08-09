using System;
using System.ComponentModel;
using System.Linq;

namespace BX.Service
{
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
    }
}
