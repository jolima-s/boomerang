namespace Boomerang.Infrastructure.Networking;

public static class WireKinds
{
    public const string Message = "message";
    public const string Prompt = "prompt";
    public const string End = "end";
}

public sealed record WireMessage(string Kind, string Text);