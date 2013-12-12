(*
0. GETTING STARTED: DATA

The dataset included in the file RelentlessDev
is a list of users, followed by the ids of people
that user follows.
Ex: 123,456,789 indicates that the user with Id
123 follows 2 users, with Ids 456 and 789.
The first line of the dataset is our root user,
@RelentlessDev ("Leeds' most relentless .Net developer!"),
every record afterwards is users he follows, and their friends.
In other words, it is a 2 levels deep network.
*)


// TODO: run the code below.


open System
open System.IO
open System.Windows.Forms

// Note the usage of Set to represent Friends,
// instead of Array or List.
type User = { UserId: int64; Friends: int64 Set }

let root = __SOURCE_DIRECTORY__
let path = root + @"\RelentlessDev"

let users = 
    File.ReadAllLines(path)
    |> Array.map (fun line -> line.Split(','))
    |> Array.map (fun line -> line |> Array.map (fun x -> x |> int64))
    |> Array.map (fun line -> { UserId = line.[0]; Friends = line.[1..] |> Set.ofArray })

// We extract our root user "me", and his friends:
let me, friends = users.[0], users.[1..]


(*
1. VIEWING A USER PROFILE

Working with the Twitter API turned out to be
surprisingly unpleasant, so here is a quick-and-dirty
utility function to pop up a browser in Winforms,
and display the profile of a user identified by
his or her user id (an int64, denoted let x = 123L)
*)

let show (uri:string) =
    let frm = new Form(TopMost = true, Visible = true, Width = 400, Height = 600)
    let web = new WebBrowser(Dock = DockStyle.Fill)
    frm.Controls.Add(web)
    frm.Show ()
    web.Navigate(uri)

let showProfile (id:int64) =
    sprintf "https://twitter.com/intent/user?user_id=%i" id |> show



// TODO: who is hiding behind the id 25663453L? 94144339L?



(*
2. FINDING PEOPLE LIKE...

As a warm-up, let's start by finding users 
similar to a given user. In this case, we'll
extract from RelentlessDev's friends the 5
users most similar to him.
We could define "similar" in a variety of ways.
In this case, we'll start by computing the
proportion of friends they have in common:
similarity = common friends between A and B / total friends of A and B
*)


//<F# Tutorial>
//The F# immutable Set should come in handy here:
let someData = [ 1; 2; 2; 2; 3; 50; ]
let someSet = someData |> Set.ofList

let newSet = someSet |> Set.add 42
let anotherSet = newSet |> Set.remove 2

let set1 = [ 1 .. 5 ] |> Set.ofList
let set2 = [ 3 .. 8 ] |> Set.ofList
let intersection = Set.intersect set1 set2
let union = Set.union set1 set2
let diff = Set.difference set1 set2
let unionMany = Set.unionMany [ someSet; set1; set2; ]
//</F# Tutorial>



// TODO: write a similarity function that takes 2 users, 
// and evaluates their similarity, computed as a float.


// TODO: among the direct friends of the root user,
// find the top 5 users most similar to him.



(*
3. FINDING SUITABLE CANDIDATES TO FOLLOW
People I might want to follow are:
* People I don't follow already (duh!)
* People my friends follow are a good place to start
*)



// TODO: create an array of all the friends
// of my friends that I might want to follow.



(*
4. BASIC RECOMMENDATION
One good place to decide who to follow
would be to look at who most of my friends follow.
*)



// TODO: from our list of candidates,
// find the top 10 Twitter users that
// most of my friends follow.


 
(*
5. REFINING THE RECOMMENDATION
One direction to improve the recommendation would be
to think about who is most similar to me. If someone
is similar to me, chances are, I will be interested
in who he/she follows more than someone totally different
from me (of course, it's not all that clear cut).
*)

//<F# Tutorial>
// The F# immutable Map would be a convenient way
// to store the similarity between me and all my friends.
let map0 = 
    Map.empty 
    |> Map.add "Alpha" 1
    |> Map.add "Bravo" 2
map0.["Alpha"]
[ "A",1; "B",2; "C",3 ] |> Map.ofSeq
//</F# Tutorial>



// TODO: create a map that associates to each
// of my friend's Id how similar they are to me



// TODO: instead of simply counting how many times
// a candidate is followed by my friends, let's
// do a weighted count