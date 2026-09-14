namespace ProJob.Input;

public sealed class ConsoleInputSource : IInputSource
{
    public ConsoleKeyInfo ReadKeyInfo() => Console.ReadKey(intercept: true);
}
