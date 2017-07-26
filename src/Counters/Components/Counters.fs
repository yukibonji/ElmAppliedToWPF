namespace Counters.Components

open System


module Parameters =   
    
    type Model = {
        First : Parameter.Model
        List : Parameter.Model list
    }
        
        

    type Msg =
        | Add
        | Remove
        | ListMsg of Parameter.Msg * Parameter.Model
        | FirstMsg of Parameter.Msg

    let init = { First = Parameter.init("Default"); List = []}

    let update msg (model : Model) : Model =
        match msg with
        | Add ->  { model with List = model.List @ [Parameter.init(sprintf "Param%d" (List.length model.List))] }
        | Remove -> { model with List = Counters.Helpers.ListExt.removeLast model.List }
        | ListMsg (udmsg, udmodel) -> { model with List = model.List |> Counters.Helpers.ListExt.replace (fun m -> m.Id = udmodel.Id) (fun m -> Parameter.update udmsg m) }
        | FirstMsg udmsg -> { model with First = Parameter.update udmsg model.First }
    
    let private greaterThan ref value =
        value > ref

    let viewBindings : Elm.ViewBindings<Model, Msg> =
        let list m = m.List
        [
            "Sum"    |> Elm.Bindings.oneWay (list >> List.map (fun m -> m.Value) >> (List.fold (+) 0.0))
            "Add"    |> Elm.Bindings.cmd (fun _ -> Add)
            "Remove" |> Elm.Bindings.cmdIf (fun _ -> Remove) (list >> List.length >> (greaterThan 0))
            "Items"  |> Elm.Bindings.collection Parameter.viewBindings list (fun (msg, model) -> ListMsg (msg, model))
            "First"  |> Elm.Bindings.toComponent (fun m -> m.First) FirstMsg Parameter.viewBindings 
        ]