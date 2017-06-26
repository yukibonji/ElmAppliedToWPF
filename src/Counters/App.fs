open Elmish
open Elmish.WPF
open System



/// The model bound to the view
type UpDownModel =
    { Value : double }

type Model =
    { UpDown1 : UpDownModel ; UpDown2 : UpDownModel }

/// Different messages that can flow through the system.
type UpDownMsg =
    | Up
    | Down

type Msg =
    | UpDownMsg1 of UpDownMsg
    | UpDownMsg2 of UpDownMsg

[<AutoOpen>]
/// Contains code for event-driven subscriptions.
module Subscriptions =
    let none = null
    

/// Start state
let initUpDown () = { Value = 0.0 }
let init _ = { UpDown1 = initUpDown(); UpDown2 = initUpDown() }
        
/// The main Msg loop.
let updateUpDown msg m =
    match msg with
    | Up    -> { m with Value = m.Value + 1.0 }
    | Down  -> { m with Value = m.Value - 1.0 }
   
let update msg model =
    match msg with
    | UpDownMsg1 (m) -> { model with UpDown1 = (updateUpDown m model.UpDown1)}
    | UpDownMsg2 (m) -> { model with UpDown2 = (updateUpDown m model.UpDown2)}


/// Hooks up WPF bindings to the model.
let viewUpDown _ _ =
    [ "Value" |> Binding.oneWay (fun m -> m.Value)
      "Up"    |> Binding.cmd (fun _ -> Up)
      "Down"  |> Binding.cmd (fun _ -> Down) ]

let view model _ =
    let upDownViewBinding : ViewBindings<UpDownModel, UpDownMsg> = 
        [ "Value" |> Binding.oneWay (fun m -> m.Value)
          "Up"    |> Binding.cmd (fun _ -> Up)
          "Down"  |> Binding.cmd (fun _ -> Down) ]
    ["UpDown1" |> Binding.vm (fun m -> m.UpDown1) upDownViewBinding (fun msg -> UpDownMsg1(msg))
     "UpDown2" |> Binding.vm (fun m -> m.UpDown2) upDownViewBinding (fun msg -> UpDownMsg2(msg)) ]

[<STAThread; EntryPoint>]
let main _ =
    Program.mkSimple init update view
    |> Program.withSubscription (fun _ -> [ ] |> List.map Cmd.ofSub |> Cmd.batch)
    |> Program.withConsoleTrace
    |> Program.runWindow (Views.MainWindow())