using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsandCategories.Models
{
    public class Categories
    {
        [Key]
        public int CategoriesId { get; set; }
        public string  Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<Products_and_Categories> Get_Products  { get; set; }
    }
}
