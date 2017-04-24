using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.Utils
{
    public static class CommonUtil
    {
        private static readonly long UnixEpochTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        public static decimal MathRound(this decimal number)
        {
            if (number >= 0 && number <= 100)
            {
                number = Math.Round(number);
            }
            else if (number > 100 && number <= 10000)
            {
                number = Math.Round(number / 100) * 100;
            }
            else if (number > 10000)
            {
                number = Math.Round(number / 1000) * 1000;
            }
            return number;
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
        public static long? ToJsonTicks(this DateTime? value)
        {
            return value == null ? (long?)null : (value.Value.ToUniversalTime().Ticks - UnixEpochTicks) / 10000;
        }
        public static string ToStringVN(this DateTime Date)
        {
            return Date.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.CreateSpecificCulture("en-US"));
        }

        public static string TimeSpanToString(this TimeSpan Date)
        {
            return Date.ToString(@"dd\.hh\:mm\:ss");
        }

        public static long? ComputedTimeSpan(DateTime? FromDate, DateTime? ToDate)
        {
            if(!FromDate.HasValue || !ToDate.HasValue)
                return null;
            
            var timeSpan = ToDate - FromDate;
            return timeSpan.Value.Ticks;
        }
        public static string ToStringDateVN(this DateTime Date)
        {
            return Date.ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("vi-VN"));
        }

        public static DateTime GetFirstDayOfMonth(this DateTime Date)
        {
            DateTime dtResult = new DateTime(DateTime.Now.Year, Date.Month, 1);
            dtResult = dtResult.AddDays((-dtResult.Day) + 1);
            return dtResult;
        }
        public static DateTime GetLastDayOfMonth(this DateTime dtInput)
        {
            DateTime dtResult = dtInput;
            dtResult = dtResult.AddMonths(1);
            dtResult = dtResult.AddDays(-(dtResult.Day));
            return dtResult;
        }
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
        public static DateTime ToMaxDate(this DateTime Date)
        {

            return new DateTime(Date.Year, Date.Month, Date.Day, 23, 59, 00);
        }

        public static DateTime ToMidDate(this DateTime Date)
        {

            return new DateTime(Date.Year, Date.Month, Date.Day);
        }
        public static DateTime ToMinDate(this DateTime Date)
        {

            return new DateTime(Date.Year, Date.Month, Date.Day, 00, 00, 00);
        }
        public static DateTime ToDate(this DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day);
        }

        public static string ConvertDayOfWeekVN(DayOfWeek data)
        {
            string result = string.Empty;
            switch (data)
            {
                case DayOfWeek.Friday:
                    result= "Thứ 6";
                    break;
                case DayOfWeek.Monday:
                    result = "Thứ 2";
                    break;
                case DayOfWeek.Saturday:
                    result = "Thứ 7";
                    break;
                case DayOfWeek.Sunday:
                    result = "Chủ nhật";
                    break;
                case DayOfWeek.Thursday:
                    result = "Thứ 5";
                    break;
                case DayOfWeek.Tuesday:
                    result = "Thứ 3";
                    break;
                case DayOfWeek.Wednesday:
                    result = "Thứ 4";
                    break;
               
            }
            return result;
        }

        public static string ToAscii(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            str = str.Normalize(NormalizationForm.FormD);
            str = regex.Replace(str, String.Empty)
                        .Replace('\u0111', 'd').Replace('\u0110', 'D');

            //Remove Special Char
            regex = new Regex(@"[^a-zA-Z0-9_\.]");
            str = regex.Replace(str, "_");

            return str;
        }

        public static string UploadFileEx(Stream file, string fileName, string serviceUrl)
        {

            var fileFormName = "file";
            var contenttype = "application/octet-stream";

            //string postdata = "?";

            //if (formValues != null)
            //{
            //    foreach (string key in formValues.Keys)
            //    {
            //        postdata += key + "=" + formValues.Get(key) + "&";
            //    }
            //}
            Uri uri = new Uri(serviceUrl);//+ postdata

            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Method = "POST";

            // Build up the post message header
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append(fileFormName);
            sb.Append("\"; filename=\"");
            sb.Append(fileName);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append(contenttype);
            sb.Append("\r\n");
            sb.Append("\r\n");

            string postHeader = sb.ToString();

            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

            // Build the trailing boundary string as a byte array
            // ensuring the boundary appears on a line by itself
            byte[] boundaryBytes =
                   Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            long length = postHeaderBytes.Length + file.Length +
                                                   boundaryBytes.Length;
            webrequest.ContentLength = length;

            Stream requestStream = webrequest.GetRequestStream();

            // Write out our post header
            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

            // Write out the file contents
            byte[] buffer = new Byte[checked((uint)Math.Min(4096,
                                     (int)file.Length))];
            int bytesRead = 0;
            while ((bytesRead = file.Read(buffer, 0, buffer.Length)) != 0)
                requestStream.Write(buffer, 0, bytesRead);

            // Write out the trailing boundary
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            WebResponse responce = webrequest.GetResponse();
            Stream s = responce.GetResponseStream();
            StreamReader sr = new StreamReader(s);

            return sr.ReadToEnd();
        }

        public static string ToJson(Type enumType)
        {
            Dictionary<string, string> listEnumField = new Dictionary<string, string>();
            Type type = enumType;
            foreach (var evalue in type.GetEnumValues())
            {
                var valueName = type.GetField(evalue.ToString());
                string displayLabel = "";
                DisplayAttribute displayAtt = valueName.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
                if (displayAtt != null)
                    displayLabel = displayAtt.GetName();
                listEnumField.Add(evalue.ToString(), displayLabel);
            }
            return JsonConvert.SerializeObject(listEnumField.Select(m => new { Key = m.Key, Value = m.Value }));
        }

        

        public static string ToJsonInt(Type enumType)
        {
            Dictionary<int, string> listEnumField = new Dictionary<int, string>();
            Type type = enumType;
            foreach (var evalue in type.GetEnumValues())
            {
                var valueName = type.GetField(evalue.ToString());
                string displayLabel = "";
                DisplayAttribute displayAtt = valueName.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
                if (displayAtt != null)
                    displayLabel = displayAtt.GetName();
                listEnumField.Add((int)evalue, displayLabel);
            }
            return JsonConvert.SerializeObject(listEnumField.Select(m => new { Key = m.Key, Value = m.Value }));
        }

        public static string GeneralCode(string prefix)
        {
            var date=DateTime.Now;
            string str=string.Empty;
            if(date.Month>9){
                if (date.Day > 9)
                    str = date.Year.ToString().Substring(1, 2) + date.Month + date.Day+"_"+date.Second;
                else
                    str = date.Year.ToString().Substring(1, 2) + date.Month +"0"+ date.Day + "_" + date.Second;
            }
            else
            {
                if (date.Day > 9)
                    str = date.Year.ToString().Substring(1, 2) + "0" + date.Month + date.Day + "_" + date.Second;
                else
                    str = date.Year.ToString().Substring(1, 2) + "0" + date.Month + "0" + date.Day + "_" + date.Second;
            }
            return prefix+"_"+ str;
        }

        public static string GetDisplayValue(Type value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }

        private static readonly string[] VietnameseSigns = new string[]
        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"

        };

        public static string RemoveVNmese(this string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }

    }
   
}
