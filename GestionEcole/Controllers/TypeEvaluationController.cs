using GestionEcole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestionEcole.Controllers
{
    public class TypeEvaluationController : Controller
    {
        private int nbElement = 7;
        private EcoleContainer DB = new EcoleContainer();

        #region Default CRUD

        public ActionResult Index(int numPage = 0)
        {
            var evaluations = (from t in DB.TypeEvaluations
                               select t).OrderBy(evl => evl.TypeEvaluationId).Skip(numPage * nbElement).Take(nbElement).ToList();

            ViewBag.CombienDePage = (from e in DB.TypeEvaluations
                                     select e).Count() / nbElement;

            return View(evaluations);
        }

        public ActionResult Details(int id)
        {
            var typeEvaluation = (from t in DB.TypeEvaluations
                                  where t.TypeEvaluationId == id
                                  select t).First();

            return View(typeEvaluation);
        }

        #region Create Region

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(TypeEvaluation NouvTypeEvaluation)
        {
            try
            {
                DB.TypeEvaluations.Add(NouvTypeEvaluation);

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

        public ActionResult Edit(int id)
        {
            var typeEvaluation = (from t in DB.TypeEvaluations
                                  where t.TypeEvaluationId == id
                                  select t).First();

            return View(typeEvaluation);
        }

        [HttpPost]
        public ActionResult Edit(int id, TypeEvaluation EditTypeEvaluation)
        {
            try
            {
                var typeEvaluation = (from t in DB.TypeEvaluations
                                      where t.TypeEvaluationId == id
                                      select t).First();

                typeEvaluation.Type = EditTypeEvaluation.Type;
                typeEvaluation.Ponderation = EditTypeEvaluation.Ponderation;

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

        public ActionResult Delete(int id)
        {
            var typeEvaluation = (from t in DB.TypeEvaluations
                                  where t.TypeEvaluationId == id
                                  select t).First();

            return View(typeEvaluation);
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var typeEvaluation = (from t in DB.TypeEvaluations
                                      where t.TypeEvaluationId == id
                                      select t).First();

                DB.TypeEvaluations.Remove(typeEvaluation);

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

        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }
    }
}
