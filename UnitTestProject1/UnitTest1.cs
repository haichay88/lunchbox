using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZaloPageSDK.com.vng.zalosdk.service;
using System.Net;
using System.IO;

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
    }
}
