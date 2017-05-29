using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fgo.Proxy
{
    public class AccountProxy:ProxyStub
    {

        public AccountProxy()
                : base("OutputService")
        {

        }
      

        #region Output Service

        //public KHGetTransactionsResponse KHGetTransactions(KHGetTransactionsRequest khGetTransactionsRequest)
        //{
        //    HttpResponseMessage response = Adaptor.Client.PostAsJsonAsync(Uri + "KHGetTransactions", khGetTransactionsRequest).Result;
        //    return response.Content.ReadAsAsync<KHGetTransactionsResponse>().Result;
        //}

        //public KHUpdateTransactionResponse KHUpdateTransaction(KHUpdateTransactionRequest khUpdateTransactionRequest)
        //{
        //    HttpResponseMessage response = Adaptor.Client.PostAsJsonAsync(Uri + "KHUpdateTransaction", khUpdateTransactionRequest).Result;
        //    return response.Content.ReadAsAsync<KHUpdateTransactionResponse>().Result;
        //}
    }
    #endregion
}
