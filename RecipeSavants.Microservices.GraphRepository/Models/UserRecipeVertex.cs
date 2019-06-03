﻿using Gremlin.Net.CosmosDb.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeSavants.Microservices.GraphRepository.Models
{
    public class UserRecipeVertex: ManyToManyEdge<UserVertex,RecipeVertex>
    {
    }
}
