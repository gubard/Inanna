using Avalonia.Platform.Storage;

namespace Inanna.Helpers;

public static class StorageFileExtension
{
    public static async ValueTask<byte[]> GetDataAsync(this IStorageFile file, CancellationToken ct)
    {
        await using var stream = await file.OpenReadAsync();
        await using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, ct);

        return memoryStream.ToArray();
    }

    public static FileInfo ToFile(this IStorageFile file)
    {
        return new(file.Path.LocalPath);
    }
}
