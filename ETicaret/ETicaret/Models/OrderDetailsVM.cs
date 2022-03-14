using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETicaret.Models
{
    public class OrderDetailsVM
    {
        public OrderHeader orderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
