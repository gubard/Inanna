using Gaia.Services;

namespace Inanna.Models;

public sealed class InannaSettings : ObjectStorageValue<InannaSettings>
{
    public ThemeVariantType Theme { get; set; }
    public Lang Lang { get; set; }
}

public enum ThemeVariantType
{
    System,
    Dark,
    Light,
}
