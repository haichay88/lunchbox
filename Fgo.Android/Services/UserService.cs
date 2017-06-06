using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using Fgo.AndroidApp.Common;
using Newtonsoft.Json;
using Bizkasa.Bizlunch.Business.Utils;
using Bizkasa.Bizlunch.Business.Model;
using System.Net.Http.Headers;

namespace Fgo.AndroidApp.Services
{
    public class UserService
    {
        HttpClient client;


        public UserService()
        {
            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

        }
        /// <summary>
        /// dang nhap vao he thong
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Response<LoginResultDTO> Login(LoginDTO mode)
        {
            mode.Password = CommonUtil.CreateMD5(mode.Password);
            string url = AppPreferences.Domain_API + "api/Account/Login";
            var uri = new Uri(string.Format(url, string.Empty));

            string datastr = JsonConvert.SerializeObject(mode);
            var contentSend = new StringContent(datastr, Encoding.UTF8, "application/json");
            var response = client.PostAsync(uri, contentSend).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Response<LoginResultDTO>>(content.Result);
                return result;
            }
            return null;
        }
        public Response<List<OrderDTO>> GetOrders(BaseRequest request)
        {
         
            string url = AppPreferences.Domain_API + "api/Order/GetOrders";
            var uri = new Uri(string.Format(url, string.Empty));

            string datastr = JsonConvert.SerializeObject(request);
            var contentSend = new StringContent(datastr, Encoding.UTF8, "application/json");
            var response = client.PostAsync(uri, contentSend).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Response<List<OrderDTO>>>(content.Result);
                return result;
            }
            return null;
        }
        public Response<List<FriendDTO>> GetFriends(SearchDTO request)
        {

            string url = AppPreferences.Domain_API + "api/Account/GetFriends";
            var uri = new Uri(string.Format(url, string.Empty));

            string datastr = JsonConvert.SerializeObject(request);
            var contentSend = new StringContent(datastr, Encoding.UTF8, "application/json");
            var response = client.PostAsync(uri, contentSend).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Response<List<FriendDTO>>>(content.Result);
                return result;
            }
            return null;
        }
    }
}