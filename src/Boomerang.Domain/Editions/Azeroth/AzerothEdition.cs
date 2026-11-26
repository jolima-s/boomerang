using Boomerang.Domain.Editions.Shared;
using Boomerang.Domain.Editions.Shared.DefaultRules;
using Boomerang.Domain.Extensions;
using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;
using System.ComponentModel.DataAnnotations;

namespace Boomerang.Domain.Editions.Azeroth;

public sealed class AzerothEdition : IGameEdition
{
    public const string CollectionSymbol = "Gatherables";
    public const string AnimalSymbol = "Beasts";
    public const string ActivitySymbol = "Professions";

    public string Name => "Boomerang Azeroth";
    public IReadOnlyList<Card> Cards { get; }
    public IReadOnlyList<Region> Regions { get; }
    public IReadOnlyList<IScoringRule> ScoringRules { get; }
    public int Rounds => 4;
    public int HandSize => 7;
    public int MinPlayers => 2;
    public int MaxPlayers => 4;
    public int RegionBonus => 3;
    public string TieBreakCategory => ScoreCategories.Animals;

    private static readonly IReadOnlyDictionary<string, int> CollectionValues = new Dictionary<string, int>
    {
        [AzerothCollection.Ore.GetDisplayName()] = 1,
        [AzerothCollection.Herbs.GetDisplayName()] = 2,
        [AzerothCollection.Dust.GetDisplayName()] = 3,
        [AzerothCollection.Gems.GetDisplayName()] = 5
    };

    private static readonly IReadOnlyDictionary<string, int> AnimalValues = new Dictionary<string, int>
    {
        [AzerothAnimal.Scorpids.GetDisplayName()] = 3,
        [AzerothAnimal.Murlocs.GetDisplayName()] = 4,
        [AzerothAnimal.Raptors.GetDisplayName()] = 5,
        [AzerothAnimal.Hippogryphs.GetDisplayName()] = 7,
        [AzerothAnimal.Chimaeras.GetDisplayName()] = 9
    };

    private static readonly string[] ActivityNames = 
    { 
        AzerothActivity.Alchemy.GetDisplayName(), 
        AzerothActivity.Blacksmithing.GetDisplayName(), 
        AzerothActivity.Enchanting.GetDisplayName(), 
        AzerothActivity.Jewelcrafting.GetDisplayName() 
    };
    private static readonly int[] ActivityPoints = { 0, 2, 4, 7, 10, 15 };

    public AzerothEdition()
    {
        var factory = new CardFactory(
            CollectionSymbol,
            AnimalSymbol,
            ActivitySymbol);

        Cards = factory.CreateMany(AzerothCards.Definitions);
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

public enum AzerothCollection
{
    [Display(Name = "Ore")]
    Ore,
    [Display(Name = "Herbs")]
    Herbs,
    [Display(Name = "Dust")]
    Dust,
    [Display(Name = "Gems")]
    Gems
}

public enum AzerothAnimal
{
    [Display(Name = "Scorpids")]
    Scorpids,
    [Display(Name = "Murlocs")]
    Murlocs,
    [Display(Name = "Raptors")]
    Raptors,
    [Display(Name = "Hippogryphs")]
    Hippogryphs,
    [Display(Name = "Chimaeras")]
    Chimaeras
}

public enum AzerothActivity
{
    [Display(Name = "Alchemy")]
    Alchemy,
    [Display(Name = "Blacksmithing")]
    Blacksmithing,
    [Display(Name = "Enchanting")]
    Enchanting,
    [Display(Name = "Jewelcrafting")]
    Jewelcrafting
}