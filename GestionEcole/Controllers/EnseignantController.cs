using GestionEcole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestionEcole.Controllers
{
    public class EnseignantController : Controller
    {
        private int nbElement = 7;
        private EcoleContainer DB = new EcoleContainer();

        #region Default CRUD

        public ActionResult Index(int numPage = 0)
        {
            var enseignants = (from e in DB.Enseignants
                               select e).OrderBy(c => c.Nom).Skip(nbElement * numPage).Take(nbElement).ToList();

            ViewBag.CombienDePage = (from e in DB.Enseignants
                                     select e).ToList().Count / nbElement;

            return View(enseignants);
        }

        public ActionResult Details(int id)
        {
            var enseignant = (from e in DB.Enseignants
                              where e.Id == id
                              select e).First();

            var matieresInEnseignant = enseignant.Cours.Select(i => i.Matiere).Distinct().ToList();
            
            ViewBag.MatieresInEnseignant = matieresInEnseignant;

            return View(enseignant);
        }

        #region Create Region

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Enseignant NouvEnseignant)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DB.Enseignants.Add(NouvEnseignant);
                    DB.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        #endregion

        #region Edit Region

        public ActionResult Edit(int id)
        {
            var enseignant = (from e in DB.Enseignants
                              where e.Id == id
                              select e).First();

            var matieresInEnseignant = enseignant.Cours.Select(i => i.Matiere).Distinct(new Matiere.Comparer()).ToList();

            var otherMatieres = (from m in DB.Matieres
                                 select m).Except(matieresInEnseignant).ToList();

            ViewBag.CanRemove = matieresInEnseignant;
            ViewBag.CanAdd = otherMatieres;

            return View(enseignant);
        }

        [HttpPost]
        public ActionResult Edit(int id, Enseignant EditEnseignant)
        {
            var enseignant = (from e in DB.Enseignants
                              where e.Id == id
                              select e).First();

            if (ModelState.IsValid)
            {
                try
                {
                    enseignant.Nom = EditEnseignant.Nom;
                    enseignant.Prenom = EditEnseignant.Prenom;
                    enseignant.Adresse = EditEnseignant.Adresse;
                    enseignant.DateNaissance = EditEnseignant.DateNaissance;
                    enseignant.Email = EditEnseignant.Email;

                    DB.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch
                {
                    return View(enseignant);
                }
            }
            else
            {
                return View(enseignant);
            }
        }

        #endregion

        #region Delete Region

        public ActionResult Delete(int id)
        {
            var enseignant = (from e in DB.Enseignants
                              where e.Id == id
                              select e).First();

            return View(enseignant);
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var enseignant = (from e in DB.Enseignants
                              where e.Id == id
                              select e).First();

            try
            {
                DB.Enseignants.Remove(enseignant);

                DB.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View(enseignant);
            }
        }

        #endregion

        #endregion

        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }
    }
}
