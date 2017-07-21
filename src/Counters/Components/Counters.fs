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
        | Add -> List.append model [Guid.NewGuid(), UpDown.init]
        | Remove -> Counters.Helpers.ListExt.removeLast model
        | UpDownMsg (udmsg, guid) -> model |> List.map (fun m -> (if (fst m) = guid then guid, UpDown.update udmsg (snd m) else m))
    
    let private greaterThan ref value =
        value > ref

    let viewBindings : Elm.ViewBindings<Model, Msg> =
        [
            "Sum" |> Elm.Bindings.oneWay ((List.map snd) >> List.map (fun m -> m.Value) >> (List.fold (+) 0.0))
            "Add" |> Elm.Bindings.cmd (fun _ -> Add)
            "Remove" |> Elm.Bindings.cmdCanExecute (fun _ -> Remove) (List.length >> (greaterThan 0))
            "Items" |> Elm.Bindings.toCollection UpDown.viewBindings snd (fun (msg, (guid, model)) -> UpDownMsg (msg, guid))
        ]