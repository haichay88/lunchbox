using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.Model
{
   public class Enum
    {
    }

    public enum AppCode
    {
        [Display(Name ="Mobile")]
        Mobile=1,
       [Display(Name = "Web")]
        Web = 2
    }
    public enum EmailTemplate
    {
        [Display(Name = "NewInvite")]
        NewInvite = 1,
      
    }
}
