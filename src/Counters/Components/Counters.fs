namespace Counters.Components

open System

open Gjallarhorn
open Gjallarhorn.Bindable

open UpDown
open Counters.Helpers

module Counters =   
    
    type Model =
        (Guid * UpDown.Model) list
        

    type Msg =
        | Add
        | Remove
        | UpDownMsg of UpDown.Msg * Guid

    let init _ = []


    let update msg (model : Model) : Model =
        match msg with
        | Add -> List.append model [Guid.NewGuid(), UpDown.init()]
        | Remove -> Counters.Helpers.ListExt.removeLast model
        | UpDownMsg (udmsg, guid) -> model |> List.map (fun m -> (if (fst m) = guid then guid, UpDown.update udmsg (snd m) else m))
    
    let private greaterThan ref value =
        value > ref

    let viewBindings source (model : ISignal<Model>) =
        model |> Signal.map (List.length >> (greaterThan 0)) |> Binding.toView source "RemoveActivated"
        model |> Signal.map ((List.map snd) >> List.map (fun m -> m.Value) >> (List.fold (+) 0.0)) |> Binding.toView source "Sum"
        [ 

          BindingCollection.toView source "Items" model (fun source2 tup -> (UpDown.viewBindings source2 (tup |> Signal.map snd)))
          |> Observable.map (fun (msg : UpDown.Msg, counter : Guid * UpDown.Model) -> UpDownMsg (msg, (fst counter)))
          Binding.createMessage "Add" Add source
          Binding.createMessage "Remove" Remove source
          
        ]