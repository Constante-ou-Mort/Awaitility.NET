using Awaitility.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Xunit.Sdk;

namespace Awaitility.ExecutorServices;

public abstract class BaseExecutorService(ConditionSettings conditionSettings) : IExecutorService
{
    public abstract void Until(Func<bool> func);
    public abstract void Until<T>(T obj, Func<T, bool> predicate);
    public abstract TSource Until<TSource>(Func<TSource> supplier, Func<TSource, bool> predicate);
    public abstract void UntilAsserted(Action assertAction);

    protected void ApplyPollIntervalIfDefined()
    {
        if (conditionSettings.PollInterval > TimeSpan.Zero)
        {
            Thread.Sleep(conditionSettings.PollInterval);
        }
    }

    protected void ApplyPollDelayIfDefined()
    {
        if (conditionSettings.PollDelay > TimeSpan.Zero)
        {
            Thread.Sleep(conditionSettings.PollDelay);
        }
    }

    protected void CheckDuringMetCondition(bool duringConditionMet)
    {
        if (conditionSettings.During > TimeSpan.Zero && !duringConditionMet)
        {
            throw new DuringConditionNotMetException(
                $"During condition was not met within the specified time '{conditionSettings.AtMost.TotalMilliseconds}' millisecond(s)");
        }
    }

    protected void ThrowTimeoutConditionException()
    {
        throw new TimeoutConditionException(
            string.IsNullOrEmpty(conditionSettings.Alias)
                ? $"Condition was not met within the specified time '{conditionSettings.AtMost.TotalMilliseconds}' millisecond(s)"
                : $"Condition with alias '{conditionSettings.Alias}' was not met within " +
                  $"the specified time '{conditionSettings.AtMost.TotalMilliseconds}' millisecond(s)");
    }

    protected void CheckFailFastCondition(bool failFastCondition)
    {
        if (failFastCondition)
        {
            throw new FailFastConditionException(string.IsNullOrEmpty(conditionSettings.FailFastMessage)
                ? "Fail fast condition has been reached"
                : conditionSettings.FailFastMessage);
        }
    }

    protected void CheckAtLeastCondition(DateTime startTime)
    {
        var taskEndTime = DateTime.Now;
        var elapsedTime = taskEndTime - startTime;

        if (elapsedTime < conditionSettings.AtLeast)
        {
            throw new AtLeastConditionException(
                $"Condition completed earlier than the specified minimum duration." +
                $" AtLeast: '{conditionSettings.AtLeast.TotalMilliseconds}' millisecond(s), " +
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
        catch (AssertionException)
        {
            //NUnit
            //ignore
            return false;
        }
        catch (XunitException)
        {
            //XUnit
            //ignore
            return false;
        }
        catch (UnitTestAssertException)
        {
            //MS TEST
            //ignore
            return false;
        }
    }
}