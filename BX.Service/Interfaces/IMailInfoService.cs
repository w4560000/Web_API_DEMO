using BX.Service.Model;
using System.Net.Mail;

namespace BX.Service
{
    public interface IMailInfoService
    {
        /// <summary>
        /// 設定Razor寄送郵件內容
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel</typeparam>
        /// <param name="emailTemplateDto">收件者資訊的 Dto</param>
        /// <param name="sendEmailDto">收件人資訊的 Dto</param>
        /// <param name="model">信件內容的 ViewModel</param>
        void SendMail<TViewModel>(
            EmailTemplateDto emailTemplateDto,
            SendEmailDto sendEmailDto,
            TViewModel model);
    }
}
