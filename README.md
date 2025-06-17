# Nerv.Repository

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](https://github.com/tuo-utente/Nerv.Repository/actions)
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
- Ready-to-use in-memory tests

---

## 📦 Installation

```bash
dotnet add package Nerv.Repository
```

---

## 🛠️ Usage

### 1. Define your entity
```csharp
public class User : Entity<Guid>
{
    public string Name { get; set; }
}
```

### 2. Configure your DbContext
```csharp
public class AppDbContext : DbContextBase<Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options, ActorContext<Guid> actor)
        : base(options, actor) { }
}
```

### 3. Register dependencies
```csharp
services.AddNervRepository<AppDbContext, Guid>();
```

### 4. Use in your services
You only need to inject `IUnitOfWork` in your services. All repositories are accessed through it:

```csharp
public class UserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateUserAsync(string name)
    {
        var user = new User { Id = Guid.NewGuid(), Name = name };

        var userRepo = _unitOfWork.GetRepository<User>();
        await userRepo.AddAsync(user);
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

## 🧱 Project Structure

```
src/
  Nerv.Repository/
    Abstractions/
    Contexts/
    Extensions/
    LogEntities/
tests/
  Nerv.Repository.Tests/
    Context/
    Entities/
    Fixtures/
    Repositories/
    UnitOfWork/
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