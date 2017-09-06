namespace Counters.Helpers

module ListExt = 
    let rec removeLast (l:'a list) : 'a list =
        match l with
        | [] -> []
        | [x] -> []
        | x::xs -> x::removeLast xs

    let rec replace predicate transform =
        List.map (fun a -> (if predicate a then transform a else a))