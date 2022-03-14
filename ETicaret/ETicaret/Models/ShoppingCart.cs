using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ETicaret.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }
        [Key]
        public int id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Count { get; set; }
        [NotMapped]
        public double price { get; set; }
    }
}
