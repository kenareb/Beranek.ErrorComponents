# ErrorComponents

The `ErrorComponents` library is a set of fault tolerant wrapper classes around Action, Action<T>, Func<TResult>, Func<T, TResult>...
The wrapper classes offer a retry mechanism in case of an exception. Retry interval can be set to be linear or exponential.

It contains the following wrapper classes:

```C#
FaultTolerantAction
FaultTolerantAction<T>
FaultTolerantAction<T1, T2>
FaultTolerantFunc<TResult>
FaultTolerantFunc<T, TResult>
```

## Usage

```C#
int invocationCounter = 0;
var sw = new Stopwatch();

// This action will fail with an exception:
var error = new Action(() =>
{
    invocationCounter++;
    Console.WriteLine("Invocation " + invocationCounter);
    throw new NotImplementedException();
});

// Initialize a wrapper class, which will try to invoke the
// action 5 times before giving up. The default delay
// between invocations is one second:
var retry = new FaultTolerantAction(error, 5);

sw.Start();
var success = retry.Invoke();
sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds);

// Initialize a wrapper class, which will try to invoke the
// action 5 times before giving up. The default delay
// between invocations increases exponentially:
var retry2 = new FaultTolerantAction(error, 5, new ExponentialRetryStrategy(1000));

sw.Start();
retry2.Invoke();
sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds);

// Initialize a wrapper class, which will try to invoke the
// action 5 times before giving up, when the exception message
ends with 1 or 2:
var retry3 = new FaultTolerantAction(error, 5)
    .Filter(e => e.Message.EndsWith("1") || e.Message.EndsWith("2"));

sw.Start();
retry3.Invoke();
sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds);
```
