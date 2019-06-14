using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecipeSavants.Microservices.GraphRepository;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            GraphClient g = new GraphClient("users");
            await g.AddUserVertex(new RecipeSavants.Microservices.GraphRepository.Models.UserVertex() { UserName = "Robert3" });
            await g.AddUserVertex(new RecipeSavants.Microservices.GraphRepository.Models.UserVertex() { UserName = "Robert4" });
            await g.AddUserFriend("Robert3", "Robert4");
        }
    }
}
