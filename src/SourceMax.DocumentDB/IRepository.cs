using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.Azure.Documents.Linq;

namespace SourceMax.DocumentDB {

    public interface IRepository {

        IQueryable<T> AsQueryable<T>();

        IQueryable<T> AsQueryable<T>(string sql);

        IQueryable<T> AsQueryable<T>(Expression<Func<T, bool>> predicate);


        IDocumentQuery<T> AsDocumentQuery<T>();

        IDocumentQuery<T> AsDocumentQuery<T>(string sql);

        IDocumentQuery<T> AsDocumentQuery<T>(Expression<Func<T, bool>> predicate);


        Task<List<T>> GetItemsAsync<T>();

        Task<List<T>> GetItemsAsync<T>(string sql);

        Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate);


        Task<T> GetItemAsync<T>(string id);

        Task<T> CreateItemAsync<T>(T item);

        Task<bool> UpdateItemAsync<T>(string id, T item);

        Task<T> UpsertItemAsync<T>(T item);

        Task<bool> DeleteItemAsync<T>(string id);
    }
}