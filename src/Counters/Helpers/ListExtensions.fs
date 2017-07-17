namespace Elm.Helpers

module ListExt = 
    let rec removeLast (l:'a list) : 'a list =
        match l with
        | [] -> []
        | [x] -> []
        | x::xs -> x::removeLast xs
