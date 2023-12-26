using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using Agric.Models;

namespace Agric.Controllers
{
    public class LaboController : Controller
    {
        private AgricEntities db = new AgricEntities();
        public string laboname , userid;
        // GET: Labo
        public ActionResult Index(string option, string search)
        {
            if ((bool)Session["labo"] == false)
            { return Redirect("/"); }
            laboname = (string)Session["laboname"] ;
            ViewBag.username = Session["laboname"];
            LoginController l = new LoginController();
           // var essai = db.Essai.Include(e => e.Users).Where(n => n.LaboName == laboname);
            if (option == "Code")
            {
                var essai2 = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == false && e.Code == search || e.Code.StartsWith(search) || search == null && e.LaboName == laboname).OrderByDescending(e => e.Date_Modife);
                return View(essai2.ToList());
            }
            else
            {
                var essai = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == false && e.LaboName == laboname).OrderByDescending(e => e.Date_Modife);
                return View(essai.ToList());
            }

          
        }

        public ActionResult Details(Guid? id)
        {
            if ((bool)Session["labo"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            ViewBag.username = Session["laboname"];
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
            userid = (string)Session["userid"];
            ViewBag.username = Session["laboname"];
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
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            ViewBag.ProfileId = new SelectList(db.Profile, "Id", "Name", users.ProfileId);
            return View(users);
        }

        // GET: Labo/Create
        public ActionResult Create()
        {
            ViewBag.username = Session["laboname"];
            ViewBag.UserId = new SelectList(db.Users, "Id", "Fullname");
            return View();
        }

        // POST: Labo/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,CodeClient,TA,Formulation,DP,MA,TE,PE,Date_Reception_Produit,PR,Produit,Culture,Cible,Date_Reception_ACB,Region,Code,Date_Cloture,Rapport,RapportRemis,DateRemisRapport,ACB,FDS")] Essai essai)
        {
            if (ModelState.IsValid)
            {
                try { 
                essai.Id = Guid.NewGuid();
                db.Essai.Add(essai);
                db.SaveChanges();
                return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    MessageBox.Show("Vérifier les cases !!!!!");
                }
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "Fullname", essai.UserId);
            return View(essai);
        }


        // GET: Tech/Edit/5
        public ActionResult Edit(Guid? id)
        {
            ViewBag.username = Session["techname"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Essai essai = db.Essai.Find(id);
            if (essai == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Fullname", essai.UserId);
            return View(essai);
        }

        // POST: Tech/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,CodeClient,TA,Formulation,DP,MA,DateNotation,PE,Date_Reception_Produit,PR,Produit,Culture,Cible,Date_Reception_ACB,Region,Code,Date_Cloture,Rapport,RapportRemis,DateRemisRapport,ACB,FDS,LaboName,TechName,TE,FNNotation,FDEFicheEssai,Mode_action,DevisDemander,Payer,NonPayer,Payer50,EtatEssai,FormulationProduitRéf,DPRéf,MARéf,Etat_Mesure,ModeAction,Epoque_application,Facture,id_devis")] Essai essai, HttpPostedFileBase _FNNotation, HttpPostedFileBase _FDEFicheEssai)
        {



            if (ModelState.IsValid)
            {

                userid = (string)Session["userid"];
                laboname = (string)Session["laboname"];
                ViewBag.username = Session["laboname"];

                db.Journal.Add(new Models.Journal
                {
                    Id = Guid.NewGuid(),
                    Operation = "Labo Ajouter des données",
                    Date_Modification = DateTime.Now,
                    Id_Essai = essai.Id,
                    Id_User = new Guid(userid),
                    Name = laboname,

                });
                db.Entry(essai).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Fullname", essai.UserId);
            return View(essai);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public FileResult DownloadPE(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();
            string fileName = essai.FDS;

            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);



        }
        public FileResult DownloadFDEFicheEssai(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();
            string fileName = essai.FDEFicheEssai;

            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);



        }
        public FileResult DownloadFNNotation(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();
            string fileName = essai.FNNotation;

            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        public FileResult DownloadCD()
        {
            string fileName = "calcule dose.xlsx";
            string path = Server.MapPath("~/fichiers/MesureLabo/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);


            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "calcule dose.xlsx");
        }

    }
}
