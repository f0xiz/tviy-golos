using Microsoft.AspNet.Mvc.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Privacy
    {
        public string value {get;set;}
    }

    public class FacebookPost:IPost
    {
        public Privacy Privacy { get; set; }
        public string Status_type { get; set; }


    }
}