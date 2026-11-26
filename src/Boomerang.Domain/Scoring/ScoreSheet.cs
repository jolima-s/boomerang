namespace Boomerang.Domain.Scoring;

/// <summary>A player's score sheet: what has been visited, which one-time options are used up, and all round scores.</summary>
public sealed class ScoreSheet
{
    private readonly HashSet<string> _visited = new();
    private readonly HashSet<string> _claimed = new();
    private readonly List<RoundScore> _rounds = new();

    public IReadOnlyCollection<string> VisitedSites => _visited.AsReadOnly();
    public IReadOnlyList<RoundScore> Rounds => _rounds.AsReadOnly();
    public int Total => _rounds.Sum(r => r.Total);

    public bool HasVisited(string site) => _visited.Contains(site);
    public void MarkVisited(IEnumerable<string> sites) => _visited.UnionWith(sites);

    /// <summary>One-time options (such as an Activity) are identified by a key.</summary>
    public bool IsClaimed(string key) => _claimed.Contains(key);

    public void Claim(string key)
    {
        if (!_claimed.Add(key))
            throw new InvalidOperationException($"'{key}' has already been scored in this game.");
    }

    public void AddRound(RoundScore score) => _rounds.Add(score);
    public int CategoryTotal(string category) => _rounds.Sum(r => r.PointsFor(category));
}