using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Windows;
using System.Windows.Forms;
using Agric.Models;
using Agric.Models.ViewModel;
using MongoDB.Bson;
using MongoDB.Driver;
using OfficeOpenXml;
using System.Web.Mvc.Html;
using MessageBox = System.Windows.MessageBox;


namespace Agric.Controllers
{
    public class MyAdminController : Controller
    {
        private AgricEntities db = new AgricEntities();
        public string userid;


        // ======================================Essais=====================================

        public ActionResult Index()
        {

            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            ViewBag.Username = Session["adminname"];
            ViewBag.User = new SelectList(db.Users, "Id", "Fullname");

            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
          /*  if (search != null)
            {
                var nts = db.Notation.Where(e => e.D_N_P == search).Select(e => e.Essai_Id).ToList();
                var essai2 = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == false && nts.Contains(e.Id)).ToList();
                return View(essai2);
            }
            else
            {*/
                var essai1 = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == false).OrderByDescending(e => e.Date_Modife);
                return View(essai1.ToList());
            
        }
        public ActionResult Datepicker(DateTime? search )
        {

            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            ViewBag.Username = Session["adminname"];
            ViewBag.User = new SelectList(db.Users, "Id", "Fullname");

            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            if (search != null)
            {
                var nts = db.Notation.Where(e => e.D_N_P == search).Select(e => e.Essai_Id).ToList();
                var essai2 = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == false && nts.Contains(e.Id)).ToList();
                return View(essai2);
            }
            else
            {
                var essai2 = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == false && e.Id==null).ToList();
                return View(essai2);
            }

        }
        public ActionResult EssaiSemaine()
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            ViewBag.Username = Session["adminname"];
            ViewBag.User = new SelectList(db.Users, "Id", "Fullname");

            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            
            var essai1 = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == false).OrderByDescending(e => e.Date_Modife);
            return View(essai1.ToList());
        }
        public ActionResult Modification()
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            ViewBag.Username = Session["adminname"];
            var journal = db.Journal.Include(e => e.Users).OrderByDescending(e => e.Date_Modification);
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            return View(journal.ToList());
        }

        // GET: Essais/Details/5
        public ActionResult DetailsEssai(Guid? id)
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            ViewBag.Username = Session["adminname"];
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
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

        // GET: Essais/Details/5
        public ActionResult DetailsEssaiJournal(Guid? id, Guid? id1)
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            ViewBag.Username = Session["adminname"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Essai essai = db.Essai.Find(id);
            var journal = db.Journal.Where(x => x.Id == id1).FirstOrDefault();

            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            if (journal != null)
            {
                journal.VuAdmin = true;
            }

            db.SaveChanges();

            if (essai == null)
            {
                return HttpNotFound();
            }

            return View(essai);
        }

        public ActionResult Notations(Guid? essai_id)
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            var Notation = db.Notation.Where(e => e.Essai_Id == essai_id);
            Essai essai = db.Essai.Find(essai_id);
            ViewBag.id_essai = essai_id;
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            ViewBag.Username = Session["adminname"];
            if (essai == null)
            {
                return HttpNotFound();
            }
            return View(Notation.ToList());
        }
        public ActionResult AddNotation(Guid? essai_id)
        {
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            Session["id_essai"] = essai_id;
            ViewBag.Username = Session["adminname"];
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNotation([Bind(Include = "Id,D_N_P,D_N_R,essai_Id,FNotation")] Notation notation,HttpPostedFileBase _FNotation)
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            var essai_id = (Guid)Session["id_essai"];
            var essai = db.Essai.Where(x => x.Id == essai_id).FirstOrDefault();
            if (ModelState.IsValid)
            {
                if (_FNotation != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(_FNotation.FileName);
                    string FileExtension = Path.GetExtension(_FNotation.FileName);
                    FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss") + FileExtension;
                    string UploadPath1 = "~/fichiers/";
                    notation.FNotation = FileName;
                    _FNotation.SaveAs(Server.MapPath(UploadPath1 + FileName));
                }
                notation.Id = Guid.NewGuid();
                
                notation.Essai_Id = essai.Id;
                db.Notation.Add(notation);
                db.SaveChanges();
                return RedirectToAction("Notations", new { essai_id });
            }

            return View(notation);
        }
        public ActionResult EditNotation(Guid? id,Guid essai_id)
        {
         
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }

            userid = (string)Session["userid"];
            ViewBag.Username = Session["adminname"];
            Session["id_essai"] = essai_id;
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
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

        // POST: Notations/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNotation([Bind(Include = "Id,D_N_P,D_N_R,FNotation,Essai_Id,Date_Add")] Notation notation,HttpPostedFileBase _FNotation)
        {
            
            var essai_id = (Guid)Session["id_essai"];
            if (ModelState.IsValid)
            {
                if (_FNotation != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(_FNotation.FileName);
                    string FileExtension = Path.GetExtension(_FNotation.FileName);
                    FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss") + FileExtension;
                    string UploadPath1 = "~/fichiers/";
                    notation.FNotation = FileName;
                    _FNotation.SaveAs(Server.MapPath(UploadPath1 + FileName));
                }
                db.Entry(notation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Notations", new { essai_id });
            }
            return View(notation);
        }

        // GET: Notations/Delete/5
      

        // POST: Notations/Delete/5
      
        public ActionResult DeleteNotation(Guid? id, Guid Essai_id)
        {
            Session["id_essai"] = Essai_id;
          
            var essai_id = (Guid)Session["id_essai"];
            Notation notation = db.Notation.Find(id);
            db.Notation.Remove(notation);
            db.SaveChanges();
            return RedirectToAction("Notations", new { essai_id });
        }
        public ActionResult Details(Guid? id)
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            id = Guid.Parse(userid);
            ViewBag.Username = Session["adminname"];
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
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
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];
            ViewBag.Username = Session["adminname"];
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
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
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

        // GET: MyAdmin/Edit/5
        public ActionResult Edit(Guid? id)
        {
        ViewBag.Username = Session["adminname"];
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Essai essai = db.Essai.Find(id);
            if (essai == null)
            {
                return HttpNotFound();
            }


            ViewBag.UserId = new SelectList(db.Users.Where(g => g.ProfileId.ToString() == ("6bf5738f-852c-4ea9-8d5c-aad7f7e4ac89")), "Id", "Fullname", essai.UserId);
            ViewBag.LaboId = new SelectList(db.Users.Where(g => g.ProfileId.ToString() == ("d762c993-32b3-4519-9f59-35a7cb0b229f") || g.ProfileId.ToString() == ("58e6e325-2ebc-4aa1-b652-2a76c3e03a02")), "Id", "Fullname", essai.UserId);
            ViewBag.TechId = new SelectList(db.Users.Where(g => g.ProfileId.ToString() == ("58e6e325-2ebc-4aa1-b652-2a76c3e03a02")), "Id", "Fullname", essai.UserId);
            ViewBag.ClientId = new SelectList(db.Users.Where(g => g.ProfileId.ToString() == ("6BF5738F-852C-4EA9-8D5C-AAD7F7E4AC89")), "Id", "Fullname", essai.UserId);



            return View(essai);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,CodeClient,TA,DateNotation,Formulation,DP,Usage,MA,Date_Instalation,PE,PR,Produit,Culture,ACB,FDS,Cible,Nb,Region,Code,Date_Cloture,Rapport,RapportRemis,DateRemisRapport,EtatEssai,LaboName,TechName,DevisDemander,ACB,FDS,Payer,NonPayer,Paiment_Regler50,Payer50,Devis,FDEFicheEssai,FNNotation,FormulationProduitRéf,DPRéf,Facture,MARéf,ComportementCulture,id_devis,Devis")] Essai essai, HttpPostedFileBase _Rapport, HttpPostedFileBase _PE, HttpPostedFileBase _ACB, HttpPostedFileBase _FDS, HttpPostedFileBase _Facture)
        {

            userid = (string)Session["userid"];
            if (ModelState.IsValid)
            {
                DateTime now = DateTime.Now;
                essai.Date_Modife = now;


                if (_Rapport != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(_Rapport.FileName);
                    string FileExtension = Path.GetExtension(_Rapport.FileName);

                    FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss")
                        + FileExtension;

                    string UploadPath = "~/fichiers/";
                    essai.Rapport = FileName;


                    _Rapport.SaveAs(Server.MapPath(UploadPath + FileName));
                }
                if (_PE != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(_PE.FileName);
                    string FileExtension = Path.GetExtension(_PE.FileName);

                    FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss")
                        + FileExtension;

                    string UploadPath = "~/fichiers/";
                    essai.PE = FileName;


                    _PE.SaveAs(Server.MapPath(UploadPath + FileName));
                }
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

                    string UploadPath = "~/fichiers/";
                    essai.FDS = FileName;


                    _FDS.SaveAs(Server.MapPath(UploadPath + FileName));
                }
                if (_Facture != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(_Facture.FileName);
                    string FileExtension = Path.GetExtension(_Facture.FileName);

                    FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss")
                        + FileExtension;

                    string UploadPath = "~/fichiers/";
                    essai.Facture = FileName;


                    _Facture.SaveAs(Server.MapPath(UploadPath + FileName));
                }

                db.Entry(essai).State = EntityState.Modified;
                db.Journal.Add(new Models.Journal
                {
                    Id = Guid.NewGuid(),
                    Operation = "Des données ajouter",
                    Date_Modification = DateTime.Now,
                    Id_Essai = essai.Id,
                    Id_User = essai.UserId,


                });
                db.SaveChanges();
                return RedirectToAction("Index");


            }
            // ViewBag.User = new SelectList(, "Id", "Fullname");
            //ViewBag.UserId = new SelectList(db.Users, "Id", "Fullname",essai.UserId);
            return View(essai);
        }


        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users.Where(g => g.ProfileId.ToString() == ("6bf5738f-852c-4ea9-8d5c-aad7f7e4ac89")), "Id", "Fullname");
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            ViewBag.Username = Session["adminname"];
            return View();


        }

        // POST: Essais/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Produit,Culture,Cible")] Essai essai)
        {
            if (ModelState.IsValid)
            {
                essai.Id = Guid.NewGuid();
                essai.Date_Modife = DateTime.Now;
                essai.EtatEssai = "Non installer";
                db.Essai.Add(essai);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(essai);
        }

       
        // GET: MyAdmin/Delete/5
        public ActionResult Delete(Guid? id, Guid Essai_id)
        {
            Session["id_essai"] = Essai_id;
            var essai_id = (Guid)Session["id_essai"];
            Essai essai = db.Essai.Find(Essai_id);
            db.Essai.Remove(essai);
            db.SaveChanges();
            return RedirectToAction("EssaisArchiver", new { essai_id });
          
        }



        // ======================================Users=====================================

        public ActionResult Historique()
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            ViewBag.Username = Session["adminname"];
            ViewBag.User = new SelectList(db.Users, "Id", "Fullname");


            var essai1 = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == false && DateTime.Now.Year > e.Date_Cloture.Value.Year && e.Rapport != null && e.id_devis != null);
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            return View(essai1.ToList());
        }
        public ActionResult ListeUsers()
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            ViewBag.Username = Session["adminname"];

            var users = db.Users.Include(u => u.Profile).OrderByDescending(c => c.CreatedOn);
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            return View(users.ToList());
        }

        public ActionResult ListeClients()
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            ViewBag.Username = Session["adminname"];
            var users = db.Users.Include(u => u.Profile).Where(c => c.ProfileId.ToString() == ("6bf5738f-852c-4ea9-8d5c-aad7f7e4ac89")).OrderByDescending(c=> c.CreatedOn);
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            return View(users.ToList());
        }

        public ActionResult ListeLabos()
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            ViewBag.Username = Session["adminname"];
            var users = db.Users.Include(u => u.Profile).Where(c => c.ProfileId.ToString() == ("d762c993-32b3-4519-9f59-35a7cb0b229f"));
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            return View(users.ToList());
        }
        //==============================================================================================================
        public ActionResult ListeDevisDemander()
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            ViewBag.Username = Session["adminname"];

            var devis = db.Devis.Include(e => e.Users).Where(e => e.Devis1 == null).OrderByDescending(e => e.date_demande);
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            return View(devis.ToList());
        }
        public ActionResult ListeDevisEnvoyer()
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            ViewBag.Username = Session["adminname"];

            var devis = db.Devis.Include(e => e.Users).Where(e => e.Devis1 != null).OrderByDescending(d=> d.date_demande);
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            return View(devis.ToList());
        }
        //=====================================================================================================================
        public ActionResult ListeTechniciens()
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            ViewBag.Username = Session["adminname"];
            var users = db.Users.Include(u => u.Profile).Where(c => c.ProfileId.ToString() == ("58e6e325-2ebc-4aa1-b652-2a76c3e03a02"));
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            return View(users.ToList());
        }
        // GET: Users/Create



        public ActionResult CreateUser()
        {
            ViewBag.Username = Session["adminname"];

            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            ViewBag.ProfileId = new SelectList(db.Profile, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser([Bind(Include = "Id,Fullname,Email,Address,Phone,IsActive,CreatedOn,ProfileId,Password")] Users users)
        {
            DateTime now = DateTime.Now;
            users.CreatedOn = now;

            if (ModelState.IsValid)
            {
                try
                {
                    users.Id = Guid.NewGuid();

                    db.Users.Add(users);
                    db.SaveChanges();
                    return RedirectToAction("ListeUsers");
                }
                catch (Exception)
                {
                    MessageBox.Show("veuillez remplir les case vide s'il vous plait !!!!!");
                }
            }

            ViewBag.ProfileId = new SelectList(db.Profile, "Id", "Name", users.ProfileId);
            return View(users);
        }

        // GET: Users/Edit/5
        public ActionResult EditUser(Guid? id)
        {
            ViewBag.Username = Session["adminname"];
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProfileId = new SelectList(db.Profile.Where(g => g.Id.ToString() != ("f20e48e5-8304-4208-93b0-488dc208ed79")), "Id", "Name", users.ProfileId);
            return View(users);
        }

        // POST: Users/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "Id,Fullname,Email,Address,Phone,IsActive,CreatedOn,ProfileId,Password")] Users users)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(users).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ListeUsers");
                }
                catch (Exception)
                {
                    MessageBox.Show("Vérifier les informations !!!!!");
                }
            }
            ViewBag.ProfileId = new SelectList(db.Profile, "Id", "Name", users.ProfileId);
            return View(users);
        }

        // GET: Users/Delete/5
        public ActionResult DeleteUser(Guid? id)
        {
            ViewBag.Username = Session["adminname"];
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
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

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedUser(Guid id)
        {
           
                Users users = db.Users.Find(id);
                var ND = users.Devis.Count();
                var NJ = users.Journal.Count();
                if (ND == 0 && NJ==0 )
                {
                    db.Users.Remove(users);
                    db.SaveChanges();
                    return RedirectToAction("ListeUsers");
                }
                else{
                MessageBox.Show("il y a des essais , devis , notifications à ce client ", "Probléme de suppression !!!");
                return RedirectToAction("ListeClients");
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
        
        public ActionResult Archive(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();

            if (essai != null)
            {
                essai.EssaiDelets = true;
            }

            db.SaveChanges();

            return RedirectToAction("Index");
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
        public FileResult DownloadFDS(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();
            string fileName = essai.FDS;

            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);


            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);



        }
        public FileResult DownloadPE(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();
            string fileName = essai.PE;

            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);



            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);



        }
        public FileResult DownloadACB(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();
            string fileName = essai.ACB;
            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

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
        public FileResult DownloadDevis(Guid Id)
        {
            var devis = db.Devis.Where(x => x.id == Id).FirstOrDefault();
            string fileName = devis.Devis1;
            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);


            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public FileResult DownloadFacture(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();
            string fileName = essai.Facture;
            string path = Server.MapPath("~/fichiers/") + fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);


            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        public ActionResult EssaisArchiver()
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            ViewBag.Username = Session["adminname"];
            var essai = db.Essai.Include(e => e.Users).Where(e => e.EssaiDelets == true);
            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            return View(essai.ToList());
        }
        public ActionResult RécupérerEssai(Guid Id)
        {
            var essai = db.Essai.Where(x => x.Id == Id).FirstOrDefault();

            if (essai != null)
            {
                essai.EssaiDelets = false;
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult AddDevis(Guid? id)
        {
            ViewBag.username = Session["techname"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Devis devis = db.Devis.Find(id);
            if (devis == null)
            {
                return HttpNotFound();
            }

            ViewBag.id_client = new SelectList(db.Users, "Id", "Fullname", devis.id_client);
            return View(devis);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDevis([Bind(Include = "id , id_client , date_demande , DemandeDevis , DevisDelete, DevisAccepter , NumDevis , Devis , DevieEnvoyer")] Devis devis, HttpPostedFileBase _Devis)
        {
            
            if (ModelState.IsValid)
            {

                if (_Devis != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(_Devis.FileName);
                    string FileExtension = Path.GetExtension(_Devis.FileName);

                    FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss")
                        + FileExtension;

                    string UploadPath1 = "~/fichiers/";
                    devis.Devis1 = FileName;
                    devis.DemandeDevis = true;

                    _Devis.SaveAs(Server.MapPath(UploadPath1 + FileName));
                    devis.DevieEnvoyer = true;
                }


                db.Entry(devis).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListeDevisDemander");

            }
            ViewBag.id_client = new SelectList(db.Users, "Id", "Fullname", devis.id_client);
            return View(devis);
        }



        // POST: Journals/Delete/5        
        public ActionResult DeleteJournal(Guid? id)
        {
            Journal journal = db.Journal.Find(id);
            if (journal.VuClient!=false)
            {
                db.Journal.Remove(journal);
                db.SaveChanges();
                return RedirectToAction("Modification");
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Pas encore vu par client !!!!!!!!!!');</script>");
        }
       

        public ActionResult EssaisDevis(Guid? id)
        {
            if ((bool)Session["admin"] == false)
            { return Redirect("/"); }
            userid = (string)Session["userid"];

            ViewBag.username = Session["Clientname"];

            var cpt = db.Journal.Where(e => e.VuAdmin == false).Count();
            ViewBag.nbrnotif = cpt;
            var essai = db.Essai.Where(e => e.id_devis == id);
            return View(essai.ToList());

        }
        // GET: Devis/Create
        public ActionResult CreateDevis(Essai essai)
        {

            ViewBag.id_client = new SelectList(db.Users.Where(g => g.ProfileId.ToString() == ("6BF5738F-852C-4EA9-8D5C-AAD7F7E4AC89")), "Id", "Fullname", essai.UserId);
            return View();
        }

        // POST: Devis/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDevis([Bind(Include = "id , id_client , date_demande , DemandeDevis , DevisDelete, DevisAccepter , NumDevis , Devis , DevieEnvoyer")] Devis devis, HttpPostedFileBase _Devis)
        {



            devis.id = Guid.NewGuid();
            devis.date_demande = DateTime.Now;
                
                if (_Devis != null)
                {
                    string FileName = Path.GetFileNameWithoutExtension(_Devis.FileName);
                    string FileExtension = Path.GetExtension(_Devis.FileName);

                    FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss")
                        + FileExtension;

                    string UploadPath1 = "~/fichiers/";
                    devis.Devis1 = FileName;
                    devis.DemandeDevis = true;

                    _Devis.SaveAs(Server.MapPath(UploadPath1 + FileName));
                    devis.DevieEnvoyer = true;
                }
                db.Entry(devis).State = EntityState.Added;
                db.SaveChanges();
                return RedirectToAction("ListeDevisDemander");

        }
        // Modifier un essai envoyer

        // GET: Devis/Edit/5
        public ActionResult EditEssaisDevisEnvoyer(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Devis devis = db.Devis.Find(id);
            if (devis == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_client = new SelectList(db.Users, "Id", "Fullname", devis.id_client);
            return View(devis);
        }

        // POST: Devis/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEssaisDevisEnvoyer([Bind(Include = " id,id_client , date_demande , DemandeDevis , DevisDelete, DevisAccepter , NumDevis , Devis , DevieEnvoyer")] Devis devis, HttpPostedFileBase _Devis)
        {

            if (_Devis != null)
            {
                string FileName = Path.GetFileNameWithoutExtension(_Devis.FileName);
                string FileExtension = Path.GetExtension(_Devis.FileName);

                FileName = FileName.Trim() + "_" + DateTime.Now.ToString("dd-mm-ss")
                    + FileExtension;

                string UploadPath1 = "~/fichiers/";
                devis.Devis1 = FileName;

                _Devis.SaveAs(Server.MapPath(UploadPath1 + FileName));
            }
            db.Entry(devis).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ListeDevisEnvoyer");
        }
    }
}
