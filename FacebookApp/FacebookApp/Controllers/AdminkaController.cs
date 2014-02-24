using Microsoft.AspNet.Mvc.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Model;
using Model.Services;

namespace FacebookApp.Controllers
{
    public class AdminkaController : Controller
    {
        private ServiceUsers serviceUsers = new ServiceUsers();
        private ServiceVoting serviceVoting = new ServiceVoting();

        public ActionResult Index(FacebookContext context)
        {
            Session["context"] = context;

            if (ModelState.IsValid && Authorite("Administrator"))
            {
                return View();
            }

            return View("Error");
        }

        public async Task<ActionResult> SetBals(FacebookContext context, string UserId)
        {
            if (!Authorite("Administrator")) return View("Error");

            if (context == null) context = (FacebookContext)Session["context"];

            bool result = await serviceUsers.AdministrationAvtoritetAdd(context, UserId, "");

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult VotingsList()
        {
            if (!Authorite("Administrator")) return View("Error");

            var pm = new List<pmVoting>();

            var dbVotings = serviceVoting.GetAllVotings();

            foreach (var dbVoting in dbVotings)
            {
                pm.Add(new pmVoting { Id = dbVoting.Id, Name = dbVoting.Name, IsLock = dbVoting.IsClosed, IsPersonal = dbVoting.IsPeople });
            }


            return View(pm);
        }


        private bool Authorite(string Roles)
        {
            string UserId = ((FacebookContext)Session["context"]).UserId;

            if (Roles.Contains("Administrator")) return serviceUsers.IsInRole(UserId, "Administrator");
            if (Roles.Contains("VIP")) return serviceUsers.IsInRole(UserId, "VIP");

            return false;
        }
    }
}
