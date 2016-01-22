using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace SourceMax.DocumentDB {

    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Repository : IRepository {

        protected DocumentClient Client { get; set; }

        protected Database Database { get; set; }

        protected DocumentCollection Collection { get; set; }
        
        public Repository(string connectionString) {

            // This will ensure that we do not serialize null values, which will keep the json smaller
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore
            };

            //this.ParseConnectionString(connectionString).Wait();

            var accountName = this.ParseConnectionStringValue(connectionString, "Account");
            var accountKey = this.ParseConnectionStringValue(connectionString, "Key");
            var databaseName = this.ParseConnectionStringValue(connectionString, "Database");
            var collectionName = this.ParseConnectionStringValue(connectionString, "Collection");

            var endpoint = new Uri($"https://{accountName}.documents.azure.com:443/");

            this.Client = new DocumentClient(endpoint, accountKey);
            this.Database = this.CreateOrReadDatabaseAsync(databaseName).Result;
            this.Collection = this.CreateOrReadCollectionAsync(collectionName).Result;
        }



        public IQueryable<T> AsQueryable<T>() {
            return this.Client.CreateDocumentQuery<T>(this.Collection.SelfLink);
        }

        public IQueryable<T> AsQueryable<T>(string sql) {
            return this.Client.CreateDocumentQuery<T>(this.Collection.SelfLink, sql);
        }

        public IQueryable<T> AsQueryable<T>(Expression<Func<T, bool>> predicate) {
            return this.Client.CreateDocumentQuery<T>(this.Collection.SelfLink).Where(predicate);
        }

        

        public virtual IDocumentQuery<T> AsDocumentQuery<T>() {
            return this.Client.CreateDocumentQuery<T>(this.Collection.SelfLink).AsDocumentQuery();
        }

        public virtual IDocumentQuery<T> AsDocumentQuery<T>(string sql) {
            return this.Client.CreateDocumentQuery<T>(this.Collection.SelfLink, sql).AsDocumentQuery();
        }

        public virtual IDocumentQuery<T> AsDocumentQuery<T>(Expression<Func<T, bool>> predicate) {

            var query = this.Client
                .CreateDocumentQuery<T>(this.Collection.SelfLink)
                .Where(predicate)
                .AsDocumentQuery();

            return query;
        }
        


        public virtual async Task<List<T>> GetItemsAsync<T>() {

            var query = await this.Client
                .CreateDocumentQuery<T>(this.Collection.SelfLink)
                .AsDocumentQuery()
                .ExecuteNextAsync<T>();

            return query.ToList();
        }

        public virtual async Task<List<T>> GetItemsAsync<T>(string sql) {

            var query = await this.Client
                .CreateDocumentQuery<T>(this.Collection.SelfLink, sql)
                .AsDocumentQuery()
                .ExecuteNextAsync<T>();

            return query.ToList();
        }

        public virtual async Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate) {

            var query = await this.Client
                .CreateDocumentQuery<T>(this.Collection.SelfLink)
                .Where(predicate)
                .AsDocumentQuery()
                .ExecuteNextAsync<T>();

            return query.ToList();
        }



        public virtual async Task<T> GetItemAsync<T>(string id) {

            var query = await this.Client
                .CreateDocumentQuery(this.Collection.SelfLink)
                .Where(d => d.Id == id)
                .AsDocumentQuery()
                .ExecuteNextAsync<T>();

            return query.SingleOrDefault();
        }

        public virtual async Task<T> CreateItemAsync<T>(T item) {

            var result = await this.Client.CreateDocumentAsync(this.Collection.SelfLink, item);
            return item;
        }

        public virtual async Task<bool> UpdateItemAsync<T>(string id, T item) {

            var doc = await this.GetDocumentAsync(id);
            var result = await this.Client.ReplaceDocumentAsync(doc.SelfLink, item);
            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public virtual async Task<T> UpsertItemAsync<T>(T item) {

            await this.Client.UpsertDocumentAsync(this.Collection.SelfLink, item);
            return item;
        }

        public virtual async Task<bool> DeleteItemAsync<T>(string id) {

            var doc = await this.GetDocumentAsync(id);
            var result = await this.Client.DeleteDocumentAsync(doc.SelfLink);
            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }

        protected virtual async Task<Document> GetDocumentAsync(string id) {

            var query = this.Client
                .CreateDocumentQuery(this.Collection.SelfLink)
                .Where(d => d.Id == id)
                .AsDocumentQuery();

            var result = await query.ExecuteNextAsync<Document>();

            return result.Single();
        }

        protected virtual string ParseConnectionStringValue(string connectionString, string parameter) {

            string regex = @"(^|[;])" + parameter + @"=(?<Value>.*?)([;]|$)";
            Match match = Regex.Match(connectionString, regex, RegexOptions.IgnoreCase);
            string result = match.Groups["Value"].Value;

            return result;
        }

        protected virtual async Task<Database> CreateOrReadDatabaseAsync(string databaseName) {

            if (this.Client.CreateDatabaseQuery().Where(x => x.Id == databaseName).AsEnumerable().Any()) {
                return this.Client.CreateDatabaseQuery().Where(x => x.Id == databaseName).AsEnumerable().FirstOrDefault();
            }

            return await this.Client.CreateDatabaseAsync(new Database { Id = databaseName });
        }

        protected virtual async Task<DocumentCollection> CreateOrReadCollectionAsync(string collectionName) {

            if (this.Client.CreateDocumentCollectionQuery(this.Database.SelfLink).Where(c => c.Id == collectionName).ToArray().Any()) {
                return this.Client.CreateDocumentCollectionQuery(this.Database.SelfLink).Where(c => c.Id == collectionName).ToArray().FirstOrDefault();
            }

            return await this.Client.CreateDocumentCollectionAsync(this.Database.SelfLink, new DocumentCollection { Id = collectionName });
        }
    }
}
