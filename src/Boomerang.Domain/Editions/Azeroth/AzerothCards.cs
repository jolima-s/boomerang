using Boomerang.Domain.Editions.Shared;
using Boomerang.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Boomerang.Domain.Editions.Azeroth;

public static class AzerothCards
{
    public static IReadOnlyList<CardDefinition> Definitions { get; } =
    [
        // Northern Kalimdor
        new("Teldrassil", "A", AzerothRegion.NorthernKalimdor.GetDisplayName(), 5, Animal: AzerothAnimal.Hippogryphs.GetDisplayName(), Activity: AzerothActivity.Alchemy.GetDisplayName()),
        new("Azuremyst Isle", "B", AzerothRegion.NorthernKalimdor.GetDisplayName(), 5, Collection: AzerothCollection.Gems.GetDisplayName(), Animal: AzerothAnimal.Murlocs.GetDisplayName()),
        new("Winterspring", "C", AzerothRegion.NorthernKalimdor.GetDisplayName(), 5, Animal: AzerothAnimal.Chimaeras.GetDisplayName(), Activity: AzerothActivity.Jewelcrafting.GetDisplayName()),
        new("Ashenvale", "D", AzerothRegion.NorthernKalimdor.GetDisplayName(), 5, Collection: AzerothCollection.Herbs.GetDisplayName(), Animal: AzerothAnimal.Hippogryphs.GetDisplayName()),
        new("Azshara", "E", AzerothRegion.NorthernKalimdor.GetDisplayName(), 2, Collection: AzerothCollection.Dust.GetDisplayName(), Animal: AzerothAnimal.Chimaeras.GetDisplayName()),

        // Central Kalimdor
        new("The Barrens", "F", AzerothRegion.CentralKalimdor.GetDisplayName(), 2, Animal: AzerothAnimal.Raptors.GetDisplayName(), Activity: AzerothActivity.Blacksmithing.GetDisplayName()),
        new("Durotar", "G", AzerothRegion.CentralKalimdor.GetDisplayName(), 2, Animal: AzerothAnimal.Raptors.GetDisplayName(), Activity: AzerothActivity.Jewelcrafting.GetDisplayName()),
        new("Mulgore", "H", AzerothRegion.CentralKalimdor.GetDisplayName(), 2, Collection: AzerothCollection.Ore.GetDisplayName(), Activity: AzerothActivity.Enchanting.GetDisplayName()),
        new("Dustwallow Marsh", "I", AzerothRegion.CentralKalimdor.GetDisplayName(), 1, Animal: AzerothAnimal.Raptors.GetDisplayName(), Activity: AzerothActivity.Alchemy.GetDisplayName()),
        new("Desolace", "J", AzerothRegion.CentralKalimdor.GetDisplayName(), 1, Collection: AzerothCollection.Dust.GetDisplayName(), Activity: AzerothActivity.Blacksmithing.GetDisplayName()),

        // Southern Kalimdor
        new("Feralas", "K", AzerothRegion.SouthernKalimdor.GetDisplayName(), 1, Animal: AzerothAnimal.Hippogryphs.GetDisplayName(), Activity: AzerothActivity.Alchemy.GetDisplayName()),
        new("Silithus", "L", AzerothRegion.SouthernKalimdor.GetDisplayName(), 1, Animal: AzerothAnimal.Scorpids.GetDisplayName(), Activity: AzerothActivity.Jewelcrafting.GetDisplayName()),
        new("Tanaris", "M", AzerothRegion.SouthernKalimdor.GetDisplayName(), 4, Collection: AzerothCollection.Herbs.GetDisplayName(), Animal: AzerothAnimal.Murlocs.GetDisplayName()),
        new("Uldum", "N", AzerothRegion.SouthernKalimdor.GetDisplayName(), 4, Collection: AzerothCollection.Ore.GetDisplayName(), Activity: AzerothActivity.Alchemy.GetDisplayName()),

        // Upper Eastern Kingdoms
        new("Isle of Quel'Danas", "O", AzerothRegion.UpperEasternKingdoms.GetDisplayName(), 4, Collection: AzerothCollection.Herbs.GetDisplayName(), Activity: AzerothActivity.Jewelcrafting.GetDisplayName()),
        new("Eastern Plaguelands", "P", AzerothRegion.UpperEasternKingdoms.GetDisplayName(), 4, Animal: AzerothAnimal.Scorpids.GetDisplayName(), Activity: AzerothActivity.Enchanting.GetDisplayName()),
        new("Western Plaguelands", "Q", AzerothRegion.UpperEasternKingdoms.GetDisplayName(), 3, Collection: AzerothCollection.Ore.GetDisplayName(), Activity: AzerothActivity.Blacksmithing.GetDisplayName()),
        new("Tirisfal Glades", "R", AzerothRegion.UpperEasternKingdoms.GetDisplayName(), 3, Animal: AzerothAnimal.Murlocs.GetDisplayName(), Activity: AzerothActivity.Alchemy.GetDisplayName()),
        new("Hillsbrad Foothills", "S", AzerothRegion.UpperEasternKingdoms.GetDisplayName(), 3, Collection: AzerothCollection.Dust.GetDisplayName(), Activity: AzerothActivity.Enchanting.GetDisplayName()),

        // Mid Eastern Kingdoms
        new("Dun Morogh", "T", AzerothRegion.MidEasternKingdoms.GetDisplayName(), 3, Collection: AzerothCollection.Ore.GetDisplayName(), Activity: AzerothActivity.Blacksmithing.GetDisplayName()),
        new("Searing Gorge", "U", AzerothRegion.MidEasternKingdoms.GetDisplayName(), 6, Animal: AzerothAnimal.Scorpids.GetDisplayName(), Activity: AzerothActivity.Jewelcrafting.GetDisplayName()),
        new("Badlands", "V", AzerothRegion.MidEasternKingdoms.GetDisplayName(), 6, Animal: AzerothAnimal.Scorpids.GetDisplayName(), Activity: AzerothActivity.Enchanting.GetDisplayName()),
        new("Twilight Highlands", "W", AzerothRegion.MidEasternKingdoms.GetDisplayName(), 6, Animal: AzerothAnimal.Scorpids.GetDisplayName(), Activity: AzerothActivity.Alchemy.GetDisplayName()),

        // Lower Eastern Kingdoms
        new("Elwynn Forest", "X", AzerothRegion.LowerEasternKingdoms.GetDisplayName(), 6, Collection: AzerothCollection.Herbs.GetDisplayName(), Activity: AzerothActivity.Enchanting.GetDisplayName()),
        new("Westfall", "Y", AzerothRegion.LowerEasternKingdoms.GetDisplayName(), 7, Animal: AzerothAnimal.Murlocs.GetDisplayName(), Activity: AzerothActivity.Blacksmithing.GetDisplayName()),
        new("Deadwind Pass", "Z", AzerothRegion.LowerEasternKingdoms.GetDisplayName(), 7, Collection: AzerothCollection.Ore.GetDisplayName(), Activity: AzerothActivity.Enchanting.GetDisplayName()),
        new("Blasted Lands", "@", AzerothRegion.LowerEasternKingdoms.GetDisplayName(), 7, Animal: AzerothAnimal.Murlocs.GetDisplayName(), Activity: AzerothActivity.Jewelcrafting.GetDisplayName()),
        new("Stranglethorn Vale", "#", AzerothRegion.LowerEasternKingdoms.GetDisplayName(), 7, Animal: AzerothAnimal.Raptors.GetDisplayName(), Activity: AzerothActivity.Blacksmithing.GetDisplayName())
    ];
}

public enum AzerothRegion
{
    [Display(Name = "Northern Kalimdor")]
    NorthernKalimdor,

    [Display(Name = "Central Kalimdor")]
    CentralKalimdor,

    [Display(Name = "Southern Kalimdor")]
    SouthernKalimdor,

    [Display(Name = "Upper Eastern Kingdoms")]
    UpperEasternKingdoms,

    [Display(Name = "Mid Eastern Kingdoms")]
    MidEasternKingdoms,

    [Display(Name = "Lower Eastern Kingdoms")]
    LowerEasternKingdoms
}