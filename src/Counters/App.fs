open System

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Wpf

open Elm.Components.Counters

[<STAThread; EntryPoint>]
let main _ =
    let comp = Framework.basicApplication (Elm.Components.Counters.init()) Elm.Components.Counters.update Elm.Components.Counters.viewBindings
    let window = Views.MainWindow
    Framework.runApplication System.Windows.Application window comp