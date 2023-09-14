<Query Kind="FSharpProgram">
  <NuGetReference>Giraffe.ViewEngine</NuGetReference>
</Query>

open Giraffe.ViewEngine

let generateVersionHtml versionPath =
    let iconPath = Path.Combine (versionPath, "Icons")
    let htmlPath = Path.Combine (versionPath, "index.htm")

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
        div [
            attr "style" "display: flex; flex-direction: column; align-items: center; width: 128px;"
        ] [
            img [
                attr "src" iconPath
                attr "width" "64"
                attr "height" "64"
                attr "style" "cursor: pointer; margin-bottom: 2em;"
                attr "onclick" "navigator.clipboard.writeText(this.src);"
            ]
            span [ attr "style" "text-align: center; margin-bottom: 2em;" ] [ generateIconText iconPath ]
        ]
        
    let generateIconHtml (iconPaths: string seq) =
        iconPaths
        |> Seq.map generateIconImage
        |> Seq.toList
        |> fun nodes -> div [attr "style" "display: flex; flex-wrap: wrap;"] nodes
        
    let generateCategoryHtml (category: string, iconPaths: string seq) =
        iconPaths
        |> generateIconHtml
        |> fun nodes -> div [] [
            h1 [] [str category]
            nodes
        ]
        
    let generateHtmlPage content =
        html [] [
            head [] []
            body [] content
        ]

    let htmlContent =
        Directory.GetFiles (iconPath, "*", SearchOption.AllDirectories)
        |> Seq.map (fun x -> x[versionPath.Length + 1..])
        |> Seq.groupBy getCategoryFromRelativePath
        |> Seq.map generateCategoryHtml
        |> Seq.toList
        |> generateHtmlPage

    htmlContent
    |> RenderView.AsString.htmlDocument
    |> (fun x -> File.WriteAllText (htmlPath, x))

let isVersionPath (path: string) =
    let version =
        path
        |> Path.GetFileNameWithoutExtension
    
    match Int32.TryParse version with
    | true, _ -> true
    | _ -> false

let basePath = Path.GetDirectoryName Util.CurrentQueryPath

basePath
|> Directory.GetDirectories
|> Seq.filter isVersionPath
|> Seq.iter generateVersionHtml