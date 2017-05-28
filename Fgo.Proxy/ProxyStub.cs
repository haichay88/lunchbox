using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fgo.Proxy
{
    public abstract class ProxyStub
    {
        protected string Uri { get; private set; }
        public readonly IAdaptor Adaptor;

        protected ProxyStub(string controllerName) :
            this(controllerName, new ApiAdaptor())
        {

        }

        protected ProxyStub(string controllerName, IAdaptor adaptor)
        {
            Adaptor = adaptor;
            Uri = Adaptor.Client.BaseAddress.AbsoluteUri + "api/" + controllerName + "/";
        }
    }
}
