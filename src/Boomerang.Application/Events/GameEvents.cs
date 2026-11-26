namespace Boomerang.Application.Events;

public sealed record PlayerConnectedEvent(int PlayerIndex, string PlayerName);
