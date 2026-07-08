using Gaia.Services;

namespace Inanna.Models;

public sealed record InannaSettings : ObjectStorageValue<InannaSettings>, IStaticFactory<InannaSettings>
{
    public required ThemeVariantType Theme { get; init; }
    public required Lang Lang { get; init; }
    
    public static InannaSettings Create()
    {
        return new()
        {
Lang = Lang.Ukrainian,
Theme = ThemeVariantType.Dark,
        };
    }
}

public enum ThemeVariantType
{
    System,
    Dark,
    Light,
}
