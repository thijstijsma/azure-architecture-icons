<Query Kind="FSharpProgram">
  <NuGetReference>Giraffe.ViewEngine</NuGetReference>
</Query>

open Giraffe.ViewEngine

let basePath = Path.GetDirectoryName Util.CurrentQueryPath
let iconPath = Path.Combine (basePath, "Icons")
let htmlPath = Path.Combine (basePath, "index.htm")

let getCategoryFromRelativePath (relativePath: string) =
    relativePath.Split (Path.DirectorySeparatorChar, 3)
    |> fun x -> x[1]
    
let generateIconText (iconPath: string) =
    iconPath
    |> Path.GetFileNameWithoutExtension
    |> fun x -> x.Split ("-", 4)
    |> Seq.last
    |> fun x -> x.Replace("-", " ")
    |> str
    
let generateIconImage (iconPath: string) =
    p [] [
        img [
            attr "src" iconPath
            attr "width" "64"
            attr "height" "64"
            attr "style" "cursor: pointer;"
            attr "onclick" "navigator.clipboard.writeText(this.src);"
        ]
        generateIconText iconPath
    ]
    
let generateIconHtml (iconPaths: string seq) =
    iconPaths
    |> Seq.map generateIconImage
    |> Seq.toList
    
let generateCategoryHtml (category: string, iconPaths: string seq) =
    iconPaths
    |> generateIconHtml
    |> fun nodes -> h1 [] [str category] :: nodes

let htmlContent =
    Directory.GetFiles (iconPath, "*", SearchOption.AllDirectories)
    |> Seq.map (fun x -> x[basePath.Length + 1..])
    |> Seq.groupBy getCategoryFromRelativePath
    |> Seq.map generateCategoryHtml
    |> Seq.collect id
    |> Seq.toList
    |> html []

htmlContent
|> RenderView.AsString.htmlDocument
|> Dump
|> (fun x -> File.WriteAllText (htmlPath, x))
