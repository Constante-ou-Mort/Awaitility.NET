using Awaitility.ExecutorServices;

namespace Awaitility;

public class ConditionAwaiter
{
    private readonly ConditionSettings _conditionSettings = new();

    private IExecutorService _executorService;

    public ConditionAwaiter()
    {
        _executorService = new TaskExecutorService(_conditionSettings);
    }

    public ConditionAwaiter Await() => this;

    public ConditionAwaiter With() => this;

    public ConditionAwaiter And() => this;

    public ConditionAwaiter AtMost(TimeSpan atMost)
    {
        _conditionSettings.AtMost = atMost;
        return this;
    }

    public ConditionAwaiter Alias(string alias)
    {
        _conditionSettings.Alias = alias;
        return this;
    }

    public ConditionAwaiter Between(TimeSpan atLeast, TimeSpan atMost)
    {
        return AtLeast(atLeast).And().AtMost(atMost);
    }

    /// <summary>
    /// Instructs Awaitility to execute the polling of the condition from the same as the test.
    /// This is an advanced feature and you should be careful when combining this with conditions that
    /// wait forever (or a long time) since Awaitility cannot interrupt the thread when it's using the same
    /// thread as the test. For safety you should always combine tests using this feature with a test framework specific timeout,
    /// for example in NUnit:
    /// [Test, Timeout(2000)]
    /// </summary>
    /// <returns></returns>
    public ConditionAwaiter PollInSameThread()
    {
        _executorService = new SameThreadExecutorService(_conditionSettings);
        return this;
    }

    public ConditionAwaiter FailFast(Func<bool> failFast, string failFastMessage = "")
    {
        _conditionSettings.FailFast = failFast;
        _conditionSettings.FailFastMessage = failFastMessage;
        return this;
    }

    public ConditionAwaiter Timeout(TimeSpan timeout)
    {
        _conditionSettings.AtMost = timeout;
        return this;
    }

    public ConditionAwaiter AtLeast(TimeSpan atLeast)
    {
        _conditionSettings.AtLeast = atLeast;
        return this;
    }

    public ConditionAwaiter During(TimeSpan during)
    {
        _conditionSettings.During = during;
        return this;
    }

    public ConditionAwaiter PollInterval(TimeSpan pollInterval)
    {
        _conditionSettings.PollInterval = pollInterval;
        return this;
    }

    public ConditionAwaiter PollDelay(TimeSpan pollDelay)
    {
        _conditionSettings.PollDelay = pollDelay;
        return this;
    }


    public ConditionAwaiter IgnoreExceptions(bool ignoreExceptions = true)
    {
        _conditionSettings.IgnoreExceptions = ignoreExceptions;
        return this;
    }

    public ConditionAwaiter IgnoreExceptions(params Type[] exceptionTypes)
    {
        _conditionSettings.ExceptionTypes = exceptionTypes;
        return this;
    }

    public TSource Until<TSource>(Func<TSource> supplier, Func<TSource, bool> predicate)
    {
        return _executorService.Until(supplier, predicate);
    }

    public void Until(Func<bool> predicate)
    {
        _executorService.Until(predicate);
    }

    public void UntilAsserted(Action assertAction)
    {
        _executorService.UntilAsserted(assertAction);
    }

    public void Until<T>(T obj, Func<T, bool> predicate)
    {
        _executorService.Until(obj, predicate);
    }
}