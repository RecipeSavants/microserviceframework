using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeSavants.Microservices.GraphRepository.Models
{
    public class UpdateModel
    {
        public GroupUpdateVertex Update { get; set;}
        public List<GroupUpdateCommentVertex> Comments { get; set; }
    }
}
