using RecipeSavants.Microservices.GraphRepository.Models;
using System.Collections.Generic;

public class GroupUpdateModel
{
   public GroupUpdateVertex Update {get;set;}
   public List<GroupUpdateCommentVertex> Comments {get;set;}
   
   public GroupUpdateModel()
   {
      Update = new GroupUpdateVertex();
      Comments = new List<GroupUpdateCommentVertex>();
   }
}
