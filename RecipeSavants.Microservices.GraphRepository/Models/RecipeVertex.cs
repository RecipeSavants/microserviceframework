using Gremlin.Net.CosmosDb.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeSavants.Microservices.GraphRepository.Models
{
    public class RecipeVertex:IVertex
    {
        public string RecipeId { get; set; }
        public UserVertex User { get; set; }
        public int Rating { get; set; }
    }
}
