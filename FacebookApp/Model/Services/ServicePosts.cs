using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Mvc.Facebook.Client;
using Microsoft.AspNet.Mvc.Facebook;
using Model;
using System.Threading;

namespace Model.Services
{
    public class ServicePosts : ServiceBase
    {
        #region FriendFeed

        private class SessionControler
        {
            private HttpSessionStateBase Session;

            public List<FQLPost> FriendsFeed
            {
                get
                {
                    return (List<FQLPost>)Session["FriendsFeed"];
                }
                set
                {
                    Session["FriendsFeed"] = value;
                }
            }

            public long FriendsFeedUserId
            {
                get
                {
                    return (long)Session["FriendsFeedUserId"];
                }
                set
                {
                    Session["FriendsFeedUserId"] = value;
                }
            }

            public int FriendsFeedPostNumber
            {
                get
                {
                    return (int)Session["FriendsFeedPostNumber"];
                }
                set
                {
                    Session["FriendsFeedPostNumber"] = value;
                }
            }

            public Task FriendsFeedTask
            {
                get
                {
                    return (Task)Session["FriendsFeedTask"];
                }
                set
                {
                    Session["FriendsFeedTask"] = value;
                }
            }

            public void AddFriendsFeed(List<FQLPost> add)
            {
                var feed = FriendsFeed;
                feed.AddRange(add);
                FriendsFeed = feed;
            }

            public void AddFriendsFeed(FQLPost add)
            {
                var feed = FriendsFeed;
                feed.Add(add);
                FriendsFeed = feed;
            }

            public void Clear()
            {
                Session["FriendsFeed"] = new List<FQLPost>();
                Session["FriendsFeedUserId"] = (long)0;
                Session["FriendsFeedPostNumber"] = 10;
            }

            public SessionControler(HttpSessionStateBase Session)
            {
                this.Session = Session;
                if (Session["FriendsFeed"] == null) Session["FriendsFeed"] = new List<FQLPost>();
                if (Session["FriendsFeedUserId"] == null) Session["FriendsFeedUserId"] = (long)0;
                if (Session["FriendsFeedPostNumber"] == null) Session["FriendsFeedPostNumber"] = 10;
            }
        }

        private async Task<bool> GetNextPosts(FacebookContext context, SessionControler Session, int need)
        {
            var user = db.Users.Single(u => u.UserId == context.UserId);
            
            while (Session.FriendsFeed.Count < need)
            {
                var friends = user.UserFriends.Where(uf => long.Parse(uf.FriendId) > Session.FriendsFeedUserId && uf.IsOwerAppUser).OrderBy(uf => long.Parse(uf.FriendId)).ToArray();
                if ((friends.Count() == 0 && Session.FriendsFeedUserId == 0) || Session.FriendsFeedPostNumber == 100) break;
                foreach (var friend in friends)
                {
                    var Posts = (await context.Client.GetTaskAsync<FQLPosts>("fql", new { q = "SELECT post_id,permalink,source_id,message,description,created_time,type,attachment FROM stream WHERE source_id = \"" + friend.FriendId + "\" limit " + Session.FriendsFeedPostNumber })).Data;

                    foreach (var Post in Posts)
                    {
                        if (!Session.FriendsFeed.Any(p => p.Id == Post.Id) && Post != null && ValidateNewPost(Post))
                        {
                            var dbPost = db.Posts.SingleOrDefault(p => p.PostId == Post.Id);
                            if (dbPost == null)
                            {
                                Post.SetAuthor(db.Users.Single(u => u.UserId == Post.Source_id));
                            }
                            else
                            {
                                Post.SetFullInfo(dbPost, context.UserId);
                            }
                            Session.AddFriendsFeed(Post);
                        }
                    }
                    Session.FriendsFeedUserId = long.Parse(friend.FriendId);
                    if (Session.FriendsFeed.Count >= need) return true;
                }

                Session.FriendsFeedPostNumber = Session.FriendsFeedPostNumber + 10;
                Session.FriendsFeedUserId = 0;
            }
            return true;
        }

        public async Task<IEnumerable<FQLPost>> GetFriendsPosts(FacebookContext context, int from, HttpSessionStateBase session)
        {
            IEnumerable<FQLPost> result = new List<FQLPost>();

            SessionControler Session = new SessionControler(session);

            if (from == 0)
            {
                Session.Clear();
                var user = db.Users.Single(u => u.UserId == context.UserId);
                var friends = user.UserFriends.Where(uf => uf.IsOwerAppUser);

                string friensString = " ";
                foreach (var friend in friends)
                {
                    friensString += "\"" + friend.FriendId + "\",";
                }
                friensString = friensString.Remove(friensString.Count() - 1);
                var BasePosts = (await context.Client.GetTaskAsync<FQLPosts>("fql", new { q = "SELECT post_id,permalink,source_id,message,description,created_time,type,attachment FROM stream WHERE source_id IN (" + friensString + ") limit 100" })).Data;
                foreach (var post in BasePosts)
                {
                    if (ValidateNewPost(post))
                    {
                        var dbPost = db.Posts.SingleOrDefault(p=>p.PostId == post.Id);
                        if (dbPost == null)
                        {
                            post.SetAuthor(db.Users.Single(u => u.UserId == post.Source_id));
                        }
                        else
                        {
                            post.SetFullInfo(dbPost, context.UserId);
                        }
                        Session.AddFriendsFeed(post);
                    }
                }

                if (Session.FriendsFeed.Count > 10)
                {
                    result = GetNext<FQLPost>(Session.FriendsFeed, from);
                }
                else
                {
                    await GetNextPosts(context, Session, 10);
                }

            }

            while (Session.FriendsFeed.Count < from + 10)
            {
                if (Session.FriendsFeedTask.Status == TaskStatus.Running)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    break;
                }
            }
            result = GetNext<FQLPost>(Session.FriendsFeed, from);


            Session.FriendsFeedTask = Task.Factory.StartNew(() => {
                var task = GetNextPosts(context, Session, from + 20);
                task.Wait();
            });


            //while (Session.FriendsFeedTask.Status != TaskStatus.RanToCompletion)
            //{
            //    Thread.Sleep(1000);
            //}

            return result;
        }

        #endregion

        public async Task<IEnumerable<FacebookPost>> GetDailyPosts(FacebookContext context, int from)
        {
            List<FacebookPost> result = new List<FacebookPost>();

            DateTime currentDate = new DateTime(2014, DateTime.Now.Month, DateTime.Now.Day) - new TimeSpan(7,0,0,0); //TODO:Когда пойдет поменяь дату

            var dbNeedPost = GetNext(db.Posts.Where(p => p.Podyaki.Any(po => po.Time >= currentDate) && p.UserId != context.UserId).OrderByDescending(p => p.PodyakiCount), from);

            foreach (var dbPost in dbNeedPost)
            {
                var fbPost = await context.Client.GetFacebookObjectAsync<FacebookPost>(dbPost.PostId);
                fbPost.SetFullInfo(dbPost,context.UserId);

                ValidateOurPost(fbPost);

                result.Add(fbPost);
            }

            return result;
        }

        public async Task<IEnumerable<FacebookPost>> GetUserPosts(FacebookContext context, string UserId, int from)
        {
            List<FacebookPost> result = new List<FacebookPost>();

            var User = db.Users.Single(u => u.UserId == UserId);

            var dbNeedPosts = GetNext(User.Posts.OrderByDescending(p => p.PodyakiCount), from);

            foreach (var dbPost in dbNeedPosts)
            {
                var fbPost = await context.Client.GetFacebookObjectAsync<FacebookPost>(dbPost.PostId);
                fbPost.SetFullInfo(dbPost, context.UserId);

                ValidateOurPost(fbPost);

                result.Add(fbPost);
            }

            return result;
        }

        public async Task<IEnumerable<FacebookPost>> GetUserPodyakaPosts(FacebookContext context, int from)
        {
            List<FacebookPost> result = new List<FacebookPost>();

            var User = db.Users.Single(u => u.UserId == context.UserId);

            var dbNeedPodyaki = GetNext<Podyaki>(User.Podyaki.OrderByDescending(p => p.Time), from);

            foreach (var dbPodyaka in dbNeedPodyaki)
            {
                var fbPost = await context.Client.GetFacebookObjectAsync<FacebookPost>(dbPodyaka.PostId);
                fbPost.SetFullInfo(dbPodyaka.Posts, context.UserId);

                ValidateOurPost(fbPost);

                result.Add(fbPost);
            }

            return result;
        }

        public async Task<IEnumerable<FacebookPost>> GetUserRealPosts(FacebookContext context, int from)
        {
            List<FacebookPost> result = new List<FacebookPost>();

            var dbUser = db.Users.Single(u => u.UserId == context.UserId);

            var fbUser = await context.Client.GetCurrentUserAsync<FacebookUserForPosts>();

            var fbPosts = fbUser.Posts.Data.Where(post => ValidateNewPost(post) &&
                post.From.Id == context.UserId &&
                !dbUser.Promote.Any(p => p.PostId == post.Id) &&
                !dbUser.Posts.Any(p => p.PostId == post.Id)&&
                post.Privacy.value == "EVERYONE").OrderBy(post => post.Created_Time);

            var fbNeedPosts = GetNext<FacebookPost>(fbPosts, from);

            foreach (var fbPost in fbNeedPosts) fbPost.From.Picture = fbUser.Picture;

            return fbNeedPosts;
        }

        public async Task<IEnumerable<FacebookPost>> GetRandomPromotePosts(FacebookContext context, int from)
        {
            List<FacebookPost> result = new List<FacebookPost>();
            var topPromotes = GetNext<Promote>(db.Promote.Where(p => p.UserId != context.UserId).OrderByDescending(p => p.Id), from);

            foreach (var promote in topPromotes)
            {
                var fbPost = await context.Client.GetFacebookObjectAsync<FacebookPost>(promote.PostId);
                fbPost.From = await context.Client.GetFacebookObjectAsync<FacebookSimpleUser>(promote.UserId);
                fbPost.PodyakiCount = 0;
                fbPost.IsPodyaka = false;

                ValidateOurPost(fbPost);

                result.Add(fbPost);
            }

            return result;
        }
    }
}
