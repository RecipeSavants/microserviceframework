
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeSavants.Microservices.GraphRepository.Models
{
    public class RecipeVertex
    {
        public string id {get;set;}
        public string RecipeId { get; set; }
        public UserVertex User { get; set; }
        public int Rating { get; set; }
        public DateTime TimeStamp { get; set; }

    }
}
