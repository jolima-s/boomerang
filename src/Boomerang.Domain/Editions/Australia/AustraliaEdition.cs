using Boomerang.Domain.Editions.Shared;
using Boomerang.Domain.Editions.Shared.DefaultRules;
using Boomerang.Domain.Extensions;
using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;
using System.ComponentModel.DataAnnotations;

namespace Boomerang.Domain.Editions.Australia;

public sealed class AustraliaEdition : IGameEdition
{
    public const string CollectionSymbol = "Collection";
    public const string AnimalSymbol = "Animal";
    public const string ActivitySymbol = "Activity";

    public string Name => "Boomerang Australia";
    public IReadOnlyList<Card> Cards { get; }
    public IReadOnlyList<Region> Regions { get; }
    public IReadOnlyList<IScoringRule> ScoringRules { get; }
    public int Rounds => 4;
    public int HandSize => 7;
    public int MinPlayers => 2;
    public int MaxPlayers => 4;
    public int RegionBonus => 3;
    public string TieBreakCategory => ScoreCategories.ThrowAndCatch;

    private static readonly IReadOnlyDictionary<string, int> CollectionValues = new Dictionary<string, int>
    {
        [AustraliaCollection.Leaves.GetDisplayName()] = 1,
        [AustraliaCollection.Wildflowers.GetDisplayName()] = 2,
        [AustraliaCollection.Shells.GetDisplayName()] = 3,
        [AustraliaCollection.Souvenirs.GetDisplayName()] = 5
    };

    private static readonly IReadOnlyDictionary<string, int> AnimalValues = new Dictionary<string, int>
    {
        [AustraliaAnimal.Kangaroos.GetDisplayName()] = 3,
        [AustraliaAnimal.Emus.GetDisplayName()] = 4,
        [AustraliaAnimal.Wombats.GetDisplayName()] = 5,
        [AustraliaAnimal.Koalas.GetDisplayName()] = 7,
        [AustraliaAnimal.Platypuses.GetDisplayName()] = 9
    };

    private static readonly string[] ActivityNames = 
    { 
        AustraliaActivity.Swimming.GetDisplayName(), 
        AustraliaActivity.Bushwalking.GetDisplayName(), 
        AustraliaActivity.IndigenousCulture.GetDisplayName(), 
        AustraliaActivity.Sightseeing.GetDisplayName() 
    };
    private static readonly int[] ActivityPoints = { 0, 2, 4, 7, 10, 15 };

    public AustraliaEdition()
    {
        var factory = new CardFactory(
            CollectionSymbol,
            AnimalSymbol,
            ActivitySymbol);

        Cards = factory.CreateMany(AustraliaCards.Definitions);
        Regions = Cards.GroupBy(c => c.Region)
                       .Select(g => new Region(g.Key, g.Select(c => c.Site).ToList()))
                       .ToList();
        ScoringRules = new IScoringRule[]
        {
            new ThrowAndCatchRule(),
            new TouristSitesRule(),
            new CollectionsRule(CollectionSymbol, CollectionValues, doubleLimit: 7),
            new AnimalsRule(AnimalSymbol, AnimalValues),
            new ActivitiesRule(ActivitySymbol, ActivityNames, ActivityPoints)
        };
    }
}

public enum AustraliaCollection
{
    [Display(Name = "Leaves")]
    Leaves,
    [Display(Name = "Wildflowers")]
    Wildflowers,
    [Display(Name = "Shells")]
    Shells,
    [Display(Name = "Souvenirs")]
    Souvenirs
}

public enum AustraliaAnimal
{
    [Display(Name = "Kangaroos")]
    Kangaroos,
    [Display(Name = "Emus")]
    Emus,
    [Display(Name = "Wombats")]
    Wombats,
    [Display(Name = "Koalas")]
    Koalas,
    [Display(Name = "Platypuses")]
    Platypuses
}

public enum AustraliaActivity
{
    [Display(Name = "Swimming")]
    Swimming,
    [Display(Name = "Bushwalking")]
    Bushwalking,
    [Display(Name = "Indigenous Culture")]
    IndigenousCulture,
    [Display(Name = "Sightseeing")]
    Sightseeing
}