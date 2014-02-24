using Microsoft.AspNet.Mvc.Facebook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class UserForMainPage
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [FacebookFieldModifier("height(50).width(50)")]
        public FacebookConnection<FacebookPicture> Picture { get; set; }

        public string Link { get; set; }

        //[JsonIgnore]
        public FacebookGroupConnection<UserFriendForMainPage> Friends { get; set; }
    }
}
