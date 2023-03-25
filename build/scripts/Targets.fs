// Copyright (C) 2023 Search Pioneer - https://www.searchpioneer.com
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//         http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

module Targets

open System.Net.Http
open Fake.Tools.Git
open Argu
open System
open System.Linq
open System.IO
open Bullseye
open CommandLine
open ProcNet

let runningOnCI = Fake.Core.Environment.hasEnvironVar "CI"
let runningOnWindows = Fake.Core.Environment.isWindows

let execWithTimeout binary args timeout =
    let opts =
        ExecArguments(binary, args |> List.map (sprintf "\"%s\"") |> List.toArray)
        
    let r = Proc.Exec(opts, timeout)

    match r.HasValue with
    | true -> r.Value
    | false -> failwithf "Invocation of `%s` timed out" binary

let exec binary args =
    execWithTimeout binary args (TimeSpan.FromMinutes 4)

let private restoreTools = lazy (exec "dotnet" [ "tool"; "restore" ])

let private currentVersion =
    lazy
        (restoreTools.Value |> ignore
         let r = Proc.Start("dotnet", "minver", "-d=preview", "-m=0.1")
         let o = r.ConsoleOut |> Seq.find (fun l -> not (l.Line.StartsWith("MinVer:")))
         o.Line)

let private currentVersionInformational =
    lazy
        (match Paths.IncludeGitHashInInformational with
         | false -> currentVersion.Value
         | true -> sprintf "%s+%s" currentVersion.Value (Information.getCurrentSHA1 (".")))

let Root =
    let mutable dir = DirectoryInfo(".")
    while dir.GetFiles("*.sln").Length = 0 do dir <- dir.Parent
    Environment.CurrentDirectory <- dir.FullName
    dir

let private clean (arguments: ParseResults<Arguments>) =

    let clientBin = DirectoryInfo(Path.Combine(Root.FullName, "src", "SearchPioneer.Weaviate.Client", "bin"))
    let clientObj = DirectoryInfo(Path.Combine(Root.FullName, "src", "SearchPioneer.Weaviate.Client", "obj"))
    let testsBin = DirectoryInfo(Path.Combine(Root.FullName, "tests", "SearchPioneer.Weaviate.Client.Tests", "bin"))
    let testsObj = DirectoryInfo(Path.Combine(Root.FullName, "tests", "SearchPioneer.Weaviate.Client.Tests", "obj"))
    let iTestsBin = DirectoryInfo(Path.Combine(Root.FullName, "tests-integration", "SearchPioneer.Weaviate.Client.IntegrationTests", "bin"))
    let iTestsObj = DirectoryInfo(Path.Combine(Root.FullName, "tests-integration", "SearchPioneer.Weaviate.Client.IntegrationTests", "obj"))

    if (clientBin.Exists) then
        clientBin.Delete(true)

    if (clientObj.Exists) then
        clientObj.Delete(true)

    if (testsBin.Exists) then
        testsBin.Delete(true)

    if (testsObj.Exists) then
        testsObj.Delete(true)

    if (iTestsBin.Exists) then
        iTestsBin.Delete(true)

    if (iTestsObj.Exists) then
        iTestsObj.Delete(true)

    if (Paths.Output.Exists) then
        Paths.Output.Delete(true)

    exec "dotnet" [ "clean" ] |> ignore

let private build (arguments: ParseResults<Arguments>) =
    exec "dotnet" [ "build"; "-c"; "Release" ] |> ignore

let private pristineCheck (arguments: ParseResults<Arguments>) =
    let doCheck = arguments.TryGetResult CleanCheckout |> Option.defaultValue true

    match doCheck, Information.isCleanWorkingCopy "." with
    | _, true -> printfn "The checkout folder does not have pending changes, proceeding"
    | false, _ -> printf "The checkout folder is dirty but -c was specified to ignore this"
    | _ -> failwithf "The checkout folder has pending changes, aborting"

type TestMode = | Unit | Integration
let private runTests (arguments: ParseResults<Arguments>) testMode =
    
    let mode = match testMode with | Unit ->  "unit" | Integration -> "integration"
        
    let filterArg =
        match testMode with
        | Unit ->  [ "--filter"; "FullyQualifiedName!~IntegrationTests" ]
        | Integration -> [ "--filter"; "FullyQualifiedName~IntegrationTests" ]

    let os = if runningOnWindows then "win" else "linux"
    let junitOutput =
        Path.Combine(Paths.Output.FullName, $"junit-%s{os}-%s{mode}-{{assembly}}-{{framework}}-test-results.xml")

    let loggerPathArgs = sprintf "LogFilePath=%s" junitOutput
    let loggerArg = $"--logger:\"junit;%s{loggerPathArgs};MethodFormat=Class;FailureBodyFormat=Verbose\""
    let settingsArg = if runningOnCI then (["-s"; ".ci.runsettings"]) else [];

    execWithTimeout "dotnet" ([ "test" ] @ filterArg @ settingsArg @ [ "-c"; "RELEASE"; "-m:1"; loggerArg ]) (TimeSpan.FromMinutes 15)
    |> ignore

let private test (arguments: ParseResults<Arguments>) =
    runTests arguments Unit

let private integrate (arguments: ParseResults<Arguments>) =
    runTests arguments Integration

let private generatePackages (arguments: ParseResults<Arguments>) =
    let output = Paths.RootRelative Paths.Output.FullName
    exec "dotnet" [ "pack"; "-c"; "Release"; "-o"; output ] |> ignore

let private validatePackages (arguments: ParseResults<Arguments>) =
    let output = Paths.RootRelative <| Paths.Output.FullName

    let nugetPackages =
        Paths.Output.GetFiles("*.nupkg")
        |> Seq.sortByDescending (fun f -> f.CreationTimeUtc)
        |> Seq.map (fun p -> Paths.RootRelative p.FullName)

    let ciOnWindowsArgs = if runningOnCI && runningOnWindows then [ "-r"; "true" ] else []

    let args =
        [ "-v"; currentVersionInformational.Value; "-k"; Paths.StrongName; "-t"; output ] @ ciOnWindowsArgs

    nugetPackages |> Seq.iter (fun p -> exec "dotnet" ([ "nupkg-validator"; p ] @ args) |> ignore)

let private release (arguments: ParseResults<Arguments>) = printfn "release"

let private publish (arguments: ParseResults<Arguments>) = printfn "publish"

let teardown () =
    if Paths.Output.Exists then
        let isSkippedFile p =
            File.ReadLines(p).FirstOrDefault() = "<testsuites />"
        Paths.Output.GetFiles("junit-*.xml")
            |> Seq.filter (fun p -> isSkippedFile p.FullName)
            |> Seq.iter (fun f ->
                printfn $"Removing empty test file: %s{f.FullName}"
                f.Delete()
            )
    Console.WriteLine "Ran teardown"

let Setup (parsed: ParseResults<Arguments>) (subCommand: Arguments) =
    let step (name: string) action =
        Targets.Target(name, Action(fun _ -> action (parsed)))

    let cmd (name: string) commandsBefore steps action =
        let singleTarget = (parsed.TryGetResult SingleTarget |> Option.defaultValue false)

        let deps =
            match (singleTarget, commandsBefore) with
            | (true, _) -> []
            | (_, Some d) -> d
            | _ -> []

        let steps = steps |> Option.defaultValue []
        Targets.Target(name, deps @ steps, Action(action))

    step Clean.Name clean
    cmd Build.Name None (Some [ Clean.Name ]) <| fun _ -> build parsed

    cmd Test.Name (Some [ Build.Name ]) None <| fun _ -> test parsed
    cmd Integrate.Name (Some [ Build.Name ]) None <| fun _ -> integrate parsed

    step PristineCheck.Name pristineCheck
    step GeneratePackages.Name generatePackages
    step ValidatePackages.Name validatePackages

    cmd
        Release.Name
        (Some [ PristineCheck.Name; Test.Name; Integrate.Name ])
        (Some [ GeneratePackages.Name; ValidatePackages.Name ])
    <| fun _ -> release parsed

    cmd Publish.Name (Some [ Release.Name ]) None <| fun _ -> publish parsed