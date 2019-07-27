using System;

namespace BX.Web.Model
{
    public interface IRedisService
    {

        //更新登入時間
        void UpdateLoginDate(string Account);

        //刪除登入時間
        void DeleteLoginDate(string Account);

        //取得登入時間
        DateTime GetRedisDate(string Account);
    }
}
