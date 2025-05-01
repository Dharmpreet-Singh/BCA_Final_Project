using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectDP.Models
{
    public class CartViewModel
    {
        public Product myproducts { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
        public double Total { get; set; }
        public int ProductId { get; set; }
    }
}