using System;

namespace BX.Service
{
    /// <summary>
    /// 狀態碼回傳屬性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ResponseStatusAttribute : Attribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">回傳狀態碼</param>
        public ResponseStatusAttribute(string statusCode) : base()
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// 回傳狀態碼
        /// </summary>
        public string StatusCode { get; set; }
    }
}