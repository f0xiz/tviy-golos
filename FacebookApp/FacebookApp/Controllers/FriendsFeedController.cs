using Microsoft.AspNet.Mvc.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Mvc.Facebook.Client;
using Model;
using Model.Services;
using System.Diagnostics;

namespace FacebookApp.Controllers
{
    public class FriendsFeedController : Controller
    {
        private ServicePosts servicePosts = new ServicePosts();

        public class pmIndex
        {
            public IEnumerable<FacebookPost> promotePosts { get; set; }
            public IEnumerable<FQLPost> friendsFeed { get; set; }
        }

        public async Task<ActionResult> Index(FacebookContext context)
        {
            Session["context"] = context;

            if (ModelState.IsValid)
            {

                var pm = new pmIndex();
                pm.promotePosts = await servicePosts.GetRandomPromotePosts(context,0);
                pm.friendsFeed = await servicePosts.GetFriendsPosts(context, 0,Session);
                return View(pm);
            }

            return View("Error");
        }

        public async Task<ActionResult> GetNextFeed(FacebookContext context, int from)
        {
            if (context == null) context = (FacebookContext)Session["context"];

            var pm = await servicePosts.GetFriendsPosts(context, from,Session);

            return Json(pm, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetNextPromote(FacebookContext context, int from)
        {
            if (context == null) context = (FacebookContext)Session["context"];

            var pm = await servicePosts.GetRandomPromotePosts(context, from);

            return Json(pm, JsonRequestBehavior.AllowGet);
        }
        
        public async Task<ActionResult> Podyaka(FacebookContext context, string PostId, bool IsLike)
        {
            if (context == null) context = (FacebookContext)Session["context"];
            string FromUserId = context.UserId;

            var post = await context.Client.GetFacebookObjectAsync<FacebookPost>(PostId);

            string ToUserId = post.From.Id;

            servicePosts.Podyaka(FromUserId, PostId, ToUserId, IsLike);

            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}
