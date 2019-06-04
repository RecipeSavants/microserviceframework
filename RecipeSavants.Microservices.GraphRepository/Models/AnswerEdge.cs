using Gremlin.Net.CosmosDb.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeSavants.Microservices.GraphRepository.Models
{
    public class AnswerEdge:IVertex
    {
        public string Answer { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
