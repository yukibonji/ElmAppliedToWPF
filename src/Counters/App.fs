open System

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Wpf

open Counters.Components.Counters

[<STAThread; EntryPoint>]
let main _ =
    let comp = Framework.basicApplication (Counters.Components.Counters.init()) Counters.Components.Counters.update Counters.Components.Counters.viewBindings
    let window = Views.MainWindow
    Framework.runApplication System.Windows.Application window comp