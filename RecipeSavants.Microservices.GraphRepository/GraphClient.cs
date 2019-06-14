using Gremlin.Linq;
using Microsoft.Extensions.Configuration;
using RecipeSavants.Microservices.GraphRepository.Models;
using System;
using System.Threading.Tasks;
using Gremlin.Linq.Linq;
using System.Collections.Generic;

namespace RecipeSavants.Microservices.GraphRepository
{
    public class GraphClient
    {
        IGraphClient client;
        public GraphClient(string Collection)
        {
            client = new GremlinGraphClient("recipesavantssocialgraph.gremlin.cosmos.azure.com", "users", "folks", "q34qF9Yf5jtfSJLJqNmqNl1JJPxhUwCUThyJvGqou9DcWceCkv4S3sTf4A8ZnaUAQakNqxpAF0qHt14UoXXfkA==");
        }
        
        public async Task AddAnswer(string QuestionID, string Body, string User)
        {
            var a = new AnswerVertex() {
                Body = Body ?? "", TimeStamp = DateTime.UtcNow
            };
            await client.Add(a).SubmitAsync();
            var u11 = await client.From<UserVertex>().Where(w => w.id == User).SubmitWithSingleResultAsync();
            var q = await client.From<QuestionVertex>().Where(w=>w.id==QuestionID).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<QuestionVertex, AnswerVertex>(q,a, "answer").BuildGremlinQuery());
            await client.SubmitAsync(client.ConnectVerticies<UserVertex, AnswerVertex>(u11, a, "answers").BuildGremlinQuery());
        }
        
        public async Task AddQuestion(QuestionVertex Question, string User)
        {
            Question.id = Guid.NewGuid().ToString();
            Question.Title = Question.Title ?? "";
            Question.Body = Question.Body ?? "";
            Question.ImageUrl = Question.ImageUrl ?? new List<string>();
            Question.TimeStamp = DateTime.UtcNow;
            await client.Add(Question).SubmitAsync();
            var u11 = await client.From<UserVertex>().Where(w => w.id == User).SubmitWithSingleResultAsync();
            var q = await client.From<QuestionVertex>().Where(w => w.id == Question.id).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<QuestionVertex, UserVertex>(q, u11, "asks").BuildGremlinQuery());
        }
        
        public async Task AddRecipeRating(string RecipeId, int Rating, string User)
        {
            if(RecipeId == null)
            {
                throw(new Exception("Empty RecipeID"));
            }
            else
            {
                RecipeVertex r = new RecipeVertex() {
                    RecipeId = RecipeId, Rating = Rating, TimeStamp = DateTime.UtcNow, id = Guid.NewGuid().ToString()
                };
                await client.Add(r).SubmitAsync();
                var u11 = await client.From<UserVertex>().Where(w => w.id == User).SubmitWithSingleResultAsync();
                var recipe = await client.From<RecipeVertex>().Where(w => w.id == r.id).SubmitWithSingleResultAsync();
                await client.SubmitAsync(client.ConnectVerticies<RecipeVertex, UserVertex>(recipe,u11, "rating").BuildGremlinQuery());
            }
        }

        public async Task AddSocialComment(SocialCommentVertex comment, string SocialId, string User)
        {
            comment.id = Guid.NewGuid().ToString();
            comment.Title = comment.Title ?? "";
            comment.Body = comment.Body ?? "";
            comment.ImageUrl = comment.ImageUrl ?? new List<string>();
            await client.Add(comment).SubmitAsync();
            var s = await client.From<SocialUpdateVertex>().Where(w => w.id == SocialId).SubmitWithSingleResultAsync();
            var u = await client.From<UserVertex>().Where(w => w.id == User).SubmitWithSingleResultAsync();
            var c = await client.From<SocialCommentVertex>().Where(w => w.id == comment.id).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<SocialCommentVertex, SocialUpdateVertex>(c,s, "comment").BuildGremlinQuery());
            await client.SubmitAsync(client.ConnectVerticies<SocialCommentVertex, UserVertex>(c, u, "socialcomment").BuildGremlinQuery());
        }

        public async Task CommentLike(string SocialCommentId, string User)
        {
            var u = await client.From<UserVertex>().Where(w => w.id == User).SubmitWithSingleResultAsync();
            var c = await client.From<SocialCommentVertex>().Where(w => w.id == SocialCommentId).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<SocialCommentVertex, UserVertex>(c, u, "like").BuildGremlinQuery());

        }

        public async Task SocialPostLike(string PostId, string User)
        {
            var u = await client.From<UserVertex>().Where(w => w.id == User).SubmitWithSingleResultAsync();
            var c = await client.From<SocialUpdateVertex>().Where(w => w.id == PostId).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<SocialUpdateVertex, UserVertex>(c, u, "like").BuildGremlinQuery());
        }
        
        public async Task AddSocialPost(SocialUpdateVertex Update, string User)
        {
            Update.id = Guid.NewGuid().ToString();
            Update.Title = Update.Title ?? "";
            Update.Body = Update.Body ?? "";
            Update.Url = Update.Url ?? new List<string>();
            Update.ImageUrl = Update.ImageUrl ?? new List<string>();
            await client.Add(Update).SubmitAsync();
            var social = await client.From<SocialUpdateVertex>().Where(w => w.id == Update.id).SubmitWithSingleResultAsync();
            var u11 = await client.From<UserVertex>().Where(w => w.id == User).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<SocialUpdateVertex, UserVertex>(social,u11, "update").BuildGremlinQuery());
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
                FullName = User.FullName ?? "",
                id = User.UserName.ToLower(),
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
                UserName = User.UserName
            };
            await client.Add(v).SubmitAsync();
        }

        public async Task AddUserFriend(string User1, string User2)
        {
            //find the users
            var u11 = await client.From<UserVertex>().Where(w => w.id == User1).SubmitWithSingleResultAsync();
            var u22 = await client.From<UserVertex>().Where(w => w.id == User2).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<UserVertex, UserVertex>(u11, u22, "follows").BuildGremlinQuery());
        }
    }
}
