using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Mvc.Facebook;
using Microsoft.AspNet.Mvc.Facebook.Client;
using Model;
using Model.Services;
using System.Threading;

namespace FacebookApp.Controllers
{
    public class HomeController : Controller
    {
        private ServiceUsers serviceUsers = new ServiceUsers();
        private ServicePromote servicePromote = new ServicePromote();

        [FacebookAuthorize("read_stream", "export_stream", "user_friends")]
        public async Task<ActionResult> Index(FacebookContext context)
        {
            if (ModelState.IsValid)
            {
                var Posts = (await context.Client.GetTaskAsync<FQLPosts>("fql", new { q = "SELECT post_id,permalink,source_id,message,description,created_time,type,attachment FROM stream WHERE source_id = \"" + 100001898002905 + "\" limit " + 20 })).Data;
                //await serviceUsers.UpdateAllUsersInfo(context);             
                var user = await context.Client.GetCurrentUserAsync<UserForMainPage>();
                serviceUsers.AddUser(user);
                return View(user);
            }

            return View("Error");
        }

        // This action will handle the redirects from FacebookAuthorizeFilter when 
        // the app doesn't have all the required permissions specified in the FacebookAuthorizeAttribute.
        // The path to this action is defined under appSettings (in Web.config) with the key 'Facebook:AuthorizationRedirectPath'.
        public ActionResult Permissions(FacebookRedirectContext context)
        {
            if (ModelState.IsValid)
            {
                return View(context);
            }

            return View("Error");
        }

        public ActionResult Error()
        {
            return View("Error");
        }
    }
}
