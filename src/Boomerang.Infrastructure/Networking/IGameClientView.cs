namespace Boomerang.Infrastructure.Networking;

/// <summary>Presentation side of the game client; implemented by the UI layer.</summary>
public interface IGameClientView
{
    void ShowMessage(string text);
    string? ReadResponse(string prompt);
}
