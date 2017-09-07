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
    let getDelta (e:MouseWheelEventArgs) = e.Delta/System.Windows.SystemParameters.WheelScrollLines
    let greaterThan limit (e:MouseWheelEventArgs) = (getDelta e) > limit
    let lessThan limit (e:MouseWheelEventArgs) = (getDelta e) < limit

type MouseWheelUp (modifiers) =
    inherit MouseWheelGesture((methods.greaterThan 0), modifiers)
    new() = 
        MouseWheelUp(ModifierKeys.None)

type MouseWheelDown (modifiers) =
    inherit MouseWheelGesture((methods.lessThan 0), modifiers)
    new() = 
        MouseWheelDown(ModifierKeys.None)
