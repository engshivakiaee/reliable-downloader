# Questions

## How did you approach solving the problem?

- The first thing I did was to read the problem very carefully. I understood that clinicians need to receive files completely even if they experience intermittent internet disconnection and slow internet speeds. My approach should take care of internet speed. For HTTPClient, I changed the timeout to `Timeout.InfiniteTimeSpan` to avoid `TaskCancelledExceptions` when the speed is low. This is because the default timeout is 100 seconds.

- Second, we may encounter an `OutOfMemoryException` when receiving high volume files. In order to prevent this exception from occurring, I used `ReadAsStreamAsync`. In other words, we are dealing with streams of bytes instead of large arrays of bytes.

- Thirdly, the program hangs when it receives a file. To resolve this issue, I set the MaxConnectionsPerServer property of HttpClientHandler to `int.MaxValue`.

## How did you verify your solution works correctly?

I have done a smoke test on the program. Also, I have written 2 unit tests.Moreover I limited the internet speed by NetLimiter and waited to finish the download.
I canceled downloading when about 20 percent of file had beed downloaded, and started the program again to check `DownloadPartialContent` method works correctly.

## How long did you spend on the exercise?

It took about three hours.

## What would you add if you had more time and how?

- There are more unit tests I could add in separate files to test each service.
- Time remaining to report to the end user could have been implemented.
- I could consider design patterns at implementing the program.
- I could write README.
