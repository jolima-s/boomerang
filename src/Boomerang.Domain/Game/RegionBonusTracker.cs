using Boomerang.Domain.Models;

namespace Boomerang.Domain.Game;

public sealed class RegionBonusTracker
{
    private readonly IReadOnlyList<Region> _regions;
    private readonly HashSet<string> _claimed = new();

    public RegionBonusTracker(IReadOnlyList<Region> regions) => _regions = regions;

    public IReadOnlyCollection<string> ClaimedRegions => _claimed;

    /// <param name="visitedAfterRound">For each player: all sites visited including this round's cards.</param>
    /// <returns>For each player: the regions that earn the bonus this round.</returns>
    public IReadOnlyList<IReadOnlyList<Region>> Award(IReadOnlyList<HashSet<string>> visitedAfterRound)
    {
        var awarded = visitedAfterRound.Select(_ => new List<Region>()).ToList();
        var newlyClaimed = new List<string>();

        foreach (Region region in _regions)
        {
            if (_claimed.Contains(region.Name)) continue;

            bool anyCompleted = false;
            for (int p = 0; p < visitedAfterRound.Count; p++)
            {
                HashSet<string> visited = visitedAfterRound[p];
                if (region.Sites.All(s => visited.Contains(s)))
                {
                    awarded[p].Add(region);
                    anyCompleted = true;
                }
            }
            if (anyCompleted) newlyClaimed.Add(region.Name);
        }

        _claimed.UnionWith(newlyClaimed);
        return awarded.Select(list => (IReadOnlyList<Region>)list).ToList();
    }
}