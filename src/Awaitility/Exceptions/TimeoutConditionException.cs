using System;

namespace Awaitility.Exceptions;

public class TimeoutConditionException : Exception
{
    public TimeoutConditionException(string message) : base(message)
    {
    }
}