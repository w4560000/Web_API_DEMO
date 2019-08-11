namespace BX.Service.Model
{
    /// <summary>
    /// 組成Email元素的物件
    /// </summary>
    public class EmailTemplateDto
    {
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 內文
        /// </summary>
        public string Content { get; set; }
    }
}
