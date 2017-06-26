open Elmish
open Elmish.WPF
open System



module UpDown =
    type Model =
        { Value : double }
       
    type Msg =
    | Up
    | Down

    let init () = { Value = 0.0 }

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
        { UpDown1 : UpDown.Model ; UpDown2 : UpDown.Model }

    type Msg =
        | UpDownMsg1 of UpDown.Msg
        | UpDownMsg2 of UpDown.Msg

    let init _ = { UpDown1 = UpDown.init(); UpDown2 = UpDown.init() }

    let update msg model =
        match msg with
        | UpDownMsg1 (m) -> { model with UpDown1 = (UpDown.update m model.UpDown1)}
        | UpDownMsg2 (m) -> { model with UpDown2 = (UpDown.update m model.UpDown2)}
        
    let view model _ =
        let upDownViewBinding : ViewBindings<UpDown.Model, UpDown.Msg> = 
            [ "Value" |> Binding.oneWay (fun m -> m.Value)
              "Up"    |> Binding.cmd (fun _ -> UpDown.Up)
              "Down"  |> Binding.cmd (fun _ -> UpDown.Down) ]
        ["UpDown1" |> Binding.vm (fun m -> m.UpDown1) upDownViewBinding (fun msg -> UpDownMsg1(msg))
         "UpDown2" |> Binding.vm (fun m -> m.UpDown2) upDownViewBinding (fun msg -> UpDownMsg2(msg)) ]

[<STAThread; EntryPoint>]
let main _ =
    Program.mkSimple Counters.init Counters.update Counters.view
    |> Program.withSubscription (fun _ -> [ ] |> List.map Cmd.ofSub |> Cmd.batch)
    |> Program.withConsoleTrace
    |> Program.runWindow (Views.MainWindow())