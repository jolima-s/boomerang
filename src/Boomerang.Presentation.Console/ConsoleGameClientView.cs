using Boomerang.Infrastructure.Networking;

namespace Boomerang.Presentation.Console;

public sealed class ConsoleGameClientView : IGameClientView
{
    public void ShowMessage(string text) => System.Console.WriteLine(text);

    public string? ReadResponse(string prompt)
    {
        System.Console.Write(prompt);
        return System.Console.ReadLine();
    }
}
