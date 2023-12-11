using System;
using System.Threading;
using Awaitility.Exceptions;

namespace Awaitility.ExecutorServices;

public abstract class BaseExecutorService : IExecutorService
{
    private readonly ConditionSettings _conditionSettings;

    protected BaseExecutorService(ConditionSettings conditionSettings)
    {
        _conditionSettings = conditionSettings;
    }

    public abstract void Until(Func<bool> func);
    public abstract void Until<T>(T obj, Func<T, bool> predicate);
    public abstract TSource Until<TSource>(Func<TSource> supplier, Func<TSource, bool> predicate);
    public abstract void UntilAsserted(Action assertAction);

    protected void ApplyPollIntervalIfDefined()
    {
        if (_conditionSettings.PollInterval > TimeSpan.Zero)
        {
            Thread.Sleep(_conditionSettings.PollInterval);
        }
    }

    protected void ApplyPollDelayIfDefined()
    {
        if (_conditionSettings.PollDelay > TimeSpan.Zero)
        {
            Thread.Sleep(_conditionSettings.PollDelay);
        }
    }

    protected void CheckDuringMetCondition(bool duringConditionMet)
    {
        if (_conditionSettings.During > TimeSpan.Zero && !duringConditionMet)
        {
            throw new DuringConditionNotMetException(
                $"During condition was not met within the specified time '{_conditionSettings.AtMost.TotalMilliseconds}' millisecond(s)");
        }
    }

    protected void ThrowTimeoutConditionException()
    {
        throw new TimeoutConditionException(
            string.IsNullOrEmpty(_conditionSettings.Alias)
                ? $"Condition was not met within the specified time '{_conditionSettings.AtMost.TotalMilliseconds}' millisecond(s)"
                : $"Condition with alias '{_conditionSettings.Alias}' was not met within " +
                  $"the specified time '{_conditionSettings.AtMost.TotalMilliseconds}' millisecond(s)");
    }

    protected void CheckFailFastCondition(bool failFastCondition)
    {
        if (failFastCondition)
        {
            throw new FailFastConditionException(string.IsNullOrEmpty(_conditionSettings.FailFastMessage)
                ? "Fail fast condition has been reached"
                : _conditionSettings.FailFastMessage);
        }
    }

    protected void CheckAtLeastCondition(DateTime startTime)
    {
        var taskEndTime = DateTime.Now;
        var elapsedTime = taskEndTime - startTime;

        if (elapsedTime < _conditionSettings.AtLeast)
        {
            throw new AtLeastConditionException(
                $"Condition completed earlier than the specified minimum duration." +
                $" AtLeast: '{_conditionSettings.AtLeast.TotalMilliseconds}' millisecond(s), " +
                $"ElapsedTime: '{elapsedTime.Seconds}' second(s)");
        }
    }

    protected bool GetResultOfAssertAction(Action assertAction)
    {
        try
        {
            assertAction();
            return true;
        }
        catch (Exception e)
        {
            var type = e.GetType().FullName;

            if (type.Contains("AssertionException")
                || type.Contains("Xunit.Sdk.")
                || type.Contains("AssertFailedException"))
            {
                return false;
            }

            throw;
        }
    }
}