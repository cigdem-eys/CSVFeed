using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CSVFeed.Models
{
    public class Product
    {
        [Key]
        public int Product_Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public string Barcode { get; set; }
        public decimal Price { get; set; }
        public string User_Id { get; set; }

        public User User { get; set; }
        public List<Image> ProductImage { get; set; }
    }
}