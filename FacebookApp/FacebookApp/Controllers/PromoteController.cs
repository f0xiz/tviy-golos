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
    public class PromoteController : Controller
    {
        private ServicePosts servicePosts = new ServicePosts();
        private ServicePromote servicePromote = new ServicePromote();

        public async Task<ActionResult> Index(FacebookContext context)
        {
            Session["context"] = context;

            if (ModelState.IsValid)
            {
                var pm = await servicePosts.GetUserRealPosts(context, 0);
                return View(pm);
            }

            return View("Error");
        }

        public async Task<ActionResult> GetNext(FacebookContext context, int from)
        {
            if (context == null) context = (FacebookContext)Session["context"];

            var pm = await servicePosts.GetUserRealPosts(context, from);

            return Json(pm, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Promote(FacebookContext context, string PostId)
        {
            if (context == null) context = (FacebookContext)Session["context"];

            servicePromote.Promote(context.UserId, PostId);

            return Json(1, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> PromoteLink(FacebookContext context, string Link)
        {
            if (context == null) context = (FacebookContext)Session["context"];

            var res= await servicePromote.PromoteLink(context, Link);
            System.Threading.Thread.Sleep(200);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }

}
