using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Model.Services
{
    public class ServiceBase
    {
        protected FacebookEntities db;


        internal ServiceBase()
            : this(new FacebookEntities())
        {
        }

        internal ServiceBase(FacebookEntities db)
        {
            this.db = db;

        }

        private string MakeLinksInString(string sourse)
        {
            if (sourse != null && sourse != "")
            {
                var links = Regex.Matches(sourse, @"(https?\://)?[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(/\S*)?");
                foreach (Match link in links)
                {
                    if (link.Value != "")
                    {
                        if (link.Value.StartsWith("http"))
                        {
                            sourse = sourse.Replace(link.Value, "<a target=\"_blank\" href=\"" + link.Value + "\">" + link.Value + "</a>");
                        }
                        else
                        {
                            sourse = sourse.Replace(link.Value, "<a target=\"_blank\" href=\"http://" + link.Value + "\">" + link.Value + "</a>");
                        }
                    }
                }

            }

            return sourse;
        }

        private void MakeLink(IPost post)
        {
            post.Message = MakeLinksInString(post.Message);
            post.Caption = MakeLinksInString(post.Caption);
            post.Description = MakeLinksInString(post.Description);
        }

        protected bool ValidateNewPost(IPost post)
        {
            MakeLink(post);

            if (post is FacebookPost)
            {
                var newPost = (FacebookPost)post;
                if (newPost.Status_type != "approved_friend"
                            && (newPost.Message != null || newPost.Caption != null || newPost.Description != null)) return true;
            }
            else
            {
                var newPost = (FQLPost)post;
                if (newPost.Type != "8" && (newPost.Link == null || !newPost.Link.Contains("groups")) &&
                    ((newPost.Message != null && newPost.Message != "") || (newPost.Caption != null && newPost.Caption != "") || (newPost.Description != null && newPost.Description != ""))) return true;
            }
            return false;
        }

        protected void ValidateOurPost(IPost post)
        {
            MakeLink(post);

            if (post.Caption == null && post.Message == null && post.Description == null)
            {
                post.Message = "<p style=\"color:red\">Помилка посту. Напевно, він став приватним.</p>";
            }
        }

        public void Podyaka(string UserId, string PostId, string ToUserId, bool IsLike)
        {
            db.PodyakaLog.Add(new PodyakaLog { FromUserId = UserId, ToUserId = ToUserId, PostId = PostId, IsDone = false, IsLike = IsLike, Time = DateTime.Now });
            db.SaveChanges();
        }

        protected IEnumerable<T> GetNext<T>(IEnumerable<T> objects, int from, int increment = 10)
        {
            List<T> result = new List<T>();

            var dbPosts = objects.Take(from + increment).ToArray();

            for (int i = from; i < dbPosts.Count(); i++)
            {
                result.Add(dbPosts[i]);
            }

            return result;
        }

        public double AvtoritetConts
        {
            get
            {
                return double.Parse(db.ServiseData.Single(s => s.Id == 1).Value);
            }
        }
    }
}
