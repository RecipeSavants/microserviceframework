
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeSavants.Microservices.GraphRepository.Models
{
    public class SocialUpdateVertex
    {
        public string id {get;set;}
        public DateTime TimeStamp { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public UpdateType UpdateType { get; set; }
        public List<string> Url { get; set; }
        public List<string> ImageUrl { get; set; }
    }
}
