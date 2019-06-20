using System.Collections.Generic;

public class GroupVertex
{
  public string id {get;set;}
  public string Name {get;set;}
  public List<string> FocusTags {get;set;}
  public string BackgroundPhotoUrl {get;set;}
  public string Description {get;set;}
  public bool IsPublic {get;set;}
    public List<string> Admins { get; set;}
    public List<string> Members { get; set; }

    public GroupVertex()
    {
        FocusTags = new List<string>();
        Admins = new List<string>();
        Members = new List<string>();
    }
}
