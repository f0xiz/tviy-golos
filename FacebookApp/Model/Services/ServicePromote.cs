using Microsoft.AspNet.Mvc.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Facebook.Client;

namespace Model.Services
{
    public class ServicePromote : ServiceBase
    {
        public void Promote(string UserId, string PostId)
        {
            db.Promote.Add(new Promote { UserId = UserId, PostId = PostId });
            db.SaveChanges();
        }

        public async Task<int> PromoteLink(FacebookContext context, string Link)
        {
            var dbUser = db.Users.Single(u => u.UserId == context.UserId);

            string postId = Regex.Match(Link, @"(?<=story_fbid=)\d+").Value;
            string userId = Regex.Match(Link, @"(?<=&id=)\d+").Value;


            if (postId == "" || userId == "")
            {
                postId = Regex.Match(Link, @"(?<=\/)\d+").Value;
                string userName = Regex.Match(Link, @"(?<=com\/).+(?=\/posts)").Value;
                var fbUser = await context.Client.GetFacebookObjectAsync<FacebookSimpleUser>(userName);
                userId = fbUser.Id;
            }

            if (postId != "" && userId != "")
            {
                if (!db.Users.Any(u => u.UserId == userId)) return 2;

                var fbPost = await context.Client.GetFacebookObjectAsync<FacebookPost>(userId + "_" + postId);

                if (!ValidateNewPost(fbPost)) return 3;

                Podyaka(context.UserId, fbPost.Id, userId, true);
                return 0;
            }

            return 1;
        }
    }
}
