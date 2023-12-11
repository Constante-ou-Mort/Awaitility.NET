using System;

namespace Awaitility.Exceptions;

public class DuringConditionNotMetException : Exception
{
    public DuringConditionNotMetException(string message) : base(message) { }
}