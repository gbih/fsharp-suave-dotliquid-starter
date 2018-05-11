# Quick Setup

```sh
$ dotnet new console -lang F# -n MyApp
$ cd MyApp
$ dotnet add package Suave --version 2.4.0
$ dotnet add package DotLiquid --version 2.0.262
$ dotnet add package Suave.DotLiquid --version 2.4.0
```

Next, setup /templates and /assets folders:
```sh
$ mkdir {projectroot}/templates
$ mkdir {projectroot}/assets
        
$ cd templates
$ touch index.liquid
```

Create a sample index.liquid file:
```
    <h1>{{model.title}}</h1>
```

Edit .fsproj file to allow Suave to serve all files inside /templates and /assets:
```
  <ItemGroup>
    <Compile Include="Program.fs" />
    <None Include="templates/**/*" Link="views/%(RecursiveDir)%(Filename)%(Extension)" CopyToOutputDirectory="PreserveNewest" />
    <!-- Manually constructing Link metadata, works in classic projects as well -->
    <None Include="assets/**/*" Link="assets/%(RecursiveDir)%(Filename)%(Extension)" CopyToOutputDirectory="PreserveNewest" />
  </ItemGroup>
```

Sample Program.fs:
```fsharp
open Suave
open Suave.Filters
open Suave.Operators
open Suave.DotLiquid
open System.IO
open System.Reflection

let currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

let myHomeFolder = Path.Combine(Directory.GetCurrentDirectory(), "assets") 

let initDotLiquid () =
    let templatesDir = Path.Combine(currentPath, "views")
    setTemplatesDir templatesDir

let cfg = { 
    defaultConfig with
        bindings = [ HttpBinding.createSimple HTTP "127.0.0.1" 8080  ] 
        homeFolder = Some(myHomeFolder)
      }

type Model = { title : string }

let o = { title = "Hello World" }

[<EntryPoint>]
let main argv =
    printf "%s" myHomeFolder
    initDotLiquid ()
    setCSharpNamingConvention ()

    let app = 
        choose [
            pathRegex "(.*)\.(css|png|gif|js|ico)" >=> Files.browseHome
            path "/" >=> choose [
                GET >=> page "index.liquid" o ]
            ]
        
    startWebServer cfg app
    0
```

Build and run
```
$ dotnet build
$ dotnet run
```