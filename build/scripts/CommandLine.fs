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

module CommandLine

open Argu
open Microsoft.FSharp.Reflection

type Arguments =
    | [<CliPrefix(CliPrefix.None); SubCommand>] Clean
    | [<CliPrefix(CliPrefix.None); SubCommand>] Build
    | [<CliPrefix(CliPrefix.None); SubCommand>] Test
    | [<CliPrefix(CliPrefix.None); SubCommand>] Integrate

    | [<CliPrefix(CliPrefix.None); Hidden; SubCommand>] PristineCheck
    | [<CliPrefix(CliPrefix.None); Hidden; SubCommand>] GeneratePackages
    | [<CliPrefix(CliPrefix.None); Hidden; SubCommand>] ValidatePackages
    | [<CliPrefix(CliPrefix.None); SubCommand>] Release

    | [<CliPrefix(CliPrefix.None); SubCommand>] Publish

    | [<Inherit; AltCommandLine("-s")>] SingleTarget of bool
    | [<Inherit; AltCommandLine("-c")>] CleanCheckout of bool

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Clean _ -> "Clean known output locations"
            | Build _ -> "Run build"
            | Test _ -> "Run all of the unit tests"
            | Integrate _ -> "Run all of the integration tests "
            | Release _ -> "Runs build, all tests, and create and validates the packages without publishing them"
            | Publish _ -> "Runs the full release"
            | SingleTarget _ -> "Runs the provided sub command without running their dependencies"
            | CleanCheckout _ -> "Skip the clean checkout check that guards the release/publish targets"
            | PristineCheck
            | GeneratePackages
            | ValidatePackages _ -> "Undocumented, dependent target"

    member this.Name =
        match FSharpValue.GetUnionFields(this, typeof<Arguments>) with
        | case, _ -> case.Name.ToLowerInvariant()
