using Gremlin.Net.CosmosDb.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeSavants.Microservices.GraphRepository.Models
{
    public class QuestionVertex:IVertex
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public List<string> ImageUrl { get; set; }
        public List<AnswerEdge> Answers { get; set; }
    }
}
