using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GestionEcole.Controllers
{
    public class AdministrationController : Controller
    {
        private int nbElement = 7;

        public ActionResult Index(int numPage = 0)
        {
            var roles = Roles.GetAllRoles().OrderBy(o => o).Skip(nbElement * numPage).Take(nbElement).ToList();

            ViewBag.CombienDePage = Roles.GetAllRoles().Length / nbElement;
            ViewBag.Title = "Index";

            return View(roles);
        }

        public ActionResult DeleteRole(string role)
        {
            if (!role.Equals("Admin"))
            {
                try
                {
                    Roles.DeleteRole(role);
                }
                catch
                {

                }
            }

            return RedirectToAction("Index");
        }

        #region Create a Role

        public ActionResult AddRole()
        {
            return View();
        }

        [HttpPost, ActionName("AddRole")]
        public ActionResult ConfirmAdd(FormCollection collection)
        {
            var role = collection["role"];

            Roles.CreateRole(role);

            return RedirectToAction("Index");
        }

        #endregion

        #region DetailsRole View Region

        public ActionResult DetailsRole(string role)
        {
            List<string> total = new List<string>();

            foreach (var r in Roles.GetAllRoles())
            {
                string[] them = Roles.GetUsersInRole(r);

                total.AddRange(them);
            }

            var newTotal = total.Distinct();

            ViewBag.AllUsers = newTotal;
            ViewBag.Role = role;

            return View();
        }

        [HttpPost]
        public ActionResult UsersInRole(string role)
        {
            var users = Roles.GetUsersInRole(role);

            ViewBag.Role = role;

            return PartialView("_UsersInRole", users);
        }

        [HttpPost]
        public ActionResult DeleteUserFromRole(string leUser, string leRole)
        {
            if (!(leRole.Equals("Admin") && leUser.Equals("Admin")))            
            {
                Roles.RemoveUserFromRole(leUser, leRole);
            }

            return UsersInRole(leRole);           
        }

        [HttpPost]
        public ActionResult AjouterUserAuRole(FormCollection collection)
        {
            var user = collection["utilisateur"];
            var role = collection["leRole"];

            var them = Roles.GetUsersInRole(role as string);

            if (!them.Contains(user as string))
            {
                Roles.AddUserToRole(user as string, role as string);
            }

            return UsersInRole(role); 
        }

        #endregion
    }
}
