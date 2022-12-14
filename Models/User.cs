using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace CSVFeed.Models
{
    public class User
    {
        [Key]
        public int User_Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int Active { get; set; }
        public string Password { get; set; }
        public List<Product> Products { get; set; }
    }
}