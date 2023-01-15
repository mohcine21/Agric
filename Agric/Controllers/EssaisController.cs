using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Agric.Models;

namespace Agric.Controllers
{
    public class EssaisController : Controller
    {
        private AgricEntities db = new AgricEntities();

        // GET: Essais
        public ActionResult Index()
        {
            var essai = db.Essai.Include(e => e.Users);
            return View(essai.ToList());
        }

        // GET: Essais/Details/5
        public ActionResult Details(Guid? id)
        {
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

        // GET: Essais/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "Fullname");
            return View();
        }

        // POST: Essais/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,CodeClient,TA,Formulation,DP,MA,DateNotation,PE,Usage,PR,Produit,Culture,Cible,Nb,Region,Code,Date_Cloture,Rapport,RapportRemis,DateRemisRapport,ACB,FDS,LaboName,TechName,FNNotation,FDEFicheEssai,EssaiDelets,DPRéf,FormulationProduitRéf,Devis,EtatEssai,DevisDemander,EtatPaiment,Paiment_Regler100,Paiment_Regler50,Payer,NonPayer,Payer50,Facture,MARéf,Date_Modife,id_devis,Numdevie,nbr,ComportementCulture,Date_Instalation")] Essai essai)
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

        // GET: Essais/Edit/5
        public ActionResult Edit(Guid? id)
        {
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

        // POST: Essais/Edit/5
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,CodeClient,TA,Formulation,DP,MA,DateNotation,PE,Usage,PR,Produit,Culture,Cible,Nb,Region,Code,Date_Cloture,Rapport,RapportRemis,DateRemisRapport,ACB,FDS,LaboName,TechName,FNNotation,FDEFicheEssai,EssaiDelets,DPRéf,FormulationProduitRéf,Devis,EtatEssai,DevisDemander,EtatPaiment,Paiment_Regler100,Paiment_Regler50,Payer,NonPayer,Payer50,Facture,MARéf,Date_Modife,id_devis,Numdevie,nbr,ComportementCulture,Date_Instalation")] Essai essai)
        {
            if (ModelState.IsValid)
            {
                db.Entry(essai).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Fullname", essai.UserId);
            return View(essai);
        }

        // GET: Essais/Delete/
        public ActionResult Delete(Guid? id)
        {
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

        // POST: Essais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Essai essai = db.Essai.Find(id);
            db.Essai.Remove(essai);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
