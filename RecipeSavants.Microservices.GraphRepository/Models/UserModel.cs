using RecipeSavants.Microservices.GraphRepository.Models;
using System.Collections.Generic;

public class UserModel
{
   public UserVertex Profile {get;set;}
   public List<SocialUpdateModel> Updates {get;set;}
   public List<RecipeVertex> RatedRecipes {get;set;}
   
   public UserModel()
   {
      Profile = new UserVertex();
      Updates = new List<SocialUpdateModel>();
      RatedRecipes = new List<RecipeVertex>();
   }
}
