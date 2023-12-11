using System;
using System.Linq;

namespace Awaitility.ExecutorServices;

public class SameThreadExecutorService : BaseExecutorService
{
    private readonly ConditionSettings _conditionSettings1;

    public SameThreadExecutorService(ConditionSettings conditionSettings) : base(conditionSettings)
    {
        _conditionSettings1 = conditionSettings;
    }

    public override void Until(Func<bool> predicate)
    {
        var duringConditionMet = false;
        var failFastCondition = false;
        var isFirstSuccessPredicateExecution = true;
        var startTime = DateTime.Now;

        ApplyPollDelayIfDefined();

        var duringStart = DateTime.Now;

        while (true)
        {
            try
            {
                if (_conditionSettings1.FailFast != null && _conditionSettings1.FailFast())
                {
                    failFastCondition = true;
                    break;
                }

                if (predicate())
                {
                    if (_conditionSettings1.During == TimeSpan.Zero) break;

                    if (isFirstSuccessPredicateExecution)
                    {
                        duringStart = DateTime.Now;
                        isFirstSuccessPredicateExecution = false;
                    }

                    if (DateTime.Now - duringStart >= _conditionSettings1.During)
                    {
                        duringConditionMet = true;
                        break;
                    }
                }
                else if (_conditionSettings1.During != TimeSpan.Zero)
                    duringStart = DateTime.Now;
            }
            catch (Exception exception)
            {
                if (!_conditionSettings1.IgnoreExceptions
                    && !_conditionSettings1.ExceptionTypes.Any(x => x.IsInstanceOfType(exception)))
                    throw;
            }

            ApplyPollIntervalIfDefined();
            CheckTimeoutConditions(startTime, duringConditionMet);
        }

        CheckFailFastCondition(failFastCondition);
        CheckAtLeastCondition(startTime);
    }

    public override void Until<T>(T obj, Func<T, bool> predicate)
    {
        var duringConditionMet = false;
        var failFastCondition = false;
        var isFirstSuccessPredicateExecution = true;
        var startTime = DateTime.Now;

        ApplyPollDelayIfDefined();

        var duringStart = DateTime.Now;

        while (true)
        {
            try
            {
                if (_conditionSettings1.FailFast != null && _conditionSettings1.FailFast())
                {
                    failFastCondition = true;
                    break;
                }

                if (predicate(obj))
                {
                    if (_conditionSettings1.During == TimeSpan.Zero) break;

                    if (isFirstSuccessPredicateExecution)
                    {
                        duringStart = DateTime.Now;
                        isFirstSuccessPredicateExecution = false;
                    }

                    if (DateTime.Now - duringStart >= _conditionSettings1.During)
                    {
                        duringConditionMet = true;
                        break;
                    }
                }
                else if (_conditionSettings1.During != TimeSpan.Zero)
                    duringStart = DateTime.Now;
            }
            catch (Exception exception)
            {
                if (!_conditionSettings1.IgnoreExceptions
                    && !_conditionSettings1.ExceptionTypes.Any(x => x.IsInstanceOfType(exception)))
                    throw;
            }

            ApplyPollIntervalIfDefined();
            CheckTimeoutConditions(startTime, duringConditionMet);
        }

        CheckFailFastCondition(failFastCondition);
        CheckAtLeastCondition(startTime);
    }

    public override TSource Until<TSource>(Func<TSource> supplier, Func<TSource, bool> predicate)
    {
        var value = default(TSource);
        var duringConditionMet = false;
        var failFastCondition = false;
        var isFirstSuccessPredicateExecution = true;
        var startTime = DateTime.Now;

        ApplyPollDelayIfDefined();

        var duringStart = DateTime.Now;

        while (DateTime.Now - startTime <= _conditionSettings1.AtMost)
        {
            try
            {
                if (_conditionSettings1.FailFast != null && _conditionSettings1.FailFast())
                {
                    failFastCondition = true;
                    break;
                }

                value = supplier();

                if (predicate(value))
                {
                    if (_conditionSettings1.During == TimeSpan.Zero) break;

                    if (isFirstSuccessPredicateExecution)
                    {
                        duringStart = DateTime.Now;
                        isFirstSuccessPredicateExecution = false;
                    }

                    if (DateTime.Now - duringStart >= _conditionSettings1.During)
                    {
                        duringConditionMet = true;
                        break;
                    }
                }
                else if (_conditionSettings1.During != TimeSpan.Zero)
                {
                    duringStart = DateTime.Now;
                }
            }
            catch (Exception exception)
            {
                if (!_conditionSettings1.IgnoreExceptions
                    && !_conditionSettings1.ExceptionTypes.Any(x => x.IsInstanceOfType(exception)))
                    throw;
            }

            ApplyPollIntervalIfDefined();
            CheckTimeoutConditions(startTime, duringConditionMet);
        }

        CheckFailFastCondition(failFastCondition);
        CheckAtLeastCondition(startTime);

        return value;
    }

    public override void UntilAsserted(Action assertAction)
    {
        Until(() => GetResultOfAssertAction(assertAction));
    }

    private void CheckTimeoutConditions(DateTime startTime, bool duringConditionMet)
    {
        if (DateTime.Now - startTime >= _conditionSettings1.AtMost)
        {
            CheckDuringMetCondition(duringConditionMet);
            ThrowTimeoutConditionException();
        }
    }
}