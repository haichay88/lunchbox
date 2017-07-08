using Bizkasa.Bizlunch.Business.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSendEmail();
        }
        public static void TestSendEmail()
        {
            string line = string.Empty;
            StringBuilder html = new StringBuilder();
            using (StreamReader reader = new StreamReader(@"C:\Users\WIN10\Downloads\beefree-5be2f4o8pow\beefree-5be2f4o8pow.html"))
            {

                while ((line = reader.ReadLine()) != null)
                {
                    html.Append(line);
                }

            }




            var baseAddress = "http://localhost:61708/api/account/AddTemplateEmail";
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
    }
}
