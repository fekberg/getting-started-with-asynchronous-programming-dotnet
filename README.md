# Module 2 - Getting Started with Asynchronous Programming in .NET

## Demo: Creating Your First Asynchronous Method
Introducing the async / await keywords with a simple Task.Delay to get the user comfortable with the keywords
We will look at:
- A simple async method with a Task.Delay
- Updating the UI after the await keyword
- Mentioning async void in an event

**Technology: WPF**

## Demo: Leveraging Existing Asynchronous APIs
This demo will load two files from disk and group them together using the asynchronous System.IO, this will be exposed through an API in ASP.NET.

We will then use the HttpClient in WPF to load a particular stock from the API.

The ASP.NET site will act as an API that we can use in our WPF examples as well

**Technology: WPF & ASP.NET**

## Demo: Understanding Continuations
This demo will talk about the UI thread and the continuation, we will also look at a nested asynchronous operation and that the continuation in the nested context does NOT run on the UI thread

Call our ASP.NET API using the HttpClient

**Technology: WPF**

## Demo: Handling Exceptions
As the previous demo raised an exception, this demo will cover how to handle exceptions in asynchronous context, and the fact that exceptions in async void are not possible to catch

We will ask for an incorrect stock in our application which will cause a 404 not found.

**Technology: WPF**

## Demo: Understanding the Flow of Asynchronous Methods with Multiple Continuations
We will use multiple continuations in one method, and call to methods that await multple different things.

**Technology: WPF**

# Module 3 - Using the Task Parallel Library in .NET

## Demo: Creating and Executing a Task
When the data is loaded from disk, we want to avoid locking up the UI when doing the grouping of the data
In this demo we will offload that work to a different Thread

**Technology: WPF**

## Demo: Working with the Result of a Task
When the data comes back from the execution of the Task, we want to display the result in the UI.

We will look at both await as well as ContinueWith.

**Technology: WPF**

## Demo: Chaning Tasks
When the grouping of data is done, we want to locate some of the stocks, and we do this using a ContiueWith, after that has executed, we want to handle that result in a different continue with to summarize the data

**Technology: WPF**

## Demo: Handling Exceptions withing Tasks
Try to update the UI thread from one of the Tasks or Continueations and show that this causes an exception

Show how to get the information about the exception and how to avoid it.

**Technology: WPF**

## Demo: Differences between Continuations
Show the differences between ContinueWith and Await

Covers both AggregatedException and the fact that await validates the results

**Technology: WPF**

## Demo: Cancelling a Running Task
Now that we have the data loaded in the app, introduce a search box to make it easier to filter the stocks

Each time you write a character the search stops, and when the user have not written a character for 300ms, the search triggers

**Technology: WPF**

## Demo: Waiting for One or All Tasks to Complete
Illustrate how we can load both of the files at once, and wait for one, or both to complete

**Technology: WPF**

# Module 4 - Parallel Programing Using the Parallel Extensions

## Demo: Introducing Parallel Processing with Parallel Extensions
Process earch group of stocks, and calculate a yearly average of the stock progress, do this in parallel using both Parallel For and ForEach

**Technology: WPF & ASP.NET**

## Demo: Combining Asynchronous and Parallel Programming
Wrap the Parallel execution in a Task, as it used the calling Thread

**Technology: WPF**

## Demo: Working with the Result of a Parallel Execution
Display the result of the parallel execution

**Technology: WPF**

## Demo: Working with Shared Variables
Illustrate what happens if we introduce a global counter for the stock price

**Technology: WPF**

# Module 5 - Asynchronous Programming Deep-dive

## Demo: Leveraging the Task Completion Source
Start Notepad using a Process, and in the Excited event display how Events is a way to call TaskCompletionSource to make it awaitable

**Technology: WPF**

## Demo: Working with Child and Parent Tasks
Show the difference between attached and detached child Tasks, and how they report completion/exceptions differently.

**Technology: WPF**

## Demo: Understanding the Async & Await Internals
Show the code that is generated for our async methods, display the difference between returning a Task and making something as async and using await in the final line.

Show the impact of the state machine and talk about why async void is a problem.

**Technology: WPF**

## Demo: Avoiding Deadlocks
With the knowledge of a State Machine and what happens with the Method, show why there is a potential deadlock when using Result or Wait, as well as how to avoid it.

**Technology: WPF**

## Demo: Getting the Progress of a Task
From the First # Module where we loaded the large files from disk, now use the IProgress interface to report on the progress of loading the files from disk.

**Technology: WPF**


# Module 6 - Asynchronous Programming Deep-dive in ASP.NET

## Demo: Understanding Configure Await
Talk about the context of where ConfigureAwait changes the behaviour

Display the difference it makes to accessing variables on the HttpContext

We will be using a web-version of the stocks application

**Technology: ASP.NET**

## Demo: Understanding the Synchronization Context
Illustrate how the Synchronization Context is captured when using ConfigureAwait, and how to use it to send messages back to the original thread.

**Technology: ASP.NET**

## Demo: Static Variables in Asynchronous Contexts
Show what happens if we try to access static variables in ASP.NET when using async and await

Introduce AsyncLocal to solve the same problem

**Technology: ASP.NET**