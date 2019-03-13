AppVeyor: [![Build status](https://ci.appveyor.com/api/projects/status/github/jannesrsa/todo-api?svg=true)](https://ci.appveyor.com/project/jannesrsa/sourcecode-todo-api)
Codecov: [![Code Coverage](https://codecov.io/gh/jannesrsa/todo-api/coverage.svg)](https://codecov.io/gh/jannesrsa/todo-api)
CodeFactor: [![CodeFactor](https://www.codefactor.io/repository/github/jannesrsa/todo-api/badge)](https://www.codefactor.io/repository/github/jannesrsa/todo-api)

# ASP.NET Core Todo REST API

Sample ASP.NET Core reference application, implementing a simple Todo API.

This application illustrates:

- Multi-targeting for .NET Framework 4.6.1 and .NET Core 2.1
- Hosting an ASP.NET Core application as a Windows Service
- Logging with Serilog
- Client Certificate Authentication
- Swagger Documentation
- Hosting with Docker

## Getting Started

This project is broken up into release branches with step-by-step guidance on implementing the various concepts.

## Required

- Visual Studio 2017 Update 3 or later
- .NET Core 2.1 Runtime
- Docker



# Release-1.0

### Implement the Todo API

*This release implements the steps on [Create a Web API with ASP.NET Core and Visual Studio for Windows](https://docs.micosoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-2.0)*

- Navigate to the `src` directory of your repository:

```powershell
dotnet new webapi -n TodoApi
```

- Open the project in Visual Studio.
- [Add a model class](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-2.0#add-a-model-class)
- [Create the database context](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-2.0#create-the-database-context)
- [Register the database context](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-2.0#register-the-database-context)
- [Add a controller](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-2.0#add-a-controller)
- [Get to-do items](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-2.0#get-to-do-items)
- [Implement the other CRUD operations](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-2.0#implement-the-other-crud-operations)

If you wish to skip the guide you can access the source code directly at [TodoApi](https://github.com/aspnet/Docs/tree/master/aspnetcore/tutorials/first-web-api/samples/2.0/TodoApi).

# Release-2.0

### Multi-targeting for .NET Framework 4.6.1 and .NET Core 2.1

- Edit your .csproj
- Replace the `<TargetFramework>` node with the following:

```xml
<TargetFrameworks>netcoreapp2.1;net461</TargetFrameworks>
```

- The `Microsoft.AspNetCore.All` reference is not compatible with net461.
- Replace that reference with the following:

```xml
<PackageReference Include="Microsoft.AspNetCore" Version="2.1.1" />
<PackageReference Include="Microsoft.AspNetCore.Mvc" Version="2.1.1" />
<PackageReference Include="Microsoft.AspNetCore.StaticFiles" Version="2.1.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="2.1.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="2.1.1" />
```

- Ensure you can use the latest version of the C# compiler and FxCop
- Add the following:

```xml
<PackageReference Include="Microsoft.Net.Compilers" Version="2.8.2">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
</PackageReference>
<PackageReference Include="Microsoft.CodeAnalysis.FxCopAnalyzers" Version="2.6.1">
    <PrivateAssets>All</PrivateAssets>
</PackageReference>
```

- Reload and rebuild.
- Notice the tooling support for dealing with multiple frameworks:
  - Dependencies
  - Launch