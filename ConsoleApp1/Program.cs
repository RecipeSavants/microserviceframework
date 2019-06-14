using RecipeSavants.Microservices.GraphRepository;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            GraphClient g = new GraphClient("users");
            await g.AddUserVertex(new RecipeSavants.Microservices.GraphRepository.Models.UserVertex() { UserName = "Robert3" });
            await g.AddUserVertex(new RecipeSavants.Microservices.GraphRepository.Models.UserVertex() { UserName = "Robert4" });
            await g.AddUserFriend("Robert3", "Robert4");
        }
    }
}
