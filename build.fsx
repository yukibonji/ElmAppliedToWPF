// include Fake libs
#r "./packages/build/FAKE/tools/FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile

// Directories
let buildDir  = "./build/"
let deployDir = "./deploy/"


// Filesets
let appReferences  =
    !! "/**/*.csproj"
    ++ "/**/*.fsproj"

// version info
let version = "0.0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "Build" (fun _ ->
    CreateFSharpAssemblyInfo "src/Elm/Properties/AssemblyInfo.fs"
        [
            Attribute.Title "Elm"
            Attribute.Description "Helpers to write WPF application using Unidirectional flow (like Elm)"
            Attribute.Guid "052ff1a5-ec46-40bd-9be8-b77dd0b5ec76"
            Attribute.Product "Elm"
            Attribute.Version version
            Attribute.FileVersion version
        ]
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
    |> Log "AppBuild-Output: "
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
    -- "*.zip"
    |> Zip buildDir (deployDir + "ApplicationName." + version + ".zip")
)

// Build order
"Clean"
  ==> "Build"
  ==> "Deploy"

// start build
RunTargetOrDefault "Build"
