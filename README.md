
Azure DocumentDB Repository
===

A .NET repository implementation built for Microsoft's Azure DocumentDB. Although usable in any .NET project, it was built specifically with ASP.NET 5 in mind.

The source code is available here, but you can install directory into your project using the [NuGet package](https://www.nuget.org/packages/SourceMax.DocumentDB).

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

To use the repository in a controller, for example, you just inject it and you are good to go:

```C#
public class TestController : Controller {

    private readonly IRepository Repository;

    public TestController(IRepository repository) {
        this.Repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Get() {
            
        // Get a single item
        var item = await this.Repository.GetItemAsync<MyModel>("SomeId");

        // Get all Items
        var items = await this.Repository
            .AsQueryable<MyModel>()
            .ToListAsync();

        // More complex query with projection with a where clause and projection
        var asyncItems = await this.Repository
            .AsQueryable<MyModel>()
            .Where(x => x.Description == "Test load #1")
            .Select(x => new { Id = x.Id, Type = x.Type })
            .ToListAsync();

        return this.Ok();
    }
}
```

Acknowledgements
---

The code for the repository was derived, in large part, from Ryan CrawCour's 
[article](https://azure.microsoft.com/en-us/documentation/articles/documentdb-dotnet-application/) and 
[repository](https://github.com/Azure-Samples/documentdb-dotnet-todo-app) 
on DocumentDB. Not sure what his actual title is, but Ryan is the public figurehead for Microsoft's
DocumentDB offering, so he knows what he is talking about.