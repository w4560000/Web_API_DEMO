using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI_DEMO.ViewModel;

namespace WebAPI_DEMO.Model
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
