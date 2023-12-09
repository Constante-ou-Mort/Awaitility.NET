# Awaitility.NET
Port of Awaitility from Java to NET world. Reference to original: https://github.com/awaitility/awaitility

# Currently there are 2 implementations of wait logic:
1. We wait the func in the task that runs on the new thread. In this case if timeout is reached, Awaitility will interrupt the execution and TimeoutException will be thrown. It's by default behaviour.
2. We wait the func on the same thread. In this case we cannot interrupt the execution, so be carefull with that. To use it just call PollInSameThread() method.
3. Async implementation will be added in 2024. Happy New Year!
 
# Examples of code usage:
Currently examples of code usage can be found in Awaitility.Tests.
It is recommended to use static import: using static Awaitility.Awaitility;

//Examples will be added later...
Await()
    .AtMost(OneHundredMilliseconds)
    .PollInterval(TenSeconds)
    .Until(() => OrderService.GetOrder(order.Id).Status == OrderStatus.Ready);

If you want to use 1-st wait logic implementation, but you have a getter that depends on the thread, there is Until(driver, (dr) => dr.Displayed) implementation where you pass your instance as an argument.
