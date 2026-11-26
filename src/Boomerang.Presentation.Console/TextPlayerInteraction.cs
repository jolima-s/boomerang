using Boomerang.Application.Interfaces;
using Boomerang.Application.Players;
using Boomerang.Domain.Game;
using Boomerang.Domain.Models;
using Boomerang.Domain.Scoring;
using System.Text;

namespace Boomerang.Presentation.Console;

/// <summary>A human player, reached through any <see cref="ITextChannel"/> (console or network).</summary>
public sealed class TextPlayerInteraction : IPlayerInteraction
{
    private readonly ITextChannel _channel;

    public TextPlayerInteraction(ITextChannel channel) => _channel = channel;

    public async Task<Card> ChooseCardAsync(PickContext context, CancellationToken cancellationToken = default)
    {
        string prompt = TextRenderer.FormatPickPrompt(context);
        while (true)
        {
            string input = (await _channel.PromptAsync(prompt, cancellationToken)).Trim();
            Card? chosen = context.Hand.FirstOrDefault(c => string.Equals(c.Site, input, StringComparison.OrdinalIgnoreCase));
            if (chosen is not null) return chosen;

            prompt = $"\"{input}\" is not a site in your hand. Type one of: "
                     + string.Join(" ", context.Hand.Select(c => c.Site)) + ": ";
        }
    }

    public async Task<string?> ChooseOptionalScoreAsync(string category, IReadOnlyList<ScoreOption> options, CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine($"{category}: you may score ONE of these this round (each only once per game):");
        for (int i = 0; i < options.Count; i++)
            sb.AppendLine($"  {i + 1}. {options[i].Label} = {options[i].Points} points");
        sb.AppendLine("  0. Score nothing");
        sb.Append("Your choice: ");

        string prompt = sb.ToString();
        while (true)
        {
            string input = (await _channel.PromptAsync(prompt, cancellationToken)).Trim();
            if (int.TryParse(input, out int n) && n >= 0 && n <= options.Count) return n == 0 ? null : options[n - 1].Key;
            prompt = $"Please type a number between 0 and {options.Count}: ";
        }
    }

    public Task OnCardsRevealedAsync(IReadOnlyList<PlayerReveal> reveals, CancellationToken cancellationToken = default) =>
        _channel.SendAsync(TextRenderer.FormatReveals(reveals), cancellationToken);

    public Task OnRoundScoredAsync(RoundResult result, CancellationToken cancellationToken = default) =>
        _channel.SendAsync(TextRenderer.FormatRoundResult(result), cancellationToken);

    public Task OnGameFinishedAsync(GameResult result, CancellationToken cancellationToken = default) =>
        _channel.SendAsync(TextRenderer.FormatGameResult(result), cancellationToken);
}
