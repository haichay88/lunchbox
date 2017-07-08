using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using System.Net.Mail;
using Bizkasa.Bizlunch.Business.Extention;
using Bizkasa.Bizlunch.Business.BusinessLogic;
using Bizkasa.Bizlunch.Business.Model;
using System.Web;
using Microsoft.Practices.Unity;
using System.Net.Http.Formatting;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string serverKey = "AAAAY8UVqoU:APA91bHD9ICFvT1CdO-gcHyo4p69tfQfXNJa_dM0Y5JyXrqzezUZt0cG-ax_DOCg-bvDspgUBOTxpRb2IXvOhyiE6o7RBYFMzkVJct65LniMec0LIo8rQF3pMUDybc4gNc8jlcgeAq1D";

            try
            {
                var result = "-1";
                var webAddr = "https://fcm.googleapis.com/fcm/send";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization:key=" + serverKey);
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"to\": \"c7cOP-6Sn_4:APA91bEaj-PBS5c91p1FiPll08DTzpCZRf3RmOJcqvj4wWQqvB-6OTgrI3n_320lkL-d2rpPkNhtIeSSIX6zS8w287hQabHP8g6Yitv8YhtXAZaQTIz9D3emLyq7MN_GueDyG-qJWZNy\",\"data\": {\"message\": \"This is a Firebase Cloud Messaging Topic Message!\",\"title\": \"This is a title!\",\"id\": \"12\", \"actions\": [{ \"icon\": \"play\", \"title\": \"Play Music\", \"callback\": \"playMusic\"}, { \"icon\": \"archive\", \"title\": \"Archive\", \"callback\": \"archive\"} ]}}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                // return result;
            }
            catch (Exception ex)
            {
                //  Response.Write(ex.Message);
            }
        }

        [TestMethod]
        public void Cal()
        {
            var a = 106.6094098;


        }

        [TestMethod]
        public void SendEmail()
        {
            MailMessage Message = new MailMessage() {
                
            };
            
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.googlemail.com";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("myemail@gmail.com", "password");
            client.Send(Message);


        }

        [TestMethod]
        public void TestSendEmail()
        {
             string line = string.Empty;
            StringBuilder html = new StringBuilder();
            using (StreamReader reader = new StreamReader(@"C:\Users\WIN10\Downloads\beefree-5be2f4o8pow\beefree-5be2f4o8pow.html"))
            {
               
                while ((line = reader.ReadLine()) != null)
                {
                    html.Append( line);
                }
                
            }


         

            var baseAddress = "http://localhost:61708/api/account/BuildEmailInvite";
            var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";
            TemplateEmailDTO data = new TemplateEmailDTO()
            {
                Body = html.ToString()
            };
            var jsonstr = JsonConvert.SerializeObject(data);

            string parsedContent = jsonstr;

            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            var response = http.GetResponse();

            var stream = response.GetResponseStream();
         
            var sr = new StreamReader(stream);
            var result = sr.ReadToEnd();
          
            


        }
        public class HttpContextLifetimeManager<T> : LifetimeManager, IDisposable
        {
            public override object GetValue()
            {
                return HttpContext.Current.Items[typeof(T).AssemblyQualifiedName];
            }
            public override void RemoveValue()
            {
                HttpContext.Current.Items.Remove(typeof(T).AssemblyQualifiedName);
            }
            public override void SetValue(object newValue)
            {
                HttpContext.Current.Items[typeof(T).AssemblyQualifiedName] = newValue;
            }
            public void Dispose()
            {
                RemoveValue();
            }
        }
    }
}
