using Agric.Models;
using Agric.Models.ViewModel;

using IdentityModel;
using Microsoft.AspNet.Identity;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows;

namespace Agric.Controllers
{
    public class LoginController : Controller
    {
        private AgricEntities db = new AgricEntities();

        public ActionResult Index()
        {
            
          Session["aut"] = false;


            return View();
        }
        public ActionResult Apropos()
        {

            Session["aut"] = false;


            return View();
        }
        public ActionResult Nos_services()
        {

            Session["aut"] = false;


            return View();
        }
        public ActionResult Equipe()
        {

            Session["aut"] = false;


            return View();
        }
        public ActionResult Contact()
        {

            Session["aut"] = false;


            return View();
        }
        public ActionResult Login()
        {
            Session["userid"] = null;


            Session["Clientname"] = false;

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Login(UserModel model)
        {
            var user = db.Users.FirstOrDefault(u => u.Fullname == model.Username && u.Password == model.Password);

            Session["admin"] = false;
            Session["client"] = false;
            Session["tech"] = false;
            Session["labo"] = false;

            if (user != null)
            {
                
                    Session["userid"] = user.Id.ToString();
               
                if (user.Profile.Name == "Admin")

                    {
                    Session["admin"] = true;
                    Session["adminname"] = user.Fullname;
                        return RedirectToAction("Index", "MyAdmin");
                    }
                    if (user.Profile.Name == "Technicien" && user.IsActive == true)
                    {
                    
                    Session["tech"] = true;
                    Session["techname"] = user.Fullname;

                        return RedirectToAction("Index", "Tech");
                    }
                    if (user.Profile.Name == "Labo" && user.IsActive == true)
                    {
                    Session["labo"] = true;
                    Session["laboname"] = user.Fullname;
                        return RedirectToAction("Index", "Labo");
                    }
                    if (user.Profile.Name == "Client" && user.IsActive == true)
                    {
                    Session["client"] = true;
                    Session["Clientname"] = user.Fullname;
                        return RedirectToAction("Index", "Client");
                    }
                    else if (user == null)

                        return View(model);
                

               
            }
          
            return View();
        }

        public ActionResult DisconectedAdmin()
        {
            Session["admin"] = false;
            return RedirectToAction("Index");
        }
        public ActionResult DisconectedTech()
        {
            Session["tech"] = false;
            return RedirectToAction("Index");
        }
        public ActionResult DisconectedClient()
        {
            Session["client"] = false;
            return RedirectToAction("Index");
        }
        public ActionResult DisconectedLabo()
        {
            Session["labo"] = false;
            return RedirectToAction("Index");
        }
    }
}


        

    
