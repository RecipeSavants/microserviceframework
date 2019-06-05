using RecipeSavants.Microservices.GraphRepository;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            GraphClient g = new GraphClient("users");
            await g.AddUserVertex("Robert3");
            //await g.AddUserVertex("Robert2");
            //await g.AddUserFriend("robert1", "robert2");
        }
    }
}
