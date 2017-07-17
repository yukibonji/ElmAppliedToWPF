namespace Elm.Components

open Gjallarhorn
open Gjallarhorn.Bindable


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