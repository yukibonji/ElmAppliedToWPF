namespace Elm.Components

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Validation.Converters


open System

module UpDown =
    type Model =
        { Value : double }

       
    type Msg =
    | Up
    | Down
    | Edit of string

    let init _ = 
        { Value = 0.0 }

    let update msg m =
        let op inc = {m with Value = m.Value + inc}
        match msg with
        | Up -> op 1.0
        | Down -> op -1.0
        | Edit s -> { m with Value = Double.Parse s }

    let viewBindings source (model : ISignal<Model>) = 
        [ 
            model 
            |> Signal.map (fun m -> m.Value) 
            |> Binding.toFromViewValidated source "Value" fromTo 
            |> Observable.map (fun m -> Edit m.Value)
            Binding.createMessage "Up" Up source
            Binding.createMessage "Down" Down source
        ]