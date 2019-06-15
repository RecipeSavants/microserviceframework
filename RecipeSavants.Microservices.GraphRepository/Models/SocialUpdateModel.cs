using RecipeSavants.Microservices.GraphRepository.Models;
using System.Collections.Generic;

public class SocialUpdateModel
{
   public SocialUpdateVertex Update {get;set;}
   public List<SocialCommentVertex> Comments {get;set;}
   
   public SocialUpdateModel()
   {
      Update = new SocialUpdateVertex();
      Comments = new List<SocialCommentVertex>();
   }
}
