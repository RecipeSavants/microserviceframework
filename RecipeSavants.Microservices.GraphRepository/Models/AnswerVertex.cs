
using System;

namespace RecipeSavants.Microservices.GraphRepository.Models
{
    public class AnswerVertex
    {
        public string id {get;set;}
        public string Body { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
