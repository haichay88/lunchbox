using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Bizkasa.Bizlunch.Business.Utils
{
    public class MailUtility
    {
        #region Constructors

        public MailUtility(string smtpServer, int port, string fromEmail, string fromEmailName, string userName, string passWord, string urlService)
        {
            this.SmtpServer = smtpServer;
            this.Port = port;
            this.FromEmail = fromEmail;
            this.FromEmailName = fromEmailName;
            this.UserName = userName;
            this.PassWord = passWord;
            this.UrlService = urlService;
            this.ExchangeVersion = Microsoft.Exchange.WebServices.Data.ExchangeVersion.Exchange2010;
        }

        #endregion Constructors

        #region Properties

        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string FromEmail { get; set; }
        public string FromEmailName { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string UrlService { get; set; }

        public ExchangeVersion ExchangeVersion { get; set; }

        #endregion Properties

        #region Methods

        public void SendMail(string Subject, string Body, string Recipient)
        {
            SendMail(Subject, Body, Recipient, "");
        }

        public void SendMail(string Subject, string Body, string Recipient, string CcRecipient)
        {
            ExchangeService service = new ExchangeService(this.ExchangeVersion);

            System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate(object sender1, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; };
            // logged on to the domain.

            service.Credentials = new WebCredentials(this.UserName, this.PassWord);
            WebProxy proxyObj = new WebProxy();
            proxyObj.Credentials = System.Net.CredentialCache.DefaultCredentials;
            service.WebProxy = proxyObj;
            // Or use NetworkCredential directly (WebCredentials is a wrapper
            // around NetworkCredential).
            service.Url = new Uri(this.UrlService);
            //service.AutodiscoverUrl("bangn.ho@mbbank.com.vn");

            EmailMessage message = new EmailMessage(service);

            message.Sender = this.FromEmailName;
            message.Subject = Subject;
            message.Body = Body;

            if (!string.IsNullOrEmpty(Recipient) && (Recipient != "") && (Recipient.IndexOf("@") > 0))
            {
                if (!string.IsNullOrEmpty(Recipient))
                {
                    if (Recipient.Contains(";"))
                    {
                        string[] arrCcEmail = Recipient.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string email in arrCcEmail)
                        {
                            message.ToRecipients.Add(email);
                        }
                    }
                    else
                    {
                        message.ToRecipients.Add(Recipient);
                    }
                }
            }

            if (!string.IsNullOrEmpty(CcRecipient) && (CcRecipient != "") && (CcRecipient.IndexOf("@") > 0))
            {
                if (!string.IsNullOrEmpty(CcRecipient))
                {
                    if (CcRecipient.Contains(";"))
                    {
                        string[] arrCcEmail = CcRecipient.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string email in arrCcEmail)
                        {
                            message.CcRecipients.Add(email);
                        }
                    }
                    else
                    {
                        message.CcRecipients.Add(CcRecipient);
                    }
                }
            }

            message.Send();
        }

        public string GetTemplateData(string TemplatePath)
        {
            string m_TemplateData = string.Empty;
            try
            {
                string fileExtension = System.IO.Path.GetExtension(TemplatePath).ToLower();
                if (fileExtension == ".html" || fileExtension == ".htm")
                {
                    using (StreamReader sr = new StreamReader(TemplatePath))
                    {
                        String line = sr.ReadToEnd();
                        m_TemplateData = line.ToString();
                    }
                }
            }
            catch (Exception)
            {
            }

            return m_TemplateData;
        }

        public string FieldTemplateMail(object objectSource, string text)
        {
            string m_Text = text;
            List<string> m_PatternParts = GetFieldFromPattern(text);

            PropertyInfo[] m_Properties = objectSource.GetType().GetProperties();
            foreach (PropertyInfo m_Property in m_Properties)
            {
                string m_Key = string.Format("{0}{1}{2}", MailUtility.KeyBegin, m_Property.Name, MailUtility.KeyEnd);
                string m_Key2 = m_PatternParts.Where(a => a.Contains(m_Property.Name)).ToList().FirstOrDefault();
                string[] m_Keys = null;
                if (!string.IsNullOrEmpty(m_Key2) && m_Key2.Split(':').Count() > 1)
                {
                    m_Key = string.Format("{0}{1}{2}", MailUtility.KeyBegin, m_Key2, MailUtility.KeyEnd);
                    m_Keys = m_Key2.Split(':');
                }
                try
                {
                    if (m_Text.Contains(m_Key))
                    {
                        string m_Value = string.Empty;
                        //Check DateTime
                        object m_ValueObject = m_Property.GetValue(objectSource, null);
                        if (m_ValueObject is DateTime)
                            m_Value = ((DateTime)m_ValueObject).ToString("dd/MM/yyyy");
                        else
                            m_Value = m_ValueObject + string.Empty;

                        m_Value = string.Format("{0}", m_Value);
                        if (m_Keys != null && m_Keys.Count() > 1)
                            m_Value = m_Value.Replace("@@@@", m_Keys[1]);
                        m_Text = m_Text.Replace(m_Key, m_Value);
                    }
                }
                catch (Exception)
                {
                }
            }

            return m_Text;
        }

        private List<string> GetFieldFromPattern(string pattern)
        {
            List<string> m_PatternParts = new List<string>();
            // First we see the input string.
            string m_Pattern = pattern;

            string[] m_Parts = m_Pattern.Split(new string[] { "[%" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var m_Part in m_Parts)
            {
                string m_PatternPart = m_Part.Split(new string[] { "%]" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (!string.IsNullOrEmpty(m_PatternPart))
                    m_PatternParts.Add(m_PatternPart);
            }

            return m_PatternParts.Distinct().ToList();
        }

        #endregion Methods

        #region Constants

        public const string KeyBegin = "[%";
        public const string KeyEnd = "%]";

        #endregion Constants
    }
}