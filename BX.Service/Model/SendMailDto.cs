using System.Collections.Generic;

namespace BX.Service.Model
{
    public class SendEmailDto
    {
        /// <summary>
        /// 收件者
        /// </summary>
        public List<string> To { get; set; } = new List<string>();

        /// <summary>
        /// View 檔名
        /// </summary>
        public string ViewFile { get; set; }
    }
}
