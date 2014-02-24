using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model;
using Model.Services;
using Microsoft.AspNet.Mvc.Facebook;
using System.Threading.Tasks;

namespace FacebookApp.Controllers
{
    public class UserInfoController : Controller
    {
        private ServiceUsers serviceUsers = new ServiceUsers();
        private ServicePosts servicePosts = new ServicePosts();
        private ServiceVoting serviceVoting = new ServiceVoting();

        public class pmPersonalInfo
        {
            public FacebookSimpleUser basicInfo { get; set; }

            public IEnumerable<FacebookPost> personalPosts { get; set; }

            public IEnumerable<FacebookPost> votetPosts { get; set; }
        }

        public async Task<ActionResult> Index(FacebookContext context, string UserId=null)
        {
            Session["context"] = context;

            if (ModelState.IsValid)
            {
                if (UserId == null) UserId = context.UserId;
                var pm = new pmPersonalInfo();

                pm.basicInfo = serviceUsers.GetUserInfo(context, UserId);
                pm.personalPosts = await servicePosts.GetUserPosts(context, UserId, 0);
                pm.votetPosts = await servicePosts.GetUserPodyakaPosts(context, 0);

                if (context.UserId == UserId) return View(pm);
                else return View("OtherUser", pm);
            }

            return View("Error");      
        }
      

        public async Task<ActionResult> GetNextUserPosts(FacebookContext context, int from, string UserId)
        {
            if (context == null) context = (FacebookContext)Session["context"];

            var pm = await servicePosts.GetUserPosts(context, UserId, from);

            return Json(pm, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetNextUserPodyakaPosts(FacebookContext context, int from)
        {
            if (context == null) context = (FacebookContext)Session["context"];

            var pm = await servicePosts.GetUserPodyakaPosts(context, from);

            return Json(pm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetUserToVoteList(FacebookContext context, bool IsSet)
        {
            if (context == null) context = (FacebookContext)Session["context"];

            var pm =  serviceVoting.ChangePersonVotingStatus(context.UserId, IsSet);
            return Json(pm, JsonRequestBehavior.AllowGet);
        }

    }
}
