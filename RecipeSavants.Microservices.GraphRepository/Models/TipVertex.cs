public class TipVertex
{
  public string id {get;set;}
  public string Title {get;set;}
  public string Body {get;set;}
  public List<string> Urls {get;set;}
  public List<string> Images {get;set;}
  public DateTime TimeStamp {get;set;}
  
  public TipVertex()
  {
    Urls = new List<string>();
    Images = new List<string>();
  }
}
