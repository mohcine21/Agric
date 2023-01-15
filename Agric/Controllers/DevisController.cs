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
    public class DevisController : Controller
    {
        private AgricEntities db = new AgricEntities();

        // GET: Devis
        public ActionResult Index()
        {
            var devis = db.Devis.Include(d => d.Users);
            return View(devis.ToList());
        }

        // GET: Devis/Details/5
        public ActionResult Details(Guid? id)
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
            return View(devis);
        }

        // GET: Devis/Create
        public ActionResult Create()
        {
            ViewBag.id_client = new SelectList(db.Users, "Id", "Fullname");
            return View();
        }

        // POST: Devis/Create
        // Pour vous protéger des attaques par survalidation, activez les propriétés spécifiques auxquelles vous souhaitez vous lier. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,id_client,date_demande,DemandeDevis,DevisDelete,DevisAccepter,NumDevis,Devis1,DevieEnvoyer")] Devis devis)
        {
            if (ModelState.IsValid)
            {
                devis.id = Guid.NewGuid();
                db.Devis.Add(devis);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_client = new SelectList(db.Users, "Id", "Fullname", devis.id_client);
            return View(devis);
        }

        // GET: Devis/Edit/5
        public ActionResult Edit(Guid? id)
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
        public ActionResult Edit([Bind(Include = "id,id_client,date_demande,DemandeDevis,DevisDelete,DevisAccepter,NumDevis,Devis1,DevieEnvoyer")] Devis devis)
        {
            if (ModelState.IsValid)
            {
                db.Entry(devis).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_client = new SelectList(db.Users, "Id", "Fullname", devis.id_client);
            return View(devis);
        }

        // GET: Devis/Delete/5
        public ActionResult Delete(Guid? id)
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
            return View(devis);
        }

        // POST: Devis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Devis devis = db.Devis.Find(id);
            db.Devis.Remove(devis);
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
