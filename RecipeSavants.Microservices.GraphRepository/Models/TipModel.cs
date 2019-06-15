using RecipeSavants.Microservices.GraphRepository.Models;
using System.Collections.Generic;

public class TipModel
{
  public TipVertex Tip {get;set;}
  public List<SocialCommentVertex> Comments {get;set;}
  
  public TipModel()
  {
    Tip = new TipVertex();
    Comments = new List<SocialCommentVertex>();
  }
}
