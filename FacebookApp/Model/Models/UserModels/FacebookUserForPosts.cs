using Microsoft.AspNet.Mvc.Facebook;
using Newtonsoft.Json;

// Add any fields you want to be saved for each user and specify the field name in the JSON coming back from Facebook
// http://go.microsoft.com/fwlink/?LinkId=273889

namespace Model
{
    public class FacebookUserForPosts
    {
        public string Id { get; set; }

        [FacebookFieldModifier("height(50).width(50)")]
        public FacebookConnection<FacebookPicture> Picture { get; set; }

        public FacebookGroupConnection<FacebookPost> Posts { get; set; }
    }
}
