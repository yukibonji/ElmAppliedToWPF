open System

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Wpf


module UpDown =
    type Model =
        { Value : double }
       
    type Msg =
    | Up
    | Down

    let init _ = { Value = 0.0 }

    let update msg m =
        let op inc = {m with Value = m.Value + inc}
        match msg with
        | Up -> op 1.0
        | Down -> op -1.0

    let viewBindings source (model : ISignal<Model>) = 
        model |> Signal.map (fun m -> m.Value) |> Binding.toView source "Value"
        [ 
            Binding.createMessage "Up" Up source
            Binding.createMessage "Down" Down source
        ]

module Counters =   
    
    type Model =
        (Guid * UpDown.Model) list
        

    type Msg =
        | Add
        | Remove
        | UpDownMsg of UpDown.Msg * Guid

    let init _ = []

    let rec removeLast (l:'a list) : 'a list =
        match l with
        | [] -> []
        | [x] -> []
        | x::xs -> x::removeLast xs

    let update msg (model : Model) : Model =
        match msg with
        | Add -> List.append model [Guid.NewGuid(), UpDown.init()]
        | Remove -> removeLast model
        | UpDownMsg (udmsg, guid) -> model |> List.map (fun m -> (if (fst m) = guid then guid, UpDown.update udmsg (snd m) else m))
    
    let viewBindings source (model : ISignal<Model>) =
        model |> Signal.map ((List.map snd) >> List.map (fun m -> m.Value) >> (List.fold (+) 0.0)) |> Binding.toView source "Sum"
        [ 
          BindingCollection.toView source "Items" model (fun source2 tup -> (UpDown.viewBindings source2 (tup |> Signal.map snd)))
          |> Observable.map (fun (msg : UpDown.Msg, counter : Guid * UpDown.Model) -> UpDownMsg (msg, (fst counter)))
          Binding.createMessage "Add" Add source
          Binding.createMessage "Remove" Remove source
        ]


[<STAThread; EntryPoint>]
let main _ =
    let comp = Framework.basicApplication (Counters.init()) Counters.update Counters.viewBindings
    let window = Views.MainWindow
    Framework.runApplication System.Windows.Application window comp