using GestionEcole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestionEcole.Controllers
{
    public class EvaluationController : Controller
    {
        private int nbElement = 7;
        private EcoleContainer DB = new EcoleContainer();

        #region Default CRUD

        public ActionResult Index(int numPage = 0)
        {
            var evaluations = (from e in DB.Evaluations
                               select e).ToList();

            var lstPage = (from e in DB.Evaluations
                               select e).OrderBy(ev => ev.EtudiantId).Skip(numPage * nbElement).Take(nbElement).ToList();

            evaluations.GroupBy(g => g.Etudiant.Id).Select(o => new { State = o.Key, evaluations = o.OrderBy(c => c.Etudiant.Nom).ToList() });
            ViewBag.CombienDePage = (from e in DB.Evaluations
                                     select e).Count() / nbElement;

            return View(lstPage);
        }

        public ActionResult Details(int etudiantId, int matiereId, int enseignantId)
        {
            var evaluation = (from e in DB.Evaluations
                              where e.EtudiantId == etudiantId && e.MatiereId == matiereId && e.EnseignantId == enseignantId
                              select e).First();

            return View(evaluation);
        }

        #region Create Region

        public ActionResult Create()
        {
            List<Etudiant> etudiants = (from e in DB.Etudiants
                                        select e).ToList();

            ViewBag.Etudiants = etudiants.Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Prenom + " " + e.Nom });

            List<Matiere> matieres = (from m in DB.Matieres
                                      select m).ToList();

            ViewBag.Matieres = matieres.Select(m => new SelectListItem { Value = m.MatiereId.ToString(), Text = m.Titre });

            List<Enseignant> enseignants = (from e in DB.Enseignants
                                            select e).ToList();

            ViewBag.Enseignants = enseignants.Select(e => new SelectListItem { Value = e.Id.ToString(), Text = e.Prenom + " " + e.Nom });

            List<TypeEvaluation> typesEvaluation = (from t in DB.TypeEvaluations
                                                    select t).ToList();

            ViewBag.TypesEvaluation = typesEvaluation.Select(e => new SelectListItem { Value = e.TypeEvaluationId.ToString(), Text = e.Type });

            return View();
        }

        [HttpPost]
        public ActionResult Create(Evaluation NouvEvaluation)
        {
            try
            {
                DB.Evaluations.Add(NouvEvaluation);

                DB.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #endregion

        #region Edit Region

        public ActionResult Edit(int etudiantId, int matiereId, int enseignantId)
        {
            var evaluation = (from e in DB.Evaluations
                              where e.EtudiantId == etudiantId && e.MatiereId == matiereId && e.EnseignantId == enseignantId
                              select e).First();

            return View(evaluation);
        }

        [HttpPost]
        public ActionResult Edit(Evaluation EditEvaluation)
        {
            int etudiantId = EditEvaluation.EtudiantId, matiereId = EditEvaluation.MatiereId, enseignantId = EditEvaluation.EnseignantId;

            try
            {
                var evaluation = (from e in DB.Evaluations
                                  where e.EtudiantId == etudiantId && e.MatiereId == matiereId && e.EnseignantId == enseignantId
                                  select e).First();

                evaluation.Note = EditEvaluation.Note;

                DB.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #endregion

        #region Delete Region

        public ActionResult Delete(int etudiantId, int matiereId, int enseignantId)
        {
            var evaluation = (from e in DB.Evaluations
                              where e.EtudiantId == etudiantId && e.MatiereId == matiereId && e.EnseignantId == enseignantId
                              select e).First();

            return View(evaluation);
        }

        [HttpPost]
        public ActionResult Delete(Evaluation evaluationToBeDeleted)
        {
            int etudiantId = evaluationToBeDeleted.EtudiantId, matiereId = evaluationToBeDeleted.MatiereId, enseignantId = evaluationToBeDeleted.EnseignantId;

            try
            {
                var evaluation = (from e in DB.Evaluations
                                  where e.EtudiantId == etudiantId && e.MatiereId == matiereId && e.EnseignantId == enseignantId
                                  select e).First();

                DB.Evaluations.Remove(evaluation);

                DB.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #endregion

        #endregion

        public ActionResult EvaluationsEtudiant(int etudiantId)
        {
            var evaluations = (from e in DB.Evaluations
                               where e.EtudiantId == etudiantId
                               select e).ToList();

            return View(evaluations);
        }

        public ActionResult EvaluationsEnseignantMatiere(int enseignantId, int matiereId)
        {
            var evaluations = (from e in DB.Evaluations
                               where e.EnseignantId == enseignantId && e.MatiereId == matiereId
                               select e).ToList();

            return View(evaluations);
        }

        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }
    }
}
