using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.Utils
{
    public interface IEmailProvider
    {
        void Send(string recipient, string ccRecipient, string subject, string content);
        void Send(string recipient, string ccRecipient, string subject, string content, string[] attachments);
    }

    public class EmailProvider : IEmailProvider
    {
        #region Properties

        public IDictionary<string, string> ParameterDictionary { get; set; }
        public IList<EmailParameterField> EmailParameterFields;
        public EmailConfig EmailConfig { get; set; }
        public ExchangeVersion ExchangeVersion { get; set; }
        public string EmailMessageBody { get; set; }
        #endregion

        #region Constructors
        public EmailProvider()
        {
            this.ParameterDictionary = new Dictionary<string, string>();
            this.EmailConfig = new EmailConfig();
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
            this.EmailMessageBody = content;

            //1. Read Config
            this.ParseConfig(); ;

            //2. Fill Data
            this.FillParameter();
            this.FillData();

            //3. Config Mail
            
            ExchangeService m_ExchangeService = new ExchangeService(this.ExchangeVersion);
            System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate(object sender1, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
            m_ExchangeService.Credentials = new WebCredentials(EmailConfig.UserName, EmailConfig.Password);

            WebProxy m_Proxy = new WebProxy();
            m_Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            m_ExchangeService.WebProxy = m_Proxy;
            m_ExchangeService.Url = new Uri(EmailConfig.UrlService);

            //4. Send Mail

            EmailMessage m_EmailMessage = new EmailMessage(m_ExchangeService);
            m_EmailMessage.Sender = EmailConfig.FromEmail;
            m_EmailMessage.Subject = subject;
            m_EmailMessage.Body = EmailMessageBody;

            if (!string.IsNullOrEmpty(recipient) && (recipient != "") && (recipient.IndexOf("@") > 0))
            {
                if (recipient.Contains(";"))
                {
                    string[] arrCcEmail = recipient.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string email in arrCcEmail)
                    {
                        m_EmailMessage.ToRecipients.Add(email);
                    }
                }
                else
                {
                    m_EmailMessage.ToRecipients.Add(recipient);
                }
            }

            if (!string.IsNullOrEmpty(ccRecipient) && (ccRecipient != "") && (ccRecipient.IndexOf("@") > 0))
            {
                if (ccRecipient.Contains(";"))
                {
                    string[] arrCcEmail = ccRecipient.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string email in arrCcEmail)
                    {
                        m_EmailMessage.CcRecipients.Add(email);
                    }
                }
                else
                {
                    m_EmailMessage.CcRecipients.Add(ccRecipient);
                }
            }

            if (!string.IsNullOrEmpty(recipient) && (recipient.IndexOf("@") > 0))
            {
                m_EmailMessage.Send();
            }
        }

        private void ParseConfig()
        {
            EmailParameterFields = new List<EmailParameterField>();

            var m_BodyMatches = System.Text.RegularExpressions.Regex.Matches(this.EmailMessageBody, Regex);

            foreach (Match m in m_BodyMatches)
            {
                var m_TextNoKey = m.Groups[1].ToString();
                string[] m_TextNoKeyParts = m_TextNoKey.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                if (m_TextNoKeyParts.Length == 2)
                {
                    var m_FieldInfo = new EmailParameterField()
                    {
                        Type = m_TextNoKeyParts[0],
                        Name = m_TextNoKeyParts[1]
                    };
                    EmailParameterFields.Add(m_FieldInfo);
                }
            }
        }

        private void FillParameter()
        {
            EmailParameterField[] m_FieldInfos = this.EmailParameterFields.Where(f => f.Type == KeyType_Parameter).ToArray();
            foreach (var m_FieldInfo in m_FieldInfos)
            {
                string m_Value = string.Empty;
                if (this.ParameterDictionary.TryGetValue(m_FieldInfo.Name, out m_Value))
                {
                    var m_Key = String.Format("{0}{1}{2}", m_FieldInfo.Type, Key_Seperator, m_FieldInfo.Name);
                    var m_Parameter = String.Format("{0}{1}{2}", Key_Start, m_Key, Key_End);

                    EmailMessageBody = EmailMessageBody.Replace(m_Parameter, m_Value.ToString());
                }
            }
        }

        private void FillData()
        {
            EmailParameterField[] m_FieldInfos = this.EmailParameterFields.Where(f => f.Type == KeyType_Field).ToArray();
            if (m_FieldInfos.Length > 0)
            {

                string m_Value = string.Empty;
                foreach (var m_FieldInfo in m_FieldInfos)
                {
                    if (this.ParameterDictionary.TryGetValue(m_FieldInfo.Name, out m_Value))
                    {
                        var m_Key = String.Format("{0}{1}{2}", m_FieldInfo.Type, Key_Seperator, m_FieldInfo.Name);
                        var m_Field = String.Format("{0}{1}{2}", Key_Start, m_Key, Key_End);

                        EmailMessageBody = EmailMessageBody.Replace(m_Field, m_Value.ToString());
                    }
                }
            }
        }
        #endregion

        #region Constants

        public const string Key_Start = "[%";
        public const string Key_End = "%]";
        public const string Key_Seperator = ":";
        public const string Regex = @"\[%([^]]*)%\]";
        public const string KeyType_Parameter = "Parameter";
        public const string KeyType_Field = "Field";

        #endregion
    }

    public class EmailParameterField
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class EmailConfig
    {
        public string SmtpServer { get; set; }
        public string UrlService { get; set; }
        public int Port { get; set; }
        public string FromEmail { get; set; }
        public string FromEmailName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
