namespace Elm

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Validation.Converters
open Gjallarhorn.Wpf

open System

type Getter<'model, 'a> = 'model -> 'a
type Setter<'model, 'msg> = 'model -> 'msg

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
     
    let convert source model (viewBindings : ViewBindings<'model, 'msg>) =
        viewBindings
        |> List.map (fun vb -> (vb source model))
        |> List.where Option.isSome
        |> List.map Option.get

    let app init update bindings : Framework.ApplicationCore<'model, 'msg> =
        Framework.basicApplication init update (fun s m -> convert s m bindings)
        

    // let twoWay (getter : Getter<'model, 'a>) (setter : Setter<'model, 'msg>) name =
    //     (fun (source : BindingSource) (model : ISignal<'model>) ->
    //         model 
    //         |> Signal.map getter 
    //         |> Binding.toFromViewValidated source name fromTo 
    //         |> Observable.map setter
    //         )