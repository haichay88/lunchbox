using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LacViet.HPS.Common.Utilities
{
    public interface IEmail3Provider
    {
        void Send(string recipient, string ccRecipient, string subject, string content);
        void Send(string recipient, string ccRecipient, string subject, string content, string[] attachments);
    }

    public class Email3Provider : IEmail3Provider
    {
        #region Properties

        public IDictionary<string, string> ParameterDictionary { get; set; }
        public IList<Email3ParameterField> Email3ParameterFields;
        public Email3Config Email3Config { get; set; }
        public ExchangeVersion ExchangeVersion { get; set; }
        public string Email3MessageBody { get; set; }
        #endregion

        #region Constructors
        public Email3Provider()
        {
            this.ParameterDictionary = new Dictionary<string, string>();
            this.Email3Config = new Email3Config();
            this.ExchangeVersion = Microsoft.Exchange.WebServices.Data.ExchangeVersion.Exchange2010;
        }
        #endregion

        #region Method

        public void Send(string recipient, string ccRecipient, string subject, string content)
        {
            Send(recipient, ccRecipient, subject, content, null);
        }

        public void Send(string recipient, string ccRecipient, string subject, string content, string[] attachments)
        {
            this.Email3MessageBody = content;

            //1. Read Config
            this.ParseConfig(); ;

            //2. Fill Data
            this.FillParameter();
            this.FillData();
            

            //3. Config Mail
            
            ExchangeService m_ExchangeService = new ExchangeService(this.ExchangeVersion);
            System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate(object sender1, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
            m_ExchangeService.Credentials = new WebCredentials(Email3Config.UserName, Email3Config.Password);

            WebProxy m_Proxy = new WebProxy();
            m_Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            m_ExchangeService.WebProxy = m_Proxy;
            m_ExchangeService.Url = new Uri(Email3Config.UrlService);

            //4. Send Mail

            EmailMessage m_Email3Message = new EmailMessage(m_ExchangeService);
            this.ParseImageBody(m_Email3Message);
            m_Email3Message.Sender = Email3Config.FromEmail3;
            m_Email3Message.Subject = subject;
            m_Email3Message.Body = Email3MessageBody;

            if (!string.IsNullOrEmpty(recipient) && (recipient != "") && (recipient.IndexOf("@") > 0))
            {
                if (recipient.Contains(";"))
                {
                    string[] arrCcEmail3 = recipient.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string Email3 in arrCcEmail3)
                    {
                        m_Email3Message.ToRecipients.Add(Email3);
                    }
                }
                else
                {
                    m_Email3Message.ToRecipients.Add(recipient);
                }
            }

            if (!string.IsNullOrEmpty(ccRecipient) && (ccRecipient != "") && (ccRecipient.IndexOf("@") > 0))
            {
                if (ccRecipient.Contains(";"))
                {
                    string[] arrCcEmail3 = ccRecipient.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string Email3 in arrCcEmail3)
                    {
                        m_Email3Message.CcRecipients.Add(Email3);
                    }
                }
                else
                {
                    m_Email3Message.CcRecipients.Add(ccRecipient);
                }
            }

            if (!string.IsNullOrEmpty(recipient) && (recipient.IndexOf("@") > 0))
            {
                m_Email3Message.Send();
            }
        }

        private void ParseImageBody(EmailMessage m_Email3Message)
        {
            List<string> links = new List<string>();
            string regexImgSrc = @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>";
            MatchCollection matchesImgSrc = Regex.Matches(Email3MessageBody, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in matchesImgSrc)
            {
                if (Uri.IsWellFormedUriString(m.Groups[1].Value, UriKind.Absolute))
                {
                    links.Add(m.Groups[1].Value);
                }
            }
            links.ForEach(e =>
            {
                try
                {
                    string NameImage = Guid.NewGuid().ToString();
                    Email3MessageBody = Email3MessageBody.Replace(e, "cid:" + NameImage + ".jpg");
                    var webClient = new WebClient();
                    byte[] imageBytes = webClient.DownloadData(e);
                    m_Email3Message.Attachments.AddFileAttachment(NameImage + ".jpg", imageBytes);
                }
                catch
                {
                }
            });
        }
        private void ParseConfig()
        {
            Email3ParameterFields = new List<Email3ParameterField>();

            var m_BodyMatches = System.Text.RegularExpressions.Regex.Matches(this.Email3MessageBody, RegexExpression);

            foreach (Match m in m_BodyMatches)
            {
                var m_TextNoKey = m.Groups[1].ToString();
                string[] m_TextNoKeyParts = m_TextNoKey.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (m_TextNoKeyParts.Length == 2)
                {
                    var m_FieldInfo = new Email3ParameterField()
                    {
                        Type = m_TextNoKeyParts[0],
                        Name = m_TextNoKeyParts[1]
                    };
                    Email3ParameterFields.Add(m_FieldInfo);
                }
            }
        }

        private void FillParameter()
        {
            Email3ParameterField[] m_FieldInfos = this.Email3ParameterFields.Where(f => f.Type == KeyType_Parameter).ToArray();
            foreach (var m_FieldInfo in m_FieldInfos)
            {
                string m_Value = string.Empty;
                if (this.ParameterDictionary.TryGetValue(m_FieldInfo.Name, out m_Value))
                {
                    var m_Key = String.Format("{0}{1}{2}", m_FieldInfo.Type, Key_Seperator, m_FieldInfo.Name);
                    var m_Parameter = String.Format("{0}{1}{2}", Key_Start, m_Key, Key_End);

                    Email3MessageBody = Email3MessageBody.Replace(m_Parameter, m_Value.ToString());
                }
            }
        }

        private void FillData()
        {
            Email3ParameterField[] m_FieldInfos = this.Email3ParameterFields.Where(f => f.Type == KeyType_Field).ToArray();
            if (m_FieldInfos.Length > 0)
            {

                string m_Value = string.Empty;
                foreach (var m_FieldInfo in m_FieldInfos)
                {
                    if (this.ParameterDictionary.TryGetValue(m_FieldInfo.Name, out m_Value))
                    {
                        var m_Key = String.Format("{0}{1}{2}", m_FieldInfo.Type, Key_Seperator, m_FieldInfo.Name);
                        var m_Field = String.Format("{0}{1}{2}", Key_Start, m_Key, Key_End);

                        Email3MessageBody = Email3MessageBody.Replace(m_Field, m_Value.ToString());
                    }
                }
            }
        }
        #endregion

        #region Constants

        public const string Key_Start = "[%";
        public const string Key_End = "%]";
        public const string Key_Seperator = ":";
        public const string RegexExpression = @"\[%([^]]*)%\]";
        public const string KeyType_Parameter = "Parameter";
        public const string KeyType_Field = "Field";

        #endregion
    }

    public class Email3ParameterField
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class Email3Config
    {
        public string SmtpServer { get; set; }
        public string UrlService { get; set; }
        public int Port { get; set; }
        public string FromEmail3 { get; set; }
        public string FromEmail3Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
