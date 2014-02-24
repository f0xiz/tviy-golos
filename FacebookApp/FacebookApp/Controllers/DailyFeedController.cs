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
    public class DailyFeedController : Controller
    {
        //
        // GET: /DailyFeed/

        private ServicePosts servicePosts = new ServicePosts();

        public async Task<ActionResult> Index(FacebookContext context)
        {
            Session["context"] = context;

            if (ModelState.IsValid)
            {
                var pm = await servicePosts.GetDailyPosts(context,0);
                return View(pm);
            }

            return View("Error");
        }

        public async Task<JsonResult> GetNext(FacebookContext context, int from)
        {
            if (context == null) context = (FacebookContext)Session["context"];

            var pm = await servicePosts.GetDailyPosts(context, from);

            return Json(pm,JsonRequestBehavior.AllowGet);
        }

    }
}
