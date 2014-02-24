using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Model.Services;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Facebook;

namespace FacebookApp.Controllers
{
    public class GlobalRatingController : Controller
    {
        private ServiceUsers serviceUsers = new ServiceUsers();

        public ActionResult Index(FacebookContext context)
        {
            Session["context"] = context;

            if (ModelState.IsValid)
            {
                var pm = serviceUsers.GetUsersForRating(context,0);
                return View(pm);
            }

            return View("Error");
        }

        public ActionResult GetNext(FacebookContext context, int from)
        {
            if (context == null) context = (FacebookContext)Session["context"];

            var pm = serviceUsers.GetUsersForRating(context, from);

            return Json(pm,JsonRequestBehavior.AllowGet);
        }

    }
}
