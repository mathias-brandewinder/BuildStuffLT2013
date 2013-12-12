let source = __SOURCE_DIRECTORY__

#load "NaiveBayes.fs"
open MachineLearning.NaiveBayes

open System
open System.IO
open System.Text
open System.Text.RegularExpressions

(* **********************************************
0. PREFACE: GETTING DATA

First let's grab some data!
The following code is boring, but will spare you some time
loading up a training and validation set.
The dataset is a collection of SMS messages, 
marked as "Spam" or "Ham".
The original dataset has been taken from 
the UC Irvine Machine Learning Repository:
http://archive.ics.uci.edu/ml/datasets/SMS+Spam+Collection
*)



// TODO: SIT BACK, RELAX, RUN THE CODE BELOW :) 



let trainingPath = source + "\SpamTraining"
let validationPath = source + "\SpamValidation"

// we define 2 classes, Ham or Spam
type Class = Spam | Ham

let spamOrHam (line: string) =
    if line.StartsWith("ham") then (Ham, line.Remove(0,4))
    elif line.StartsWith("spam") then (Spam, line.Remove(0,5))
    else failwith "What is this?"

let read path = 
    File.ReadAllLines(path)
    |> Array.map spamOrHam

let trainingSample = read trainingPath
let validationSample = read validationPath


(* **********************************************
CHAPTER 1: GET TO KNOW YOUR DATA

It's always a good idea to spend some time
to know your data, and become "intimate" with it.
The more you understand it, the better you can
help your machine get smart!
*)


// let's look at the 20 first "ham" items
let ham_20 = 
    trainingSample
    |> Array.filter (fun (cl, txt) -> cl = Ham)
    |> Seq.take 20
    |> Seq.iter (fun (cl, txt) -> printfn "%s" txt)

// TODO: DISPLAY 20 FIRST SPAM SMS



(* **********************************************
CHAPTER 2: ESTABLISH A BASELINE

It is crucial to establish a baseline for what
"a good/bad prediction" is. What we have to beat
here is a "naive" prediction, the most likely class.

What is the probability that a SMS message 
from the training set is spam or ham?
How about the validation sample?
Proba(SMS is Spam) = count(Spam SMS) / count(SMS)
*)



// TODO: COMPUTE PROBABILITY OF HAM, SPAM



(* **********************************************
CHAPTER 3: MEET REVEREND BAYES AND HIS THEOREM

Bayes Theorem enables incorporating additional data,
to refine a prediction, using the formula:
P(A|B) = P(B|A) x P(A) / P(B)
In this case: *)
// Proba (SMS is Spam, if SMS contains "chat) = 
//    Proba (SMS contains "chat", if SMS is Spam) x
//    Proba (SMS is Spam) / Proba (SMS contains "chat") 
(*
Our sample contains 4000 SMS: 
Spam SMS             13.350% (534)
Spam SMS with "chat" 03.558% (19)
Ham SMS              86.650% (3466)
Ham SMS with "chat"  00.288% (10)
SMS with "chat"      00.725% (29)
*)

let p_spam = 0.13350
let p_ham = 1. - p_spam

let p_chat = 0.00725

let p_chat_if_spam = 0.03558
let p_chat_if_ham = 0.00288



// TODO: IF A SMS MESSAGE CONTAINS "chat",
// WHAT IS THE PROBABILITY IT IS SPAM? HAM?



(* **********************************************
CHAPTER 4: CLASSIFY A MESSAGE BASED ON CONTENT

What is the probability that a spam SMS message
contains the word "ringtone"? "mom"? "800"? 
Quick math recap, just in case: 
*)
// Proba(Spam SMS contains "ringtone") = 
//    count(Spam SMS containing "ringtone") / count(Spam SMS)
// Proba(SMS is Spam if contains "ringtone") =
//    Proba(SMS contains "ringtone" if it is Spam) * 
//    Proba(SMS is Spam) / Proba(SMS contains "ringtone")

(*
This is a direct application of Bayes' Theorem:
(See Chapter 3)
Note that if we just want to decide whether
a message is ham or spam, we can ignore the 
Proba(SMS contains "chat") part.
*)



// TODO: COMPUTE PROBABILITY THAT 
// HAM, SPAM MESSAGE CONTAINS "ringtone"

// TODO: PROBA THAT MESSAGE IS HAM OR SPAM
// IF CONTAINS "ringtone", "800", ...



(* **********************************************
CHAPTER 5: NAIVE BAYES CLASSIFIER DEMO

The Naive Bayes classifier uses the same idea,
but instead of using one token, it will combine
the probabilities of each token into one aggregate
probability.
Instead of coding it from scratch, we'll use then
basic implementation from NaiveBayes.fs
Below is an illustration on how to train a classifier,
and use some of the built-in functions.
*)



// TODO: SIT BACK, RELAX, RUN THE CODE BELOW :) 



// select what tokens to use:
// a large part of how good the classifier is,
// depends on chosing good tokens.
let demoTokens = Set.ofList [ "chat"; "800"; "mom"; "ringtone"; "prize"; "you"]
// train a classifier using a sample and tokens
let demoClassifier = classifier bagOfWords trainingSample demoTokens

// look at what the classifier is doing :)
validationSample.[0..19]
|> Array.iter (fun (cl, text) -> 
printfn "%A -> %A / %s" cl (demoClassifier text) text)

// Let's compute the % correctly classified
validationSample
|> Seq.averageBy (fun (cl,txt) -> if cl = demoClassifier txt then 1. else 0.)
|> printfn "Correct: %f"


(* **********************************************
CHAPTER 6: SENTIMENT ANALYSIS

Looking at what words are frequently used
in different groups can give insight into
what "defines" these groups. This is often
referred to a "sentiment" analysis.
*)

// Extract tokens from training sample
let tokens = extractWords trainingSample
// Compute count of token in sample
let frequency = bagOfWords (prepare trainingSample) tokens



// TODO: MOST FREQUENT TOKENS IN HAM, SPAM?
// Hint: Map.toSeq will convert a Map<a,b> 
// into a sequence of tuples (a,b)


(* **********************************************
CHAPTER 7: STOP WORDS!

Did you note that some of the top words in
both Ham and Spam are just very common English
words, like "i", "you", "to"... ?
These are probably not very informative, and 
often called "stop words".
Let's create a "clean" list of tokens by removing
the stop words, and check our top tokens again.
*)

// http://www.textfixer.com/resources/common-english-words.txt
let stopWords = 
    let asString = "a,able,about,across,after,all,almost,also,am,among,an,and,any,are,as,at,be,because,been,but,by,can,cannot,could,dear,did,do,does,either,else,ever,every,for,from,get,got,had,has,have,he,her,hers,him,his,how,however,i,if,in,into,is,it,its,just,least,let,like,likely,may,me,might,most,must,my,neither,no,nor,not,of,off,often,on,only,or,other,our,own,rather,said,say,says,she,should,since,so,some,than,that,the,their,them,then,there,these,they,this,tis,to,too,twas,us,wants,was,we,were,what,when,where,which,while,who,whom,why,will,with,would,yet,you,your"
    asString.Split(',') |> Set.ofArray



// TODO: CLEAN TOKENS = TOKENS - STOP WORDS


// TODO, AGAIN: MOST FREQUENT TOKENS IN HAM, SPAM?



(* **********************************************
CHAPTER 8: OUR FIRST CLASSIFIER

Now that we have a decent list of tokens 
to start with, let's train a classifier.
*)



// TODO: PICK TOP 10 SPAM + TOP 10 HAM CLEAN TOKENS

// TODO: TRAIN CLASSIFIER WITH THESE TOKENS
let betterTokens = Set.ofList [ ] // THIS IS NOT RIGHT
// train a classifier using a sample and tokens
let betterClassifier = classifier bagOfWords trainingSample betterTokens

// TODO: COMPUTE % CORRECTLY CLASSIFIED ON VALIDATION SET



(* **********************************************
CHAPTER 9: BUT HERE'S MY NUMBER, SO CALL ME MAYBE

Remember in Chapter 4, when we checked for messages
containing "800"? Did you notice how many Spam SMSs
contain numbers (phone or text)?  
Can we make them into a feature / token?
*)

let numbersRegex = Regex(@"\d{3,}")
let replaceNumbers (text: string) = numbersRegex.Replace(text, "__number__")
let exampleReplacement = "Call 1800123456 for your free spam" |> replaceNumbers


// TODO: PRE-PROCESS TEXT TO DEAL WITH "NUMBERS":
// REPLACE NUMBERS WITH __number__


// TODO: TRAIN A CLASSIFIER ON PRE-PROCESSED
// TRAINING SET, AND EVALUATE QUALITY BY
// COMPUTING % CORRECTLY CLASSIFIED ON VALIDATION SET



(* **********************************************
EPILOGUE...

*)