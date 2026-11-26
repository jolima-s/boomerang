using Boomerang.Domain.Editions.Australia;
using Boomerang.Domain.Editions.Shared;
using Boomerang.Domain.Editions.Shared.DefaultRules;
using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;

namespace Boomerang.Test.Domain;

public class TouristSitesRuleTests
{
    private static Card At(string site) => new($"Card {site}", site, "R", 1, Array.Empty<Symbol>());

    private static RoundContext Draft(ScoreSheet sheet, params string[] sites) =>
        new(sites.Select(At).ToList(), sheet);

    [Fact]
    public void Evaluate_OnePointPerNewSite()
    {
        RuleOutcome outcome = new TouristSitesRule().Evaluate(Draft(new ScoreSheet(), "A", "B", "C", "D"));

        Assert.Equal(4, outcome.Points);
        Assert.Empty(outcome.Options);
    }

    [Fact]
    public void Evaluate_SitesVisitedInEarlierRoundsScoreNothing()
    {
        var sheet = new ScoreSheet();
        sheet.MarkVisited(new[] { "A", "C" });

        RuleOutcome outcome = new TouristSitesRule().Evaluate(Draft(sheet, "A", "B", "C", "D"));

        Assert.Equal(2, outcome.Points);
    }

    [Fact]
    public void Evaluate_AllSitesAlreadyVisited_ScoresZero()
    {
        var sheet = new ScoreSheet();
        sheet.MarkVisited(new[] { "A", "B" });

        Assert.Equal(0, new TouristSitesRule().Evaluate(Draft(sheet, "A", "B")).Points);
    }

    [Fact]
    public void Evaluate_DoesNotMarkTheSitesAsVisited()
    {
        var sheet = new ScoreSheet();

        new TouristSitesRule().Evaluate(Draft(sheet, "A", "B"));

        Assert.Empty(sheet.VisitedSites);
    }

    [Fact]
    public void Category_IsTouristSites() =>
        Assert.Equal(ScoreCategories.TouristSites, new TouristSitesRule().Category);
}

public class AustraliaEditionTests
{
    private static readonly AustraliaEdition Edition = new();

    [Fact]
    public void Deck_Has28CardsWithDistinctSites()
    {
        Assert.Equal(28, Edition.Cards.Count);
        Assert.Equal(28, Edition.Cards.Select(c => c.Site).Distinct().Count());
    }

    [Fact]
    public void Deck_CoversLettersAToZ()
    {
        var sites = Edition.Cards.Select(c => c.Site).ToHashSet();
        foreach (char letter in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            Assert.Contains(letter.ToString(), sites);
    }

    [Fact]
    public void Deck_EveryCardHasTwoSymbols() =>
        Assert.All(Edition.Cards, c => Assert.Equal(2, c.Symbols.Count));

    [Fact]
    public void Deck_EveryCardHasAtMostOneSymbolPerCategory() =>
        Assert.All(Edition.Cards, c =>
            Assert.Equal(c.Symbols.Count, c.Symbols.Select(s => s.Category).Distinct().Count()));

    [Fact]
    public void Deck_UsesOnlyTheThreeAustralianSymbolCategories() =>
        Assert.All(
            Edition.Cards.SelectMany(c => c.Symbols).Select(s => s.Category).Distinct(),
            category => Assert.Contains(category, new[]
            {
                AustraliaEdition.CollectionSymbol, AustraliaEdition.AnimalSymbol, AustraliaEdition.ActivitySymbol
            }));

    [Fact]
    public void Regions_AreTheSevenAustralianRegionsWithFourSitesEach()
    {
        Assert.Equal(
            new[]
            {
                "Western Australia", "Northern Territory", "Queensland", "South Australia",
                "New South Wales", "Victoria", "Tasmania"
            },
            Edition.Regions.Select(r => r.Name));
        Assert.All(Edition.Regions, r => Assert.Equal(4, r.Sites.Count));
    }

    [Theory]
    [InlineData("Western Australia", "ABCD")]
    [InlineData("Northern Territory", "EFGH")]
    [InlineData("Queensland", "IJKL")]
    [InlineData("South Australia", "MNOP")]
    [InlineData("New South Wales", "QRST")]
    [InlineData("Victoria", "UVWX")]
    public void Regions_ContainTheSitesOfTheRulesLetterRanges(string region, string letters) =>
        Assert.Equal(
            letters.Select(l => l.ToString()),
            Edition.Regions.Single(r => r.Name == region).Sites.OrderBy(s => s, StringComparer.Ordinal));

    [Fact]
    public void Regions_TasmaniaStartsWithYAndZ() =>
        Assert.Contains("Y", Edition.Regions.Single(r => r.Name == "Tasmania").Sites);

    [Fact]
    public void Regions_EveryCardBelongsToTheRegionThatListsItsSite()
    {
        foreach (Card card in Edition.Cards)
            Assert.Contains(card.Site, Edition.Regions.Single(r => r.Name == card.Region).Sites);
    }

    [Fact]
    public void Parameters_MatchTheRules()
    {
        Assert.Equal(4, Edition.Rounds);
        Assert.Equal(7, Edition.HandSize);
        Assert.Equal(2, Edition.MinPlayers);
        Assert.Equal(4, Edition.MaxPlayers);
        Assert.Equal(3, Edition.RegionBonus);
        Assert.Equal(ScoreCategories.ThrowAndCatch, Edition.TieBreakCategory);
        // Four players with seven cards each use the whole deck.
        Assert.Equal(Edition.MaxPlayers * Edition.HandSize, Edition.Cards.Count);
    }

    [Fact]
    public void ScoringRules_AreTheFiveCategoriesOfTheRules() =>
        Assert.Equal(
            new[]
            {
                ScoreCategories.ThrowAndCatch, ScoreCategories.TouristSites, ScoreCategories.Collections,
                ScoreCategories.Animals, ScoreCategories.Activities
            },
            Edition.ScoringRules.Select(r => r.Category));
}
