using Avalonia.Platform;

namespace Inanna.Helpers;

public static class AssetHelper
{
    public static string LoadString(string uri)
    {
        using var assetStream = AssetLoader.Open(new(uri));
        using var reader = new StreamReader(assetStream);
        var text = reader.ReadToEnd();

        return text;
    }
}
