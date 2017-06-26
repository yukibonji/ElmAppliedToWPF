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
        | UpDownMsg of UpDown.Msg

    let init _ = []

    let rec remove i l =
        match i, l with
        | 0, x::xs -> xs
        | i, x::xs -> x::remove (i - 1) xs
        | i, [] -> []

    let update msg model =
        match msg with
        | Add -> List.append model [UpDown.init()]
        | Remove -> remove (List.length model - 1) model
    
    let view model _ =
        let upDownViewBinding : ViewBindings<UpDown.Model, UpDown.Msg> = 
            [ "Value" |> Binding.oneWay (fun m -> m.Value)
              "Up"    |> Binding.cmd (fun _ -> UpDown.Up)
              "Down"  |> Binding.cmd (fun _ -> UpDown.Down) ]

        [ 
          "Items" |> Binding.oneWay (fun m -> m)
          "Add" |> Binding.cmd (fun _ -> Add)
          "Remove" |> Binding.cmd (fun _ -> Remove)
        ]

[<STAThread; EntryPoint>]
let main _ =
    Program.mkSimple Counters.init Counters.update Counters.view
    |> Program.withSubscription (fun _ -> [ ] |> List.map Cmd.ofSub |> Cmd.batch)
    |> Program.withConsoleTrace
    |> Program.runWindow (Views.MainWindow())