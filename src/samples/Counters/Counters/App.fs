open System

open Serilog


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
    let comp = Elm.App.app Counters.Components.Parameters.init update Counters.Components.Parameters.viewBindings
    let window = Views.MainWindow
    Elm.App.run window comp