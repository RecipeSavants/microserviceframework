using System;
using System.Collections.Generic;

public class GroupUpdateCommentVertex
{
    public string id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public List<string> ImageUrl { get; set; }
    public DateTime TimeStamp { get; set; }

    public GroupUpdateCommentVertex()
    {
        ImageUrl = new List<string>();
    }
}
