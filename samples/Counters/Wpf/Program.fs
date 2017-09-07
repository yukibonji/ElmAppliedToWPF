open System

open Serilog

open Gjallarhorn.Wpf
open Gjallarhorn.Bindable.Framework



[<STAThread; EntryPoint>]
let main _ =

    Log.Logger <- LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.ColoredConsole()
        .CreateLogger()
    let log (update:'a -> 'b -> 'b) = 
        (fun e m ->
            let state = update e m
            Log.Information("State {@State}", state)
            state)
    let update = log Counters.Components.Parameters.update
    let comp = Framework.basicApplication Counters.Components.Parameters.init update Counters.Components.Parameters.viewBindings
    Gjallarhorn.Wpf.Framework.RunApplication (Func<_>(Views.MainWindow), comp)
    1