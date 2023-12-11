using System;
using System.Collections.Generic;
using System.Threading;

namespace Awaitility.Tests.Utils;

public class BooleanSwitcher
{
    private int _countFail;
    public int Count;
    private readonly int _howManyFalse;
    private readonly int _returnFalseOn;
    private readonly TimeSpan _timeout;

    public BooleanSwitcher(int howManyFalse = 0, int returnFalseOn = 0, TimeSpan timeout = default)
    {
        _howManyFalse = howManyFalse;
        _returnFalseOn = returnFalseOn;
        _timeout = timeout;
    }

    public bool Switch(IEnumerable<string> enumerableOfStrings)
    {
        Count++;
        var result = true;

        Thread.Sleep(_timeout);

        if (_countFail < _howManyFalse)
        {
            _countFail++;
            result = false;
        }

        if (Count == _returnFalseOn)
        {
            result = false;
        }

        Console.WriteLine("[BooleanSwitcher] Method Switch() was called, result: " + result);

        return result;
    }

    public bool Switch()
    {
        Count++;
        var result = true;

        Thread.Sleep(_timeout);

        if (_countFail < _howManyFalse)
        {
            _countFail++;
            result = false;
        }

        if (Count == _returnFalseOn)
        {
            result = false;
        }

        Console.WriteLine("[BooleanSwitcher] Method Switch() was called, result: " + result);

        return result;
    }
}