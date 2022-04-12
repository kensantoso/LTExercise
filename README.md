# LTExercise


 
### Assumptions

First off was that I assumed logs were in integers representing military time(1330 == 13:30).
I didn't do any validation on this, but it can be easily added. I also assumed names would be string, but the solution can be based off any key name.
As was in the spec I assumed there will be a logout for every login event. If given more time I would have definitely put exception handling around this.

### Problem solving approach
I just want to say that this approach that I have chosen is definitely not the most efficient. I'm not really a pro with algorithms
but given time I believe I could find a more efficient solution. I just wanted to get this thing working. First thing I thought of was using interval trees, but saw that the requirement would need subsets. 
I first sorted the logs based on time, then looped through chronologically and recorded login/logout events. At the end of each iteration I stored a list of current open sessions that had 2 or more users(i.e. if a user was still logged in). 
At the end of it I had a list of login sessions which I then created a alphabetical key/value store so the same group of users would have the same key and a list of sessions and I also logged users out if the time matched a login time to solve the interval problem. 
From there I just sorted by largest list of users online at the same time with a list of greater than 2. 

#### What I would have done if I had more time
First off, I have a few linq queries that could be optimised. My first thought for using interval trees could probably work. I haven't used them professionally, but in one of my personal projects I have.
I think putting start/end times in the interval tree with login session, then iterating through time to find the max number of sessions could be very efficient. 
Once you had a max number of concurrent sessions you could increment the max number and then record the names and sessions. This would need loading of the whole list. 


I usually do development this way and then use a profiler to test code hot spots to see where my inefficient algorithms are. 

### The solution. 

I created a few unit tests to cover the logic(no overlaps, make sure to return correct values etc). The integration test just covers the API. I initially did this as a console app but I think a webapi is a bit nicer. 
I could have done some more on editor configs, linting, and more abstraction if that's what you need or what your coding standards are. The tests are setup in a way that you can just feel free to edit the code and it should catch. 
I just loaded the data via a csv but it should be fairly easy to sub in your own data store. 

### CI/CD

I used github actions to deploy this to a lambda. The endpoint is live.
In the github folder the following pipeline runs on any changes to `main`.
* Setting up .Net SDK and AWS Lambda CLI
* Run unit and integration tests
* Build and package app into a zip file suitable for upload to AWS
* Log into AWS (this requires you to [configure AWS creds in GitHub][aws-action])
* Use CloudFormation to deploy the Lambda function and HTTP API

