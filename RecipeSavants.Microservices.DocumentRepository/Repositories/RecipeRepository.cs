using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using RecipeSavants.Microservices.DocumentRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSavants.Microservices.DocumentRepository
{
    public class RecipeRepository : IRecipeRepoistory
    {
        private DocumentClient client;
        private Database database;
        private DocumentCollection collection;

        public RecipeRepository()
        {
            string EndpointUrl = "https://recipesavants.documents.azure.com:443/";
            string AuthorizationKey = "RMRFqTXDcQD6nhyRTRc6zETgkKWaMUGEjuR8E5CrjHEpq0gicJRCsA9A9f47ojSN70YVeddhRAdFFSy4mpnU0g==";
            client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            database = client.CreateDatabaseQuery().Where(db => db.Id == "recipes").AsEnumerable().FirstOrDefault();
            collection = client.CreateDocumentCollectionQuery("dbs/" + database.Id).Where(c => c.Id == "documents")
                .AsEnumerable().FirstOrDefault();
        }

        public void Delete(string id)
        {
            Document doc = client.CreateDocumentQuery("dbs/" + database.Id + "/colls/" + collection.Id).Where(f => f.Id == id).Select(s => s).AsEnumerable().FirstOrDefault();
            if (doc != null)
            {
                client.DeleteDocumentAsync(doc.SelfLink);
            }
        }

        public async Task<List<T>> ExecuteSql<T>(string Sql) where T : class
        {
            return client.CreateDocumentQuery<T>(collection.SelfLink, Sql).ToList();
        }

        public async Task<List<T>> GetByDocumentType<T>(int DocumentType) where T : class
        {
            return client.CreateDocumentQuery<T>(collection.SelfLink, "SELECT * FROM c where c.DocumentType=" + DocumentType).ToList();
        }

        public async Task<List<T>> GetByDocumentTypeWhere<T>(int? DocumentType, Expression<Func<T, bool>> expression) where T : class
        {
            var select = "SELECT * FROM c " + new QueryBuilder<T>(DocumentType).Where(expression).Where();
            return client.CreateDocumentQuery<T>(collection.SelfLink, select).ToList();
        }

        public async Task<List<T>> GetByDocumentTypeWhere<T>(int? DocumentType, Expression<Func<T, bool>> expression, Expression<Func<T, object>> select) where T : class
        {
            string columns = "SELECT ";
            if (select != null && select.Body is NewExpression)
            {
                var ne = select.Body as NewExpression;
                foreach (var item in ne.Arguments)
                {
                    if (item is MemberExpression)
                    {
                        columns += $"{(columns == "SELECT " ? "" : ",")} c.{(item as MemberExpression).Member.Name}";
                    }
                }
            }
            var query = $"{columns} FROM c " + new QueryBuilder<T>(DocumentType).Where(expression).Where();
            return client.CreateDocumentQuery<T>(collection.SelfLink, query).ToList();
        }

        public async Task<T> GetById<T>(string id) where T : class
        {
            var t = client.CreateDocumentQuery("dbs/" + database.Id + "/colls/" + collection.Id)
                .Where(f => f.Id == id).Select(s => s).AsEnumerable().FirstOrDefault();
            if (t != null)
            {
                return JsonConvert.DeserializeObject<T>(t.ToString());
            }
            return null;
        }

        public void InsertOrUpdate<T>(string id, T input) where T : class
        {
            var document = client.CreateDocumentQuery("dbs/" + database.Id + "/colls/" + collection.Id)
                .Where(f => f.Id == id).Select(s => s).AsEnumerable().FirstOrDefault();
            if (document == null)
            {
                client.CreateDocumentAsync(collection.SelfLink, input);
            }
            else
            {
                client.UpsertDocumentAsync(collection.SelfLink, input);
            }
        }

        public async Task<Tuple<string, IList<T>>> QueryAndContinue<T>(string continuationToken, int take, string sql) where T : class
        {
            var queryOptions = new FeedOptions { MaxItemCount = take };
            if (!string.IsNullOrEmpty(continuationToken))
            {
                queryOptions.RequestContinuation = continuationToken;
            }
            var dquery = client.CreateDocumentQuery<T>(collection.SelfLink, sql, queryOptions).AsDocumentQuery();
            string queryContinuationToken = null;
            var page = await dquery.ExecuteNextAsync<T>();
            var queryResult = page.ToList();
            if (dquery.HasMoreResults)
            {
                queryContinuationToken = page.ResponseContinuation;
            }

            return new Tuple<string, IList<T>>(queryContinuationToken, queryResult);
        }

        public async Task<List<T>> GetByList<T>(string id) where T : class
        {
            List<T> TT = new List<T>();
            id.Replace(" ", string.Empty);
            var l = id.Split(',');
            List<string> L = new List<string>();
            foreach (var item in l)
            {
                L.Add(item.Replace(" ", String.Empty));
            }
            var x =
            (client.CreateDocumentQuery("dbs/" + database.Id + "/colls/" + collection.Id)
                .Where(f => L.Contains(f.Id))
                .Select(s => s)
                .AsEnumerable()
                .ToList());
            foreach (var item in x)
            {
                string s = item.ToString();
                s = s.Replace("{}", "[]");
                TT.Add(JsonConvert.DeserializeObject<T>(s));
            }
            return TT;
        }
    }
}
