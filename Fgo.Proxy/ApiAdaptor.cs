using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fgo.Proxy
{
    public class ConfigKey
    {
        public static readonly string WEB_API = ConfigurationManager.AppSettings["WEB_API"];
    }
    public interface IAdaptor
    {
        HttpClient Client { get; }
    }
    public class ApiAdaptor : IAdaptor
    {
        public HttpClient Client { get; private set; }

        public ApiAdaptor()
        {

            Client = new HttpClient();
            Client.BaseAddress = new Uri(ConfigKey.WEB_API);
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("ClientIp", GetIpHost());
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

        }

        public string GetIpHost()
        {
            var ipAddress = string.Empty;
            if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
            {
                ipAddress = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            }
            return ipAddress;
        }

        //protected T POSTService<T>(object model, string url) where T : class
        //{
        //    var baseAddress = ConfigKey.WEB_API + url;
        //    var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
        //    http.Accept = "application/json";
        //    http.ContentType = "application/json";
        //    http.Method = "POST";
        //    var jsonstr = JsonConvert.SerializeObject(model);

        //    string parsedContent = jsonstr;

        //    UTF8Encoding encoding = new UTF8Encoding();
        //    Byte[] bytes = encoding.GetBytes(parsedContent);

        //    Stream newStream = http.GetRequestStream();
        //    newStream.Write(bytes, 0, bytes.Length);
        //    newStream.Close();

        //    var response = http.GetResponse();

        //    var stream = response.GetResponseStream();
        //    if (stream == null)
        //        return null;
        //    var sr = new StreamReader(stream);
        //    var data = sr.ReadToEnd();
        //    var result = JsonConvert.DeserializeObject<T>(data);
        //    return result;
        //}


    }
}
