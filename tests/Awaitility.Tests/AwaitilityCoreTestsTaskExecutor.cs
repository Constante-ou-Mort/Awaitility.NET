using Awaitility.Exceptions;
using Awaitility.Tests.Utils;
using static System.TimeSpan;
using static Awaitility.Awaitility;
using static Awaitility.Durations;
using static NUnit.Framework.ParallelScope;

namespace Awaitility.Tests;

[Parallelizable(All)]
public class AwaitilityCoreTestsTaskExecutor
{
    private List<string> _listToCheckUntilSupplier;

    [Test]
    public void Should_Throw_TimeoutConditionException_When_Func_Takes_More_Time_Than_Expected()
    {
        Assert.Throws<TimeoutConditionException>(() =>
            Await()
                .AtMost(OneHundredMilliseconds)
                .Until(() => DoThingWithTimeout(TwoHundredMilliseconds)));
    }

    [Test]
    public void Should_Not_Throw_TimeoutConditionException_When_Func_Takes_Less_Time_Than_Expected()
    {
        Assert.DoesNotThrow(() =>
            Await()
                .AtMost(OneHundredMilliseconds)
                .Until(() => DoThingWithTimeout(FiftyMilliseconds)));
    }

    [Test]
    public void Should_Not_Throw_DuringConditionNotMetException_When_Func_Executes_During_InTime()
    {
        var bs = new BooleanSwitcher(howManyFalse: 2, timeout: OneHundredMilliseconds);

        Assert.DoesNotThrow(() =>
            With()
                .During(TwoHundredMilliseconds)
                .AtMost(FromMilliseconds(600))
                .Until(bs.Switch));
    }

    [Test]
    public void Should_Throw_DuringConditionNotMetException_When_Func_Does_Not_Execute_During_InTime()
    {
        var bs = new BooleanSwitcher(howManyFalse: 4, timeout: FromMilliseconds(100));

        Assert.Throws<DuringConditionNotMetException>(() =>
            With()
                .During(TwoHundredMilliseconds)
                .AtMost(FiveHundredMilliseconds)
                .Until(bs.Switch));
    }

    [Test]
    public void Should_Throw_AtLeastConditionException_When_Condition_Executes_Faster_Than_AtLeast_Time()
    {
        Assert.Throws<AtLeastConditionException>(() =>
            With()
                .AtLeast(TwoHundredMilliseconds)
                .Until(() => DoThingWithTimeout(OneHundredMilliseconds)));
    }

    [Test]
    public void Should_Not_Throw_AtLeastConditionException_When_Condition_Executes_Later_Than_AtLeast_Time()
    {
        Assert.DoesNotThrow(() =>
            With()
                .AtLeast(OneHundredMilliseconds)
                .Until(() => DoThingWithTimeout(TwoHundredMilliseconds)));
    }

    [Test]
    public void Should_Throw_TimeoutConditionException_When_IgnoreExceptions()
    {
        Assert.Throws<TimeoutConditionException>(() =>
            Await()
                .AtMost(OneHundredMilliseconds)
                .IgnoreExceptions()
                .Until(() => throw new InvalidProgramException()));
    }

    [Test]
    public void Should_Throw_TimeoutConditionException_When_IgnoreExceptions_For_Specific_Exception()
    {
        Assert.Throws<TimeoutConditionException>(() =>
            Await()
                .AtMost(OneHundredMilliseconds)
                .IgnoreExceptions(typeof(InvalidProgramException))
                .Until(() => throw new InvalidProgramException()));
    }

    [Test]
    public void Should_Throw_InvalidProgramException_When_IgnoreExceptions_Is_Not_Used()
    {
        var exception = Assert.Catch(() =>
            Await()
                .AtMost(OneHundredMilliseconds)
                .Until(() => throw new InvalidProgramException()));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception?.InnerException, Is.TypeOf<InvalidProgramException>());
    }

    [Test]
    public void Should_Throw_FailFastConditionException_When_Fail_Fast_Condition_Is_Met()
    {
        Assert.Throws<FailFastConditionException>(() =>
            Given()
                .AtMost(OneHundredMilliseconds)
                .FailFast(() => true)
                .And()
                .Until(() => DoThingWithTimeout(FiftyMilliseconds)));
    }

    [Test]
    public void Should_Not_Throw_FailFastConditionException_When_Fail_Fast_Condition_Is_Not_Met()
    {
        Assert.DoesNotThrow(() =>
            Given()
                .AtMost(OneHundredMilliseconds)
                .FailFast(() => false)
                .And()
                .Until(() => DoThingWithTimeout(FiftyMilliseconds)));
    }

    [Test]
    public void Should_Have_Alias_In_Error_Message()
    {
        const string alias = "test";
        var exceptionMessage = $"Condition with alias '{alias}' was not met within" +
                               $" the specified time '{OneMillisecond.Milliseconds}' millisecond(s)";

        var exception = Assert.Catch(() =>
            With()
                .AtMost(OneMillisecond)
                .Alias(alias)
                .Until(() => false));

        Assert.That(exception, Is.Not.Null);
        Assert.That(exception?.Message, Is.EqualTo(exceptionMessage));
    }

    [Test]
    public void Should_Be_Called_With_Interval_When_PollInterval_Is_Used()
    {
        var bs = new BooleanSwitcher(howManyFalse: 4, timeout: FromMilliseconds(10));
        var pollInterval = FromMilliseconds(10);

        Await()
            .AtMost(OneHundredMilliseconds)
            .PollInterval(pollInterval)
            .Until(bs.Switch);

        Assert.That(bs.Count, Is.EqualTo(OneHundredMilliseconds.Milliseconds / pollInterval.Milliseconds / 2));
    }

    [Test]
    public void Should_Throw_TimeoutConditionException_When_PollInterval_Is_Larger_Than_AtMost_Timeout()
    {
        var bs = new BooleanSwitcher(howManyFalse: 1, timeout: FromMilliseconds(1));

        Assert.Throws<TimeoutConditionException>(() =>
            Await()
                .AtMost(OneHundredMilliseconds)
                .PollInterval(TwoHundredMilliseconds)
                .Until(bs.Switch));
    }

    [Test]
    public void Should_Return_Last_Value_When_Until_Executes_Successfully()
    {
        _listToCheckUntilSupplier = new List<string> { "", "", "", "", "" };
        var bs = new BooleanSwitcher(howManyFalse: 1);

        var results = With()
            .AtMost(OneHundredMilliseconds)
            .Until(GetListOfStrings, bs.Switch)
            .ToList();

        Assert.That(results, Has.Count.EqualTo(3));
    }

    [Test]
    public void UntilAsserted_Should_Not_Throw_NUnit_AssertionException()
    {
        Assert.Throws<TimeoutConditionException>(() =>
            Await()
                .AtMost(OneHundredMilliseconds)
                .UntilAsserted(() => Assert.That(true, Is.False)));
    }

    [Test]
    public void UntilAsserted_Should_Not_Throw_XunitException()
    {
        Assert.Throws<TimeoutConditionException>(() =>
            Await()
                .AtMost(OneHundredMilliseconds)
                .UntilAsserted(() => Xunit.Assert.True(false)));
    }

    [Test]
    public void UntilAsserted_Should_Not_Throw_MsTest_UnitTestAssertException()
    {
        Assert.Throws<TimeoutConditionException>(() =>
            Await()
                .AtMost(OneHundredMilliseconds)
                .UntilAsserted(() =>
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(true)));
    }

    private IEnumerable<string> GetListOfStrings()
    {
        _listToCheckUntilSupplier.RemoveAt(0);
        return _listToCheckUntilSupplier;
    }

    private static bool DoThingWithTimeout(TimeSpan time)
    {
        Thread.Sleep(time);
        return true;
    }
}