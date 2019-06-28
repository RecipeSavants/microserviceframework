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
            collection = client.CreateDocumentCollectionQuery("dbs/" + database.Id).Where(c => c.Id == "recipes")
                .AsEnumerable().FirstOrDefault();
        }

       public List<T> GetByDocumentType<T>(int DocumentType) where T : class
        {
            List<T> TT = new List<T>();
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var x =
                client.CreateDocumentQuery(_collection.SelfLink, "SELECT * FROM c where c.DocumentType=" + DocumentType)
                    .ToList();
            foreach (var item in x)
            {
                TT.Add(JsonConvert.DeserializeObject<T>(item.ToString()));
            }
            return TT;
        }

        public List<T> GetAllApproved<T>() where T : class
        {
            List<T> TT = new List<T>();
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var x = client.CreateDocumentQuery(_collection.SelfLink, "SELECT * FROM c where c.IsApproved=true").ToList();
            foreach (var item in x)
            {
                string s = item.ToString();
                s = s.Replace("{}", "[]");
                TT.Add(JsonConvert.DeserializeObject<T>(s));
            }
            return TT;
        }
        public List<T> GetAllApprovedHasVideo<T>() where T : class
        {
            List<T> TT = new List<T>();
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var x = client.CreateDocumentQuery(_collection.SelfLink, "SELECT * FROM c where c.IsApproved=true and c.HasVideo=true").ToList();
            foreach (var item in x)
            {
                string s = item.ToString();
                s = s.Replace("{}", "[]");
                TT.Add(JsonConvert.DeserializeObject<T>(s));
            }
            return TT;
        }

        public List<T> GetAllUserSumbittedNotApproved<T>() where T : class
        {
            List<T> TT = new List<T>();
            var query = String.Format("select * from c where c.ProviderType=2 and c.IsApproved=false and c.IsInstructions=false",
                "");
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var x = client.CreateDocumentQuery(_collection.SelfLink, query).ToList();
            foreach (var item in x)
            {
                string s = item.ToString();
                s = s.Replace("{}", "[]");
                TT.Add(JsonConvert.DeserializeObject<T>(s));
            }
            return TT;
        }

        public List<T> GetAllUnapprovedNonNullTag<T>() where T : class
        {
            List<T> TT = new List<T>();
            var query = String.Format("select * from c where c.IsApproved=false and c.IsInstructions=false and c.Tags != null",
                "");
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var x = client.CreateDocumentQuery(_collection.SelfLink, query).ToList();
            foreach (var item in x)
            {
                string s = item.ToString();
                s = s.Replace("{}", "[]");
                TT.Add(JsonConvert.DeserializeObject<T>(s));
            }
            return TT;
        }

        public List<T> GetUnapproved<T>() where T : class
        {
            List<T> TT = new List<T>();
            var query = String.Format("select top 1000 * from c where c.IsApproved=false and c.IsInstructions=false",
                "");
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var x = client.CreateDocumentQuery(_collection.SelfLink, query).ToList();
            foreach (var item in x)
            {
                string s = item.ToString();
                s = s.Replace("{}", "[]");
                TT.Add(JsonConvert.DeserializeObject<T>(s));
            }
            return TT;
        }

        public List<T> GetByUser<T>(string User, bool isEdit) where T : class
        {
            List<T> TT = new List<T>();
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var sql = String.Format("select * from c where c.UpdateFK='{0}'", User);
            var sql1 = String.Format("select * from c where c.UserFK='{0}'", User);
            if (isEdit)
            {
                var x = client.CreateDocumentQuery(_collection.SelfLink, sql).ToList();
                foreach (var item in x)
                {
                    try
                    {
                        string s = item.ToString();
                        s = s.Replace("{}", "[]");
                        TT.Add(JsonConvert.DeserializeObject<T>(s));
                    }
                    catch
                    {

                    }
                }
                return TT;
            }
            else
            {
                var x = client.CreateDocumentQuery(_collection.SelfLink, sql1).ToList();
                foreach (var item in x)
                {
                    TT.Add(JsonConvert.DeserializeObject<T>(item.ToString()));
                }
                return TT;
            }
        }


        public List<string> GetIdList()
        {
            List<string> TT = new List<string>();
            var t =
                _client.CreateDocumentQuery("dbs/" + _database.Id + "/colls/" + _collection.Id)
                    .Select(s => s.Id)
                    .AsEnumerable()
                    .ToList();
            foreach (var item in t)
            {
                TT.Add(item);
            }
            return TT;
        }

        public virtual T GetById<T>(string id) where T : class
        {
            try
            {
                var t =
                    _client.CreateDocumentQuery("dbs/" + _database.Id + "/colls/" + _collection.Id)
                        .Where(f => f.Id == id)
                        .Select(s => s)
                        .AsEnumerable()
                        .FirstOrDefault();
                if (t != null)
                {
                    string s = t.ToString();
                    s = s.Replace("{}", "[]");
                    return JsonConvert.DeserializeObject<T>(s);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<T> GetByList<T>(string id) where T : class
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
            (_client.CreateDocumentQuery("dbs/" + _database.Id + "/colls/" + _collection.Id)
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

        public virtual void InsertOrUpdate<T>(string id, T input) where T : class
        {
            try
            {
                var document =
                    _client.CreateDocumentQuery("dbs/" + _database.Id + "/colls/" + _collection.Id)
                        .Where(d => d.Id == id)
                        .AsEnumerable()
                        .FirstOrDefault();
                if (document == null)
                {
                    _client.CreateDocumentAsync("dbs/" + _database.Id + "/colls/" + _collection.Id, input);
                }
                else
                {
                    _client.UpsertDocumentAsync(_collection.SelfLink, input);
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public void Delete(string Id)
        {
            Document doc = GetDocument(Id);
            _client.DeleteDocumentAsync(doc.SelfLink);
        }

        private Document GetDocument(string id)
        {
            return _client.CreateDocumentQuery(_collection.DocumentsLink)
                .Where(d => d.Id == id)
                .AsEnumerable()
                .FirstOrDefault();
        }

        public List<T> GetByRandom<T>() where T : class
        {
            List<T> TT = new List<T>();
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var sql =
                "select * from c where c.IsApproved=true and c.IsInstructions=false and (c.Synopsis = '' or c.Synopsis = null)";

            var x = client.CreateDocumentQuery(_collection.SelfLink, sql).ToList();
            foreach (var item in x)
            {
                try
                {
                    string s = item.ToString();
                    s = s.Replace("{}", "[]");
                    TT.Add(JsonConvert.DeserializeObject<T>(s));
                }
                catch
                {

                }
            }
            return TT;
        }

        public List<T> GetByNotInList<T>(List<string> List) where T : class
        {
            List<T> TT = new List<T>();
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var qualifier = "";
            foreach (var item in List)
            {
                qualifier += String.Format("'{0}',", item);
            }
            qualifier = qualifier.Substring(0, qualifier.Length - 1);
            var sql =
                String.Format(
                    "select top 20 * from c where c.IsApproved=true and c.IsInstructions=false and c.id not in ({0})",
                    qualifier);

            var x = client.CreateDocumentQuery(_collection.SelfLink, sql).ToList();
            foreach (var item in x)
            {
                try
                {
                    string s = item.ToString();
                    s = s.Replace("{}", "[]");
                    TT.Add(JsonConvert.DeserializeObject<T>(s));
                }
                catch
                {

                }
            }
            return TT;
        }

        public List<T> ExecuteSql<T>(string Sql) where T : class
        {
            List<T> TT = new List<T>();
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var x = client.CreateDocumentQuery(_collection.SelfLink, Sql).ToList();
            foreach (var item in x)
            {
                try
                {
                    string s = item.ToString();
                    s = s.Replace("{}", "[]");
                    TT.Add(JsonConvert.DeserializeObject<T>(s));
                }
                catch
                {

                }
            }
            return TT;
        }
    }
}
