using BX.Repository;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace BX.Service
{
    public class RedisService : IRedisService, IDisposable
    {
        /// <summary>
        /// Destructor
        /// </summary>
        ~RedisService()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 取得帳號登入時間
        /// </summary>
        /// <param name="account">帳號</param>
        /// <returns>帳號登入時間</returns>
        public DateTime GetAccountLoginDate(string Account)
        {
            DateTime accountLoginDate = DateTime.Now;
            if (this.IsSet(Account + "_LoginAccountDate"))
            {
                accountLoginDate = this.Get<DateTime>(Account + "_LoginAccountDate");
            }

            return accountLoginDate;
        }

        /// <summary>
        /// 更新帳號登入時間
        /// </summary>
        /// <param name="account">帳號</param>
        public void UpdateAccountLoginDate(string Account)
        {
            this.Set(Account + "_LoginAccountDate", DateTime.Now.ToString(), 86400);
        }

        /// <summary>
        /// 刪除帳號登入時間
        /// </summary>
        /// <param name="account">帳號</param>
        public void DeleteAccountLoginDate(string Account)
        {
            this.Remove(Account + "_LoginAccountDate");
        }

        #region Redis連線

        /// <summary>
        /// Represents an inter-related group of connections to redis servers
        /// </summary>
        private ConnectionMultiplexer Redis;

        /// <summary>
        /// 鎖定物件
        /// </summary>
        private static readonly object LockObject = new object();

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool Disposed = false;

        /// <summary>
        /// 取得儲存在Azure上的資料庫連線字串
        /// </summary>
        private static string GetBxRedisConnectionFromAzureKeyVault()
        {
            return AzureKeyvaultHelper.GetAzureSecretVaule("bxRedisLabConnection");
        }

        /// <summary>
        /// 取得RedisDb
        /// </summary>
        /// <returns></returns>
        private IDatabase RedisDb()
        {
            return this.Connection.GetDatabase(0);
        }

        /// <summary>
        /// Redis 連線
        /// </summary>
        /// <exception cref="AggregateException">Redis Server 連線錯誤</exception>
        private ConnectionMultiplexer Connection
        {
            get
            {
                if (this.Redis == null || !this.Redis.IsConnected)
                {
                    lock (LockObject)
                    {
                        if (this.Redis == null || !this.Redis.IsConnected)
                        {
                            string connectionString = GetBxRedisConnectionFromAzureKeyVault();
                            this.Redis = ConnectionMultiplexer.Connect(connectionString + ",abortConnect=false");
                        }
                    }
                }

                return this.Redis;
            }
        }

        /// <summary>
        /// Release all resources associated with this object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            // This object will be cleaned up by the Dispose method. Therefore, you should call
            // GC.SupressFinalize to take this object off the finalization queue and prevent
            // finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">
        /// If disposing equals true, dispose all managed and unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (this.Disposed)
            {
                return;
            }

            // If disposing equals true, dispose all managed and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
                this.Redis?.Dispose();
            }

            // Note disposing has been done.
            this.Disposed = true;
        }

        #endregion Redis連線

        #region Redis操作

        /// <summary>
        /// 取得快取內容
        /// </summary>
        private T Get<T>(string key)
        {
            Task<RedisValue> getTask = this.RedisDb().StringGetAsync(key);
            RedisValue cacheData = this.RedisDb().Wait(getTask);

            if (cacheData.IsNullOrEmpty)
            {
                return default;
            }

            if (typeof(T).IsValueType || typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(cacheData.ToString(), typeof(T));
            }

            return cacheData.ToString().ToTypedObject<T>();
        }

        /// <summary>
        /// 指定快取內容
        /// </summary>
        private void Set(string key, object data, int cacheSeconds)
        {
            if (data == null)
            {
                return;
            }

            if (data is string || !data.GetType().IsClass)
            {
                Task.Factory.StartNew(() => this.RedisDb().StringSetAsync(key, data.ToString(), TimeSpan.FromSeconds(cacheSeconds)));

                return;
            }

            string cacheData = data.ToJson();
            Task.Factory.StartNew(() => this.RedisDb().StringSetAsync(key, cacheData, TimeSpan.FromSeconds(cacheSeconds)));
        }

        /// <summary>
        /// 確定快取鍵值是否存在
        /// </summary>
        public bool IsSet(string key) => this.RedisDb().KeyExists(key);

        /// <summary>
        /// 移除快取內容
        /// </summary>
        public void Remove(string key) => this.RedisDb().KeyDelete(key);

        #endregion Redis操作
    }
}