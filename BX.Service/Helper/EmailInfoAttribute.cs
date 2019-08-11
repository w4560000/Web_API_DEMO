using System;

namespace BX.Service
{
    /// <summary>
    /// Action 連結標籤
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EmailInfoAttribute : Attribute
    {
        /// <summary>
        /// Href
        /// </summary>
        public string Href { get; set; } = "#";

        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }
    }
}