using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionEcole.Models;
using System.Web.Script.Serialization;

namespace GestionEcole.Controllers
{

    public class PresenceCoursController : Controller
    {

        private EcoleContainer DB = new EcoleContainer();

        #region Affichage des presences au cours
        //
        // GET: /PresenceCours/

        private int nbElement = 7;
        /// <summary>
        /// Affichage d'une vue selon un certain nombre de presence aux cours par page
        /// </summary>
        /// <param name="numPage">Le numero de page</param>
        /// <returns>La page des cours ordonnees par un certain nombre d'element par pag</returns>
        public ActionResult Index(int numPage = 0)
        {
            List<PresenceCours> LstPresCours = (from pc in DB.PresenceCours
                                                select pc).OrderBy(pc => pc.EtudiantId).Skip(nbElement * numPage).Take(nbElement).ToList();

            ViewBag.CombienDePage = LstPresCours.Count / nbElement;

            return View(LstPresCours);
        }
        #endregion

        #region Detail de presence aux cours
        //
        // GET: /PresenceCours/Details/5
        /// <summary>
        /// Retoure la vue de detail d'une presence au cours
        /// </summary>
        /// <param name="idEtudiant">L'id de l'etudiant</param>
        /// <param name="idCours">L'id d'un cours</param>
        /// <returns>La page de detail d'une presence au cours</returns>
        public ActionResult Details(int idEtudiant, int idCours)
        {
            PresenceCours presCours = (from pc in DB.PresenceCours
                                       where pc.EtudiantId == idEtudiant && pc.CoursId == idCours
                                       select pc).First();

            TempData["idCours"] = idCours;

            return View(presCours);
        }
        #endregion

        #region Creation d'une presence au cours
        //
        // GET: /PresenceCours/Create
        /// <summary>
        /// Affichage de la vue de creation d'une presence au cours
        /// </summary>
        /// <returns>La page de creation d'une presence a un cours </returns>
        public ActionResult Create()
        {

            IEnumerable<Etudiant> Etudiants = DB.Etudiants.ToList().OrderBy(e => e.Nom);

            ViewBag.Etudiants = Etudiants.Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Nom + " " + e.Prenom });

            IEnumerable<Cours> Cours = DB.Cours.ToList().OrderBy(e => e.DateDebut);

            ViewBag.Cours = Cours.Select(e => new SelectListItem { Value = e.CoursId.ToString(), Text = e.DateDebut.ToString() });

            return View();
        }

        //
        // POST: /PresenceCours/Create
        /// <summary>
        /// Creation d'une presence cours dans la base de donnee
        /// </summary>
        /// <param name="NouvPresCours">Un nouveau cours a creer</param>
        /// <returns> La vue Index des Presences cours si la creation est valider sinon la vue de la creation</returns>
        [HttpPost]

        public ActionResult Create(PresenceCours NouvPresCours)
        {
            try
            {
                // TODO: Add insert logic here

                if (ModelState.IsValid)
                {
                    DB.PresenceCours.Add(NouvPresCours);

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

        #region Inscription aux cours

        // inscription Client 
        /// <summary>
        /// Affichage de la page d'inscription d'un etudiant a un cours
        /// </summary>
        /// <param name="etudiantId">l'id de l'etudiant</param>
        /// <returns>La page d'inscription</returns>
        public ActionResult Inscription(int etudiantId)
        {

            var Etudiant = (from e in DB.Etudiants
                            where e.Id == etudiantId
                            select e).First();

            ViewBag.Etudiant = Etudiant;

            var matieres = DB.Matieres.ToList()
                .OrderBy(e => e.Titre).ToList()
                .Select(m => new SelectListItem { Value = m.MatiereId.ToString(), Text = m.Titre.ToString() });

            ViewBag.matieres = matieres;

            return View(new PresenceCours { EtudiantId = etudiantId, Etudiant = Etudiant });

        }

        //
        // POST: /PresenceCours/Create
        /// <summary>
        /// Inscription d'un etudiant a un cours
        /// </summary>
        /// <param name="NouvPresCours">Le nouveau cours a jouter</param>
        /// <returns>La vue Index des etudiants si l'inscription est valider sinon la vue de l'inscription</returns>
        [HttpPost]

        public ActionResult Inscription(PresenceCours NouvPresCours)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    DB.PresenceCours.Add(NouvPresCours);

                    DB.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(NouvPresCours);
            }
        }

        /// <summary>
        /// Selection d'une matiere dans la page inscription: Utilise Ajax pour l'affichage
        /// </summary>
        /// <param name="idSelect">la matiere a selectionner</param>
        /// <returns>une vue partielle des cours</returns>
        [HttpPost]

        public ActionResult SelectionnerMatiere(string idSelect)
        {
            int idMat = Int32.Parse(idSelect);
            IEnumerable<SelectListItem> lesCours = (from m in DB.Matieres
                                                    where m.MatiereId == idMat
                                                    select m).First().
                                                        Cours.ToList().
                                                        Select(c => new SelectListItem
                                                        {
                                                            Value = c.CoursId.ToString(),
                                                            Text = c.DateDebut.ToLongDateString() +
                                                            " a " +
                                                            c.DateDebut.ToLongTimeString() + " fini le " +
                                                            c.DateFin.ToLongDateString() +
                                                            " a " +
                                                            c.DateFin.ToLongTimeString()
                                                        });

            ViewBag.LesCours = lesCours;
            ViewBag.Etudiant = TempData["etudiant"];
            ViewBag.matieres = TempData["matieres"];

            Etudiant etudiant = ViewBag.Etudiant;

            var Etudiant = (from e in DB.Etudiants
                            where e.Id == etudiant.Id
                            select e).First();

            return PartialView("_CoursPartial", new PresenceCours { EtudiantId = Etudiant.Id, Etudiant = Etudiant });
        }
        #endregion

        #region Edition d'une presence au cours
        //
        // GET: /PresenceCours/Edit/5
        /// <summary>
        /// Edition de la presence de cours
        /// </summary>
        /// <param name="idEtudiant">L'id de l'etudiant</param>
        /// <param name="idCours">L'id du cours</param>
        /// <returns>La page d'Edition d'une presence cours</returns>
        public ActionResult Edit(int idEtudiant, int idCours)
        {
            PresenceCours presCours = (from pc in DB.PresenceCours
                                       where pc.EtudiantId == idEtudiant && pc.CoursId == idCours
                                       select pc).First();

            return View(presCours);
        }

        //
        // POST: /PresenceCours/Edit/5
        /// <summary>
        /// Enregistrer une presence au cours dans la base de donnee 
        /// </summary>
        /// <param name="idEtudiant">L'id d'un etudiant</param>
        /// <param name="idCours"></param>
        /// <param name="EditPresCours">La presence au cours a editer</param>
        /// <returns> La vue Index des presences au cours si l'edition est valider sinon la vue de l'edition</returns>
        [HttpPost]

        public ActionResult Edit(int idEtudiant, int idCours, PresenceCours EditPresCours)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    PresenceCours presCours = (from pc in DB.PresenceCours
                                               where pc.EtudiantId == idEtudiant && pc.CoursId == idCours
                                               select pc).First();

                    presCours.Motif = EditPresCours.Motif;
                    presCours.Absence = EditPresCours.Absence;

                    DB.SaveChanges();
                    return RedirectToAction("Index","Enseignant");
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

        #region Suppression d'une  presence au cours
        //
        // GET: /PresenceCours/Delete/5
        /// <summary>
        /// Affichage de la page de suppression d'une presence au cours
        /// </summary>
        /// <param name="idEtudiant">L'id de l'etudiant</param>
        /// <param name="idCours">L'id du cours</param>
        /// <returns>La page de presence au cours</returns>
        public ActionResult Delete(int idEtudiant, int idCours)
        {
            PresenceCours presCours = (from pc in DB.PresenceCours
                                       where pc.EtudiantId == idEtudiant && pc.CoursId == idCours
                                       select pc).First();

            return View(presCours);
        }

        //
        // POST: /PresenceCours/Delete/5
        /// <summary>
        /// Suppression d'une presence cours de la base de donnee
        /// </summary>
        /// <param name="idEtudiant">Id de l'etudiant</param>
        /// <param name="idCours">Id du cours</param>
        /// <param name="collection">le formulaire de la presence au cours</param>
        /// <returns>La vue Index des presences au cours si la suppression est valider sinon la vue de suppression</returns>
        [HttpPost]

        public ActionResult Delete(int idEtudiant, int idCours, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                PresenceCours presCours = (from pc in DB.PresenceCours
                                           where pc.EtudiantId == idEtudiant && pc.CoursId == idCours
                                           select pc).First();

                DB.PresenceCours.Remove(presCours);

                DB.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region Fermer la page
        /// <summary>
        /// Ferme La page presenceCours
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }
        #endregion
    }
}