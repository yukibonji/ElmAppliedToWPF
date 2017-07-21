namespace Counters.Components

open System


open UpDown
open Counters.Helpers


module Counters =   
    
    type Model =
        (Guid * UpDown.Model) list
        

    type Msg =
        | Add
        | Remove
        | UpDownMsg of UpDown.Msg * Guid

    let init = []

    let update msg (model : Model) : Model =
        match msg with
        | Add ->  model @ [Guid.NewGuid(), UpDown.init]
        | Remove -> Counters.Helpers.ListExt.removeLast model
        | UpDownMsg (udmsg, guid) -> model |> Counters.Helpers.ListExt.replace (fun (g, m) -> g = guid) (fun (g, m) -> g, UpDown.update udmsg m)
    
    let private greaterThan ref value =
        value > ref

    let viewBindings : Elm.ViewBindings<Model, Msg> =
        [
            "Sum" |> Elm.Bindings.oneWay ((List.map snd) >> List.map (fun m -> m.Value) >> (List.fold (+) 0.0))
            "Add" |> Elm.Bindings.cmd (fun _ -> Add)
            "Remove" |> Elm.Bindings.cmdCanExecute (fun _ -> Remove) (List.length >> (greaterThan 0))
            "Items" |> Elm.Bindings.toCollection UpDown.viewBindings snd (fun (msg, (guid, model)) -> UpDownMsg (msg, guid))
        ]