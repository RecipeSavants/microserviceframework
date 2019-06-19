public class GroupUpdteVertex
{
        public string id {get;set;}
        public string GroupId {get;set;}
        public DateTime TimeStamp { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public UpdateType UpdateType { get; set; }
        public List<string> Url { get; set; }
        public List<string> ImageUrl { get; set; }
}
