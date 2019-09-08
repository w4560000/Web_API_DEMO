using System;

namespace BX.Service
{
    public interface IRedisService
    {
        /// <summary>
        /// 取得帳號登入時間
        /// </summary>
        /// <param name="account">帳號</param>
        /// <returns>帳號登入時間</returns>
        DateTime GetAccountLoginDate(string account);

        /// <summary>
        /// 更新帳號登入時間
        /// </summary>
        /// <param name="account">帳號</param>
        void UpdateAccountLoginDate(string account);


        /// <summary>
        /// 刪除帳號登入時間
        /// </summary>
        /// <param name="account">帳號</param>
        void DeleteAccountLoginDate(string account);

    }
}
