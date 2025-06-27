# Nerv.Repository

[![Repository](https://img.shields.io/badge/code-GitHub-blue.svg)](https://github.com/dev-github-main/Nerv.Repository)
[![NuGet](https://img.shields.io/nuget/v/Nerv.Repository.svg)](https://www.nuget.org/packages/Nerv.Repository/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A lightweight, flexible and extensible implementation of the Repository and Unit of Work patterns using Entity Framework Core.  
Designed for clean architecture, testability, and ease of integration across any .NET application.

---

## ✨ Features

- Generic `IRepository<T>` and `IReadOnlyRepository<T>`
- `IUnitOfWork` with transaction support
- Audit support (`CreatedOn`, `UpdatedOn`, `DeletedOn`, `CreatedBy`, etc.)
- Soft delete support
- Actor context integration for automatic user tracking
- Global `DbContextBase` with auto-audit and soft-delete filters
- Pagination and filtering utilities
- Repository model configuration via `UnitOfWorkOptions` (including table name pluralization using [Humanizer](https://github.com/Humanizr/Humanizer))
- Ready-to-use in-memory tests

---

## 📦 Installation

```bash
dotnet add package Nerv.Repository
```

---

## 🛠️ Usage (Single or Multi-DbContext)

### 1. Define your entity
```csharp
public class User : Entity<Guid>
{
    public string Name { get; set; }
}
```

### 2. Configure your DbContext
```csharp
public class AppDbContext : DbContextBase<Guid, AppDbContext>
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ActorContext<Guid> actor,
        UnitOfWorkOptions uowOptions) : base(options, actor, uowOptions)
        {
        }
}
```

### 3. Register everything in `Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register IHttpContextAccessor (needed to access HTTP context)
builder.Services.AddHttpContextAccessor();

// Register everything using the AddRepositoryPattern extension
builder.Services.AddRepositoryPattern<AppDbContext, Guid>(
    // Define the context name in a multi-database scenario,
    // must be used also in a single database scenario
    contextName: "Main",
    // DbContext configuration
    options =>
    {
        // Retrieve connection string from configuration
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        
        // Configure EF Core provider (in this case, SQL Server)
        options.UseSqlServer(connectionString);
    },
    // ActorContext factory configuration (used for audit tracking)
    provider =>
    {
        // Resolve IHttpContextAccessor to access the current HTTP context
        var httpContext = provider.GetRequiredService<IHttpContextAccessor>()?.HttpContext;

        // Extract the user ID from claims (e.g. OpenID Connect "sub" claim)
        var userIdClaim = httpContext?.User?.FindFirst("sub")?.Value;

        // Parse the user ID or fallback to Guid.Empty if not authenticated
        Guid userId = userIdClaim != null 
            ? Guid.Parse(userIdClaim)
            : Guid.Empty;

        // Return ActorContext to be injected into DbContextBase
        return new ActorContext<Guid> { UserId = userId };
    },
    // UnitOfWork options configuration (repository model customization)
    uowOptions =>
    {
        // Enable pluralization of table names (e.g. User -> Users)
        uowOptions.ModelOptions.UsePluralization = true;
    }
);

// Register UnitOfWorkFactory only once
builder.services.AddUnitOfWorkFactory();
```

### 4. Inject `IUnitOfWorkFactory` to resolve a unit of work
You only need to inject `IUnitOfWorkFactory` in your services. All unit of work are accessed through it:

```csharp
public class UserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWorkFactory factory)
    {
        _unitOfWork = factory.GetUnitOfWork("Main");
    }

    public async Task CreateAsync()
    {
        var userRepo = _unitOfWork.Repository<User>();
        await userRepo.AddAsync(new User { Id = Guid.NewGuid(), Name = "Test" });
        await _unitOfWork.SaveChangesAsync();
    }
}
```

---

## 🔍 ActorContext for audit
All changes are tracked by `ActorContext<TUserId>`:

```csharp
public class ActorContext<TUserId>
{
    public TUserId UserId { get; set; }
}
```

---

## 🧪 Testing

You can test your repositories and unit of work using the in-memory SQLite provider.

```csharp
public class UserTests
{
    [Fact]
    public async Task Add_Should_Persist_User()
    {
        using var fixture = RepositoryFixture.Create();
        var user = new User { Id = Guid.NewGuid(), Name = "Test" };
        await fixture.Repository.AddAsync(user);
        await fixture.UnitOfWork.SaveChangesAsync();

        var result = await fixture.Repository.GetByIdAsync(user.Id);
        Assert.NotNull(result);
    }
}
```

---

## 📄 License

This project is licensed under the MIT License.

---

## 🤝 Contributing

Feel free to open issues or pull requests. Contributions are welcome!

---

## 🔗 Related Projects

- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Humanizer](https://github.com/Humanizr/Humanizer) — Used for automatic pluralization of table names