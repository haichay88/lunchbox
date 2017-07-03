using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.Model
{
   public class RestaurantDTO:BaseRequest
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Retaurant Name")]
        public string Name { get; set; }
        [DisplayName("Menu Url")]
        public string MenuUrl { get; set; }
        public string Phone { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public Nullable<bool> IsDelivery { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
