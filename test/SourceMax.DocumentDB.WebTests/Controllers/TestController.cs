using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;

using Microsoft.AspNet.Mvc;

using SourceMax.DocumentDB.WebTests.Models;

namespace SourceMax.DocumentDB.WebTests.Controllers {

    [Route("api/[controller]")]
    public class TestController : Controller {

        private readonly IRepository Repository;

        public TestController(IRepository repository) {
            this.Repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {
            
            // Get a single item
            var item = await this.Repository.GetItemAsync<TestModel>("20151222180555530");
            //Debug.Assert(item.Type == "Load");
            //Debug.Assert(item.Id == "20151222180555530");

            // Get all Items
            var items = await this.Repository.AsQueryable<TestModel>().ToListAsync();
            //Debug.Assert(items.Count > 0);

            // More complex query with projection
            var asyncItems = await this.Repository.AsQueryable<TestModel>().Where(x => x.Description == "Test load #1").Select(x => new { Id = x.Id, Type = x.Type }).ToListAsync();
            //Debug.Assert(asyncItems.Count > 0);

            return this.Ok();
        }
    }
}