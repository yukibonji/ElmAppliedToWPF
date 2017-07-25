namespace Counters.Components

open System


module Parameters =   
    
    type Model =
        (Guid * Parameter.Model) list
        

    type Msg =
        | Add
        | Remove
        | UpDownMsg of Parameter.Msg * Guid

    let init = []

    let update msg (model : Model) : Model =
        match msg with
        | Add ->  model @ [Guid.NewGuid(), Parameter.init(sprintf "Param%d" (List.length model))]
        | Remove -> Counters.Helpers.ListExt.removeLast model
        | UpDownMsg (udmsg, guid) -> model |> Counters.Helpers.ListExt.replace (fun (g, m) -> g = guid) (fun (g, m) -> g, Parameter.update udmsg m)
    
    let private greaterThan ref value =
        value > ref

    let viewBindings : Elm.ViewBindings<Model, Msg> =
        [
            "Sum"    |> Elm.Bindings.oneWay ((List.map snd) >> List.map (fun m -> m.Value) >> (List.fold (+) 0.0))
            "Add"    |> Elm.Bindings.cmd (fun _ -> Add)
            "Remove" |> Elm.Bindings.cmdIf (fun _ -> Remove) (List.length >> (greaterThan 0))
            "Items"  |> Elm.Bindings.collection Parameter.viewBindings snd (fun (msg, (guid, model)) -> UpDownMsg (msg, guid))
        ]