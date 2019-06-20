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

        #endregion

        #region users

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

        public async Task AddUserFollows(string User1, string User2)
        {
            //find the users
            var u1 = await FetchMember(User1);
            var u2 = await FetchMember(User2);
            await client.SubmitAsync(client.ConnectVerticies<UserVertex, UserVertex>(u1, u2, "follows").BuildGremlinQuery());
        }

        public async Task RemoveUserFollows(string User1, string User2)
        {
            var query = $"g.V('{User1.ToLower()}').outE('follows').where(inV().has('id', '{User2.ToLower()}')).drop()";
            await client.SubmitAsync(query);
        }

        #endregion

        #region Questions / Answers
        public async Task AddAnswer(string QuestionID, string Body, string User)
        {
            var a = new AnswerVertex()
            {
                Body = Body ?? "",
                TimeStamp = DateTime.UtcNow
            };
            await client.Add(a).SubmitAsync();
            var u = await FetchMember(User);
            var q = await FetchQuestion(QuestionID);
            await client.SubmitAsync(client.ConnectVerticies<QuestionVertex, AnswerVertex>(q, a, "answers").BuildGremlinQuery());
            await client.SubmitAsync(client.ConnectVerticies<UserVertex, AnswerVertex>(u, a, "answers").BuildGremlinQuery());
        }

        public async Task AddQuestion(QuestionVertex Question, string User)
        {
            Question.id = Guid.NewGuid().ToString();
            Question.Title = Question.Title ?? "";
            Question.Body = Question.Body ?? "";
            Question.ImageUrl = Question.ImageUrl ?? new List<string>();
            Question.TimeStamp = DateTime.UtcNow;
            await client.Add(Question).SubmitAsync();
            var u = await FetchMember(User);
            var q = await client.From<QuestionVertex>().Where(w => w.id == Question.id).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<QuestionVertex, UserVertex>(q, u, "asks").BuildGremlinQuery());
        }

        #endregion

        #region Tips
        public async Task AddTip(TipVertex Tip, string User)
        {
            Tip.id = Guid.NewGuid().ToString();
            Tip.Title = Tip.Title ?? "";
            Tip.Body = Tip.Body ?? "";
            await client.Add(Tip).SubmitAsync();
            var u = await FetchMember(User);
            var t = await client.From<TipVertex>().Where(w => w.id == Tip.id).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<TipVertex, UserVertex>(t, u, "tips").BuildGremlinQuery());
        }

        public async Task AddTipComment(SocialCommentVertex comment, string TipId, string User)
        {
            comment.id = Guid.NewGuid().ToString();
            comment.Title = comment.Title ?? "";
            comment.Body = comment.Body ?? "";
            comment.ImageUrl = comment.ImageUrl ?? new List<string>();
            await client.Add(comment).SubmitAsync();
            var t = await FetchTip(TipId);
            var u = await FetchMember(User);
            await client.SubmitAsync(client.ConnectVerticies<TipVertex, SocialCommentVertex>(t,comment, "comment").BuildGremlinQuery());
            await client.SubmitAsync(client.ConnectVerticies<UserVertex, SocialCommentVertex>(u,comment, "tipcomment").BuildGremlinQuery());
        }

        #endregion

        #region Social Updates / Comments

        public async Task AddSocialComment(SocialCommentVertex comment, string SocialId, string User)
        {
            comment.id = Guid.NewGuid().ToString();
            comment.Title = comment.Title ?? "";
            comment.Body = comment.Body ?? "";
            comment.ImageUrl = comment.ImageUrl ?? new List<string>();
            await client.Add(comment).SubmitAsync();
            var s = await FetchUpdate(SocialId);
            var u = await FetchMember(User);
            await client.SubmitAsync(client.ConnectVerticies<SocialUpdateVertex, SocialCommentVertex>(s, comment, "comment").BuildGremlinQuery());
            await client.SubmitAsync(client.ConnectVerticies<UserVertex, SocialCommentVertex>(u, comment, "socialcomment").BuildGremlinQuery());
        }

        public async Task SocialPostLike(string PostId, string User)
        {
            var u = await FetchMember(User);
            var c = await FetchUpdate(PostId);
            await client.SubmitAsync(client.ConnectVerticies<SocialUpdateVertex, UserVertex>(c, u, "likes").BuildGremlinQuery());
        }

        public async Task AddSocialPost(SocialUpdateVertex Update, string User)
        {
            Update.id = Guid.NewGuid().ToString();
            Update.Title = Update.Title ?? "";
            Update.Body = Update.Body ?? "";
            Update.Url = Update.Url ?? new List<string>();
            Update.ImageUrl = Update.ImageUrl ?? new List<string>();
            await client.Add(Update).SubmitAsync();
            var u = await FetchMember(User);
            await client.SubmitAsync(client.ConnectVerticies<UserVertex, SocialUpdateVertex>(u,Update, "update").BuildGremlinQuery());
        }

        public async Task CommentLike(string SocialCommentId, string User)
        {
            var u = await FetchMember(User);
            var c = await FetchComment(SocialCommentId);
            await client.SubmitAsync(client.ConnectVerticies<UserVertex, SocialCommentVertex>(u,c, "likes").BuildGremlinQuery());
        }

        #endregion

        #region Groups / Updates / Comments
        public async Task AddGroupUpdateComment(GroupUpdateCommentVertex comment, string GroupUpdateId, string User)
        {
            comment.id = Guid.NewGuid().ToString();
            comment.Title = comment.Title ?? "";
            comment.Body = comment.Body ?? "";
            comment.ImageUrl = comment.ImageUrl ?? new List<string>();
            await client.Add(comment).SubmitAsync();
            var s = await client.From<GroupUpdateVertex>().Where(w => w.id == GroupUpdateId).SubmitWithSingleResultAsync();
            var u = await client.From<UserVertex>().Where(w => w.id == User.ToLower()).SubmitWithSingleResultAsync();
            var c = await client.From<GroupUpdateCommentVertex>().Where(w => w.id == comment.id).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<GroupUpdateCommentVertex, GroupUpdateVertex>(c, s, "comment").BuildGremlinQuery());
            await client.SubmitAsync(client.ConnectVerticies<GroupUpdateCommentVertex, UserVertex>(c, u, "groupcomment").BuildGremlinQuery());
        }

        public async Task GroupUpdateCommentLike(string SocialCommentId, string User)
        {
            var u = await client.From<UserVertex>().Where(w => w.id == User.ToLower()).SubmitWithSingleResultAsync();
            var c = await client.From<GroupUpdateCommentVertex>().Where(w => w.id == SocialCommentId).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<GroupUpdateCommentVertex, UserVertex>(c, u, "like").BuildGremlinQuery());
        }

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
        public async Task AddGroupMember(string GroupId, string User)
        {
            var g = await client.From<GroupVertex>().Where(w => w.id == GroupId).SubmitWithSingleResultAsync();
            var u = await client.From<UserVertex>().Where(w => w.id == User.ToLower()).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<GroupVertex, UserVertex>(g, u, "member").BuildGremlinQuery());
        }

        public async Task DeactivateGroupMember(string GroupId, string User)
        {
            var gg = await client.From<GroupVertex>().Where(w => w.id == GroupId).SubmitWithSingleResultAsync();
            var u = await client.From<UserVertex>().Where(w => w.id == User.ToLower()).SubmitWithSingleResultAsync();
            await client.SubmitAsync($"g.V().has('id','{gg.Id}').outE('member').where(inV().has('id','{User.ToLower()}')).drop())");
        }

        public async Task AddGroupPost(GroupUpdateVertex Update, string User)
        {
            Update.id = Guid.NewGuid().ToString();
            Update.Title = Update.Title ?? "";
            Update.Body = Update.Body ?? "";
            Update.Url = Update.Url ?? new List<string>();
            Update.ImageUrl = Update.ImageUrl ?? new List<string>();
            await client.Add(Update).SubmitAsync();
            var social = await client.From<GroupUpdateVertex>().Where(w => w.id == Update.id).SubmitWithSingleResultAsync();
            var u11 = await client.From<UserVertex>().Where(w => w.id == User.ToLower()).SubmitWithSingleResultAsync();
            var g = await client.From<GroupVertex>().Where(w => w.id == Update.GroupId).SubmitWithSingleResultAsync();
            await client.SubmitAsync(client.ConnectVerticies<GroupUpdateVertex, UserVertex>(social, u11, "group update").BuildGremlinQuery());
            await client.SubmitAsync(client.ConnectVerticies<GroupUpdateVertex, GroupVertex>(social, g, "post").BuildGremlinQuery());
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
                var u = await FetchMember(User);
                await client.SubmitAsync(client.ConnectVerticies<UserVertex, RecipeVertex>(u, r, "rating").BuildGremlinQuery());
            }
        }

        public async Task HydrateGroupModel(string GroupId)
        {
            var g = await client.From<GroupVertex>().Where(w => w.id == GroupId).SubmitWithSingleResultAsync();
            var u = await client.From<GroupUpdateVertex>().Where(w => w.GroupId == GroupId).SubmitAsync();
            List<GroupUpdateVertex> l = new List<GroupUpdateVertex>();
            foreach (var item in u)
            {
                l.Add(item.Entity);
            }
            var model = new GroupModel()
            {
                Group = g.Entity,
                Updates = l
            };
        }
    }
}
