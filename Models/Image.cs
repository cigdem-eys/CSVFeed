using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSVFeed.Models
{
    public class Image
    {
        public int Id { get; set; } 
        public string Photo { get; set; }
        public int Product_Id { get; set; }

        public Product Product { get; set; }
    }
}