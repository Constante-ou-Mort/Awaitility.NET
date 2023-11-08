using Awaitility.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Xunit.Sdk;

namespace Awaitility;

public class ConditionAwaiter
{
    private TimeSpan _atMost = Durations.TenSeconds;
    private TimeSpan _atLeast = TimeSpan.Zero;
    private TimeSpan _during = TimeSpan.Zero;
    private TimeSpan _pollInterval = TimeSpan.Zero;
    private TimeSpan _pollDelay = TimeSpan.Zero;
    private Type[] _exceptionTypes = Array.Empty<Type>();

    private Func<bool> _failFast;
    private string _failFastMessage;
    private bool _ignoreExceptions;
    private string _alias;

    public ConditionAwaiter Await() => this;

    public ConditionAwaiter With() => this;

    public ConditionAwaiter And() => this;

    public ConditionAwaiter AtMost(TimeSpan atMost)
    {
        _atMost = atMost;
        return this;
    }

    public ConditionAwaiter Alias(string alias)
    {
        _alias = alias;
        return this;
    }

    public ConditionAwaiter Between(TimeSpan atLeast, TimeSpan atMost)
    {
        return AtLeast(atLeast).And().AtMost(atMost);
    }

    public ConditionAwaiter FailFast(Func<bool> failFast, string failFastMessage = "")
    {
        _failFast = failFast;
        _failFastMessage = failFastMessage;
        return this;
    }

    public ConditionAwaiter Timeout(TimeSpan timeout)
    {
        _atMost = timeout;
        return this;
    }

    public ConditionAwaiter AtLeast(TimeSpan atLeast)
    {
        _atLeast = atLeast;
        return this;
    }

    public ConditionAwaiter During(TimeSpan during)
    {
        _during = during;
        return this;
    }

    public ConditionAwaiter PollInterval(TimeSpan pollInterval)
    {
        _pollInterval = pollInterval;
        return this;
    }

    public ConditionAwaiter PollDelay(TimeSpan pollDelay)
    {
        _pollDelay = pollDelay;
        return this;
    }


    public ConditionAwaiter IgnoreExceptions(bool ignoreExceptions = true)
    {
        _ignoreExceptions = ignoreExceptions;
        return this;
    }

    public ConditionAwaiter IgnoreExceptions(params Type[] exceptionTypes)
    {
        _exceptionTypes = exceptionTypes;
        return this;
    }

    public TSource Until<TSource>(Func<TSource> supplier, Func<TSource, bool> predicate)
    {
        var value = default(TSource);
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var duringConditionMet = false;
        var failFastCondition = false;
        var isFirstSuccessPredicateExecution = true;
        var startTime = DateTime.Now;

        ApplyPollDelayIfDefined();

        try
        {
            var task = Task.Factory.StartNew(() =>
            {
                var duringStart = DateTime.Now;
                cancellationToken.ThrowIfCancellationRequested();

                while (true)
                {
                    try
                    {
                        if (_failFast != null && _failFast())
                        {
                            failFastCondition = true;
                            return false;
                        }

                        value = supplier();

                        if (predicate(value))
                        {
                            if (_during == TimeSpan.Zero) return true;

                            if (isFirstSuccessPredicateExecution)
                            {
                                duringStart = DateTime.Now;
                                isFirstSuccessPredicateExecution = false;
                            }

                            if (DateTime.Now - duringStart >= _during)
                            {
                                duringConditionMet = true;
                                return true;
                            }
                        }
                        else if (_during != TimeSpan.Zero)
                        {
                            duringStart = DateTime.Now;
                        }
                    }
                    catch (Exception exception)
                    {
                        if (!_ignoreExceptions && !_exceptionTypes.Any(x => x.IsInstanceOfType(exception)))
                            throw;
                    }

                    ApplyPollIntervalIfDefined();
                }
            }, cancellationToken);

            if (!task.Wait(_atMost, cancellationToken))
            {
                CheckDuringMetCondition(duringConditionMet);
                ThrowTimeoutConditionException();
            }

            CheckFailFastCondition(failFastCondition);
            CheckAtLeastCondition(startTime);
        }
        finally
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        return value;
    }

    public void UntilAsserted(Action assertAction)
    {
        Until(() => GetResultOfAssertAction(assertAction));
    }

    public void Until(Func<bool> predicate)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var duringConditionMet = false;
        var failFastCondition = false;
        var isFirstSuccessPredicateExecution = true;
        var startTime = DateTime.Now;

        ApplyPollDelayIfDefined();

        try
        {
            var task = Task.Factory.StartNew(() =>
            {
                var duringStart = DateTime.Now;
                cancellationToken.ThrowIfCancellationRequested();

                while (true)
                {
                    try
                    {
                        if (_failFast != null && _failFast())
                        {
                            failFastCondition = true;
                            return false;
                        }

                        if (predicate())
                        {
                            if (_during == TimeSpan.Zero) return true;

                            if (isFirstSuccessPredicateExecution)
                            {
                                duringStart = DateTime.Now;
                                isFirstSuccessPredicateExecution = false;
                            }

                            if (DateTime.Now - duringStart >= _during)
                            {
                                duringConditionMet = true;
                                return true;
                            }
                        }
                        else if (_during != TimeSpan.Zero)
                            duringStart = DateTime.Now;
                    }
                    catch (Exception exception)
                    {
                        if (!_ignoreExceptions && !_exceptionTypes.Any(x => x.IsInstanceOfType(exception)))
                            throw;
                    }

                    ApplyPollIntervalIfDefined();
                }
            }, cancellationToken);

            if (!task.Wait(_atMost, cancellationToken))
            {
                CheckDuringMetCondition(duringConditionMet);
                ThrowTimeoutConditionException();
            }

            CheckFailFastCondition(failFastCondition);
            CheckAtLeastCondition(startTime);
        }
        finally
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }

    private void ApplyPollIntervalIfDefined()
    {
        if (_pollInterval > TimeSpan.Zero)
        {
            Thread.Sleep(_pollInterval);
        }
    }

    private void ApplyPollDelayIfDefined()
    {
        if (_pollDelay > TimeSpan.Zero)
        {
            Thread.Sleep(_pollDelay);
        }
    }

    private void CheckDuringMetCondition(bool duringConditionMet)
    {
        if (_during > TimeSpan.Zero && !duringConditionMet)
        {
            throw new DuringConditionNotMetException(
                $"During condition was not met within the specified time '{_atMost.TotalMilliseconds}' millisecond(s)");
        }
    }

    private void ThrowTimeoutConditionException()
    {
        throw new TimeoutConditionException(
            string.IsNullOrEmpty(_alias)
                ? $"Condition was not met within the specified time '{_atMost.TotalMilliseconds}' millisecond(s)"
                : $"Condition with alias '{_alias}' was not met within the specified time '{_atMost.TotalMilliseconds}' millisecond(s)");
    }

    private void CheckFailFastCondition(bool failFastCondition)
    {
        if (failFastCondition)
        {
            throw new FailFastConditionException(string.IsNullOrEmpty(_failFastMessage)
                ? "Fail fast condition has been reached"
                : _failFastMessage);
        }
    }

    private void CheckAtLeastCondition(DateTime startTime)
    {
        var taskEndTime = DateTime.Now;
        var elapsedTime = taskEndTime - startTime;

        if (elapsedTime < _atLeast)
        {
            throw new AtLeastConditionException(
                $"Condition completed earlier than the specified minimum duration." +
                $" AtLeast: '{_atLeast.TotalMilliseconds}' millisecond(s), " +
                $"ElapsedTime: '{elapsedTime.Seconds}' second(s)");
        }
    }

    private bool GetResultOfAssertAction(Action assertAction)
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