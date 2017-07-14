open Elmish
open Elmish.WPF
open System



module UpDown =
    type Model =
        { Value : double
          Id : Guid}
       
    type Msg =
    | Up of Guid
    | Down of Guid

    let init _ = { Value = 0.0; Id = Guid.NewGuid() }

    let update msg m =
        let op inc id model = if (id = model.Id) then {model with Value = model.Value + inc} else model
        match msg with
        | Up id   -> op 1.0 id m
        | Down id -> op -1.0 id m

    let viewBindings : ViewBindings<Model, Msg> = 
        [ "Value" |> Binding.oneWay (fun m -> m.Value)
          "Up"    |> Binding.cmd (fun _ m -> Up m.Id)
          "Down"  |> Binding.cmd (fun _ m -> Down m.Id) ]

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
        | UpDownMsg msg -> model |> List.map (UpDown.update msg)
    
    let view model dispatch : ViewBindings<Model, Msg>=
        [ 
          "Items" |> Binding.oneWay id
          "Add" |> Binding.cmd (fun _ _ -> Add)
          "Remove" |> Binding.cmd (fun _ _ -> Remove)
          "Up" |> Binding.cmd (fun p _ -> p :?> Guid |> UpDown.Msg.Up |> UpDownMsg)
          "Down" |> Binding.cmd (fun p _ -> p :?> Guid |> UpDown.Msg.Down |> UpDownMsg)
        ]

[<STAThread; EntryPoint>]
let main _ =
    Program.mkSimple Counters.init Counters.update Counters.view
    |> Program.withSubscription (fun _ -> [ ] |> List.map Cmd.ofSub |> Cmd.batch)
    |> Program.withConsoleTrace
    |> Program.runWindow (Views.MainWindow())