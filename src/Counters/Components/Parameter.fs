namespace Counters.Components

open System

module Parameter =
    type Model = { 
        Name: string
        Value : double 
    }

    type Msg =
    | Up
    | Down
    | EditValue of double
    | EditName of string

    let init name= 
        { Name = name; Value = 0.0 }

    let update msg m =
        let op inc = {m with Value = m.Value + inc}
        match msg with
        | Up -> op 1.0
        | Down -> op -1.0
        | EditValue d -> { m with Value = d }
        | EditName s -> { m with Name = s }

    let viewBindings : Elm.ViewBindings<Model, Msg> = 
        [
            "Up"    |> Elm.Bindings.cmd (fun _ -> Up)
            "Down"  |> Elm.Bindings.cmd (fun _ -> Down)
            "Value" |> Elm.Bindings.twoWay (fun m -> m.Value) EditValue
            "Name"  |> Elm.Bindings.twoWay (fun m -> m.Name) EditName
        ]