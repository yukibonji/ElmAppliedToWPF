namespace Counters.Components

open System
open Gjallarhorn.Bindable

module Parameter =
    type Model = { 
        Name: string
        Value : double 
        Id : Guid
    }

    type Msg =
    | Up
    | Down
    | EditValue of double
    | EditName of string

    let init name= 
        { Name = name; Value = 0.0; Id = Guid.NewGuid() }

    let update msg m =
        let op inc = {m with Value = m.Value + inc}
        match msg with
        | Up -> op 1.0
        | Down -> op -1.0
        | EditValue d -> { m with Value = d }
        | EditName s -> { m with Name = s }
    
    [<CLIMutable>]
    type ViewModel = {
        Title : string
        Current : double
        Up : VmCmd<Msg>
        Down : VmCmd<Msg>
    }

    let design = { Title = ""; Current = 0.0; Up = Vm.cmd Up; Down = Vm.cmd Down }

    let viewBindings : Component<Model, Msg> = 
        Component.fromBindings<Model, Msg> [
            <@ design.Up @> |> Bind.cmd
            <@ design.Down @> |> Bind.cmd
            <@ design.Title @> |> Bind.twoWay (fun m -> m.Name) EditName
            <@ design.Current @> |> Bind.twoWay (fun m -> m.Value) EditValue
        ]