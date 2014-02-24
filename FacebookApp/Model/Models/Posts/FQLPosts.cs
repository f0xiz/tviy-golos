using Microsoft.AspNet.Mvc.Facebook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class FQLPosts
    {
        public List<FQLPost> Data { get; set; }
    }

    public class FQLAttachment
    {
        public List<FQLMedia> Media { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
    }

    public class FQLMedia
    {
        public string Src { get; set; }
    }

    public class FQLPost : IPost
    {
        [JsonProperty("Post_Id")]
        override public string Id { get; set; }
        [JsonProperty("Permalink")]
        override public string Link { get; set; }


        public string Type { get; set; }
        public string Source_id { get; set; }
        public FQLAttachment Attachment { get; set; }


        [JsonIgnore]
        override public string Description
        {
            get
            {
                if (Attachment != null) return Attachment.Description;
                else return null;
            }
            set
            {
                if (Description != null) Attachment.Description = value;
            }
        }
        [JsonIgnore]
        override public string Picture
        {
            get
            {
                if (Attachment != null && Attachment.Media != null && Attachment.Media.Count > 0 && Attachment.Media[0] != null) return Attachment.Media[0].Src;
                else return null;
            }
            set
            {
                if (Picture != null) Attachment.Media[0].Src = value;
            }
        }
        [JsonIgnore]
        override public string Caption
        {
            get
            {
                if (Attachment != null) return Attachment.Caption;
                else return null;
            }
            set
            {
                if (Caption != null) Attachment.Caption = value;
            }
        }
        [JsonIgnore]
        override public FacebookSimpleUser From { get; set; }
    }
}
