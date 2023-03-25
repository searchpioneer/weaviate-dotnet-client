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

module Program

open System
open Argu
open Bullseye
open ProcNet
open CommandLine

[<EntryPoint>]
let main argv =
    let parser = ArgumentParser.Create<Arguments>(programName = "./build.sh")

    let parsed =
        try
            let parsed = parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)
            let arguments = parsed.GetSubCommand()
            Some(parsed, arguments)
        with e ->
            printfn "%s" e.Message
            None

    match parsed with
    | None -> 2
    | Some (parsed, arguments) ->

        let target = arguments.Name

        Targets.Setup parsed arguments
        let swallowTypes = [ typeof<ProcExecException>; typeof<ExceptionExiter> ]

        let exitCode =
            try
                try
                    Targets.RunTargetsWithoutExiting([ target ], (fun e -> swallowTypes |> List.contains (e.GetType())), ":")
                    0
                with
                | :? InvalidUsageException as ex ->
                    Console.WriteLine ex.Message
                    2
                | :? TargetFailedException as ex -> 1
            finally
                Targets.teardown()

        exitCode
