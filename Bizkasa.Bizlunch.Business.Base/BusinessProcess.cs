
using Bizkasa.Bizlunch.Business.Extention;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bizkasa.Bizlunch.Business.Base
{
    public class BusinessProcess : IDisposable
    {
        private object _resultObj = null;
        private Dictionary<string, List<object>> _errors = null;
        private Dictionary<string, List<object>> _infos = null;
        private bool _hasError = false;
        private bool _hasInfo = false;
        private Exception ex = null;

        public BusinessProcess Process(Action<BusinessProcess> action)
        {
            //Check Error
            if (this.HasError) return this;
            try
            {
                //if (DateTime.Now.Date <= new DateTime(2016, 1, 31)) { action(this); }
                action(this);
            }
            catch (System.Data.Entity.Core.OptimisticConcurrencyException ex)
            {
                this.ex = ex;
                __handleException(ex);
            }
            //catch (System.Security.SecurityException ex)
            //{
            //    this.ex = ex;
            //    __handleException(ex);
            //}

            return this;
        }

        public BusinessProcess Process(Func<BusinessProcess, object> func)
        {
            //Check Error
            if (this.HasError) return this;

            try
            {
                _resultObj = func(this);
            }
            catch (System.Data.Entity.Core.OptimisticConcurrencyException ex)
            {
                __handleException(ex);
            }
            catch (System.Security.SecurityException ex)
            {
                __handleException(ex);
            }

            return this;
        }

        public void ErrorProcess(Action<BusinessProcess> action)
        {
            if (this.HasError)
            {
                action(this);
            }
        }

        private void __handleException(Exception ex)
        {
            //_log.Error(ex);
            if (ex is System.Data.Entity.Core.OptimisticConcurrencyException)
            {
                this.AddError(this.GetType(), "Dữ liệu đã bị thay đổi");
            }

            if (ex is System.Security.SecurityException)
            {
                this.AddError(this.GetType(), "Bạn không có quyền thực hiện chức năng này");
            }
        }

        public Exception Exception
        {
            get { return ex; }
        }

        public static BusinessProcess Current
        {
            get
            {
                return IoC.Get<BusinessProcess>();
            }
        }

        public T GetLastResult<T>() where T : class
        {
            return _resultObj as T;
        }

        public object GetLastResult()
        {
            return _resultObj;
        }

        public bool HasError
        {
            get
            {
                return _hasError;
            }
            private set
            {
                _hasError = value;
            }
        }
        /// <summary>
        /// Chỉ sử dụng khi nào thực sự cần
        /// </summary>
        internal void ClearError()
        {
            if (_errors != null) _errors.Clear();
            _hasError = false;
        }

        public bool HasInfo
        {
            get
            {
                return _hasInfo;
            }
            private set
            {
                _hasInfo = value;
            }
        }

        #region Errors

        public void AddError<Business>(object errorObj)
        {
            this.AddError(typeof(Business), errorObj);
        }

        public void AddError(object errorObj)
        {
            AddError(null, errorObj);
        }

        public void AddError(Type businessType, object errorObj)
        {
            if (errorObj == null) errorObj = "";

            if (!HasError)
            {
                HasError = true;
                _errors = new Dictionary<string, List<object>>();
            }
            string typeName = businessType == null ? "Null" : businessType.Name;
            List<object> listObj;
            if (!_errors.ContainsKey(typeName))
            {
                listObj = new List<object>();
                _errors.Add(typeName, listObj);
            }
            else
                listObj = _errors[typeName];

            listObj.Add(errorObj);
        }

        public List<Result> GetErrors<Business, Result>()
        {
            if (HasError)
            {
                string typeName = typeof(Business).Name;
                if (_errors.ContainsKey(typeName))
                {
                    return _errors[typeName].OfType<Result>().ToList();
                }
            }
            return null;
        }

        public List<object> GetErrors<Business>()
        {
            if (HasError)
            {
                string typeName = typeof(Business).Name;
                if (_errors.ContainsKey(typeName))
                {
                    return _errors[typeName];
                }
            }
            return null;
        }

        public string ToErrorMsg()
        {
            if (HasError)
            {
                return GetErrors().Aggregate((a, b) =>
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
                return GetInfos().Aggregate((a, b) =>
                {
                    return a.ToString() + "\n" + b.ToString();
                }).ToString();
            }
            return null;
        }

        public List<object> GetErrors()
        {
            if (HasError)
            {
                return _errors.Values.Aggregate((r, a) =>
                {
                    r.AddRange(a);
                    return r;
                });
            }
            return null;
        }

        #endregion

        #region Infos

        public void AddInfo<Business>(object infoObj)
        {
            AddInfo(typeof(Business), infoObj);
        }

        public void AddInfo(object infoObj)
        {
            AddInfo(null, infoObj);
        }

        public void AddInfo(Type businessType, object infoObj)
        {
            if (infoObj == null) infoObj = "";

            if (!HasInfo)
            {
                HasInfo = true;
                _infos = new Dictionary<string, List<object>>();
            }
            string typeName = businessType == null ? "Null" : businessType.Name;
            List<object> listObj;
            if (!_infos.ContainsKey(typeName))
            {
                listObj = new List<object>();
                _infos.Add(typeName, listObj);
            }
            else
                listObj = _infos[typeName];

            listObj.Add(infoObj);
        }

        public List<object> GetInfos(Type businessType, Type resultType)
        {
            if (HasInfo)
            {
                string typeName = businessType.Name;
                if (_infos.ContainsKey(typeName))
                {
                    return _infos[typeName].Where(m => m.GetType() == resultType).ToList();
                }
            }
            return null;
        }

        public List<Result> GetInfos<Business, Result>()
        {
            return GetInfos(typeof(Business), typeof(Result)).Cast<Result>().ToList();
        }

        public List<object> GetInfos(Type businessType)
        {
            if (HasInfo)
            {
                string typeName = businessType.Name;
                if (_infos.ContainsKey(typeName))
                {
                    return _infos[typeName];
                }
            }
            return null;
        }

        public List<object> GetInfos<Business>()
        {
            return GetInfos(typeof(Business));
        }

        public List<object> GetInfos()
        {
            if (HasInfo)
            {
                return _infos.Values.Aggregate((r, a) =>
                {
                    r.AddRange(a);
                    return r;
                });
            }
            return null;
        }

        #endregion

        private bool disposing = false;

        public void Dispose()
        {
            if (!disposing)
            {
                disposing = true;
                if (_resultObj is IDisposable)
                {
                    ((IDisposable)_resultObj).Dispose();
                }
                if (_errors != null) _errors.Clear();
                if (_infos != null) _infos.Clear();
                _hasError = false;
                _hasInfo = false;
            }
        }
    }
}
