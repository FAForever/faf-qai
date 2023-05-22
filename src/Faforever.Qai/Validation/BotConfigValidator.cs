using FluentValidation;
using System;

namespace Faforever.Qai.Validation
{
    public class BotConfigValidator : AbstractValidator<BotConfig>
    {
        public BotConfigValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty();
            RuleFor(x => x.Irc).SetValidator(new IrcSettingsValidator());
            RuleFor(x => x.Twitch).SetValidator(new TwitchSettingsValidator());
            RuleFor(x => x.BotPrefix).NotEmpty();
            RuleFor(x => x.Faf).SetValidator(new FafSettingsValidator());
            RuleFor(x => x.Discord).SetValidator(new DiscordSettingsValidator());
            //RuleFor(x => x.Host).NotEmpty();
        }
    }

    public class IrcSettingsValidator : AbstractValidator<BotConfig.IrcSettings>
    {
        public IrcSettingsValidator()
        {
            RuleFor(x => x.Connection).NotEmpty();
            RuleFor(x => x.User).NotEmpty();
            //RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Channels).NotEmpty();
        }
    }

    public class FafSettingsValidator : AbstractValidator<BotConfig.FafSettings>
    {
        public FafSettingsValidator()
        {
            //RuleFor(x => x.ClientId).NotEmpty();
            //RuleFor(x => x.ClientSecret).NotEmpty();
            RuleFor(x => x.Api).NotEmpty().Must(UriValidator.IsValidUri).WithMessage("Invalid URL format");
            RuleFor(x => x.Callback).NotEmpty();
        }
    }

    public class DiscordSettingsValidator : AbstractValidator<BotConfig.DiscordSettings>
    {
        public DiscordSettingsValidator()
        {
            RuleFor(x => x.Callback).NotEmpty();
            RuleFor(x => x.Api).NotEmpty().Must(UriValidator.IsValidUri).WithMessage("Invalid URL format");
            RuleFor(x => x.Scope).NotEmpty();
            //RuleFor(x => x.ClientId).NotEmpty();
            //RuleFor(x => x.ClientSecret).NotEmpty();
            RuleFor(x => x.TokenEndpoint).NotEmpty().Must(UriValidator.IsValidUri).WithMessage("Invalid URL format");
            RuleFor(x => x.Token).NotEmpty();
        }
    }

    public class TwitchSettingsValidator : AbstractValidator<BotConfig.TwitchSettings>
    {
        public TwitchSettingsValidator()
        {
            //RuleFor(x => x.ClientId).NotEmpty();
            //RuleFor(x => x.ClientSecret).NotEmpty();
        }
    }

    public static class UriValidator
    {
        public static bool IsValidUri(string uri)
        {
            return Uri.TryCreate(uri, UriKind.Absolute, out _);
        }
    }
}
