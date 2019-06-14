using System;
using System.Collections.Generic;

namespace RecipeSavants.Microservices.GraphRepository.Models
{
    public class SocialCommentVertex
    {
        public string id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public List<string> ImageUrl { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
