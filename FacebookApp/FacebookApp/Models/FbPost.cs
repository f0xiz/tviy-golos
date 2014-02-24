using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookApp.Models
{
    public class FbPost
    {
        public string Id;
        public string Link;
        public string Message;
        public object[] Actions;

        public object Privacy;
        public string Object_attachment;
       // public string Place;
    }
}