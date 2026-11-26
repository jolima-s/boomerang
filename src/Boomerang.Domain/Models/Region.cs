namespace Boomerang.Domain.Models;

public sealed record Region(string Name, IReadOnlyList<string> Sites);