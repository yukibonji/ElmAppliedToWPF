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
        | ListMsg of Parameter.Msg * int
        | FirstMsg of Parameter.Msg

    let init = { First = Parameter.init("Default"); List = []}

    let update msg (model : Model) : Model =
        match msg with
        | Add ->  { model with List = model.List @ [Parameter.init(sprintf "Param%d" (List.length model.List))] }
        | Remove -> { model with List = Counters.Helpers.ListExt.removeLast model.List }
        | ListMsg (udmsg, idx) -> { model with List = model.List |> List.mapi (fun i m -> (if i = idx then Parameter.update udmsg m else m)) }
        | FirstMsg udmsg -> { model with First = Parameter.update udmsg model.First }
    


    let viewBindings : Elm.ViewBindings<Model, Msg> =
        let greaterThan ref value = value > ref
        let getList m = m.List
        [
            "Sum"    |> Elm.Bindings.oneWay (getList >> List.map (fun m -> m.Value) >> List.fold (+) 0.0)
            "Add"    |> Elm.Bindings.cmd (fun _ -> Add)
            "Remove" |> Elm.Bindings.cmdIf (fun _ -> Remove) (getList >> List.length >> (greaterThan 0))
            "Items"  |> Elm.Bindings.vmCollection getList (fun msg idx -> ListMsg (msg, idx)) Parameter.viewBindings
            "First"  |> Elm.Bindings.vm (fun m -> m.First) FirstMsg Parameter.viewBindings 
        ]