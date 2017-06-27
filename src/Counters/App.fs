open Elmish
open Elmish.WPF
open System



module UpDown =
    type Model =
        { Value : double }
       
    type Msg =
    | Up
    | Down

    let init _ = { Value = 1.2 }

    let update msg m =
        match msg with
        | Up    -> { m with Value = m.Value + 1.0 }
        | Down  -> { m with Value = m.Value - 1.0 }

    let viewBindings : ViewBindings<Model, Msg> = 
        [ "Value" |> Binding.oneWay (fun m -> m.Value)
          "Up"    |> Binding.cmd (fun _ -> Up)
          "Down"  |> Binding.cmd (fun _ -> Down) ]

    let view _ _ : ViewBindings<Model, Msg> = 
        viewBindings

module Counters = 
    
    type Model =
        UpDown.Model list
        

    type Msg =
        | Add
        | Remove
        | UpDownMsg of UpDown.Msg

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
    
    let view model dispatch : ViewBindings<Model, Msg>=
        [ 
          "Items" |> Binding.oneWay id
          "Add" |> Binding.cmd (fun _ -> Add)
          "Remove" |> Binding.cmd (fun _ -> Remove)
          "asddf" |> Binding.vm UpDown.init UpDown.viewBindings UpDownMsg
        ]

[<STAThread; EntryPoint>]
let main _ =
    Program.mkSimple Counters.init Counters.update Counters.view
    |> Program.withSubscription (fun _ -> [ ] |> List.map Cmd.ofSub |> Cmd.batch)
    |> Program.withConsoleTrace
    |> Program.runWindow (Views.MainWindow())