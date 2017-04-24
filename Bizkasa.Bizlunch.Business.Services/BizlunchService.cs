using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.Services
{
    public interface IBizlunchService: IAccountService,IRestaurantService,IOrderService
    {

    }
    public partial class BizlunchService: IBizlunchService
    {
    }
}
