namespace Boomerang.Domain.Game;

/// <summary>Highest total wins, ties are decided by the tie-break category.</summary>
public static class WinnerSelector
{
    public static IReadOnlyList<PlayerResult> Select(IReadOnlyList<PlayerResult> players, string tieBreakCategory)
    {
        int best = players.Max(p => p.Total);
        var top = players.Where(p => p.Total == best).ToList();
        int bestTieBreak = top.Max(p => p.CategoryTotals.GetValueOrDefault(tieBreakCategory));
        return top.Where(p => p.CategoryTotals.GetValueOrDefault(tieBreakCategory) == bestTieBreak).ToList();
    }
}