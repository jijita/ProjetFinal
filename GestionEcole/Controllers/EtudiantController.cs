using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionEcole.Models;

namespace GestionEcole.Controllers
{
    public class EtudiantController : Controller
    {
        private EcoleContainer DB = new EcoleContainer();



        #region Affichage de la liste d'etudiant

        private int nbElement = 7;
        //
        // GET: /Etudiant/
        /// <summary>
        /// Affichage d'une vue selon un certain nombre d'etudiants par page
        /// </summary>
        /// <param name="numPage">Le numero de la page</param>
        /// 
        /// <returns>Une liste d'etudiants</returns>
        public ActionResult Index(int numPage = 0)
        {
            var lstEtud = (from e in DB.Etudiants
                           select e).ToList();

            var lstPage = (from e in DB.Etudiants
                           select e).OrderBy(e => e.Id).Skip(nbElement * numPage).Take(nbElement).ToList();


            ViewBag.CombienDePage = lstEtud.Count / nbElement;

            return View(lstPage);
        }
        #endregion

        #region Detail de l'Etudiant
        // GET: /Etudiant/Details/5

        /// <summary>
        /// Affichage du detail des etudiants
        /// </summary>
        /// <param name="id"> id de l'etudiant</param>
        /// <returns> La page de detail d'un etudiant donné</returns>
        /// 
        public ActionResult Details(int id)
        {
            Etudiant etudiant = (from e in DB.Etudiants
                                 where e.Id == id
                                 select e).First();

            // les presences de cours d un etudiant
            var presencesAucours = (from pc in DB.PresenceCours
                                    where pc.EtudiantId == id
                                    select pc).ToList();

            var coursEtudiant = presencesAucours.Select(pc => pc.Cours).ToList();
            var matiereEtudiant = coursEtudiant.Select(mt => mt.Matiere).ToList();

            ViewBag.coursEtudiant = coursEtudiant;

            ViewBag.matiereEtudiant = matiereEtudiant;

            var sommeNotes = etudiant.Evaluations.Sum(e => e.Note);
            var sumPonderation = etudiant.Evaluations.Sum(e => e.TypeEvaluation.Ponderation);


            decimal Moyenne;

            if (sumPonderation != 0)
            {
                Moyenne = (sommeNotes / sumPonderation)*100;

                etudiant.Moyenne = decimal.Round(Moyenne, 2);
                
            }

            return View(etudiant);
        }
        #endregion

        #region Creation de l'Etudiant
        // GET: /Etudiant/Create
        /// <summary>
        /// Retoure la vue de creation d'un etudiant
        /// </summary>
        /// <returns>Vue de creation</returns>
        public ActionResult Create()
        {
            return View();
        }


        // POST: /Etudiant/Create
        /// <summary>
        /// Creation d'un etudiant dans la base de donnee
        /// </summary>
        /// <param name="NouvEtudiant"> Le nouveau Etudiant a creer </param>
        /// <returns>La vue Index des etudiants si la creation est valider sinon la vue de la creation</returns>
        [HttpPost]
        public ActionResult Create(Etudiant NouvEtudiant)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    int c = (from e in DB.Etudiants
                             where e.Email.Equals(NouvEtudiant.Email)
                             select e).Count();

                    if (c > 0)
                    {
                        ModelState.AddModelError("Email", "Email existe deja!");
                    }

                    else
                    {
                        DB.Etudiants.Add(NouvEtudiant);

                        DB.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }

                {
                    return View();
                }

            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region Edition un Etudiant
        // GET: /Etudiant/Edit/5
        /// <summary>
        /// Affichage de l'etudiant a editer
        /// </summary>
        /// <param name="id"> L'id de l'etudiant a editer</param>
        /// <returns> la vue des etudiants</returns>
        /// 
        public ActionResult Edit(int id)
        {
            Etudiant etudiant = (from e in DB.Etudiants
                                 where e.Id == id
                                 select e).First();

            return View(etudiant);
        }


        // POST: /Etudiant/Edit/5
        /// <summary>
        /// Edition d'un etudiant et l'enregistrer dans la base de donnees 
        /// </summary>
        /// <param name="id"> L'id de l'etudiant passer en parametre a editer </param>
        /// <param name="EditEtudiant">L'etudiant editer</param>
        /// <returns> La vue Index des etudiants si l'edition est valider sinon la vue de l'edition</returns>
        [HttpPost]
        public ActionResult Edit(int id, Etudiant EditEtudiant)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    Etudiant etudiant = (from e in DB.Etudiants
                                         where e.Id == id
                                         select e).First();

                    etudiant.Nom = EditEtudiant.Nom;
                    etudiant.Prenom = EditEtudiant.Prenom;
                    etudiant.Email = EditEtudiant.Email;
                    etudiant.DateNaissance = EditEtudiant.DateNaissance;
                    etudiant.Adresse = EditEtudiant.Adresse;
                    etudiant.Photo = EditEtudiant.Photo;

                    DB.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        #endregion

        #region Suppression un Etudiant
        //
        // GET: /Etudiant/Delete/5
        /// <summary>
        ///  Affichage de la vue de suppression d'un etudiant
        /// </summary>
        /// <param name="id"> L'id de l'etudiant a supprimer</param>
        /// <returns> La vue de suppression d'un etudiant</returns>
        public ActionResult Delete(int id)
        {
            Etudiant etudiant = (from e in DB.Etudiants
                                 where e.Id == id
                                 select e).First();

            return View(etudiant);
        }

        //
        // POST: /Etudiant/Delete/5
        /// <summary>
        /// Suppression de la base de donnee d'un etudiant donnee
        /// </summary>
        /// <param name="id">L'id de l'etudiant a supprimer </param>
        /// <param name="collection">Le formulaire d'un etudiant</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                Etudiant etudiant = (from e in DB.Etudiants
                                     where e.Id == id
                                     select e).First();

                DB.Etudiants.Remove(etudiant);
                DB.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region fermeture de la page
        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }
        #endregion

    }
}