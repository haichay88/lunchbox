using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.Utils
{

    public class Response
    {
        public Response()
        {
            Infos = new List<object>();
            Errors = new List<object>();
        }

        public bool HasError
        {
            get
            {
                if (Errors != null && Errors.Any())
                {
                    return true;
                }
                else
                    return false;
            }
        }
        public bool HasInfo
        {
            get
            {
                if (Infos != null && Infos.Any())
                {
                    return true;
                }
                else
                    return false;
            }
        }
        public List<object> Infos { get; set; }
        public List<object> Errors { get; set; }

        public string ToErrorMsg()
        {
            if (HasError)
            {
                return Errors.Aggregate((a, b) =>
                {
                    return (a ?? "").ToString() + "\n" + (b ?? "").ToString();
                }).ToString();
            }
            return null;
        }

        public string ToInfoMsg()
        {
            if (HasInfo)
            {
                return Infos.Aggregate((a, b) =>
                {
                    return a.ToString() + "\n" + b.ToString();
                }).ToString();
            }
            return null;
        }


        public static Response<T> FromData<T>(T data)
        {
            Response<T> rp = new Response<T>(data);
            return rp;
        }
    }

    public class Response<T> : Response
    {
        public Response()
        {

        }

        public Response(T data)
        {
            Data = data;
        }
        public T Data { get; set; }
    }

    public class DataPaging
    {
        public static DataPaging<T> Create<T>(T data, int totalRecords) where T : class
        {
            DataPaging<T> d = new DataPaging<T>();
            d.Data = data;
            d.TotalRecord = totalRecords;
            return d;
        }

        public int TotalRecord { get; set; }
    }
    public class DataPaging<T> : DataPaging where T : class
    {
        public T Data { get; set; }
    }
}
