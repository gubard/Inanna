using Avalonia.Platform.Storage;

namespace Inanna.Helpers;

public static class StorageFolderExtension
{
    public static DirectoryInfo ToDirectory(this IStorageFolder folder)
    {
        return new DirectoryInfo(folder.Path.LocalPath);
    }
}