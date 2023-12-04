namespace Awaitility;

public class ConditionSettings
{
    public TimeSpan AtMost { get; set; } = Durations.TenSeconds;
    public TimeSpan AtLeast { get; set; } = TimeSpan.Zero;
    public TimeSpan During { get; set; } = TimeSpan.Zero;
    public TimeSpan PollInterval { get; set; } = TimeSpan.Zero;
    public TimeSpan PollDelay { get; set; } = TimeSpan.Zero;
    public Type[] ExceptionTypes { get; set; } = Array.Empty<Type>();
    public Func<bool> FailFast { get; set; }
    public string FailFastMessage { get; set; }
    public bool IgnoreExceptions { get; set; }
    public string Alias { get; set; }
}