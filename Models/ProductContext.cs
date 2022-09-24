using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CSVFeed.Models
{
    public class ProductContext : DbContext
    {
        public ProductContext():base("FeedConnection")
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<User> Users { get; set; }

    }
}