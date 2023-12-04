namespace Awaitility.ExecutorServices;

public class TaskExecutorService(ConditionSettings conditionSettings) : BaseExecutorService(conditionSettings)
{
    public override void Until(Func<bool> predicate)
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
                        if (conditionSettings.FailFast != null && conditionSettings.FailFast())
                        {
                            failFastCondition = true;
                            return false;
                        }

                        if (predicate())
                        {
                            if (conditionSettings.During == TimeSpan.Zero) return true;

                            if (isFirstSuccessPredicateExecution)
                            {
                                duringStart = DateTime.Now;
                                isFirstSuccessPredicateExecution = false;
                            }

                            if (DateTime.Now - duringStart >= conditionSettings.During)
                            {
                                duringConditionMet = true;
                                return true;
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
                }
            }, cancellationToken);

            if (!task.Wait(conditionSettings.AtMost, cancellationToken))
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

    public override void Until<T>(T obj, Func<T, bool> predicate)
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
                        if (conditionSettings.FailFast != null && conditionSettings.FailFast())
                        {
                            failFastCondition = true;
                            return false;
                        }

                        if (predicate(obj))
                        {
                            if (conditionSettings.During == TimeSpan.Zero) return true;

                            if (isFirstSuccessPredicateExecution)
                            {
                                duringStart = DateTime.Now;
                                isFirstSuccessPredicateExecution = false;
                            }

                            if (DateTime.Now - duringStart >= conditionSettings.During)
                            {
                                duringConditionMet = true;
                                return true;
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
                }
            }, cancellationToken);

            if (!task.Wait(conditionSettings.AtMost, cancellationToken))
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

    public override TSource Until<TSource>(Func<TSource> supplier, Func<TSource, bool> predicate)
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
                        if (conditionSettings.FailFast != null && conditionSettings.FailFast())
                        {
                            failFastCondition = true;
                            return false;
                        }

                        value = supplier();

                        if (predicate(value))
                        {
                            if (conditionSettings.During == TimeSpan.Zero) return true;

                            if (isFirstSuccessPredicateExecution)
                            {
                                duringStart = DateTime.Now;
                                isFirstSuccessPredicateExecution = false;
                            }

                            if (DateTime.Now - duringStart >= conditionSettings.During)
                            {
                                duringConditionMet = true;
                                return true;
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
                }
            }, cancellationToken);

            if (!task.Wait(conditionSettings.AtMost, cancellationToken))
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

    public override void UntilAsserted(Action assertAction)
    {
        Until(() => GetResultOfAssertAction(assertAction));
    }
}