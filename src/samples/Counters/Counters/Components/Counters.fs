namespace Counters.Components

open System
open Gjallarhorn.Bindable;

module Parameters =   
    
    type Model = {
        First : Parameter.Model
        List : Parameter.Model list
    }
        
        
    type Msg =
        | Add
        | Remove
        | ListMsg of Parameter.Msg * Guid
        | FirstMsg of Parameter.Msg

    let init = { First = Parameter.init("Default"); List = []}

    let update msg (model : Model) : Model =
        match msg with
        | Add ->  { model with List = model.List @ [Parameter.init(sprintf "Param%d" (List.length model.List))] }
        | Remove -> { model with List = Counters.Helpers.ListExt.removeLast model.List }
        | ListMsg (udmsg, id) -> { model with List = model.List |> List.map (fun m -> (if m.Id = id then Parameter.update udmsg m else m)) }
        | FirstMsg udmsg -> { model with First = Parameter.update udmsg model.First }
    
    type ViewModel = {
        Items : Parameter.Model list
        First : Parameter.Model
        Add : VmCmd<Msg>
        Remove : VmCmd<Msg>
        Sum : double
    }

    // type ParametersViewModel = {
    //     Parameters : Parameter.Model list
    // }

    // let designParameters = { Parameters = [] }

    // let parametersComponent = 
    //     Component.fromBindings [
    //         <@ designParameters.Parameters @> |> Bind.collection id Parameter.viewBindings fst
    //     ]

    let design = { Items = []; First = Parameter.init("Default"); Add = Vm.cmd Add; Remove = Vm.cmd Remove; Sum = 0.0 }
    let viewBindings =
        let greaterThan ref value = value > ref
        let getList (m : Model) = m.List
        let getFirst (m : Model) = m.First

        let getValue (m:Parameter.Model) = m.Value
        Component.fromBindings<Model, Msg> [
            <@ design.First @>  |> Bind.comp getFirst Parameter.viewBindings (fst >> FirstMsg)
            <@ design.Sum @>    |> Bind.oneWay (fun m -> List.fold (+) (m |> getFirst |> getValue) (m |> getList |> List.map getValue))
            <@ design.Add @>    |> Bind.cmd
            <@ design.Remove @> |> Bind.cmdIf (getList >> List.length >> (greaterThan 0))
            <@ design.Items @>  |> Bind.collection getList Parameter.viewBindings (fun (msg, m) -> ListMsg(msg, m.Id))
        ]