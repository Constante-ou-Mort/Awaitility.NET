namespace Awaitility;

public static class Awaitility
{
    public static ConditionAwaiter Await()
    {
        return new ConditionAwaiter();
    }

    public static ConditionAwaiter Given() => Await();

    public static ConditionAwaiter With() => Await();
    
    public static ConditionAwaiter And() => Await();
}