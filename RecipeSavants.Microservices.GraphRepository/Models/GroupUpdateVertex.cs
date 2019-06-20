using RecipeSavants.Microservices.GraphRepository.Models;
using System;
using System.Collections.Generic;

public class GroupUpdateVertex
{
        public string id {get;set;}
        public string GroupId {get;set;}
        public DateTime TimeStamp { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public UpdateType UpdateType { get; set; }
        public List<string> Url { get; set; }
        public List<string> ImageUrl { get; set; }

    public GroupUpdateVertex()
    {
        UpdateType = new UpdateType();
        Url = new List<string>();
        ImageUrl = new List<string>();
    }
}
