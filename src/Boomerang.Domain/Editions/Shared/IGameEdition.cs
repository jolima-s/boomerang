using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;

namespace Boomerang.Domain.Editions.Shared;

public interface IGameEdition
{
    string Name { get; }
    IReadOnlyList<Card> Cards { get; }
    IReadOnlyList<Region> Regions { get; }
    IReadOnlyList<IScoringRule> ScoringRules { get; }
    int Rounds { get; }
    int HandSize { get; }
    int MinPlayers { get; }
    int MaxPlayers { get; }
    int RegionBonus { get; }
    string TieBreakCategory { get; }
}