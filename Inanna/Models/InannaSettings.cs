using Gaia.Services;

namespace Inanna.Models;

public sealed record InannaSettings
    : ObjectStorageValue<InannaSettings>,
        IStaticFactory<InannaSettings>
{
    public ThemeVariantType Theme { get; set; }
    public Lang Lang { get; set; }

    public static InannaSettings Create()
    {
        return new();
    }
}
