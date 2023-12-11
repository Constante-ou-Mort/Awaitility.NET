using System;

namespace Awaitility.ExecutorServices;

public interface IExecutorService
{
    void Until(Func<bool> func);

    void Until<T>(T obj, Func<T, bool> predicate);

    TSource Until<TSource>(Func<TSource> supplier, Func<TSource, bool> predicate);

    void UntilAsserted(Action assertAction);
}