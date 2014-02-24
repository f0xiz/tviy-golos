using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public abstract class IPost
    {
        virtual public string Id { get; set; }
        virtual public string Message { get; set; }
        virtual public string Link { get; set; }
        virtual public string Created_Time { get; set; }
        virtual public string Description { get; set; }

        virtual public string Picture { get; set; }
        virtual public string Caption { get; set; }

        public bool IsPodyaka;
        public int PodyakiCount;

        virtual public FacebookSimpleUser From { get; set; }

        public void SetAuthor(Users dbUser)
        {
            From = new FacebookSimpleUser(dbUser);
        }

        public void SetFullInfo(Posts dbPost, string currentUserId)
        {
            SetAuthor(dbPost.Users);
            PodyakiCount = dbPost.PodyakiCount;
            IsPodyaka = dbPost.Podyaki.Any(p => p.UserId == currentUserId);
        }
    }
}
