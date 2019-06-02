using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSavants.Microservices.DocumentRepository.Interfaces
{
    public interface IRepository
    {
        void Delete(string id);
        Task<List<T>> GetByDocumentTypeWhere<T>(int? DocumentType, Expression<Func<T, bool>> expression) where T : class;
        Task<List<T>> GetByDocumentTypeWhere<T>(int? DocumentType, Expression<Func<T, bool>> expression, Expression<Func<T, object>> select) where T : class;
        Task<List<T>> GetByDocumentType<T>(int DocumentType) where T : class;
        Task<List<T>> ExecuteSql<T>(string Sql) where T : class;
        Task<Tuple<string, IList<T>>> QueryAndContinue<T>(string continuationToken, int take, string sql) where T : class;
        Task<T> GetById<T>(string id) where T : class;
        void InsertOrUpdate<T>(string id, T input) where T : class;
        Task<List<T>> GetByList<T>(string id) where T : class;
    }
}
