using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using Agric.Models;
using Agric.Models.ViewModel;

namespace Agric.Controllers
{
    [HandleError]
    public class ClientController : Controller
    {
        #region Properties
        private AgricEntities db = new AgricEntities();
        LoginController l = new LoginController();
        public string userid, username;
        public int d = 0;
        #endregion

        // GET: Client
        public ActionResult Index(string option, string search)
        {
            if ((bool)Session["client"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            ViewBag.username = Session["Clientname"];
            var cpt = db.Journal.Where(e => e.VuClient == false && e.Id_User.ToString() == userid).Count();
            ViewBag.nbrnotifclient = cpt;
            var cpt2 = db.Devis.Where(e =>  e.id_client.ToString()==userid && e.DemandeDevis==true).Count();
            ViewBag.nbrdevisclient = cpt2;
            if (option == "Essai réalisé")
            {
                var essai = db.Essai.Where(e => e.UserId.ToString() == userid && e.EtatEssai == "Réalisé" || search == null && e.EssaiDelets == false).OrderByDescending(e=> e.Date_Modife);
                return View(essai.ToList());

            }
            else if (option == "Essai en cours")
            {
                var essai = db.Essai.Where(e => e.UserId.ToString() == userid && e.EtatEssai!= "Réalisé" && e.EtatEssai != "Annulé" && e.EtatEssai != "Non installer" && e.EtatEssai.ToString() != "False" || search == null && e.EssaiDelets == false);
                return View(essai.ToList());

            }
            else if (option == "Essai annulé")
            {
                var essai1 = db.Essai.Where(e => e.UserId.ToString() == userid && e.EtatEssai== "Annulé" || search == null && e.EssaiDelets == false);
                return View(essai1.ToList());
            }
            else if (option == "Non installer")
            {
                var essai1 = db.Essai.Where(e => e.UserId.ToString() == userid && e.EtatEssai == "Non installer" || search == null && e.EssaiDelets == false);
                return View(essai1.ToList());
            }
            else
            {
                var essai = db.Essai.Where(e => e.UserId.ToString() == userid && e.EssaiDelets == false);
                return View(essai.ToList());
            }
        }
     
       
        public ActionResult EssaisDevis(Guid? id)
        {
            if ((bool)Session["client"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            var cpt = db.Journal.Where(e => e.VuClient == false && e.Id_User.ToString() == userid).Count();
            ViewBag.nbrnotifclient = cpt;
            var cpt2 = db.Devis.Where(e => e.DemandeDevis==true && e.id_client.ToString() == userid).Count();
            ViewBag.nbrdevisclient = cpt2;
            ViewBag.username = Session["Clientname"];

           
            var essai = db.Essai.Where(e => e.id_devis==id);
                return View(essai.ToList());
            
        }

        public ActionResult Details(Guid? id)
        {
            if ((bool)Session["client"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            ViewBag.username = Session["Clientname"];
            var cpt = db.Journal.Where(e => e.VuClient == false && e.Id_User.ToString() == userid).Count();
            ViewBag.nbrnotifclient = cpt;
            var cpt2 = db.Devis.Where(e => e.DemandeDevis==true && e.id_client.ToString() == userid).Count();
            ViewBag.nbrdevisclient = cpt2;
            id = Guid.Parse(userid);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        public ActionResult EditProfile(Guid? id)

        {
            if ((bool)Session["client"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            ViewBag.username = Session["Clientname"];
            id = Guid.Parse(userid);
            var cpt = db.Journal.Where(e => e.VuClient == false && e.Id_User.ToString() == userid).Count();
            ViewBag.nbrnotifclient = cpt;
            var cpt2 = db.Devis.Where(e => e.id_client.ToString() == userid && e.DemandeDevis == true).Count();
            ViewBag.nbrdevisclient = cpt2;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProfileId = new SelectList(db.Profile, "Id", "Name", users.ProfileId);
            return View(users);
        }

        // POST: Users/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Include = "Id,Fullname,Email,Address,Phone,IsActive,CreatedOn,ProfileId,Password")] Users users)
        {
            var cpt = db.Journal.Where(e => e.VuClient == false && e.Id_User.ToString() == userid).Count();
            ViewBag.nbrnotifclient = cpt;
            var cpt2 = db.Devis.Where(e => e.DemandeDevis==true && e.id_client.ToString() == userid).Count();
            ViewBag.nbrdevisclient = cpt2;
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            ViewBag.ProfileId = new SelectList(db.Profile, "Id", "Name", users.ProfileId);
            return View(users);
        }
        public ActionResult Devis()
        {
            if ((bool)Session["client"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            ViewBag.username = Session["Clientname"];
            var cpt = db.Journal.Where(e => e.VuClient == false && e.Id_User.ToString() == userid).Count();
            ViewBag.nbrnotifclient = cpt;
            var cpt2 = db.Devis.Where(e => e.DemandeDevis==true && e.id_client.ToString() == userid).Count();
            ViewBag.nbrdevisclient = cpt2;
            // var essai = db.Essai.Where(e => e.UserId.ToString() == userid && e.DevisDemander == false && e.Devis == null);
            ViewBag.ListEssai = db.Essai.Where(e => e.UserId.ToString() == userid && e.DevisDemander == false && e.Devis == null&& e.CodeClient==null).ToList();
            return View();
        }


        // GET: Client/Create
        public ActionResult Create()
        {
            userid = (string)Session["userid"];
            username = (string)Session["Clientname"];
            ViewBag.username = Session["Clientname"];
            ViewBag.UserId = new SelectList(db.Users, "Id", "Fullname");
            var cpt = db.Journal.Where(e => e.VuClient == false && e.Id_User.ToString() == userid).Count();
            ViewBag.nbrnotifclient = cpt;
            var cpt2 = db.Devis.Where(e => e.DemandeDevis==true && e.id_client.ToString() == userid).Count();
            ViewBag.nbrdevisclient = cpt2;

            return View();
        }

        // POST: Client/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,Produit,Culture,Cible, Devis,ComportementCulture,ACB,FDS")] Essai essai, HttpPostedFileBase _ACB, HttpPostedFileBase _FDS)
        {
            essai.Id = Guid.NewGuid();
            userid = (string)Session["userid"];
            username = (string)Session["Clientname"];
            essai.UserId = Guid.Parse(userid);

            if (ModelState.IsValid)

            { essai.Date_Modife = DateTime.Now;
                if (_ACB != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(_ACB.FileName);
                    string FileExtension = Path.GetExtension(_ACB.FileName);

                    FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss")
                        + FileExtension;

                    string UploadPath = "~/fichiers/";
                    essai.ACB = FileName;


                    _ACB.SaveAs(Server.MapPath(UploadPath + FileName));
                    essai.Nb = DateTime.Now.ToString();
                }

                if (_FDS != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(_FDS.FileName);
                    string FileExtension = Path.GetExtension(_FDS.FileName);

                    FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss")
                        + FileExtension;

                    string UploadPath1 = "~/fichiers/";
                    essai.FDS = FileName;


                    _FDS.SaveAs(Server.MapPath(UploadPath1 + FileName));
                }



                db.Essai.Add(essai);
                db.SaveChanges();
                return RedirectToAction("Index");




            }
            return View(essai);
        }



        public FileResult DownloadFacture(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();
            string fileName = essai.Facture;

            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ListeDevisRecu()
        {
            if ((bool)Session["client"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            ViewBag.username = Session["Clientname"];
            var cpt = db.Journal.Where(e => e.VuClient == false && e.Id_User.ToString() == userid).Count();
            ViewBag.nbrnotifclient = cpt;
            var cpt2 = db.Devis.Where(e => e.DemandeDevis==true && e.id_client.ToString() == userid).Count();
            ViewBag.nbrdevisclient = cpt2;
            var devis = db.Devis.Include(e => e.Users).Where(e => e.id_client.ToString() == userid && e.Devis1 != null);
           
            return View(devis.ToList());
        }
        public ActionResult ListeDevisDemander()
        {
            if ((bool)Session["client"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            ViewBag.username = Session["Clientname"];
            var cpt = db.Journal.Where(e => e.VuClient == false && e.Id_User.ToString() == userid).Count();
            ViewBag.nbrnotifclient = cpt;
            var cpt2 = db.Devis.Where(e => e.DemandeDevis==true && e.id_client.ToString() == userid).Count();
            ViewBag.nbrdevisclient = cpt2;
            var devis = db.Devis.Include(e => e.Users).Where(e=> e.id_client.ToString() == userid && e.Devis1==null ).OrderByDescending(e => e.date_demande);
            return View(devis.ToList());
        }

        public FileResult DownloadDevis(Guid Id)
        {
            var devis = db.Devis.Where(x => x.id == Id).FirstOrDefault();
            string fileName = devis.Devis1;

            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            devis.DemandeDevis = false;
            db.SaveChanges();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public FileResult DownloadRapport(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();
            string fileName = essai.Rapport;
            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public string[] ids;
        public ActionResult CreateDevis([Bind(Include = "nbr")] Essai essai, FormCollection formCollection)
        {
        
                userid = (string)Session["userid"];
                Guid nm;
                var devis = db.Devis.Add(new Models.Devis
                {
                    id = Guid.NewGuid(),
                    date_demande = DateTime.Now,
                    id_client = new Guid(userid),
                });
                nm = devis.id;
            ids = formCollection["essaiId"].Split(new char[] { ',' });

            if (ids == null)
            {
                return RedirectToAction("Devis");
            }
            else
            {
                
                foreach (string id in ids)
                {
                    var essai1 = db.Essai.Find(Guid.Parse(id));
                    essai1.id_devis = nm;
                    essai1.DevisDemander = true;
                    db.SaveChanges();
                }
            }



                return RedirectToAction("Devis");
          
        }

       
        public ActionResult Modification()
        {
            if ((bool)Session["client"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            ViewBag.username = Session["Clientname"];
            var journal = db.Journal.Include(e => e.Users).Where(e => e.VuClient == false && e.Id_User.ToString() == userid);
            var cpt = db.Journal.Where(e => e.VuClient == false && e.Id_User.ToString() == userid).Count();
            ViewBag.nbrnotifclient = cpt;
          
            var cpt2 = db.Devis.Where(e => e.DemandeDevis==true && e.id_client.ToString() == userid).Count();
            ViewBag.nbrdevisclient = cpt2;
            return View(journal.ToList());
        }
        public ActionResult DetailsEssaiJournal(Guid? id, Guid? id1)
        {
            if ((bool)Session["client"] == false)
            { return Redirect("/"); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Essai essai = db.Essai.Find(id);
            var journal = db.Journal.Where(x => x.Id == id1 && x.Id_User.ToString() == userid).FirstOrDefault();

            var cpt = db.Journal.Where(e => e.VuClient == false && e.Id_User.ToString() == userid).Count();
            ViewBag.nbrnotifclient = cpt;
            if (journal != null)
            {
                journal.VuClient = true;
            }

            db.SaveChanges();

            if (essai == null)
            {
                return HttpNotFound();
            }

            return View(essai);
        }
        public ActionResult DeleteJournal(Guid? id)
        {
            Journal journal = db.Journal.Find(id);
            db.Journal.Remove(journal);
            db.SaveChanges();
            return RedirectToAction("Modification");
        }

    }


}
