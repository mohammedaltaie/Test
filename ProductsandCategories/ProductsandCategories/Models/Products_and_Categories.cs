using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsandCategories.Models
{
    public class Products_and_Categories
    {
        [Key]
        public int Products_and_CategoriesId { get; set; }

        public int ProductsId { get; set; }
        public Products Products { get; set; }


        public int CategoriesId { get; set; }
        public Categories Categories { get; set; }


    }
}