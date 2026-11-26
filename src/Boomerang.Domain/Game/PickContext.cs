using Boomerang.Domain.Models;

namespace Boomerang.Domain.Game;

/// <summary>What a player can see when asked to pick a card.</summary>
public sealed record PickContext(
    int Round,
    int TotalRounds,
    PickKind Kind,
    IReadOnlyList<Card> Hand,
    Card? ThrowCard,
    IReadOnlyList<Card> FaceUp,
    IReadOnlyCollection<string> VisitedSites);

public enum PickKind { Throw, Draft }