using Gremlin.Linq;
using Microsoft.Extensions.Configuration;
using RecipeSavants.Microservices.GraphRepository.Models;
using System;
using System.Threading.Tasks;
using Gremlin.Linq.Linq;

namespace RecipeSavants.Microservices.GraphRepository
{
    public class GraphClient
    {
        IGraphClient client;
        public GraphClient(string Collection)
        {
            client = new GremlinGraphClient("recipesavantssocialgraph.gremlin.cosmos.azure.com", "users", "folks", "q34qF9Yf5jtfSJLJqNmqNl1JJPxhUwCUThyJvGqou9DcWceCkv4S3sTf4A8ZnaUAQakNqxpAF0qHt14UoXXfkA==");
        }

        public async Task AddUserVertex(string User)
        {
            UserVertex v = new UserVertex()
            {
                AboutMe = "",
                Allergies = "",
                Answer = "",
                BackgroundUrl = "",
                City = "",
                CreateDate = DateTime.UtcNow,
                Cusines = "",
                Diets = "",
                Email = "",
                Facebook = "",
                FullName = "",
                id = User.ToLower(),
                Instagram = "",
                IsFacebookLinkVisible = false,
                IsInstagramVisible = false,
                IsPinterestLinkVisible = false,
                IsTwitterLinkVisible = false,
                LastActivityDate = DateTime.UtcNow,
                LastUpdateDate = DateTime.UtcNow,
                Latitude = 0,
                Longitude = 0,
                PhotoPath = "",
                Pintrest = "",
                PostalCode = "",
                Question = "",
                State = "",
                Twitter = "",
                UserName = User
            };
            await client.Add(v).SubmitAsync();
        }

        public async Task AddUserFriend(string User1, string User2)
        {
            //find the users
            QueryResult<UserVertex> u1, u2;
            var u11 = await client.From<UserVertex>().Where(w => w.id == User1).SubmitWithSingleResultAsync();
            var u22 = await client.From<UserVertex>().Where(w => w.id == User2).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<UserVertex, UserVertex>(u11, u22, "follows").BuildGremlinQuery());
        }
    }
}
