namespace Elm

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Wpf

open System

type ViewBinding<'model, 'msg> = BindingSource -> ISignal<'model> -> IObservable<'msg> option
type ViewBindings<'model, 'msg> = ViewBinding<'model, 'msg> list


module ViewBindings =
    let toComponent (viewBindings : ViewBindings<'model, 'msg>) : Component<'model, 'msg> =
        (fun source model ->
        viewBindings
        |> List.map (fun vb -> (vb source model))
        |> List.where Option.isSome
        |> List.map Option.get)



module Bindings =
    let private wrapRO getter f : ViewBinding<'model, 'msg> = 
        (fun (source : BindingSource) (model : ISignal<'model>) ->
                model
                |> Signal.map getter
                |> (f source)
                None)

    let private wrapRW getter setter f : ViewBinding<'model, 'msg> = 
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            model
            |> Signal.map getter
            |> (f source)
            |> Observable.filter Option.isSome
            |> Observable.map Option.get
            |> Observable.map setter
            |> Some)

    let vm (getter : 'model -> 'a) (setter : 'b -> 'msg) (comp : ViewBindings<'a, 'b>) name : ViewBinding<'model, 'msg> =
        wrapRW getter setter (fun source  model-> Gjallarhorn.Bindable.Binding.componentToView source name (ViewBindings.toComponent comp) model |> Observable.map Some)

    let vmCollection getter setter (comp : ViewBindings<'a, 'b>) name : ViewBinding<'model, 'msg> = 
        (fun source (model : ISignal<'model>) ->
            let models = model |> Signal.map (getter >> Seq.mapi (fun i m -> m, i))
            let c s m = ViewBindings.toComponent comp s (Signal.map fst m)
            BindingCollection.toView source name models c 
            |> Observable.map (fun (msg,(m, id)) -> setter msg id)
            |> Some)

    let oneWay getter name : ViewBinding<'model, 'msg> =
        wrapRO getter (fun source -> Gjallarhorn.Bindable.Binding.toView source name)
        
    let twoWayConverted getter setter conversion name : ViewBinding<'model, 'msg> =
        wrapRW getter setter (fun source -> Binding.toFromViewValidated source name conversion)
        
    let twoWayValidated getter setter name : ViewBinding<'model, 'msg> =
        twoWayConverted getter setter Gjallarhorn.Validation.Converters.fromTo name

    let twoWay getter setter name : ViewBinding<'model, 'msg> =
        wrapRW getter setter (fun source model -> Binding.toFromView source name model |> Observable.map Option.Some)

    let cmd msg name : ViewBinding<'model, 'msg> =
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            Gjallarhorn.Bindable.Binding.createMessage name msg source
            |> Some)

    let cmdIf msg canExecute name : ViewBinding<'model, 'msg> =
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            Gjallarhorn.Bindable.Binding.createMessageChecked name (model |> Signal.map canExecute) msg source
            |> Some)

module App =            
    let app init update bindings : Framework.ApplicationCore<'model, 'msg> =
        Framework.basicApplication init update (bindings|> ViewBindings.toComponent)

    let run window app =
        Framework.runApplication System.Windows.Application window app
