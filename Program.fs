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