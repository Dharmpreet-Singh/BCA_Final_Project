using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ProjectDP.Models;

namespace ProjectDP.Controllers
{
    public class HomeController : Controller
    {
        projectDPDataContext context = new projectDPDataContext();
        //public ActionResult Index()
        //{
        //    List<Product_Dtl_Class> model = new List<Product_Dtl_Class>();
        //    var list = (from columns in context.Product_Dtls
        //                join fields in context.Products on columns.pd_id equals fields.p_id
        //                select new
        //                {
        //                    Product_Id = columns.pd_id,
        //                    Product_Name = fields.p_name,
        //                    Product_Image = fields.p_img,
        //                    Product_Qty = columns.pd_quantity,
        //                    Product_price = columns.pd_price
        //                }).ToList();
        //    foreach (var item in list)
        //    {
        //        Product_Dtl_Class obj = new Product_Dtl_Class();
        //        obj.pd_id = item.Product_Id;
        //        obj.pd_name = item.Product_Name;
        //        obj.pd_image = item.Product_Image;
        //        obj.pd_qunatity = item.Product_Qty;
        //        obj.pd_price = (int)item.Product_price;
        //        model.Add(obj);
        //    }
        //    ViewBag.ProductList = model;
        //    return View();
        //}

        public ActionResult Index()
        {
            List<Product_All_Class> model = new List<Product_All_Class>();
            var list = (from columns in context.Products
                        join fields in context.Categories on columns.catId equals fields.c_id
                        join fields2 in context.Sub_Categories on columns.subCatId equals fields2.sc_id
                        join fields3 in context.Product_Dtls on columns.p_id equals fields3.pr_id
                        orderby fields3.pd_price
                        select new
                        {
                            Pa_Id = columns.p_id,
                            Pa_Name = columns.p_name,
                            Pa_Img = columns.p_img,
                            Pa_Cat_Name = fields.c_name,
                            Pa_Cat_Img = fields.c_image,
                            Pa_SubCat_Name = fields2.sc_name,
                            Pa_SubCat_Img = fields2.sc_img,
                            pa_Price = fields3.pd_price,
                            pa_quantity = fields3.pd_quantity,
                            pa_descript = fields3.pd_description,
                            pa_discount = fields3.pd_discount

                        });
            foreach (var item in list)
            {
                Product_All_Class obj = new Product_All_Class();
                obj.pa_id = item.Pa_Id;
                obj.pa_name = item.Pa_Name;
                obj.pa_image = item.Pa_Img;
                obj.pa_catName = item.Pa_Cat_Name;
                obj.pa_catImage = item.Pa_Cat_Img;
                obj.pa_subCatName = item.Pa_SubCat_Name;
                obj.pa_subCatImg = item.Pa_SubCat_Img;
                obj.pa_price = (double)item.pa_Price;
                obj.pa_quantity = item.pa_quantity;
                obj.pa_descript = item.pa_descript;
                obj.pa_disc = (double)item.pa_discount;
                model.Add(obj);
            }
            ViewBag.ProductList = model.Where(p => p.pa_price < 100).Take(5);
            ViewBag.FeaturedProducts = model.Where(p => p.pa_price > 120).Take(5);
            //var cat = context.Categories.ToList();
            //ViewBag.Category = cat;

            ShopProductModel model2 = new ShopProductModel
            {
                categoryMdl = context.Categories.ToList(),
           
            };

            return View(model2);
        }

        public ActionResult About()
        {

            return View();
        }

        public ActionResult Contact()
        {

            return View();

        }

        [HttpPost]
        public ActionResult Contact(Contact model)
        {
            model.date = DateTime.Today;
            context.Contacts.InsertOnSubmit(model);
            context.SubmitChanges();
            return RedirectToAction("Index");
        }

        //public ActionResult ShopTest(int? cid, int? scId)
        //{
        //    ShopProductModel model = new ShopProductModel();
        //    {

        //        model.productMdl = scId.HasValue ? context.Products.Where(p => p.subCatId == scId.Value).ToList() : (cid.HasValue ? context.Products.Where(p => p.catId == cid.Value).ToList() : new List<Product>());
        //        model.productDtlMdl = context.Product_Dtls.ToList();
        //        model.categoryMdl = context.Categories.ToList();
        //        model.subcatMdl = cid.HasValue ? context.Sub_Categories.Where(s => s.catId == cid.Value).ToList() : new List<Sub_Category>();
        //    }

        //    model.ProductDetails = (from p in model.productMdl
        //                            join d in model.productDtlMdl on p.p_id equals d.pr_id
        //                            select new ShopViewModel
        //                            {

        //                                ProductName = p.p_name,
        //                                ProductPrice = (decimal?)d.pd_price
        //                            }).ToList();

        //    ViewBag.CategoryId = cid;
        //    ViewBag.SukbcategorkyId = scId;

        //    return View(model);
        //}

        public ActionResult ShopTest(int? cid, int? scId, int page = 1)
        {
            int pageSize = 8; // Set the number of items per page

            //var productsQuery = scId.HasValue ? context.Products.Where(p => p.subCatId == scId.Value) :
            //                    cid.HasValue ? context.Products.Where(p => p.catId == cid.Value) :
            //                    context.Products;

            var productsQuery = context.Products.AsQueryable();

            if (scId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.subCatId == scId.Value);
            }
            else if (cid.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.catId == cid.Value);
            }
            else
            {
                // If neither category nor subcategory is selected, show the first 10 products by default
                productsQuery = productsQuery.Take(5);
            }


            var productDetailsQuery = from p in productsQuery
                                      join d in context.Product_Dtls on p.p_id equals d.pr_id
                                      select new ShopViewModel
                                      {
                                          ProductName = p.p_name,
                                          ProductImage = p.p_img,
                                          ProductPrice = (decimal?)d.pd_price
                                      };

            ShopProductModel model = new ShopProductModel
            {
                categoryMdl = context.Categories.ToList(),
                subcatMdl = cid.HasValue ? context.Sub_Categories.Where(s => s.catId == cid.Value).ToList() : new List<Sub_Category>(),
                ProductDetails = productDetailsQuery.OrderBy(p => p.ProductName).ToPagedList(page, pageSize)
            };

            ViewBag.CategoryId = cid;
            ViewBag.SubcategoryId = scId;

            return View(model);
        }

        public ActionResult Shop(int? cid, int? scId, int page = 1)
        {
            int pageSize = 8; // Set the number of items per page

            var productsQuery = context.Products.AsQueryable();

            if (scId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.subCatId == scId.Value);
            }
            else if (cid.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.catId == cid.Value);
            }
            else
            {
                // If neither category nor subcategory is selected, show the first 10 products by default
                productsQuery = productsQuery.Take(5);
            }


            var productDetailsQuery = from p in productsQuery
                                      join d in context.Product_Dtls on p.p_id equals d.pr_id
                                      select new ShopViewModel
                                      {
                                          ProductId = p.p_id,
                                          ProductName = p.p_name,
                                          ProductImage = p.p_img,
                                          ProductPrice = (decimal?)d.pd_price
                                      };

            ShopProductModel model = new ShopProductModel
            {
                categoryMdl = context.Categories.ToList(),
                subcatMdl = cid.HasValue ? context.Sub_Categories.Where(s => s.catId == cid.Value).ToList() : new List<Sub_Category>(),
                ProductDetails = productDetailsQuery.OrderBy(p => p.ProductName).ToPagedList(page, pageSize)
            };

            ViewBag.CategoryId = cid;
            ViewBag.SubcategoryId = scId;

            return View(model);
        }

        //public ActionResult Shop()
        //{
        //    List<Product_All_Class> model = new List<Product_All_Class>();
        //    var list = (from columns in context.Products
        //                join fields in context.Categories on columns.catId equals fields.c_id
        //                join fields2 in context.Sub_Categories on columns.subCatId equals fields2.sc_id
        //                join fields3 in context.Product_Dtls on columns.p_id equals fields3.pr_id
        //                select new
        //                {
        //                    Pa_Id = columns.p_id,
        //                    Pa_Name = columns.p_name,
        //                    Pa_Img = columns.p_img,
        //                    Pa_Cat_Name = fields.c_name,
        //                    Pa_Cat_Img = fields.c_image,
        //                    Pa_SubCat_Name = fields2.sc_name,
        //                    Pa_SubCat_Img = fields2.sc_img,
        //                    pa_Price = fields3.pd_price,
        //                    pa_quantity = fields3.pd_quantity,
        //                    pa_descript = fields3.pd_description,
        //                    pa_discount = fields3.pd_discount

        //                }).Take(10);
        //    foreach (var item in list)
        //    {
        //        Product_All_Class obj = new Product_All_Class();
        //        obj.pa_id = item.Pa_Id;
        //        obj.pa_name = item.Pa_Name;
        //        obj.pa_image = item.Pa_Img;
        //        obj.pa_catName = item.Pa_Cat_Name;
        //        obj.pa_catImage = item.Pa_Cat_Img;
        //        obj.pa_subCatName = item.Pa_SubCat_Name;
        //        obj.pa_subCatImg = item.Pa_SubCat_Img;
        //        obj.pa_price = (double)item.pa_Price;
        //        obj.pa_quantity = item.pa_quantity;
        //        obj.pa_descript = item.pa_descript;
        //        obj.pa_disc = (double)item.pa_discount;
        //        model.Add(obj);
        //    }
        //    ViewBag.ProductList = model;

        //    var cat = context.Categories.ToList();
        //    ViewBag.Category = cat;

        //    //Viewbag.subcategories = (from columns in context.Sub_Categories
        //    //                         where columns.catId == id
        //    //                         select columns).ToList();
        //    return View();
        //}

        public ActionResult Product_View(int id)
        {
            List<Product_All_Class> model = new List<Product_All_Class>();
            var list = (from columns in context.Products
                        join fields in context.Categories on columns.catId equals fields.c_id
                        join fields2 in context.Sub_Categories on columns.subCatId equals fields2.sc_id
                        join fields3 in context.Product_Dtls on columns.p_id equals fields3.pr_id
                        where columns.p_id == id
                        select new
                        {
                            Pa_Id = columns.p_id,
                            Pa_Name = columns.p_name,
                            Pa_Img = columns.p_img,
                            Pa_Cat_Name = fields.c_name,
                            Pa_Cat_Img = fields.c_image,
                            Pa_SubCat_Name = fields2.sc_name,
                            Pa_SubCat_Img = fields2.sc_img,
                            pa_Price = fields3.pd_price,
                            pa_quantity = fields3.pd_quantity,
                            pa_descript = fields3.pd_description,
                            pa_discount = fields3.pd_discount

                        }).Take(5);
            foreach (var item in list)
            {
                Product_All_Class obj = new Product_All_Class();
                obj.pa_id = item.Pa_Id;
                obj.pa_name = item.Pa_Name;
                obj.pa_image = item.Pa_Img;
                obj.pa_catName = item.Pa_Cat_Name;
                obj.pa_catImage = item.Pa_Cat_Img;
                obj.pa_subCatName = item.Pa_SubCat_Name;
                obj.pa_subCatImg = item.Pa_SubCat_Img;
                obj.pa_price = (double)item.pa_Price;
                obj.pa_quantity = item.pa_quantity;
                obj.pa_descript = item.pa_descript;
                obj.pa_disc = (double)item.pa_discount;
                model.Add(obj);
            }
            ViewBag.ProductList = model;

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "1", Value = "1", Selected = true });
            items.Add(new SelectListItem { Text = "2", Value = "2" });
            items.Add(new SelectListItem { Text = "3", Value = "3" });
            items.Add(new SelectListItem { Text = "4", Value = "4" });
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "6", Value = "6" });
            items.Add(new SelectListItem { Text = "7", Value = "7" });
            items.Add(new SelectListItem { Text = "8", Value = "8" });
            items.Add(new SelectListItem { Text = "9", Value = "9" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            ViewBag.Set = items;
            return View(model);
        }
        [HttpPost]
        public ActionResult Product_View(int id, FormCollection collection)
        {
            Product product = new Product();
            Product_Dtl productdtl = new Product_Dtl();
            List<CartViewModel> list = new List<CartViewModel>();


            product = context.Products.First(p => p.p_id == id);
            productdtl = context.Product_Dtls.First(p => p.pr_id == id);



            if (Session["MyCartItem"] == null)
            {
                CartViewModel cart = new CartViewModel();
                //cart.Qty = Convert.ToInt32(Request.Form["quantity"]);
                cart.Qty = Convert.ToInt32(collection["dllquantity"]);
                cart.Price = Convert.ToInt32(productdtl.pd_price);
                cart.Total = cart.Qty * cart.Price;
                cart.myproducts = product;
                list.Add(cart);
                Session["MyCartItem"] = list;

            }
            else
            {
                CartViewModel cart = new CartViewModel();
                list = ((List<CartViewModel>)(Session["MyCartItem"]));
                //cart.Qty = Convert.ToInt32(Request.Form["quantity"]);
                cart.Qty = Convert.ToInt32(collection["dllquantity"]);
                cart.Price = Convert.ToInt32(productdtl.pd_price);
                cart.Total = cart.Qty * cart.Price;
                cart.myproducts = product;
                list.Add(cart);
                Session["MyCartItem"] = list;
            }
            return RedirectToAction("Cart");
        }


        public ActionResult RemoveFromCart(int productId)
        {
            var cartItems = Session["MyCartItem"] as List<CartViewModel>;
            if (cartItems != null)
            {
                var itemToRemove = cartItems.FirstOrDefault(x => x.myproducts.p_id == productId);
                if (itemToRemove != null)
                {
                    cartItems.Remove(itemToRemove);
                    Session["MyCartItem"] = cartItems;
                }
            }
            return RedirectToAction("Cart");
        }





        ////public ActionResult Product_View(int id, FormCollection collection)
        //{
        //    List<Product_All_Class> model = new List<Product_All_Class>();
        //    if(Session["userCart"]==null)
        //    {
        //        var item = (from columns in context.Products
        //                    join fields in context.Categories on columns.catId equals fields.c_id
        //                    join fields2 in context.Sub_Categories on columns.subCatId equals fields2.sc_id
        //                    join fields3 in context.Product_Dtls on columns.p_id equals fields3.pr_id
        //                    where columns.p_id == id
        //                    select new
        //                    {
        //                        Pa_Id = columns.p_id,
        //                        Pa_Name = columns.p_name,
        //                        Pa_Img = columns.p_img,
        //                        Pa_Cat_Name = fields.c_name,
        //                        Pa_Cat_Img = fields.c_image,
        //                        Pa_SubCat_Name = fields2.sc_name,
        //                        Pa_SubCat_Img = fields2.sc_img,
        //                        pa_Price = fields3.pd_price,
        //                        pa_quantity = fields3.pd_quantity,
        //                        pa_descript = fields3.pd_description,
        //                        pa_discount = fields3.pd_discount

        //                    }).First();


        //            Product_All_Class obj = new Product_All_Class();
        //            obj.pa_id = item.Pa_Id;
        //            obj.pa_name = item.Pa_Name;
        //            obj.pa_image = item.Pa_Img;
        //            obj.pa_catName = item.Pa_Cat_Name;
        //            obj.pa_catImage = item.Pa_Cat_Img;
        //            obj.pa_subCatName = item.Pa_SubCat_Name;
        //            obj.pa_subCatImg = item.Pa_SubCat_Img;
        //            obj.pa_price = (double)item.pa_Price;
        //            obj.pa_quantity = item.pa_quantity;
        //            obj.pa_descript = item.pa_descript;
        //            obj.pa_disc = (double)item.pa_discount;
        //            model.Add(obj);
        //            Session["UserCart"] = model;
        //    }
        //    else
        //    {
        //        var item = (from columns in context.Products
        //                    join fields in context.Categories on columns.catId equals fields.c_id
        //                    join fields2 in context.Sub_Categories on columns.subCatId equals fields2.sc_id
        //                    join fields3 in context.Product_Dtls on columns.p_id equals fields3.pr_id
        //                    where columns.p_id == id
        //                    select new
        //                    {
        //                        Pa_Id = columns.p_id,
        //                        Pa_Name = columns.p_name,
        //                        Pa_Img = columns.p_img,
        //                        Pa_Cat_Name = fields.c_name,
        //                        Pa_Cat_Img = fields.c_image,
        //                        Pa_SubCat_Name = fields2.sc_name,
        //                        Pa_SubCat_Img = fields2.sc_img,
        //                        pa_Price = fields3.pd_price,
        //                        pa_quantity = fields3.pd_quantity,
        //                        pa_descript = fields3.pd_description,
        //                        pa_discount = fields3.pd_discount

        //                    }).First();


        //        Product_All_Class obj = new Product_All_Class();
        //        obj.pa_id = item.Pa_Id;
        //        obj.pa_name = item.Pa_Name;
        //        obj.pa_image = item.Pa_Img;
        //        obj.pa_catName = item.Pa_Cat_Name;
        //        obj.pa_catImage = item.Pa_Cat_Img;
        //        obj.pa_subCatName = item.Pa_SubCat_Name;
        //        obj.pa_subCatImg = item.Pa_SubCat_Img;
        //        obj.pa_price = (double)item.pa_Price;
        //        obj.pa_quantity = item.pa_quantity;
        //        obj.pa_descript = item.pa_descript;
        //        obj.pa_disc = (double)item.pa_discount;

        //        foreach (var items in (List<Product_All_Class>)Session["UserCart"])
        //        {
        //            Product_All_Class obj1 = new Product_All_Class();
        //            obj1.pa_id = items.pa_id;
        //            obj1.pa_name = items.pa_name;
        //            obj1.pa_image = items.pa_image;
        //            obj1.pa_catName = items.pa_catName;
        //            obj1.pa_catImage = items.pa_catImage;
        //            obj1.pa_subCatName = items.pa_subCatName;
        //            obj1.pa_subCatImg = items.pa_subCatImg;
        //            obj1.pa_price = (double)items.pa_price;
        //            obj1.pa_quantity = items.pa_quantity;
        //            obj1.pa_descript = items.pa_descript;
        //            obj1.pa_disc = (double)items.pa_disc;
        //            model.Add(obj1);
        //        }
        //        model.Add(obj);
        //        Session["UserCart"] = model;
        //    }
        //    return RedirectToAction("Cart");
        //}


        public ActionResult Cart()
        {
            if (Session["MyCartItem"] == null)
            {
                TempData["empty"] = "Your Cart is Empty";
                return View();
            }
            else
            {
                var list = (List<CartViewModel>)Session["MyCartItem"];
                ViewBag.UserProduct = list;
                ViewBag.SubTotal = list.Sum(p => p.Total);
                return View(list);
            }
        }

        private List<CartViewModel> GetCart()
        {
            var cart = Session["MyCartItem"] as List<CartViewModel>;
            if (cart == null)
            {
                cart = new List<CartViewModel>();
                Session["MyCartItem"] = cart;
            }
            return cart;
        }

        public ActionResult PlaceOrder()
        {
            if (Session["MyCartItem"] == null)
            {
                return RedirectToAction("Cart"); // Or any appropriate action
            }

            var cartItems = Session["MyCartItem"] as List<CartViewModel>;
            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Cart"); // Or any appropriate action
            }

            // Get the username from session
            string userName = Session["userName"] as string;
            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login"); // Or any appropriate action
            }

            // Calculate total amount
            double totalAmount = cartItems.Sum(item => item.Total);

           
                // Create a new order
                var order = new Order
                {
                    UserName = Session["Name"].ToString(),
                    OrderDate = DateTime.Now,
                    TotalAmount = (decimal)totalAmount
                };
                context.Orders.InsertOnSubmit(order);
                context.SubmitChanges();

                // Create order details
                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = item.myproducts.p_id,
                        Quantity = item.Qty,
                        Price = (decimal)item.Price,
                        Total = (decimal)item.Total
                    };
                    context.OrderDetails.InsertOnSubmit(orderDetail);
                }
                context.SubmitChanges();
         

            // Clear the cart
            Session["MyCartItem"] = null;

            return RedirectToAction("OrderConfirmation"); // Or any appropriate action
        }
        public ActionResult OrderConfirmation()
        {
            return View();

        }

    }
}