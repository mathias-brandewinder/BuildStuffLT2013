(*
This file is strictly "for information", and not
directly useful for the exercise; it's the script
that was used to construct the test sample.
*)

#r @"..\packages\TweetSharp.2.3.1\lib\4.0\TweetSharp.dll"
#r @"..\packages\TweetSharp.2.3.1\lib\4.0\Hammock.ClientProfile.dll"

open System
open System.IO
open System.Threading
open System.Windows.Forms
open TweetSharp

// you can get a Twitter app key here:
// https://dev.twitter.com/apps/new

let consumerKey = "YOUR KEY GOES HERE"
let consumerSecret = "YOUR SECRET GOES HERE"
let service = TwitterService(consumerKey, consumerSecret)

let requestToken = service.GetRequestToken()
let uri = service.GetAuthorizationUri(requestToken)

let frm = new Form(TopMost = true, Visible = true, Width = 500, Height = 400)
let web = new WebBrowser(Dock = DockStyle.Fill)
frm.Controls.Add(web)
frm.Show
web.Navigate(uri)

// Exchange the Request Token for an Access Token
let verifier = "123456789" // <-- This is input into your application by your user
let access = service.GetAccessToken(requestToken, verifier)

service.AuthenticateWith(access.Token, access.TokenSecret);

let friendsOf userId =
    let options = ListFriendIdsOfOptions()
    options.UserId <- Nullable<int64>(userId)
    options.Count <- Nullable<int>(5000)
    userId, (service.ListFriendIdsOf options |> Seq.toArray)

let findUser userId =
    let options = GetUserProfileForOptions()
    options.UserId <- Nullable<int64>(userId)
    options.IncludeEntities <- Nullable<bool>(false)
    service.GetUserProfileFor options

let flatten (friendsOf:int64*int64[]) =
    let id, friends = friendsOf
    let flat = [| yield id; for i in friends -> i |]
    sprintf "%s" (String.Join(",", flat))

let rateLimit resources =
    let options = GetRateLimitStatusOptions()
    options.Resources <- resources
    let result = service.GetRateLimitStatus(options)
    result.Resources.Capacity

let buildSample userId =
    [| 
        printfn "Finding friends of %i" userId
        let id, friends = friendsOf userId
        yield ((id, friends) |> flatten)
        printfn "Retrieving friends of friends"
        for friend in friends do
            // Need to slow this down
            // Damn you rate limits on Twitter
            Thread.Sleep(TimeSpan(0,1,1))
            printfn "Finding friends of %i" friend
            let user = findUser friend
            let protectd = user.IsProtected
            if (protectd.HasValue && protectd.Value = false)
            then yield (friendsOf friend |> flatten)
    |]

let saveSample fileName data = 
    File.WriteAllLines(@"c:/users/mathias/desktop/" + fileName, data)

type User = { Id:int64; Friends: int64[] }

let usersFrom (data:string[]) = 
    data 
    |> Array.map (fun x -> x.Split(','))
    |> Array.map (fun line -> line |> Array.map (fun x -> int64 x))
    |> Array.map (fun x -> { Id = x.[0]; Friends = x.[1..] })

// @RelentlessDev 107460704L
buildSample 107460704L |> saveSample "RelentlessDev"