using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Awaitility.ExecutorServices;

public class TaskExecutorService : BaseExecutorService
{
    private readonly ConditionSettings _conditionSettings;

    public TaskExecutorService(ConditionSettings conditionSettings) : base(conditionSettings)
    {
        _conditionSettings = conditionSettings;
    }

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
                        if (_conditionSettings.FailFast != null && _conditionSettings.FailFast())
                        {
                            failFastCondition = true;
                            return false;
                        }

                        if (predicate())
                        {
                            if (_conditionSettings.During == TimeSpan.Zero) return true;

                            if (isFirstSuccessPredicateExecution)
                            {
                                duringStart = DateTime.Now;
                                isFirstSuccessPredicateExecution = false;
                            }

                            if (DateTime.Now - duringStart >= _conditionSettings.During)
                            {
                                duringConditionMet = true;
                                return true;
                            }
                        }
                        else if (_conditionSettings.During != TimeSpan.Zero)
                            duringStart = DateTime.Now;
                    }
                    catch (Exception exception)
                    {
                        if (!_conditionSettings.IgnoreExceptions
                            && !_conditionSettings.ExceptionTypes.Any(x => x.IsInstanceOfType(exception)))
                            throw;
                    }

                    ApplyPollIntervalIfDefined();
                }
            }, cancellationToken);

            if (!task.Wait((int)_conditionSettings.AtMost.TotalMilliseconds, cancellationToken))
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
                        if (_conditionSettings.FailFast != null && _conditionSettings.FailFast())
                        {
                            failFastCondition = true;
                            return false;
                        }

                        if (predicate(obj))
                        {
                            if (_conditionSettings.During == TimeSpan.Zero) return true;

                            if (isFirstSuccessPredicateExecution)
                            {
                                duringStart = DateTime.Now;
                                isFirstSuccessPredicateExecution = false;
                            }

                            if (DateTime.Now - duringStart >= _conditionSettings.During)
                            {
                                duringConditionMet = true;
                                return true;
                            }
                        }
                        else if (_conditionSettings.During != TimeSpan.Zero)
                            duringStart = DateTime.Now;
                    }
                    catch (Exception exception)
                    {
                        if (!_conditionSettings.IgnoreExceptions
                            && !_conditionSettings.ExceptionTypes.Any(x => x.IsInstanceOfType(exception)))
                            throw;
                    }

                    ApplyPollIntervalIfDefined();
                }
            }, cancellationToken);

            if (!task.Wait((int)_conditionSettings.AtMost.TotalMilliseconds, cancellationToken))
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
                        if (_conditionSettings.FailFast != null && _conditionSettings.FailFast())
                        {
                            failFastCondition = true;
                            return false;
                        }

                        value = supplier();

                        if (predicate(value))
                        {
                            if (_conditionSettings.During == TimeSpan.Zero) return true;

                            if (isFirstSuccessPredicateExecution)
                            {
                                duringStart = DateTime.Now;
                                isFirstSuccessPredicateExecution = false;
                            }

                            if (DateTime.Now - duringStart >= _conditionSettings.During)
                            {
                                duringConditionMet = true;
                                return true;
                            }
                        }
                        else if (_conditionSettings.During != TimeSpan.Zero)
                        {
                            duringStart = DateTime.Now;
                        }
                    }
                    catch (Exception exception)
                    {
                        if (!_conditionSettings.IgnoreExceptions
                            && !_conditionSettings.ExceptionTypes.Any(x => x.IsInstanceOfType(exception)))
                            throw;
                    }

                    ApplyPollIntervalIfDefined();
                }
            }, cancellationToken);

            if (!task.Wait((int)_conditionSettings.AtMost.TotalMilliseconds, cancellationToken))
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