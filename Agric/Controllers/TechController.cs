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
    public class TechController : Controller
    {
        private AgricEntities db = new AgricEntities();
        public string techname , userid;
        // GET: Tech
        public ActionResult Index()
        {
            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
            {
                if ((bool)Session["tech"] == false)
                { return Redirect("/"); }
                techname = (string)Session["techname"];
                ViewBag.username = Session["techname"];

                var essai = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == false && e.TechName == techname && (e.EtatEssai == "En cours" || e.EtatEssai == "Non installer") && e.Date_Cloture == null).OrderByDescending(e => e.Date_Modife);
                return View(essai.ToList());
            }
        }
        public ActionResult EssaiSemaine()
        {
            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
            {
                if ((bool)Session["tech"] == false)
                { return Redirect("/"); }
                techname = (string)Session["techname"];
                ViewBag.username = Session["techname"];

                var essai1 = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == false && e.TechName == techname && (e.EtatEssai == "En cours" || e.EtatEssai == "Non installer") && e.Date_Cloture == null).OrderByDescending(e => e.Date_Modife);
                return View(essai1.ToList());
            }
        }
        public ActionResult Notations(Guid? essai_id)
        {
            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
            {
                if ((bool)Session["tech"] == false)
                { return Redirect("/"); }
                var Notation = db.Notation.Where(e => e.Essai_Id == essai_id);
                Essai essai = db.Essai.Find(essai_id);
                ViewBag.id_essai = essai_id;
                var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
                ViewBag.nbrnotif = cpt;
                techname = (string)Session["techname"];
                ViewBag.username = Session["techname"];
                if (essai == null)
                {
                    return HttpNotFound();
                }
                return View(Notation.ToList());
            }
        }
        public ActionResult AddNotation(Guid? essai_id, Guid? id)
        {
            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
            {
                if ((bool)Session["tech"] == false)
                { return Redirect("/"); }
                var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
                ViewBag.nbrnotif = cpt;
                Session["id_essai"] = essai_id;
                techname = (string)Session["techname"];
                ViewBag.username = Session["techname"];

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Notation notation = db.Notation.Find(id);
                if (notation == null)
                {
                    return HttpNotFound();
                }

                return View(notation);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNotation([Bind(Include = "Id,D_N_P,D_N_R,Essai_Id,Date_Add,FNotation")] Notation notation, HttpPostedFileBase _FNotation)
        {
            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
            {
                if ((bool)Session["tech"] == false)
                { return Redirect("/"); }
                var essai_id = (Guid)Session["id_essai"];
                //  var essai = db.Essai.Where(x => x.Id == essai_id).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    userid = (string)Session["userid"];
                    techname = (string)Session["techname"];
                    ViewBag.username = Session["techname"];
                    if (_FNotation != null)
                    {
                        string FileName = Path.GetFileNameWithoutExtension(_FNotation.FileName);
                        string FileExtension = Path.GetExtension(_FNotation.FileName);
                        FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss") + FileExtension;
                        string UploadPath1 = "~/fichiers/";
                        notation.FNotation = FileName;
                        _FNotation.SaveAs(Server.MapPath(UploadPath1 + FileName));
                    }

                    notation.Date_Add = DateTime.Now;
                    db.Journal.Add(new Models.Journal
                    {
                        Id = Guid.NewGuid(),
                        Operation = "Technicien Ajouter une notation .",
                        Date_Modification = DateTime.Now,
                        Id_Essai = essai_id,
                        Id_User = new Guid(userid),
                        Name = techname,
                    });
                    db.Entry(notation).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Notations", new { essai_id });
                }

                return View(notation);
            }
        }

        public ActionResult Details(Guid? id)
        {
            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
            {
                if ((bool)Session["tech"] == false)
                { return Redirect("/"); }
                userid = (string)Session["userid"];
                ViewBag.username = Session["techname"];
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
        }

        public ActionResult EditProfile(Guid? id)
        {
            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
            {
                if ((bool)Session["Tech"] == false)
                { return Redirect("/"); }
                userid = (string)Session["userid"];
                ViewBag.username = Session["techname"];
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
        }

        // POST: Users/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Include = "Id,Fullname,Email,Address,Phone,IsActive,CreatedOn,ProfileId,Password")] Users users)
        {
            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
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
        }

        // GET: Tech/Create
        public ActionResult Create()
        {
            ViewBag.username = Session["techname"];
            ViewBag.UserId = new SelectList(db.Users, "Id", "Fullname");
            return View();
        }

        // POST: Tech/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Rapport")] Essai essai)
        {
            if (ModelState.IsValid)
            {
              
                essai.Id = Guid.NewGuid();
                db.Essai.Add(essai);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "Fullname", essai.UserId);
            return View(essai);

        }

        
      
        public ActionResult Edit(Guid? id)
        {
            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
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
        }

        // POST: Tech/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
            [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,CodeClient,TA,Formulation,DP,MA,DateNotation,PE,Usage,PR,Produit,Culture,Cible,Nb,Region,Code,Date_Cloture,Rapport,RapportRemis,DateRemisRapport,ACB,FDS,LaboName,TechName,Date_Instalation,FNNotation,FDEFicheEssai,Devis,DevisDemander,Payer,NonPayer,Payer50,EtatEssai,EtatPaiment,FormulationProduitRéf,DPRéf,MARéf,ComportementCulture,Facture,id_devis")] Essai essai, HttpPostedFileBase _FNNotation,HttpPostedFileBase _FDEFicheEssai , Journal journal,Notation notation)
        {

            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
            {

                if (ModelState.IsValid)
                {

                    userid = (string)Session["userid"];
                    techname = (string)Session["techname"];
                    ViewBag.username = Session["techname"];

                    if (_FNNotation != null)
                    {
                        string FileName = Path.GetFileNameWithoutExtension(_FNNotation.FileName);
                        string FileExtension = Path.GetExtension(_FNNotation.FileName);

                        FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss")
                            + FileExtension;

                        string UploadPath1 = "~/fichiers/";
                        notation.FNotation = FileName;
                        notation.D_N_R = DateTime.Now;

                        _FNNotation.SaveAs(Server.MapPath(UploadPath1 + FileName));
                    }

                    if (_FDEFicheEssai != null)
                    {
                        string FileName = Path.GetFileNameWithoutExtension(_FDEFicheEssai.FileName);
                        string FileExtension = Path.GetExtension(_FDEFicheEssai.FileName);

                        FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss")
                            + FileExtension;

                        string UploadPath1 = "~/fichiers/";
                        essai.FDEFicheEssai = FileName;


                        _FDEFicheEssai.SaveAs(Server.MapPath(UploadPath1 + FileName));
                    }

                    db.Journal.Add(new Models.Journal
                    {
                        Id = Guid.NewGuid(),
                        Operation = "Technicien Ajouter les données",
                        Date_Modification = DateTime.Now,
                        Id_Essai = essai.Id,
                        Id_User = new Guid(userid),
                        Name = techname,
                    });



                    db.Entry(essai).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                ViewBag.UserId = new SelectList(db.Users, "Id", "Fullname", essai.UserId);
                return View(essai);
            }
            }

            // GET: Tech/Delete/5
            public ActionResult Delete(Guid? id)
            {
            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
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
                return View(essai);
            }
            }

            // POST: Tech/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(Guid id)
            {
            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
            {
                Essai essai = db.Essai.Find(id);
                db.Essai.Remove(essai);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public FileResult DownloadFDEFicheEssai(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();
            string fileName = essai.FDEFicheEssai;
            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
          


            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);



        }
        public FileResult DownloadFNotation(Guid Id)
        {
            var notation = db.Notation.Where(x => x.Id == Id).FirstOrDefault();
            string fileName = notation.FNotation;
            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public FileResult DownloadPE(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();
            string fileName = essai.PE;
            ViewBag.FileName = fileName;
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
        public ActionResult Datepicker(DateTime? search)
        {
            if (Session["tech"] == null)
            { return Redirect("/"); }
            else
            {
                if ((bool)Session["tech"] == false)
                { return Redirect("/"); }
                techname = (string)Session["techname"];
                ViewBag.username = Session["techname"];
                ViewBag.User = new SelectList(db.Users, "Id", "Fullname");

                var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
                ViewBag.nbrnotif = cpt;
                if (search != null)
                {
                    var nts = db.Notation.Where(e => e.D_N_P == search).Select(e => e.Essai_Id).ToList();
                    var essai2 = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == false && e.TechName == techname && (e.EtatEssai == "En cours" || e.EtatEssai == "Non installer") && nts.Contains(e.Id)).ToList();
                    return View(essai2);
                }
                else
                {
                    var essai2 = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == false && e.TechName == techname && (e.EtatEssai == "En cours" || e.EtatEssai == "Non installer") && e.Id == null).ToList();
                    return View(essai2);
                }
            }
        }
    }

}
