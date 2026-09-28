using Avalonia.Platform;

namespace Inanna.Helpers;

public static class AssetHelper
{
    public static string LoadString(Uri uri)
    {
        using var assetStream = AssetLoader.Open(uri);
        using var reader = new StreamReader(assetStream);
        var text = reader.ReadToEnd();

        return text;
    }
}
