using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace BX.Repository
{
    /// <summary>
    /// Azure金鑰Helper
    /// </summary>
    public static class AzureKeyvaultHelper
    {
        /// <summary>
        /// 取得AzureSecret
        /// </summary>
        /// <param name="secretName"></param>
        /// <returns></returns>
        public static string GetAzureSecretVaule(string secretName)
        {
            KeyVaultClient keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback));

            string connectionString = keyVaultClient.GetSecretAsync("https://bingxiangKeyvault.vault.azure.net/", secretName).Result.Value;

            return connectionString;
        }
    }
}