using Microsoft.AspNet.Mvc.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
     public class FacebookSimpleUser
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Link { get;set;} // Не нужно по крайне мере для постов

        [FacebookFieldModifier("height(50).width(50)")]
        public FacebookConnection<FacebookPicture> Picture { get; set; }


        public double Avtoritet;
        public int PositionInAvtoritet;
        public int PotionInVoting;
        public bool IsInPeopleVoting;

        public FacebookSimpleUser(Users dbUser)
        {
            this.Id = dbUser.UserId;
            this.Name = dbUser.Name;
            this.Link = dbUser.Link;
            this.Picture = new FacebookConnection<FacebookPicture>();
            this.Picture.Data = new FacebookPicture();
            this.Picture.Data.Url = dbUser.PictureScr;
        }
        
        public FacebookSimpleUser()
        {

        }
    }
}
