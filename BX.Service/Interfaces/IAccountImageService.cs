using System.Threading.Tasks;

namespace BX.Service
{
    public interface IAccountImageService
    {
        /// <summary>
        /// 上傳大頭貼圖檔
        /// </summary>
        /// <param name="accountData">帳號名稱</param>
        /// <param name="imageBase64String">圖檔資料</param>
        /// <returns>上傳結果</returns>
        Task<Result> UpLoadImageAsync(string accountName, string imageBase64String);


        Result<string> GetImage(string accountName);
    }
}