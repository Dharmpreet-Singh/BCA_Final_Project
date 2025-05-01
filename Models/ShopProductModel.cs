using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace ProjectDP.Models
{
    public class ShopProductModel
    {
        public List<Product> productMdl{ get; set; }
        public List<Product_Dtl> productDtlMdl{ get; set; }
        public List<Category> categoryMdl{ get; set; }
        public List<Sub_Category> subcatMdl{ get; set; }
        public IPagedList<ShopViewModel> ProductDetails { get; set; }

    }

    public class ShopViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal? ProductPrice { get; set; }
    }
}