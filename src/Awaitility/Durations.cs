using System;

namespace Awaitility;

public static class Durations
{
    public static readonly TimeSpan Forever = TimeSpan.MaxValue;
    public static readonly TimeSpan OneMillisecond = TimeSpan.FromMilliseconds(1);
    public static readonly TimeSpan FiftyMilliseconds = TimeSpan.FromMilliseconds(50);
    public static readonly TimeSpan OneHundredMilliseconds = TimeSpan.FromMilliseconds(100);
    public static readonly TimeSpan TwoHundredMilliseconds = TimeSpan.FromMilliseconds(200);
    public static readonly TimeSpan FiveHundredMilliseconds = TimeSpan.FromMilliseconds(500);
    public static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
    public static readonly TimeSpan TwoSeconds = TimeSpan.FromSeconds(2);
    public static readonly TimeSpan FiveSeconds = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan TenSeconds = TimeSpan.FromSeconds(10);
    public static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
    public static readonly TimeSpan TwoMinutes = TimeSpan.FromMinutes(2);
    public static readonly TimeSpan FiveMinutes = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan TenMinutes = TimeSpan.FromMinutes(10);
}