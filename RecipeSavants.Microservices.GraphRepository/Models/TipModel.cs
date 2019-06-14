public TipModel
{
  public TipVertex Tip {get;set;}
  public List<SocialCommmentVertex> Comments {get;set;}
  
  public TipModel()
  {
    Tip = new TipVertex();
    Comments = new List<SocialCommentVertex>();
  }
}
