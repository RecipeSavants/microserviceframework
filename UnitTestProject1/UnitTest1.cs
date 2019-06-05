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
            await g.AddUserVertex("Robert1");
            await g.AddUserVertex("Robert2");
            await g.AddUserFriend("Robert1", "Robert2");
        }
    }
}
