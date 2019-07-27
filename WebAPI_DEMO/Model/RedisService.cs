using StackExchange.Redis;
using System;

namespace BX.Web.Model
{
    public class RedisService : IRedisService
    {
        //更新登入時間
        public void UpdateLoginDate(string Account)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase(0);

            db.HashSet("LoginAccountDate", new HashEntry[] { new HashEntry(Account, DateTime.Now.ToString()) });

            redis.Dispose();

        }

        //刪除登入時間
        public void DeleteLoginDate(string Account)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase(0);

            db.HashDelete("LoginAccountDate", Account);

            redis.Dispose();
        }

        //取得登入時間
        public DateTime GetRedisDate(string Account)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase(0);

            var result = db.HashGet("LoginAccountDate", Account);

            redis.Dispose();

            return Convert.ToDateTime(result);


        }
    }
}
