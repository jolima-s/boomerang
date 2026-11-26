namespace Boomerang.Domain.Strategies;

/// <summary>Abstraction over randomness so tests can use a deterministic deck order.</summary>
public interface IShuffler
{
    void Shuffle<T>(IList<T> items);
}

public sealed class RandomShuffler : IShuffler
{
    private readonly Random _random;

    public RandomShuffler(Random? random = null) => _random = random ?? new Random();

    public void Shuffle<T>(IList<T> items)
    {
        for (int i = items.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }
}