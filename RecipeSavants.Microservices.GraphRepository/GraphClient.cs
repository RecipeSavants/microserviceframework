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

        #region private members
        private async Task<List<GroupVertex>> FetchGroups()
        {
            var t =  await client.SubmitAsync<GroupVertex>($"g.V().has('label','GroupVertex')");
            var l = new List<GroupVertex>();
            foreach (var item in t)
            {
                l.Add(item.Entity);
            }
            return l;
        }
        private async Task<QueryResult<UserVertex>> FetchMember(string User)
        {
            var u = await client.SubmitAsync<UserVertex>($"g.V().has('id','{User.ToLower()}')");
            return u.GetEnumerator().Current;
        }

        private async Task<QueryResult<QuestionVertex>> FetchQuestion(string QuestionID)
        {
            var q = await client.SubmitAsync<QuestionVertex>($"g.V().has('label','QuestionVertex').has('id','{QuestionID}')");
            return q.GetEnumerator().Current;
        }

        private async Task<QueryResult<TipVertex>> FetchTip(string TipID)
        {
            var t = await client.SubmitAsync<TipVertex>($"g.V().has('label','tip').has('id','{TipID}')");
            return t.GetEnumerator().Current;
        }

        private async Task<QueryResult<SocialUpdateVertex>> FetchUpdate(string UpdateId)
        {
            var s = await client.SubmitAsync<SocialUpdateVertex>($"g.V().has('label','update').has('id','{UpdateId}')");
            return s.GetEnumerator().Current;
        }

        private async Task<QueryResult<SocialCommentVertex>> FetchComment(string CommentId)
        {
            var s = await client.SubmitAsync<SocialCommentVertex>($"g.V().has('id','{CommentId}')");
            return s.GetEnumerator().Current;
        }

        private async Task<QueryResult<GroupVertex>> FetchGroup(string GroupId)
        {
            var s = await client.SubmitAsync<GroupVertex>($"g.V('{GroupId}')");
            return s.GetEnumerator().Current;
        }

        private async Task<QueryResult<GroupUpdateVertex>> FetchGroupUpdate(string UpdateId)
        {
            var s = await client.SubmitAsync<GroupUpdateVertex>($"g.V().has('label','update').has('id','{UpdateId}')");
            return s.GetEnumerator().Current;
        }

        private async Task<QueryResult<GroupUpdateCommentVertex>> FetchGroupComment(string CommentId)
        {
            var s = await client.SubmitAsync<GroupUpdateCommentVertex>($"g.V().has('label','comment').has('id','{CommentId}')");
            return s.GetEnumerator().Current;
        }

        private async Task<List<GroupUpdateVertex>> FetchAllPostsForGroup(string GroupId)
        {
            var s = await client.SubmitAsync<GroupUpdateVertex>($"g.V().has('label','posts').has('GroupId','{GroupId}')");
            List<GroupUpdateVertex> l = new List<GroupUpdateVertex>();
            foreach (var item in s)
            {
                l.Add(item.Entity);
            }
            return l;
        }
        #endregion

        #region users

        public async Task<string> AddUserVertex(UserVertex User)
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
                PhotoPath = User.PhotoPath ?? "",
                Pintrest = User.Pintrest ?? "",
                PostalCode = User.PostalCode ?? "",
                Question = User.Question ?? "",
                State = User.State ?? "",
                Twitter = User.Twitter ?? "",
                UserName = User.UserName
            };
            await client.Add(v).SubmitAsync();
            return v.id;
        }

        public async Task AddUserFollows(string User1, string User2)
        {
            await client.SubmitAsync($"g.V('{User1.ToLower()}').addE('follows').to(g.V('{User2.ToLower()}')")
        }

        public async Task RemoveUserFollows(string User1, string User2)
        {
            var query = $"g.V('{User1.ToLower()}').outE('follows').where(inV().has('id', '{User2.ToLower()}')).drop()";
            await client.SubmitAsync(query);
        }

        #endregion

        #region Questions / Answers
        public async Task<string> AddAnswer(string QuestionID, string Body, string User)
        {
            var a = new AnswerVertex()
            {
                id = Guid.NewGuid().ToString(),
                Body = Body ?? "",
                TimeStamp = DateTime.UtcNow
            };
            await client.Add(a).SubmitAsync();
            await client.SubmitAsync($"g.V('{QuestionId}').addE('answers').to(g.V('{a.id}'))");
            await client.SubmitAsync($"g.V('{User.ToLower()}').addE('answers').to(g.V('{QuestionID}'))");
            return a.id;
        }

        public async Task<string> AddQuestion(QuestionVertex Question, string User)
        {
            Question.id = Guid.NewGuid().ToString();
            Question.Title = Question.Title ?? "";
            Question.Body = Question.Body ?? "";
            Question.ImageUrl = Question.ImageUrl ?? new List<string>();
            Question.TimeStamp = DateTime.UtcNow;
            await client.Add(Question).SubmitAsync();
            await client.SubmitAsync($"g.V('{User.ToLower()}').addE('asks').to(g.V('{Question.id}'))");
            return Question.id;
        }

        #endregion

        #region Tips
        public async Task<string> AddTip(TipVertex Tip, string User)
        {
            Tip.id = Guid.NewGuid().ToString();
            Tip.Title = Tip.Title ?? "";
            Tip.Body = Tip.Body ?? "";
            await client.Add(Tip).SubmitAsync();
            await client.SubmitAsync($"g.V('{User.ToLower()}').addE('tips').to(g.V('{Tip.id}'))");
            return Tip.id;
        }

        public async Task<string> AddTipComment(SocialCommentVertex comment, string TipId, string User)
        {
            comment.id = Guid.NewGuid().ToString();
            comment.Title = comment.Title ?? "";
            comment.Body = comment.Body ?? "";
            comment.ImageUrl = comment.ImageUrl ?? new List<string>();
            await client.Add(comment).SubmitAsync();
            await client.SubmitAsync($"g.V('{TipId}').addE('comments').to(g.V('{comment.id}'))");
            return comment.id;
        }

        #endregion

        #region Social Updates / Comments

        public async Task<string> AddSocialComment(SocialCommentVertex comment, string SocialId, string User)
        {
            comment.id = Guid.NewGuid().ToString();
            comment.Title = comment.Title ?? "";
            comment.Body = comment.Body ?? "";
            comment.ImageUrl = comment.ImageUrl ?? new List<string>();
            await client.Add(comment).SubmitAsync();
            await client.SubmitAsync($"g.V('{SocialId}').addE('comments').to(g.V('{comment.id}'))");
            return comment.id;
        }

        public async Task SocialPostLike(string PostId, string User)
        {
            await client.SubmitAsync($"g.V('{User.ToLower()}').addE('likes').to(g.V('{PostId}'))");
        }

        public async Task CommentLike(string SocialCommentId, string User)
        {
            await client.SubmitAsync($"g.V('{User.ToLower()}').addE('likes').to(g.V('{SocialCommentId}'))");
        }

        public async Task<string> AddSocialPost(SocialUpdateVertex Update, string User)
        {
            Update.id = Guid.NewGuid().ToString();
            Update.Title = Update.Title ?? "";
            Update.Body = Update.Body ?? "";
            Update.Url = Update.Url ?? new List<string>();
            Update.ImageUrl = Update.ImageUrl ?? new List<string>();
            await client.Add(Update).SubmitAsync();
            return Update.id;
        }



        #endregion

        #region Groups / Updates / Comments
        public async Task<string> AddGroup(GroupVertex Group, string User)
        {
            try
            {
                Group.id = Guid.NewGuid().ToString();
                Group.Name = Group.Name ?? "";
                Group.BackgroundPhotoUrl = Group.BackgroundPhotoUrl ?? "";
                Group.Description = Group.Description ?? "";
                Group.Admins.Add(User);
                await client.Add(Group).SubmitAsync();
                return Group.id;
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public async Task<string> AddGroupPost(GroupUpdateVertex Update, string User)
        {
            Update.id = Guid.NewGuid().ToString();
            Update.Title = Update.Title ?? "";
            Update.Body = Update.Body ?? "";
            Update.Url = Update.Url ?? new List<string>();
            Update.ImageUrl = Update.ImageUrl ?? new List<string>();
            await client.Add(Update).SubmitAsync();
            await client.SubmitAsync($"g.V('{User.ToLower()}').addE('Update').to(g.V('{Update.id}'))");
            await client.SubmitAsync($"g.V('{Update.GroupId}').addE('Update').to(g.V('{Update.id}'))");
            return Update.id;
        }

        public async Task<string> AddGroupUpdateComment(GroupUpdateCommentVertex comment, string GroupUpdateId, string User)
        {
            comment.id = Guid.NewGuid().ToString();
            comment.Title = comment.Title ?? "";
            comment.Body = comment.Body ?? "";
            comment.ImageUrl = comment.ImageUrl ?? new List<string>();
            await client.Add(comment).SubmitAsync();
            await client.SumbitAsync($"g.V('{GroupUpdateId}').addE('comments').to(g.V('{comment.id}'))");
            return comment.id;
        }

        public async Task GroupUpdateCommentLike(string SocialCommentId, string User)
        {
            await client.SubmitAsync($"g.V('{User.ToLower()}').addE('likes').to(g.V('{SocialCommentId}'))");
            await client.SubmitAsync(client.ConnectVerticies<UserVertex, GroupUpdateCommentVertex>(u,c, "likes").BuildGremlinQuery());
        }


        public async Task AddGroupMember(string GroupId, string User)
        {
            await client.SubmitAsync($"g.V('{User.ToLower()}').addE('member').to(g.V('{GroupId}'))");
        }

        public async Task DeactivateGroupMember(string GroupId, string User)
        {
            var gg = await FetchGroup(GroupId);
            await client.SubmitAsync($"g.V().has('id','{gg.Id}').outE('member').where(inV().has('id','{User.ToLower()}')).drop())");
        }


        #endregion 

        public async Task AddRecipeRating(string RecipeId, int Rating, string User)
        {
            if (RecipeId == null)
            {
                throw (new Exception("Empty RecipeID"));
            }
            else
            {
                RecipeVertex r = new RecipeVertex()
                {
                    RecipeId = RecipeId,
                    Rating = Rating,
                    TimeStamp = DateTime.UtcNow,
                    id = Guid.NewGuid().ToString()
                };
                await client.Add(r).SubmitAsync();
                await client.SubmitAsync($"g.V('{User.ToLower()}').addE('rates').to(g.V('{RecipeId}'))");
            }
        }

        public async Task HydrateGroupModel(string GroupId)
        {
            var g = client.SubmitAsync($"g.V('{GroupId}')");
            var u = client.SubmitAsync($"g.V('{GroupId}').outE('Update').inV().hasLabel('GroupUpdateVertex').outE('Comments').inV().hasLabel('GroupUpdateCommentVertex')");
            var u = await FetchAllPostsForGroup(GroupId);
            var model = new GroupModel()
            {
                Group = g.Entity,
                Updates = u
            };
        }
    }
}
