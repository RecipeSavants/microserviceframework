using RecipeSavants.Microservices.GraphRepository;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            GraphClient g = new GraphClient("users");
            var id = await g.AddGroup(new GroupVertex() { Name = "Robert Group" }, "robert1");
            var id2 = await g.AddGroupPost(new GroupUpdateVertex() { Body = "Robert Group Id", GroupId = id }, "robert1");
            var id3 = await g.AddGroupUpdateComment(new GroupUpdateCommentVertex { Body = "Robert Group Robert Group ID Comment" }, id2, "robert1");
            var t = g.HydrateGroupModel(id);
            Console.ReadLine();
        }
    }
}
