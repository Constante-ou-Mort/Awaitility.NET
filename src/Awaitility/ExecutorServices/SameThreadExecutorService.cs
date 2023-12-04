namespace Awaitility.ExecutorServices;

public class SameThreadExecutorService(ConditionSettings conditionSettings) : BaseExecutorService(conditionSettings)
{
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
                if (conditionSettings.FailFast != null && conditionSettings.FailFast())
                {
                    failFastCondition = true;
                    break;
                }

                if (predicate())
                {
                    if (conditionSettings.During == TimeSpan.Zero) break;

                    if (isFirstSuccessPredicateExecution)
                    {
                        duringStart = DateTime.Now;
                        isFirstSuccessPredicateExecution = false;
                    }

                    if (DateTime.Now - duringStart >= conditionSettings.During)
                    {
                        duringConditionMet = true;
                        break;
                    }
                }
                else if (conditionSettings.During != TimeSpan.Zero)
                    duringStart = DateTime.Now;
            }
            catch (Exception exception)
            {
                if (!conditionSettings.IgnoreExceptions
                    && !conditionSettings.ExceptionTypes.Any(x => x.IsInstanceOfType(exception)))
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
                if (conditionSettings.FailFast != null && conditionSettings.FailFast())
                {
                    failFastCondition = true;
                    break;
                }

                if (predicate(obj))
                {
                    if (conditionSettings.During == TimeSpan.Zero) break;

                    if (isFirstSuccessPredicateExecution)
                    {
                        duringStart = DateTime.Now;
                        isFirstSuccessPredicateExecution = false;
                    }

                    if (DateTime.Now - duringStart >= conditionSettings.During)
                    {
                        duringConditionMet = true;
                        break;
                    }
                }
                else if (conditionSettings.During != TimeSpan.Zero)
                    duringStart = DateTime.Now;
            }
            catch (Exception exception)
            {
                if (!conditionSettings.IgnoreExceptions
                    && !conditionSettings.ExceptionTypes.Any(x => x.IsInstanceOfType(exception)))
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

        while (DateTime.Now - startTime <= conditionSettings.AtMost)
        {
            try
            {
                if (conditionSettings.FailFast != null && conditionSettings.FailFast())
                {
                    failFastCondition = true;
                    break;
                }

                value = supplier();

                if (predicate(value))
                {
                    if (conditionSettings.During == TimeSpan.Zero) break;

                    if (isFirstSuccessPredicateExecution)
                    {
                        duringStart = DateTime.Now;
                        isFirstSuccessPredicateExecution = false;
                    }

                    if (DateTime.Now - duringStart >= conditionSettings.During)
                    {
                        duringConditionMet = true;
                        break;
                    }
                }
                else if (conditionSettings.During != TimeSpan.Zero)
                {
                    duringStart = DateTime.Now;
                }
            }
            catch (Exception exception)
            {
                if (!conditionSettings.IgnoreExceptions
                    && !conditionSettings.ExceptionTypes.Any(x => x.IsInstanceOfType(exception)))
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
        if (DateTime.Now - startTime >= conditionSettings.AtMost)
        {
            CheckDuringMetCondition(duringConditionMet);
            ThrowTimeoutConditionException();
        }
    }
}