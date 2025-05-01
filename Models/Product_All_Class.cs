using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectDP.Models
{
    public class Product_All_Class
    {
        public int pa_id { get; set; }
        public string pa_name { get; set; }
        public string pa_image { get; set; }
        public string pa_catName { get; set; }
        public string pa_catImage { get; set; }
        public string pa_subCatName { get; set; }
        public string pa_subCatImg { get; set; }
        public double pa_price { get; set; }
        public string pa_quantity { get; set; }
        public string pa_descript { get; set; }
        public double pa_disc { get; set; }
    }
}