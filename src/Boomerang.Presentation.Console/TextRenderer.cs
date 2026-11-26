using Boomerang.Domain.Game;
using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;
using System.Text;

namespace Boomerang.Presentation.Console;

public static class TextRenderer
{
    public static string FormatCard(Card card)
    {
        string symbols = string.Join(" | ", card.Symbols.Select(s => s.Name));
        return $"  [{card.Site}] {card.Name,-26} #{card.Number}  {card.Region,-19} {symbols}";
    }

    public static string FormatCards(IReadOnlyList<Card> cards) =>
        cards.Count == 0 ? "  (none)" : string.Join(Environment.NewLine, cards.Select(FormatCard));

    public static string FormatPickPrompt(PickContext context)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"=== Round {context.Round}/{context.TotalRounds} ===");
        if (context.ThrowCard is not null)
        {
            sb.AppendLine("Your Throw card (hidden from the others):");
            sb.AppendLine(FormatCard(context.ThrowCard));
        }
        if (context.FaceUp.Count > 0)
        {
            sb.AppendLine("Your face-up cards:");
            sb.AppendLine(FormatCards(context.FaceUp));
        }
        if (context.VisitedSites.Count > 0)
            sb.AppendLine("Sites visited in earlier rounds: " + string.Join(" ", context.VisitedSites.OrderBy(s => s)));

        sb.AppendLine("Your hand:");
        sb.AppendLine(FormatCards(context.Hand));
        sb.Append(context.Kind == PickKind.Throw
            ? "Choose your Throw card (type its site letter): "
            : "Choose a card to draft (type its site letter): ");
        return sb.ToString();
    }

    public static string FormatReveals(IReadOnlyList<PlayerReveal> reveals)
    {
        var sb = new StringBuilder();
        foreach (PlayerReveal reveal in reveals)
        {
            sb.AppendLine();
            sb.AppendLine($"{reveal.PlayerName} has drafted:");
            sb.Append(FormatCards(reveal.FaceUpCards));
            sb.AppendLine();
        }
        return sb.ToString().TrimEnd();
    }

    public static string FormatRoundResult(RoundResult result)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"*** Round {result.Round}/{result.TotalRounds} scored ***");
        sb.AppendLine("Your cards (Throw first, Catch last):");
        sb.AppendLine(FormatCards(result.Draft));
        foreach (ScoreEntry entry in result.Score.Entries)
        {
            string detail = entry.Detail is null ? "" : $"  ({entry.Detail})";
            sb.AppendLine($"  {entry.Category,-15} {entry.Points,3}{detail}");
        }
        sb.AppendLine($"  {"Round total",-15} {result.Score.Total,3}   (running total {result.RunningTotal})");
        if (result.ClaimedRegions.Count > 0)
            sb.AppendLine("Regions whose bonus has been taken: " + string.Join(", ", result.ClaimedRegions));
        return sb.ToString().TrimEnd();
    }

    public static string FormatGameResult(GameResult result)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine("*********************************************");
        foreach (PlayerResult player in result.Players)
        {
            sb.AppendLine($"{player.Name}: {player.Total} points");
            sb.Append($"  {"",-15}");
            for (int r = 1; r <= player.Rounds.Count; r++) sb.Append($"{"R" + r,5}");
            sb.AppendLine($"{"Total",7}");
            foreach (string category in result.Categories)
            {
                sb.Append($"  {category,-15}");
                foreach (RoundScore round in player.Rounds) sb.Append($"{round.PointsFor(category),5}");
                sb.AppendLine($"{player.CategoryTotals[category],7}");
            }
            sb.AppendLine();
        }

        string names = string.Join(" and ", result.Winners.Select(w => w.Name));
        sb.Append(result.Winners.Count == 1
            ? $"The winner is {names} with {result.Winners[0].Total} points!"
            : $"It's a tie! The winners are {names} with {result.Winners[0].Total} points each!");
        return sb.ToString();
    }
}
