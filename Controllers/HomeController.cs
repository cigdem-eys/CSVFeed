using CSVFeed.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Collections;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Net;

namespace CSVFeed.Controllers
{
    public class HomeController : Controller
    {
        public static string dosya = "";
        ProductContext db = new ProductContext();
        public ActionResult Index()
        {
            if (Session["Kullanici"] == null)
            {
                ViewBag.Js = @"Alert(`warning`,`Bu sayfayı görüntülemek için login olmalısınız.Yönlendiriliyorsunuz...`,`AlertWrite`,true);";
                System.Threading.Thread.Sleep(3000);
                Response.Redirect("/Home/Login/");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {

            if (file != null)
            {
                string dosya = Path.GetFileName(file.FileName);
                //Dosya geçici klasöre yüklenir
                Random rnd = new Random();
                int sayi = rnd.Next(10000000, 1000000000);
                string dosya_adi = "CSV_" + sayi.ToString() + ".csv";
                var yuklenecekyer = Path.Combine(Server.MapPath("~/GeciciDosyalar/" + dosya_adi));
                
                file.SaveAs(yuklenecekyer);


                

                ArrayList esles = new ArrayList();
                esles.Add("Barcode");//Barcode
                esles.Add("Price");//Price
                esles.Add("Stock");//Stock
                esles.Add("Name");//Name
                esles.Add("Product_Code");//ProductCode
                esles.Add("Image1");//Image1
                esles.Add("Image2");//Image2
                esles.Add("Image3");//Image3
                esles.Add("Image4");//Image4
                esles.Add("Image5");//Image5
                esles.Add("Product_Id");//Product_ID
                DataTable dt = ConvertCSVtoDataTable(yuklenecekyer,esles);
                FileInfo f = new FileInfo(yuklenecekyer);
                f.Delete();
            }
            return View();
        }
        public DataTable ConvertCSVtoDataTable(string strFilePath,ArrayList col)
        {
            DataTable dt = new DataTable();
            int sil = 0;
            bool uygunVeri = false;
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');

                for (int i = 0; i < col.Count; i++)
                {
                    dt.Columns.Add(headers[i]);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    int s = 0;

                    if (rows[0].ToString().Trim().Length == 13 && SayiMi(rows[0].ToString()))
                    {
                        uygunVeri = true;

                        for (int i = 0; i < col.Count; i++)
                        {
                            if (uygunVeri)
                            {
                                if (i < rows.Length)
                                    dr[i] = rows[i];

                                else
                                    dr[i] = null;
                            }
                            else
                            {
                                sil++;
                            }

                        }
                    }
                    else
                    {
                        uygunVeri = false;
                        sil++;
                    }

                    if (uygunVeri)
                        dt.Rows.Add(dr);
                }

            }
            ViewBag.Js = @"Alert(`primary`,`Dosyada formata uygun olmayan " + sil + " adet kayıt silindi.Son hali ile işlem kuyruğuna alındı.Sistem otomatik olarak güncelleyecektir.`,`AlertWrite`,false);";

            Random rnd = new Random();
            int sayi = rnd.Next(10000000, 1000000000);
            dosya = "CSV_" + sayi.ToString() + ".csv";
            WriteToCsvFile(dt, @"C:\Dosyalar\" + dosya);
            ViewData["Product"] = ConvertDataTableToHTML(dt);
            return dt;
        }
        public Boolean SayiMi(String strVeri)
        {
            if (String.IsNullOrEmpty(strVeri) == true)
            {
                return false;
            }
            else
            {
                Regex desen = new Regex("^[0-9]*$");
                return desen.IsMatch(strVeri);
            }
        }
        public static string ConvertDataTableToHTML(DataTable dt)
        {
            
            //8697451925024   örnek barkod
            string html = "<div class='col-12'>" +
                "</div><table class='table table-responsive table-bordered' style='font-size:10px;height:500px'>" +
                "<thead class='thead-dark'>";
            //add header row
            html += "<tr>";
            for (int i = 0; i < dt.Columns.Count; i++)
                html += "<th scpoe='col' style='150px'>" + dt.Columns[i].ColumnName + "</th>";
            html += "</tr></thead><tbody>";
            //add rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                html += "</tr>";
            }
            html += "</tbody></table>";
            return html;
        }
        public static void WriteToCsvFile(DataTable dataTable, string filePath)
        {
            StringBuilder fileContent = new StringBuilder();

            foreach (var col in dataTable.Columns)
            {
                fileContent.Append(col.ToString() + ",");
            }

            fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);

            foreach (DataRow dr in dataTable.Rows)
            {
                foreach (var column in dr.ItemArray)
                {
                    fileContent.Append( column.ToString() + ",");
                }

                fileContent.Replace(",", System.Environment.NewLine, fileContent.Length - 1, 1);
            }

            System.IO.File.WriteAllText(filePath, fileContent.ToString(), Encoding.UTF8);
        
        }
        public ActionResult HowToUse()
        {
            ViewBag.Message = "";

            return View();
        }
        public ActionResult Login()
        {
            ViewBag.Message = "";

            return View();
        }
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            List<User> list = db.Users.ToList();
            string sifre = MD5Sifrele(password);
            var user = list.Where(i => i.Email == email && i.Password == sifre).ToList();
            if (user != null)
            {
                Session.Add("Kullanici", user[0].Name + " " + user[0].Surname);

                //Session["Kullanici"] = user[0].Name+" "+user[0].Surname;
                //ViewData["Kullanici"]= user[0].Name + " " + user[0].Surname;
                Response.Redirect("/Home/Index/");
            }
            else
                ViewBag.Js = @"Alert(`warning`,`Bu kullanıcı sistemde kayıtlı değil`,`AlertWrite`,true);";

            return View();
        }
        public ActionResult Register()
        {
            ViewBag.Message = "";

            return View();
        }
        [HttpPost]
        public ActionResult Register(string name, string surname, string email, string password)
        {
            if (name == "" || surname == "" || email == "" || password == "")
            {
                ViewBag.Js = @"Alert(`warning`,`Tüm alanlar zorunludur`,`AlertWrite`,true);";
                return View();
            }

            List<User> list = db.Users.ToList();

            int kontrol = list.Where(i => i.Email == email).Count();

            if (kontrol == 0)
            {
                User u = new User();
                u.Name = name;
                u.Surname = surname;
                u.Email = email;
                u.Password = MD5Sifrele(password);

                db.Users.Add(u);
                db.SaveChanges();

                ViewBag.Js = @"Alert(`success`,`Kaydınız başarıyla gerçekleşti.Login olabilirsiniz`,`AlertWrite`,false);";
            }
            else
                ViewBag.Js = @"Alert(`danger`,`Bu kullanıcı zaten kayıtlı`,`AlertWrite`,true);";

            return View();
        }
        public static string MD5Sifrele(string sifrelenecekMetin)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] dizi = Encoding.UTF8.GetBytes(sifrelenecekMetin);
            dizi = md5.ComputeHash(dizi);
            StringBuilder sb = new StringBuilder();

            foreach (byte ba in dizi)
                sb.Append(ba.ToString("x2").ToLower());
            return sb.ToString();
        }
        
    }
}