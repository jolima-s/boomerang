using Boomerang.Domain.Models;

namespace Boomerang.Domain.Game;

/// <summary>The face-up cards of one player (never including the hidden Throw card).</summary>
public sealed record PlayerReveal(int PlayerIndex, string PlayerName, IReadOnlyList<Card> FaceUpCards);