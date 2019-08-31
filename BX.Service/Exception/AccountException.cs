namespace BX.Service
{
    /// <summary>
    /// AccountException
    /// </summary>
    public class AccountException : System.Exception
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="message">錯誤訊息</param>
        public AccountException(string message) : base(message)
        {
        }

        /// <summary>
        /// 建構子
        /// </summary>
        public AccountException()
        {
        }
    }
}