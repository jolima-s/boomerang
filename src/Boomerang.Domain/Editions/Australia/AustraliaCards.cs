using Boomerang.Domain.Editions.Shared;
using Boomerang.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Boomerang.Domain.Editions.Australia;

public static class AustraliaCards
{
    public static IReadOnlyList<CardDefinition> Definitions { get; } =
    [
        new("The Bungle Bungles", "A", AustraliaRegion.WesternAustralia.GetDisplayName(), 1, Collection: AustraliaCollection.Leaves.GetDisplayName(), Activity: AustraliaActivity.IndigenousCulture.GetDisplayName()),
        new("The Pinnacles", "B", AustraliaRegion.WesternAustralia.GetDisplayName(), 1, Animal: AustraliaAnimal.Kangaroos.GetDisplayName(), Activity: AustraliaActivity.Sightseeing.GetDisplayName()),
        new("Margaret River", "C", AustraliaRegion.WesternAustralia.GetDisplayName(), 1, Collection: AustraliaCollection.Shells.GetDisplayName(), Animal: AustraliaAnimal.Kangaroos.GetDisplayName()),
        new("Kalbarri National Park", "D", AustraliaRegion.WesternAustralia.GetDisplayName(), 1, Collection: AustraliaCollection.Wildflowers.GetDisplayName(), Activity: AustraliaActivity.Bushwalking.GetDisplayName()),

        new("Uluru", "E", AustraliaRegion.NorthernTerritory.GetDisplayName(), 4, Animal: AustraliaAnimal.Emus.GetDisplayName(), Activity: AustraliaActivity.IndigenousCulture.GetDisplayName()),
        new("Kakadu National Park", "F", AustraliaRegion.NorthernTerritory.GetDisplayName(), 4, Animal: AustraliaAnimal.Wombats.GetDisplayName(), Activity: AustraliaActivity.Sightseeing.GetDisplayName()),
        new("Nitmiluk National Park", "G", AustraliaRegion.NorthernTerritory.GetDisplayName(), 4, Collection: AustraliaCollection.Shells.GetDisplayName(), Animal: AustraliaAnimal.Platypuses.GetDisplayName()),
        new("King's Canyon", "H", AustraliaRegion.NorthernTerritory.GetDisplayName(), 4, Animal: AustraliaAnimal.Koalas.GetDisplayName(), Activity: AustraliaActivity.Swimming.GetDisplayName()),

        new("The Great Barrier Reef", "I", AustraliaRegion.Queensland.GetDisplayName(), 6, Collection: AustraliaCollection.Wildflowers.GetDisplayName(), Activity: AustraliaActivity.Sightseeing.GetDisplayName()),
        new("The Whitsundays", "J", AustraliaRegion.Queensland.GetDisplayName(), 6, Animal: AustraliaAnimal.Kangaroos.GetDisplayName(), Activity: AustraliaActivity.IndigenousCulture.GetDisplayName()),
        new("Daintree Rainforest", "K", AustraliaRegion.Queensland.GetDisplayName(), 6, Collection: AustraliaCollection.Souvenirs.GetDisplayName(), Activity: AustraliaActivity.Bushwalking.GetDisplayName()),
        new("Surfers Paradise", "L", AustraliaRegion.Queensland.GetDisplayName(), 6, Collection: AustraliaCollection.Wildflowers.GetDisplayName(), Activity: AustraliaActivity.Swimming.GetDisplayName()),

        new("Barossa Valley", "M", AustraliaRegion.SouthAustralia.GetDisplayName(), 3, Animal: AustraliaAnimal.Koalas.GetDisplayName(), Activity: AustraliaActivity.Bushwalking.GetDisplayName()),
        new("Lake Eyre", "N", AustraliaRegion.SouthAustralia.GetDisplayName(), 3, Animal: AustraliaAnimal.Emus.GetDisplayName(), Activity: AustraliaActivity.Swimming.GetDisplayName()),
        new("Kangaroo Island", "O", AustraliaRegion.SouthAustralia.GetDisplayName(), 3, Animal: AustraliaAnimal.Kangaroos.GetDisplayName(), Activity: AustraliaActivity.Bushwalking.GetDisplayName()),
        new("Mount Gambier", "P", AustraliaRegion.SouthAustralia.GetDisplayName(), 3, Collection: AustraliaCollection.Wildflowers.GetDisplayName(), Activity: AustraliaActivity.Sightseeing.GetDisplayName()),

        new("Blue Mountains", "Q", AustraliaRegion.NewSouthWales.GetDisplayName(), 5, Animal: AustraliaAnimal.Wombats.GetDisplayName(), Activity: AustraliaActivity.IndigenousCulture.GetDisplayName()),
        new("Sydney Harbour", "R", AustraliaRegion.NewSouthWales.GetDisplayName(), 5, Animal: AustraliaAnimal.Emus.GetDisplayName(), Activity: AustraliaActivity.Sightseeing.GetDisplayName()),
        new("Bondi Beach", "S", AustraliaRegion.NewSouthWales.GetDisplayName(), 5, Animal: AustraliaAnimal.Wombats.GetDisplayName(), Activity: AustraliaActivity.Swimming.GetDisplayName()),
        new("Hunter Valley", "T", AustraliaRegion.NewSouthWales.GetDisplayName(), 5, Animal: AustraliaAnimal.Emus.GetDisplayName(), Activity: AustraliaActivity.Bushwalking.GetDisplayName()),

        new("Melbourne", "U", AustraliaRegion.Victoria.GetDisplayName(), 2, Animal: AustraliaAnimal.Wombats.GetDisplayName(), Activity: AustraliaActivity.Bushwalking.GetDisplayName()),
        new("The MCG", "V", AustraliaRegion.Victoria.GetDisplayName(), 2, Collection: AustraliaCollection.Leaves.GetDisplayName(), Activity: AustraliaActivity.IndigenousCulture.GetDisplayName()),
        new("Twelve Apostles", "W", AustraliaRegion.Victoria.GetDisplayName(), 2, Collection: AustraliaCollection.Shells.GetDisplayName(), Activity: AustraliaActivity.Swimming.GetDisplayName()),
        new("Royal Exhibition Building", "X", AustraliaRegion.Victoria.GetDisplayName(), 2, Collection: AustraliaCollection.Leaves.GetDisplayName(), Animal: AustraliaAnimal.Platypuses.GetDisplayName()),

        new("Salamanca Markets", "Y", AustraliaRegion.Tasmania.GetDisplayName(), 7, Collection: AustraliaCollection.Leaves.GetDisplayName(), Animal: AustraliaAnimal.Emus.GetDisplayName()),
        new("Mount Wellington", "Z", AustraliaRegion.Tasmania.GetDisplayName(), 7, Animal: AustraliaAnimal.Koalas.GetDisplayName(), Activity: AustraliaActivity.Sightseeing.GetDisplayName()),
        new("Port Arthur", "@", AustraliaRegion.Tasmania.GetDisplayName(), 7, Collection: AustraliaCollection.Leaves.GetDisplayName(), Activity: AustraliaActivity.IndigenousCulture.GetDisplayName()),
        new("Richmond", "#", AustraliaRegion.Tasmania.GetDisplayName(), 7, Animal: AustraliaAnimal.Kangaroos.GetDisplayName(), Activity: AustraliaActivity.Swimming.GetDisplayName())
    ];
}

public enum AustraliaRegion
{
    [Display(Name = "Western Australia")]
    WesternAustralia,

    [Display(Name = "Northern Territory")]
    NorthernTerritory,

    [Display(Name = "Queensland")]
    Queensland,

    [Display(Name = "South Australia")]
    SouthAustralia,

    [Display(Name = "New South Wales")]
    NewSouthWales,

    [Display(Name = "Victoria")]
    Victoria,

    [Display(Name = "Tasmania")]
    Tasmania
}