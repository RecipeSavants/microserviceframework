﻿using Gremlin.Net.CosmosDb.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeSavants.Microservices.GraphRepository.Models
{
    public class UserQuestionVertext : ManyToManyEdge<UserVertex, QuestionVertex>
    {
        public string Id { get; set; }
    }
}
