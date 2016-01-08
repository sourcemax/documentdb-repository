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


        IDocumentQuery<T> AsDocumentQuery<T>() where T : IIdentifiable;

        IDocumentQuery<T> AsDocumentQuery<T>(string sql) where T : IIdentifiable;

        IDocumentQuery<T> AsDocumentQuery<T>(Expression<Func<T, bool>> predicate) where T : IIdentifiable;


        Task<List<T>> GetItemsAsync<T>() where T : IIdentifiable;

        Task<List<T>> GetItemsAsync<T>(string sql) where T : IIdentifiable;

        Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate) where T : IIdentifiable;


        Task<T> GetItemAsync<T>(string id) where T : IIdentifiable;

        Task<T> CreateItemAsync<T>(T item) where T : IIdentifiable;

        Task<bool> UpdateItemAsync<T>(T item) where T : IIdentifiable;

        Task<T> UpsertItemAsync<T>(T item) where T : IIdentifiable;

        Task<bool> DeleteItemAsync<T>(string id) where T : IIdentifiable;
    }
}