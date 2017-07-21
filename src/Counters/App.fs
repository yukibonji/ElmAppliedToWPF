open System

open Gjallarhorn
open Gjallarhorn.Bindable
open Gjallarhorn.Wpf

[<STAThread; EntryPoint>]
let main _ =
    let comp = Elm.Bindings.app Counters.Components.Parameters.init Counters.Components.Parameters.update Counters.Components.Parameters.viewBindings
    let window = Views.MainWindow
    Framework.runApplication System.Windows.Application window comp