namespace Elm

open Gjallarhorn
open Gjallarhorn.Bindable

open System

type Getter<'model, 'a> = 'model -> 'a
type Setter<'a, 'msg> = 'a -> 'msg

type ViewBinding<'model, 'msg> = BindingSource -> ISignal<'model> -> IObservable<'msg> option
type ViewBindings<'model, 'msg> = ViewBinding<'model, 'msg> list

module Bindings =
    let oneWay (getter : Getter<'model, 'a>) name : ViewBinding<'model, 'msg> =
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            model |> Signal.map getter |> Gjallarhorn.Bindable.Binding.toView source name
            Option<IObservable<'msg>>.None)

    let cmd (setter : Setter<'model, 'msg>) name : ViewBinding<'model, 'msg> =
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            let msg = model |> Signal.map setter |> Signal.get
            Gjallarhorn.Bindable.Binding.createMessage name msg source
            |> Some)

    let cmdCanExecute (setter : Setter<'model, 'msg>) (canExecute : 'model -> bool) name : ViewBinding<'model, 'msg> =
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
        Framework.basicApplication init update (fun s m -> convert s m bindings)
    
    
    let twoWayWithConversion (getter : Getter<'model, 'a>) (setter : Setter<'b, 'msg>) (conversion : Interaction.Validation<'a, 'b>) name : ViewBinding<'model, 'msg> =
        (fun (source : BindingSource) (model : ISignal<'model>) ->
            model 
            |> Signal.map getter 
            |> Binding.toFromViewValidated source name conversion
            |> Observable.filter Option.isSome
            |> Observable.map Option.get
            |> Observable.map setter
            |> Some)

    let twoWay (getter : Getter<'model, 'a>) (setter : Setter<'b, 'msg>) name : ViewBinding<'model, 'msg> =
        twoWayWithConversion getter setter Gjallarhorn.Validation.Converters.fromTo name

    
    let toCollection (comp : Component<'a, 'b>) (getter : 'model -> 'a) (setter : 'b * 'model -> 'msg) name : ViewBinding<'model list, 'msg> = 
        (fun source model -> 
            BindingCollection.toView source name model (fun s m -> m |> Signal.map getter |> (comp s))
            |> Observable.map setter
            |> Some)