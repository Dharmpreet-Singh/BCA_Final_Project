using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectDP.Models;
using System.IO;

namespace ProjectDP.Controllers
{
    //[Authorize]
    public class AdminController : Controller
    {
        projectDPDataContext context = new projectDPDataContext();
        // GET: Admin
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Product_All()
        {
            List<Product_All_Class> model = new List<Product_All_Class>();
            var list = (from columns in context.Products
                        join fields in context.Categories on columns.catId equals fields.c_id
                        join fields2 in context.Sub_Categories on columns.subCatId equals fields2.sc_id
                        join fields3 in context.Product_Dtls on columns.p_id equals fields3.pr_id
                        select new
                        {
                            Pa_Id = columns.p_id,
                            Pa_Name = columns.p_name,
                            Pa_Img = columns.p_img,
                            Pa_Cat_Name = fields.c_name,
                            Pa_Cat_Img = fields.c_image,
                            Pa_SubCat_Name = fields2.sc_name,
                            Pa_SubCat_Img = fields2.sc_img,
                            pa_Price =fields3.pd_price,
                            pa_quantity =fields3.pd_quantity,
                            pa_descript =fields3.pd_description,
                            pa_discount =fields3.pd_discount

                        }).ToList();
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
            return View(model);
        }

        public ActionResult My_Product()
        {
            List<My_Product_Class> model = new List<My_Product_Class>();
            var list = (from columns in context.Products
                        join fields in context.Categories on columns.catId equals fields.c_id
                        join fields2 in context.Sub_Categories on columns.subCatId equals fields2.sc_id
                        select new
                        {
                            Pdt_Id = columns.p_id,
                            Pdt_Name = columns.p_name,
                            Pdt_Img = columns.p_img,
                            Pdt_Cat_Name = fields.c_name,
                            Pdt_SubCat_Name = fields2.sc_name
                        }).ToList();
            foreach (var item in list)
            {
                My_Product_Class obj = new My_Product_Class();
                obj.p_id = item.Pdt_Id;
                obj.p_name = item.Pdt_Name;
                obj.p_image = item.Pdt_Img;
                obj.p_catName = item.Pdt_Cat_Name;
                obj.p_subCatName = item.Pdt_SubCat_Name;
                model.Add(obj);
            }
            return View(model);
        }

        public ActionResult Add_Product()
        {
            var catList = context.Categories.ToList();
            ViewBag.Category = catList;
            var subcatList = context.Sub_Categories.ToList();
            ViewBag.Subcategory = subcatList;
            return View();
        }

        [HttpPost]
        public ActionResult Add_Product(Product model, HttpPostedFileBase file, FormCollection collection)
        {
            string _destinationUrl = null;
            string _fileName = null;
            //string _destinationUrl, _fileName;
            if (file != null)
            {

                _fileName = file.FileName;
                _destinationUrl = Path.Combine(Server.MapPath("~/Files/"), _fileName);
                file.SaveAs(_destinationUrl);
                model.p_img = "../../Files/" + _fileName;
            }
            int c_id = Convert.ToInt32(collection["ddlCategory"]);
            model.catId = c_id;
            int sc_id = Convert.ToInt32(collection["ddlCategory2"]);
            model.subCatId = sc_id;
            context.Products.InsertOnSubmit(model);
            context.SubmitChanges();
            return RedirectToAction("My_Product");
        }

        public ActionResult Edit_Product(int id)
        {
            Product obj = new Product();
            obj = context.Products.First(S => S.p_id == id);
            var catList = context.Categories.ToList();
            ViewBag.Category = catList;
            var subcatList = context.Sub_Categories.ToList();
            ViewBag.Subcategory = subcatList;
            return View(obj);
        }
        [HttpPost]
        public ActionResult Edit_Product(int id, Product model, HttpPostedFileBase file, FormCollection collection)
        {
            Product obj = new Product();
            obj = context.Products.First(S => S.p_id == id);
            obj.p_name = model.p_name;
            string file_name, destinationUrl;

            if (file != null)
            {
                file_name = file.FileName;
                destinationUrl = System.IO.Path.Combine(Server.MapPath("~/Files/"), file_name);
                file.SaveAs(destinationUrl);
                obj.p_img = "../../Files/" + file_name;
            }
            int c_id = Convert.ToInt32(collection["ddlCategory"]);
            obj.catId = c_id;
            int sc_id = Convert.ToInt32(collection["ddlCategory2"]);
            obj.subCatId = sc_id;
            context.SubmitChanges();
            return RedirectToAction("My_Product");
        }
        public ActionResult Del_Product(int id)
        {
            Product obj = new Product();
            obj = context.Products.First(S => S.p_id == id);
            return View(obj);
        }
        [HttpPost]
        public ActionResult Del_Product(int id, FormCollection collection)
        {
            Product obj = new Product();
            obj = context.Products.First(S => S.p_id == id);
            context.Products.DeleteOnSubmit(obj);
            context.SubmitChanges();
            return RedirectToAction("My_Product");
        }

        public ActionResult Dtl_Product(int id)
        {
            Product obj = new Product();
            obj = context.Products.First(S => S.p_id == id);
            ViewBag.Product= obj;
            return View(obj);
        }

        public ActionResult Category()
        {
            var list = context.Categories.ToList();
            return View(list);
        }

         public ActionResult Add_Category()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add_Category(Category model, HttpPostedFileBase file)
        {
            string _destinationUrl = null;
            string _fileName = null;

            //string _destinationUrl, _fileName;
            if (file != null)
            {

                _fileName = file.FileName;
                _destinationUrl = Path.Combine(Server.MapPath("~/Files/"), _fileName);
                file.SaveAs(_destinationUrl);
                model.c_image = "../../Files/" + _fileName;
            }
            context.Categories.InsertOnSubmit(model);
            context.SubmitChanges();
            return RedirectToAction("Category");
        }

        public ActionResult Edit_Category(int id)
        {
            Category obj = new Category();
            obj = context.Categories.First(S => S.c_id == id);
            return View(obj);
        }
        [HttpPost]
        public ActionResult Edit_Category(int id, Category model, HttpPostedFileBase file)
        {
            Category obj = new Category();
            obj = context.Categories.First(S => S.c_id == id);
            obj.c_name = model.c_name;
            string file_name, destinationUrl;

            if (file != null)
            {
                file_name = file.FileName;
                destinationUrl = System.IO.Path.Combine(Server.MapPath("~/Files/"), file_name);
                file.SaveAs(destinationUrl);
                obj.c_image = "../../Files/" + file_name;
            }
            context.SubmitChanges();
            return RedirectToAction("Category");
        }

        public ActionResult Del_Category(int id)
        {
            Category obj = new Category();
            obj = context.Categories.First(S => S.c_id == id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult Del_Category(int id, FormCollection collection)
        {
            Category obj = new Category();

            obj = context.Categories.First(S => S.c_id == id);
            context.Categories.DeleteOnSubmit(obj);
            context.SubmitChanges();
            return RedirectToAction("Category");
        }

        public ActionResult Dtl_Category(int id)
        {
            Category obj = new Category();
            obj = context.Categories.First(S => S.c_id == id);
            ViewBag.Category = obj;
            return View(obj);
        }

        public ActionResult Product_Dtl()
        {
            List<Product_Dtl_Class> model = new List<Product_Dtl_Class>();
            var list = (from columns in context.Product_Dtls
                        join fields in context.Products on columns.pd_id equals fields.p_id
                        select new
                        {
                            Product_Id = columns.pd_id,
                            Product_Name = fields.p_name,
                            Product_Image = fields.p_img,
                            Product_Qty = columns.pd_quantity,
                            Product_price = columns.pd_price
                        }).ToList();
            foreach (var item in list)
            {
                Product_Dtl_Class obj = new Product_Dtl_Class();
                obj.pd_id = item.Product_Id;
                obj.pd_name = item.Product_Name;
                obj.pd_image = item.Product_Image;
                obj.pd_qunatity = item.Product_Qty;
                obj.pd_price = (double)item.Product_price;
                model.Add(obj);
            }
            return View(model);
        }

        public ActionResult Pdt_Dtl_Detail(int id)
        {
            Product_Dtl obj = new Product_Dtl();
            obj = context.Product_Dtls.First(S => S.pd_id == id);
            ViewBag.ProductDtl = obj;
            return View(obj);
        }

        public ActionResult Add_Pdt_Dtl()
        {
            var pd_List = context.Products.ToList();
            ViewBag.Product = pd_List;

            return View();
        }
        [HttpPost]
        public ActionResult Add_Pdt_Dtl(Product_Dtl model, FormCollection collection)
        {         
            int p_id = Convert.ToInt32(collection["ddlProduct"]);
            model.pr_id = p_id;
            context.Product_Dtls.InsertOnSubmit(model);
            context.SubmitChanges();
            return RedirectToAction("Product_Dtl");
        }

        public ActionResult Edit_Pdt_Dtl(int id)
        {
            Product_Dtl obj = new Product_Dtl();
            obj = context.Product_Dtls.First(S => S.pd_id == id);
            var pd_List = context.Products.ToList();
            ViewBag.Product = pd_List;
            return View(obj);
        }
        [HttpPost]
        public ActionResult Edit_Pdt_Dtl(int id, Product_Dtl model, FormCollection collection)
        {
            Product_Dtl obj = new Product_Dtl();
            obj = context.Product_Dtls.First(S => S.pd_id == id);
            obj.pd_price = model.pd_price;
            obj.pd_description = model.pd_description;
            obj.pd_quantity = model.pd_quantity;
            obj.pd_discount = model.pd_discount;
            int p_id = Convert.ToInt32(collection["ddlProduct"]);
            obj.pr_id = p_id;
            context.SubmitChanges();
            return RedirectToAction("Product_Dtl");
        }

        public ActionResult Del_Pdt_Dtl(int id)
        {
            Product_Dtl obj = new Product_Dtl();
            obj = context.Product_Dtls.First(S => S.pd_id == id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult Del_Pdt_Dtl(int id, FormCollection collection)
        {
            Product_Dtl obj = new Product_Dtl();
            obj = context.Product_Dtls.First(S => S.pd_id == id);
            context.Product_Dtls.DeleteOnSubmit(obj);
            context.SubmitChanges();
            return RedirectToAction("Product_Dtl");
        }

        public ActionResult Sub_Cat()
        {
            List<Sub_Cat_Class> model = new List<Sub_Cat_Class>();
            var list = (from columns in context.Sub_Categories
                        join fields in context.Categories on columns.catId equals fields.c_id
                        select new
                        {
                            SubCat_Id = columns.sc_id,
                            Cat_Name = fields.c_name,
                            SubCat_Name = columns.sc_name,
                            SubCat_Img = columns.sc_img
                        }).ToList();

            foreach (var item in list)
            {
                Sub_Cat_Class obj = new Sub_Cat_Class();
                obj.sc_id = item.SubCat_Id;
                obj.c_name = item.Cat_Name;
                obj.sc_name = item.SubCat_Name;
                obj.sc_img = item.SubCat_Img;
                model.Add(obj);
            }
            return View(model);
        }

        public ActionResult Add_SubCat()
        {
            var catList = context.Categories.ToList();
            ViewBag.Category = catList;
            return View();
        }

        [HttpPost]
        public ActionResult Add_SubCat(Sub_Category model, HttpPostedFileBase file, FormCollection collection)
        {
            string _destinationUrl = null;
            string _fileName = null;

            if (file != null)
            {

                _fileName = file.FileName;
                _destinationUrl = Path.Combine(Server.MapPath("~/Files/"), _fileName);
                file.SaveAs(_destinationUrl);
                model.sc_img = "../../Files/" + _fileName;
            }
            int c_id = Convert.ToInt32(collection["ddlCategory"]);
            model.catId = c_id;
            context.Sub_Categories.InsertOnSubmit(model);
            context.SubmitChanges();
            return RedirectToAction("Sub_Cat");
        }

        public ActionResult Edit_Subcat(int id)
        {
            Sub_Category obj = new Sub_Category();
            obj = context.Sub_Categories.First(S => S.sc_id == id);
            var ctg = context.Categories.ToList();
            ViewBag.category = ctg;
            return View(obj);
        }
        [HttpPost]
        public ActionResult Edit_Subcat(int id, Sub_Category model, HttpPostedFileBase file, FormCollection collection)
        {
            Sub_Category obj = new Sub_Category();
            obj = context.Sub_Categories.First(S => S.sc_id == id);
            obj.sc_name = model.sc_name;
            string file_name, destinationUrl;

            if (file != null)
            {
                file_name = file.FileName;
                destinationUrl = System.IO.Path.Combine(Server.MapPath("~/Files/"), file_name);
                file.SaveAs(destinationUrl);
                obj.sc_img = "../../Files/" + file_name;
            }
            int c_id = Convert.ToInt32(collection["ddlCategory"]);
            obj.catId = c_id;
            context.SubmitChanges();
            return RedirectToAction("Sub_Cat");
        }


        public ActionResult Del_Sub_Cat(int id)
        {
            Sub_Category obj = new Sub_Category();
            obj = context.Sub_Categories.First(S => S.sc_id == id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult Del_Sub_Cat(int id, FormCollection collection)
        {
            Sub_Category obj = new Sub_Category();
            obj = context.Sub_Categories.First(S => S.sc_id == id);
            context.Sub_Categories.DeleteOnSubmit(obj);
            context.SubmitChanges();
            return RedirectToAction("Sub_Cat");
        }

        public ActionResult Dtl_Sub_Cat(int id)
        {
            Sub_Category obj = new Sub_Category();
            obj = context.Sub_Categories.First(S => S.sc_id == id);
            ViewBag.Dtl_Subcat= obj;
            return View(obj);
        }

        public ActionResult Feedback()
        {
            var list = context.Contacts.ToList();
            return View(list);
        }

        public ActionResult Feedback_Dtl(int id)
        {
            Contact obj = new Contact();
            obj = context.Contacts.First(S => S.id == id);
            ViewBag.Feedback = obj;
            return View(obj);
        }

        public ActionResult Del_Feedback(int id)
        {
            Contact obj = new Contact();
            obj = context.Contacts.First(S => S.id == id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult Del_Feedback(int id, FormCollection collection)
        {
            Contact obj = new Contact();
            obj = context.Contacts.First(S => S.id == id);
            context.Contacts.DeleteOnSubmit(obj);
            context.SubmitChanges();
            return RedirectToAction("Feedback");
        }

        public ActionResult User_Orders()
        {

            var list = context.Orders.ToList();
            return View(list);
        }

        public ActionResult Del_Order(int id)
        {
            Order obj = new Order();
            obj = context.Orders.First(S => S.OrderId == id);
            return View(obj);
        }

        [HttpPost]
        public ActionResult Del_Order(int id, FormCollection collection)
        {
            Order obj = new Order();
            obj = context.Orders.First(S => S.OrderId == id);
            context.Orders.DeleteOnSubmit(obj);
            context.SubmitChanges();
            return RedirectToAction("Order");
        }

        //public ActionResult Del_OrderDtl(int id)
        //{
        //    OrderDetail obj = new OrderDetail();
        //    obj = context.OrderDetails.First(S => S.OrderDetailId == id);
        //    return View(obj);
        //}

        //[HttpPost]
        //public ActionResult Del_OrderDtl(int id, FormCollection collection)
        //{
        //    OrderDetail obj = new OrderDetail();
        //    obj = context.OrderDetails.First(S => S.OrderDetailId == id);
        //    context.OrderDetails.DeleteOnSubmit(obj);
        //    context.SubmitChanges();
        //    return RedirectToAction("Order");
        //}
        public ActionResult OrderDetail(int id)
        {
            List<OrderDetail> orderDetail = new List<OrderDetail>();
            orderDetail = context.OrderDetails.Where(o => o.OrderId == id).ToList();
            return View(orderDetail);
        }
    }
}