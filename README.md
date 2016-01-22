
Azure DocumentDB Repository
===

A repository implementation built on the Azure DocumentDB SDK for .NET that is reusable in .NET, and specifically ASP.NET 5, applications.

[Available as a Nuget Package](https://www.nuget.org/packages/SourceMax.DocumentDB)

Installation
---

Use NuGet to install the package within your .NET project:

```
  PM> Install-Package SourceMax.DocumentDB
```

Dependency Injection Setup
---

Assuming you are building an ASP.NET 5 project, add the following code to your Startup.cs file:

```C#
public void ConfigureServices(IServiceCollection services) {
    ...

    // Setup the DocumentDB repository with the proper connection string
    //  Example: "Account=myaccount;Database=mydatabase;Collection=mycollection;Key=blahblahblah+mykey+blahblahblah==;"
    var connectionString = this.Configuration["ConnectionStrings:DocumentDB"];

    // Set an instance of the Repository class, with the connection string, as the implementation of IRepository
    services.AddSingleton<IRepository>(sp => new Repository(connectionString));
    ...
}

```

Using the Repository
---

You can simply inject the repository into any controller or service where you need to access the DocumentDB:

```C#
public class TestController : Controller {

    private readonly IRepository Repository;

    public TestController(IRepository repository) {
        this.Repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Get() {
            
        // Get a single item
        var item = await this.Repository.GetItemAsync<TestModel>("SomeId");

        // Get all Items
        var items = await this.Repository.AsQueryable<TestModel>().ToListAsync();

        // More complex query with projection
        var asyncItems = await this.Repository.AsQueryable<TestModel>().Where(x => x.Description == "Test load #1").Select(x => new { Id = x.Id, Type = x.Type }).ToListAsync();

        return this.Ok();
    }
}
```