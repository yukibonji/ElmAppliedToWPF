namespace Elm

open Gjallarhorn
open Gjallarhorn.Bindable

open System

type ViewBinding<'model, 'msg> = BindingSource -> ISignal<'model> -> IObservable<'msg> option
type ViewBindings<'model, 'msg> = ViewBinding<'model, 'msg> list

module Bindings =
    let oneWay getter name : ViewBinding<'model, 'msg> =
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            model |> Signal.map getter |> Gjallarhorn.Bindable.Binding.toView source name
            Option<IObservable<'msg>>.None)

    let cmd setter name : ViewBinding<'model, 'msg> =
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            let msg = model |> Signal.map setter |> Signal.get
            Gjallarhorn.Bindable.Binding.createMessage name msg source
            |> Some)

    let cmdIf setter canExecute name : ViewBinding<'model, 'msg> =
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            let msg = model |> Signal.map setter |> Signal.get
            Gjallarhorn.Bindable.Binding.createMessageChecked name (model |> Signal.map canExecute) msg source
            |> Some)

    let convert source model (viewBindings : ViewBindings<'model, 'msg>) =
        viewBindings
        |> List.map (fun vb -> (vb source model))
        |> List.where Option.isSome
        |> List.map Option.get

    let app init update bindings : Framework.ApplicationCore<'model, 'msg> =
        Framework.basicApplication init update (fun s m -> bindings|> convert s m)

    let toComponent (getter : 'model -> 'a) (setter : 'b -> 'msg) (comp : ViewBindings<'a, 'b>) name : ViewBinding<'model, 'msg> =
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            let c s m = convert s m comp
            model |> Signal.map getter
            |> Gjallarhorn.Bindable.Binding.componentToView source name c
            |> Observable.map setter
            |> Some)

    let twoWayConverted getter setter conversion name : ViewBinding<'model, 'msg> =
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            model 
            |> Signal.map getter 
            |> Binding.toFromViewValidated source name conversion
            |> Observable.filter Option.isSome
            |> Observable.map Option.get
            |> Observable.map setter
            |> Some)

    let twoWay getter setter name : ViewBinding<'model, 'msg> =
        twoWayConverted getter setter Gjallarhorn.Validation.Converters.fromTo name

    let collection (comp : ViewBindings<'a, 'b>) getter setter name : ViewBinding<'model, 'msg> = 
        (fun source model -> 
            let c s m = comp |> convert s m
            BindingCollection.toView source name (model |> Signal.map getter) c
            |> Observable.map setter
            |> Some)