public static class Counter
{
    private static int _value;
    public static int Value => _value;

    public static void Increment() => Interlocked.Increment(ref _value);
}