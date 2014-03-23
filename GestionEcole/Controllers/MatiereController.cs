using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestionEcole.Models;

namespace GestionEcole.Controllers
{
    public class MatiereController : Controller
    {
        private EcoleContainer DB = new EcoleContainer();


        #region Affichage de la liste des matieres

        private int nbElement = 7;
        //
        // GET: /Matiere/
        /// <summary>
        ///Affichage d'une vue selon un certain nombre de matiere par page
        /// </summary>
        /// <param name="numPage">Le numero de la page</param>
        /// <returns>une Liste de matiere</returns>
        public ActionResult Index(int numPage = 0)
        {
            List<Matiere> LstMatieres = (from m in DB.Matieres
                                         select m).ToList();

            List<Matiere> LstPage = (from m in DB.Matieres
                                     select m).OrderBy(m => m.MatiereId)
                                         .Skip(numPage * nbElement).
                                         Take(nbElement).ToList();

            //int MoitierPage = (Int32)((LstMatieres.Count / nbElement) / 2);


            ViewBag.CombienDePage = LstMatieres.Count / nbElement;
            //ViewBag.MoitierPages = MoitierPage;


            return View(LstPage);
        }

        #endregion

        #region Detail de la matiere
        //
        // GET: /Matiere/Details/5
        /// <summary>
        /// Afficher les details d'une matiere donnee
        /// </summary>
        /// <param name="id"> l'id d'une matiere donnee</param>
        /// <returns>La vue de detail d'une matiere</returns>

        public ActionResult Details(int id, int enseignantId = 0)
        {
            Matiere matiere = (from m in DB.Matieres
                               where m.MatiereId == id
                               select m).First();

          
            if (enseignantId != 0)
            {
                List<Cours> lstCours = (from e in DB.Enseignants
                            where e.Id == enseignantId
                            select e).First().Cours.ToList();

                List<Cours> lstCoursInMatiere = (from m in lstCours
                                     where m.MatiereId == id
                                     select m).ToList();

                ViewBag.lstCoursInMatiere = lstCoursInMatiere;
            }

            return View(matiere);
        }
        #endregion

        #region Creation d'une matiere
        //
        // GET: /Matiere/Create
        /// <summary>
        /// Affiche la page de creation de matiere
        /// </summary>
        /// <returns> La vue de creation de matiere</returns>
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Matiere/Create
        /// <summary>
        /// Creation de la matiere dans la base de donnee
        /// </summary>
        /// <param name="NouvMatiere"> La matiere a creer</param>
        /// <returns>la vue Index des matieres si la creation est valider sinon la vue de la creation</returns>
        [HttpPost]
        public ActionResult Create(Matiere NouvMatiere)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    DB.Matieres.Add(NouvMatiere);

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

        #region Edition d'une matiere
        //
        // GET: /Matiere/Edit/5
        /// <summary>
        /// Edition d'une matiere donnee
        /// </summary>
        /// <param name="id">L'id de la matiere a editer</param>
        /// <returns>Affichage de la vue de l'edition</returns>
        public ActionResult Edit(int id)
        {
            Matiere matiere = (from m in DB.Matieres
                               where m.MatiereId == id
                               select m).First();

            return View(matiere);
        }

        //
        // POST: /Matiere/Edit/5
        /// <summary>
        /// Edition d'une matiere et l'enregistrer dans la base de donnees 
        /// </summary>
        /// <param name="id">L'id de la matiere a editer</param>
        /// <param name="EditMatiere">La matiere Editee</param>
        /// <returns>La vue Index des matieres si l'edition est validee sinon la vue de l'edition</returns>
        [HttpPost]
        public ActionResult Edit(int id, Matiere EditMatiere)
        {
            try
            {
                // TODO: Add update logic here
                if (ModelState.IsValid)
                {
                    Matiere matiere = (from m in DB.Matieres
                                       where m.MatiereId == id
                                       select m).First();

                    matiere.Titre = EditMatiere.Titre;
                    matiere.Description = EditMatiere.Description;

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

        #region Suppresssion d'une matiere
        //
        // GET: /Matiere/Delete/5
        /// <summary>
        /// Affichage de la vue de suppression d'une matiere
        /// </summary>
        /// <param name="id">L'id de la matiere a supprimer</param>
        /// <returns>La vue de suppression d'une matiere</returns>
        public ActionResult Delete(int id)
        {
            Matiere matiere = (from m in DB.Matieres
                               where m.MatiereId == id
                               select m).First();

            return View(matiere);
        }

        //
        // POST: /Matiere/Delete/5
        /// <summary>
        /// Suppression de la base de donnee de d'une matiere donnee
        /// </summary>
        /// <param name="id">L'id de la matiere a supprimer</param>
        /// <param name="collection">Le formulaire d'une matiere</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                Matiere matiere = (from m in DB.Matieres
                                   where m.MatiereId == id
                                   select m).First();

                DB.Matieres.Remove(matiere);

                DB.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region fermeture de page
        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }
        #endregion
    }
}