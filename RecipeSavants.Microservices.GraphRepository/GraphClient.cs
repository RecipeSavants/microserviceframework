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
        
        public async Task AddSocialPost(SocialUpdateVertex Update, string User)
        {
            Update.Title == Update.Title ?? "";
            Update.Body == Update.Body ?? "";
            Update.Url == Update.Url ?? new List<string>();
            Update.ImageUrl = Update.ImageUrl ?? new List<string>();
            await client.Add(Update).SubmitAsync();
            var u11 = await client.From<UserVertex>().Where(w => w.id == User).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<SocialUpdateVertex, UserVertex>(u11, Update, "update").BuildGremlinQuery());
        }
        
        public async Task AddUserVertex(UserVertex User)
        {
            UserVertex v = new UserVertex()
            {
                AboutMe = User.AboutMe ?? "",
                Allergies = User.Allergies ?? "",
                Answer = User.Answer ?? "",
                BackgroundUrl = User.BackgroundUrl ?? "",
                City = User.City ?? "",
                CreateDate = DateTime.UtcNow,
                Cusines = User.Cusines ?? "",
                Diets = User.Diets ?? "",
                Email = User.Email ?? "",
                Facebook = User.Facebook ?? "",
                FullName = User.Fullname ?? "",
                id = User.ToLower(),
                Instagram = User.Instagram ?? "",
                IsFacebookLinkVisible = false,
                IsInstagramVisible = false,
                IsPinterestLinkVisible = false,
                IsTwitterLinkVisible = false,
                LastActivityDate = DateTime.UtcNow,
                LastUpdateDate = DateTime.UtcNow,
                Latitude = 0,
                Longitude = 0,
                PhotoPath = User.PhotoPath ?? "",
                Pintrest = User.Pintrest ?? "",
                PostalCode = User.PostalCode ?? "",
                Question = User.Question ?? "",
                State = User.State ?? "",
                Twitter = User.Twitter ?? "",
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
