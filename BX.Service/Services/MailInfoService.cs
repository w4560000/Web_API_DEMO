using BX.Service.Model;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace BX.Service
{
    /// <summary>
    /// 信件內容設定服務
    /// </summary>
    public class MailInfoService : IMailInfoService
    {
        /// <summary>
        /// 寄送mail
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel</typeparam>
        /// <param name="emailTemplateDto">收件者資訊的 Dto</param>
        /// <param name="sendEmailDto">收件人資訊的 Dto</param>
        /// <param name="model">信件內容的 ViewModel</param>
        public void SendMail<TViewModel>(
            EmailTemplateDto emailTemplateDto,
            SendEmailDto sendEmailDto,
            TViewModel model)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new System.Net.NetworkCredential("testbingxiang@gmail.com", "a33456789");
            smtpClient.SendAsync(this.SetEmailByRazorInfo(emailTemplateDto, sendEmailDto, model), null);
        }

        /// <summary>
        /// 設定Razor寄送郵件內容
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel</typeparam>
        /// <param name="emailTemplateDto">收件者資訊的 Dto</param>
        /// <param name="sendEmailDto">收件人資訊的 Dto</param>
        /// <param name="model">信件內容的 ViewModel</param>
        /// <returns>傳送電子郵件訊息</returns>
        private MailMessage SetEmailByRazorInfo<TViewModel>(
            EmailTemplateDto emailTemplateDto,
            SendEmailDto sendEmailDto,
            TViewModel model)
        {
            // 讀取 view 的內容, 傳入Cache Key、Model 物件型別、Model 物件取得套表結果
            emailTemplateDto.Content = this.SetEmailLayout(sendEmailDto, model);

            return this.SetMailMessageData(emailTemplateDto, sendEmailDto);
        }

        /// <summary>
        /// 設定 MailMessage 資料
        /// </summary>
        /// <param name="emailTemplateDto">收件人資訊的 Dto</param>
        /// <param name="sendEmailDto">組成Email元素 Dto</param>
        /// <returns>傳送電子郵件訊息</returns>
        private MailMessage SetMailMessageData(
            EmailTemplateDto emailTemplateDto,
            SendEmailDto sendEmailDto)
        {
            // 設定寄送者與收件者
            MailMessage mailMessage = this.SetMailMessageInfo(sendEmailDto);
            mailMessage.From = new MailAddress("testbingxiang@gmail.com", "test", System.Text.Encoding.UTF8);

            // 設定MailMessage的內容
            mailMessage.Subject = emailTemplateDto.Title;
            mailMessage.Body = emailTemplateDto.Content;
            mailMessage.IsBodyHtml = true;

            // 郵件內容編碼 
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.Priority = MailPriority.Normal;

            return mailMessage;
        }

        /// <summary>
        /// 設定MailMessage的寄件人內容
        /// </summary>
        /// <param name="sendEmailDto">組成Email元素 Dto</param>
        /// <returns>傳送電子郵件訊息</returns>
        private MailMessage SetMailMessageInfo(SendEmailDto sendEmailDto)
        {
            // 設定 MailMessage
            MailMessage mailMessage = new MailMessage();
            sendEmailDto.To.ForEach(emailAddress => mailMessage.To.Add(new MailAddress(emailAddress, "收件者")));

            return mailMessage;
        }

        /// <summary>
        /// 讀取 view 的內容並做Mapping
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel</typeparam>
        /// <param name="sendEmailDto">收件人資訊的 Dto</param>
        /// <param name="model">信件內容的 ViewModel</param>
        /// <returns>回傳信件內容</returns>
        private string SetEmailLayout<T>(SendEmailDto sendEmailDto, T model)
        {
            List<string> htmlFileName = sendEmailDto.ViewFile.Split(",").ToList();
            StringBuilder mailContent = new StringBuilder();
            mailContent.Append(StringExtension.ReadToString("AppData/EmailTemplate/Header.txt"));

            htmlFileName.ForEach(x =>
            {
                mailContent.Append(StringExtension.ReadToString($"AppData/EmailTemplate/{x}"));
            });
            
            string content = StringExtension.ReplaceString<T>(mailContent.ToString(), model);

            return content;
        }
    }
}
