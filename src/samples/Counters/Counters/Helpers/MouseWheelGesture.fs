namespace Helpers.Gestures

open System
open System.Windows.Input

type MouseWheelGesture (predicate ,modifiers: ModifierKeys) =
    inherit MouseGesture(MouseAction.WheelClick, modifiers)
    member private this.Predicate = predicate
    override this.Matches (targetElement, inputEventArgs) =
        if (not (base.Matches(targetElement, inputEventArgs))) then
            false
        else
            let args = inputEventArgs :?> MouseWheelEventArgs
            match args with
            | null -> false
            | _ -> (this.Predicate args)

module methods =
    let greaterThan limit (e:MouseWheelEventArgs) = e.Delta > limit
    let lessThan limit (e:MouseWheelEventArgs) = e.Delta < limit

type MouseWheelUp (modifiers) =
    inherit MouseWheelGesture((methods.greaterThan 0), modifiers)
    new() = 
        MouseWheelUp(ModifierKeys.None)

type MouseWheelDown (modifiers) =
    inherit MouseWheelGesture((methods.lessThan 0), modifiers)
    new() = 
        MouseWheelDown(ModifierKeys.None)
