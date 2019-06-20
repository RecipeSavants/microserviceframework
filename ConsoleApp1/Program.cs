using RecipeSavants.Microservices.GraphRepository;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            GraphClient g = new GraphClient("users");
            //await g.AddUserVertex(new RecipeSavants.Microservices.GraphRepository.Models.UserVertex() { UserName = "Robert3" });
            //await g.AddUserVertex(new RecipeSavants.Microservices.GraphRepository.Models.UserVertex() { UserName = "Robert4" });
            //await g.AddUserFriend("Robert3", "Robert4");

            //robert1 robert2

            //add new group
            Console.WriteLine(await g.FetchGroups());
           Console.WriteLine(await g.FetchMember("robert2"));
            //var id = await g.AddGroup(new GroupVertex() { Name = "Robert Group" }, "robert1");
            //await g.AddGroupMember(id, "robert3");
            //await g.DeactivateGroupMember(id, "robert3");
            Console.ReadLine();
        }
    }
}
