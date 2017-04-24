using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.BusinessLogic
{
    public sealed class SingletonAutoMapper
    {
        #region Singleton

        //private static readonly Lazy<VisitContext> _lazy = new Lazy<VisitContext>(() => new VisitContext());
        private static SingletonAutoMapper _instance = null;

        public static SingletonAutoMapper _Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SingletonAutoMapper();
                    _Instance.MapperConfiguration = new MapperDTO().RegisterMap();
                }
                return _instance;
            }
        }

        #endregion

        public MapperConfiguration MapperConfiguration { get; set; }
    }
}
