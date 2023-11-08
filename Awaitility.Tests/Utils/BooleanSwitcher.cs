namespace Awaitility.Tests.Utils;

public class BooleanSwitcher(int howManyFalse = 0, int returnFalseOn = 0, TimeSpan timeout = default)
{
    private int _countFail;
    public int Count;

    public bool Switch(IEnumerable<string> enumerableOfStrings)
    {
        Count++;
        var result = true;

        Thread.Sleep(timeout);

        if (_countFail < howManyFalse)
        {
            _countFail++;
            result = false;
        }

        if (Count == returnFalseOn)
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

        Thread.Sleep(timeout);

        if (_countFail < howManyFalse)
        {
            _countFail++;
            result = false;
        }

        if (Count == returnFalseOn)
        {
            result = false;
        }

        Console.WriteLine("[BooleanSwitcher] Method Switch() was called, result: " + result);

        return result;
    }
}