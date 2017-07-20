namespace Elm

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Validation.Converters

open System

type Getter<'model, 'a> = 'model -> 'a
type Setter<'model, 'msg> = 'model -> 'msg

type GjallarhornViewBinding<'model, 'msg> = BindingSource -> ISignal<'model> -> IObservable<'msg> option

type ViewBinding<'model, 'msg> = string * Variable<'model, 'msg>
and Variable<'model, 'msg> =
    | BindOneWay of GjallarhornViewBinding<'model, 'msg>

module Binding =
    let oneWay (getter : Getter<'model, 'a>) name =
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            model |> Signal.map getter |> Gjallarhorn.Bindable.Binding.toView source name
            Option<IObservable<'msg>>.None)

    let twoWay (getter : Getter<'model, 'a>) (setter : Setter<'model, 'msg>) name =
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            model 
            |> Signal.map getter 
            |> Binding.toFromViewValidated source name fromTo 
            |> Observable.map setter
            )