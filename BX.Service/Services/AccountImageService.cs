using BX.Repository;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace BX.Service
{
    /// <summary>
    /// 帳號大頭貼服務
    /// </summary>
    public class AccountImageService : IAccountImageService
    {
        /// <summary>
        /// 上傳圖檔統一副檔名
        /// </summary>
        private string ImageFilenameExtension => ".png";

        /// <summary>
        /// 帳號大頭貼Azure儲存體資料夾
        /// </summary>
        private string ImageDirectory => "AccountImage/";

        /// <summary>
        /// 帳號大頭貼Azure儲存體容器名稱
        /// </summary>
        private string BxAPIStorageContainerName => "bxapi";

        /// <summary>
        /// 上傳大頭貼圖檔
        /// </summary>
        /// <param name="accountData">帳號名稱</param>
        /// <param name="imageBase64String">圖檔資料</param>
        /// <returns>上傳結果</returns>
        public async Task<Result> UpLoadImageAsync(string accountName, string imageBase64String)
        {
            if (this.ValidateImage(imageBase64String, out Result result, out byte[] bytes))
            {
                await this.UploadAzureStorageBlogImageByBytesAsync(accountName, bytes, result);
            }

            return result;
        }

        /// <summary>
        /// 取得大頭貼網址
        /// </summary>
        /// <param name="accountName">帳號名稱</param>
        /// <returns>大頭貼網址</returns>
        public Result<string> GetImage(string accountName)
        {
            Result<string> result = new Result<string>();
            string https = "https";
            result.Data = @$"{https}://bingxiangstorage.blob.core.windows.net/{this.BxAPIStorageContainerName}/{this.ImageDirectory}{accountName}{this.ImageFilenameExtension}";
            result.SetMessage(ResponseMessageEnum.GetAccountImageSuccess);

            return result;
        }

        /// <summary>
        /// 驗證圖檔
        /// </summary>
        /// <param name="imageBase64String">圖檔資料</param>
        /// <param name="result">驗證結果</param>
        /// <param name="bytes">圖檔bytes</param>
        /// <returns>是否驗證成功</returns>
        private bool ValidateImage(string imageBase64String, out Result result, out byte[] bytes)
        {
            bool validateSuccess = true;
            result = new Result();

            //取得content-type
            string type = imageBase64String.Substring(0, imageBase64String.IndexOf(";"));

            //取得圖檔完整的base64
            imageBase64String = imageBase64String.Substring(imageBase64String.IndexOf(",") + 1);

            //base64轉換為byte
            bytes = Convert.FromBase64String(imageBase64String);

            if (!type.Contains("jpeg") && !type.Contains("png"))
            {
                result.SetErrorMode().SetMessage(ResponseMessageEnum.AccountImageTypeError);

                return !validateSuccess;
            }

            return validateSuccess;
        }

        /// <summary>
        /// 上傳圖檔至AzureStorage
        /// </summary>
        /// <param name="bytes">圖檔bytes</param>
        private async Task UploadAzureStorageBlogImageByBytesAsync(string accountName, byte[] bytes, Result result)
        {
            try
            {
                string localFileName = $"{accountName}{this.ImageFilenameExtension}";

                CloudBlockBlob cloudBlockBlob = this.GetAzureCloudBlobDirectory().GetBlockBlobReference(localFileName);

                await cloudBlockBlob.DeleteAsync();
                await cloudBlockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);

                result.SetMessage(ResponseMessageEnum.UpLoadAccountImageSuccess);
            }
            catch
            {
                result.SetErrorMode().SetMessage(ResponseMessageEnum.UpLoadAccountImageFail);
            }
        }

        /// <summary>
        /// 取得存取帳號大頭貼AzureStorage實體
        /// </summary>
        /// <returns>AzureStorage實體</returns>
        private CloudBlobDirectory GetAzureCloudBlobDirectory()
        {
            string connectionString = AzureKeyvaultHelper.GetAzureSecretVaule("bxStorage");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(this.BxAPIStorageContainerName);

            return container.GetDirectoryReference(this.ImageDirectory);
        }

        #region 原先儲存在地端的方法

        /// <summary>
        //        /// 上傳大頭貼
        //        /// </summary>
        //        /// <param name="Account"></param>
        //        /// <param name="base64data"></param>
        //        /// <param name="type"></param>
        //        public string UpLoadImage(string Account, string base64data)
        //        {
        //            string sRtn = base64data
        //            var type = "";

        //            //取得content-type
        //            type = base64data.Substring(0, base64data.IndexOf(";"));

        //            //取得圖檔完整的base64
        //            base64data = base64data.Substring(base64data.IndexOf(",") + 1);

        //            //base64轉換為byte
        //            byte[] byteimage = Convert.FromBase64String(base64data);

        //            //若無此資料夾則新增
        //            var UpDirPath = "C://vue_image";
        //            if (!System.IO.Directory.Exists(UpDirPath))
        //            {
        //                System.IO.Directory.CreateDirectory(UpDirPath);
        //            }

        //            //先刪除原先的大頭照
        //            if (File.Exists(Path.Combine(UpDirPath, Account + "_image.jpg")))
        //            {
        //                File.Delete(Path.Combine(UpDirPath, Account + "_image.jpg"));
        //            }
        //            else if (File.Exists(Path.Combine(UpDirPath, Account + "_image.png")))
        //            {
        //                File.Delete(Path.Combine(UpDirPath, Account + "_image.png"));
        //            }

        //            //判斷副檔名並建立檔案
        //            if (type.Contains("jpeg"))
        //            {
        //                System.IO.File.WriteAllBytes(Path.Combine(UpDirPath, Account + "_image.jpg"), byteimage);
        //                sRtn = "上傳成功！";
        //            }
        //            else if (type.Contains("png"))
        //            {
        //                System.IO.File.WriteAllBytes(Path.Combine(UpDirPath, Account + "_image.png"), byteimage);
        //                sRtn = "上傳成功！";
        //            }
        //            else
        //                sRtn = "上傳失敗！請上傳JPF檔orPNG檔！";

        //            return sRtn;
        //        }
        //        /// <summary>
        //        /// 回傳大頭照
        //        /// </summary>
        //        /// <param name="Account"></param>
        //        /// <returns></returns>
        //        public string GetImage(string Account)
        //        {
        //            string UpDirPath = "C://vue_image";
        //            string type = "";
        //            string[] imagePaths = { };

        //            //抓去圖檔
        //            if (File.Exists(Path.Combine(UpDirPath, Account + "_image.jpg")))
        //            {
        //                imagePaths = Directory.GetFiles(UpDirPath, "*.jpg");
        //                type = "data:image / jpeg; base64,";
        //            }
        //            else if (File.Exists(Path.Combine(UpDirPath, Account + "_image.png")))
        //            {
        //                imagePaths = Directory.GetFiles(UpDirPath, "*.png");
        //                type = "data:image/png;base64,";
        //            }

        //            //若抓不到圖檔，代表該帳號無上傳大頭貼
        //            if (imagePaths.Count() > 0)
        //            {
        //                //file轉為byte再轉為base64
        //                FileStream fs = new FileStream(imagePaths[0], FileMode.Open);
        //                byte[] buffer = new byte[fs.Length];
        //                fs.Read(buffer, 0, buffer.Length);
        //                fs.Close();

        //                string Base64Str = Convert.ToBase64String(buffer);

        //                Base64Str = string.Concat(type, Base64Str);

        //                return Base64Str;
        //            }
        //            else
        //                return "";
        //        }

        #endregion 原先儲存在地端的方法
    }
}