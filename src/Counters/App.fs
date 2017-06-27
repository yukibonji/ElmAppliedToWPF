open Elmish
open Elmish.WPF
open System



module UpDown =
    type Model =
        { Value : double }
       
    type Msg =
    | Up
    | Down

    let init () = { Value = 1.2 }

    let update msg m =
        match msg with
        | Up    -> { m with Value = m.Value + 1.0 }
        | Down  -> { m with Value = m.Value - 1.0 }

    let view _ _ =
        [ "Value" |> Binding.oneWay (fun m -> m.Value)
          "Up"    |> Binding.cmd (fun _ -> Up)
          "Down"  |> Binding.cmd (fun _ -> Down) ]

module Counters = 
    
    type Model =
        UpDown.Model list
        

    type Msg =
        | Add
        | Remove

    let init _ = []

    let rec removeLast (l:'a list) : 'a list =
        match l with
        | [] -> []
        | [x] -> []
        | x::xs -> x::removeLast xs

    let update msg model =
        match msg with
        | Add -> List.append model [UpDown.init()]
        | Remove -> removeLast model
    
    let view model _ =
        let upDownViewBinding : ViewBindings<UpDown.Model, UpDown.Msg> = 
            [ "Value" |> Binding.oneWay (fun m -> m.Value)
              "Up"    |> Binding.cmd (fun _ -> UpDown.Up)
              "Down"  |> Binding.cmd (fun _ -> UpDown.Down) ]

        [ 
          "Items" |> Binding.oneWay id
          "Add" |> Binding.cmd (fun _ -> Add)
          "Remove" |> Binding.cmd (fun _ -> Remove)
        ]

[<STAThread; EntryPoint>]
let main _ =
    Program.mkSimple Counters.init Counters.update Counters.view
    |> Program.withSubscription (fun _ -> [ ] |> List.map Cmd.ofSub |> Cmd.batch)
    |> Program.withConsoleTrace
    |> Program.runWindow (Views.MainWindow())