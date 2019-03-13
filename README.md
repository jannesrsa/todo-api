### Build & Test Status

| Metric      | Status      |
| ----- | ----- |
|AppVeyor | [![Build status](https://ci.appveyor.com/api/projects/status/github/jannesrsa/todo-api?svg=true)](https://ci.appveyor.com/project/jannesrsa/sourcecode-todo-api) |
|Codecov | [![Code Coverage](https://codecov.io/gh/jannesrsa/todo-api/coverage.svg)](https://codecov.io/gh/jannesrsa/todo-api) |
|CodeFactor | [![CodeFactor](https://www.codefactor.io/repository/github/jannesrsa/todo-api/badge)](https://www.codefactor.io/repository/github/jannesrsa/todo-api) |

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

# Release-2.1

### Hosting an ASP.NET Core application as a Windows Service

*This release implements the steps on [Host ASP.NET Core in a Windows Service](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-2.1&tabs=aspnetcore2x).*

- Edit your .csproj
- Add the following constants to your project:

```xml
<PropertyGroup Condition=" '$(TargetFramework)' == 'net461' ">
  <DefineConstants>NET461;$(DefineConstants)</DefineConstants>
  <RuntimeIdentifier>win7-x64</RuntimeIdentifier>
</PropertyGroup>

<PropertyGroup Condition=" '$(TargetFramework)' == 'netcoreapp2.1' ">
  <DefineConstants>NETCOREAPP20;$(DefineConstants)</DefineConstants>
</PropertyGroup>
```

- Add a package reference to the `Microsoft.AspNetCore.Hosting.WindowsServices`. Notice that the reference is only included for `net461`.

```xml
<ItemGroup Condition="'$(TargetFramework)' == 'net461'">
  <PackageReference Include="Microsoft.AspNetCore.Hosting.WindowsServices" Version="2.0.1" />
 </ItemGroup>
```

- Include the using for `NET461` only to avoid generating warnings.
- Conditionally check if we are running as a service for `NET461`.

```c#
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

#if NET461
using Microsoft.AspNetCore.Hosting.WindowsServices;
using System.Diagnostics;
using System.Linq;
#endif

namespace TodoApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var pathToContentRoot = Directory.GetCurrentDirectory();
#if NET461
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                pathToContentRoot = Path.GetDirectoryName(pathToExe);
            }
#endif

            var host = WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(pathToContentRoot)
                .UseStartup<Startup>()
                .Build();

#if NET461
            if (isService)
            {
                host.RunAsService();
                return;
            }
#endif
            host.Run();
        }
    }
}
```

- Launch the application for net461, the host is not running as a service.
- Now install the application as a WindowsService from an elevated powershell:

```powershell
sc.exe create TodoApi binPath="C:\Repos\aspnet-core-todo-api\src\TodoApi\bin\Debug\net461\win7-x64\TodoApi.exe"
sc.exe start TodoApi
```

- Access the API, by default it should be available on `htpp://localhost:5000/api/todo`