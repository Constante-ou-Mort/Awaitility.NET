using System;

namespace Awaitility.Exceptions;

public class FailFastConditionException : Exception
{
    public FailFastConditionException(string message) : base(message)
    {
    }
}