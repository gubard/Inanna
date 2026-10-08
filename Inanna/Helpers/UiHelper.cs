using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Gaia.Helpers;

namespace Inanna.Helpers;

public static class UiHelper
{
    public static readonly SelectionMode MultipleToggle =
        SelectionMode.Multiple | SelectionMode.Toggle;

    public static readonly FilePickerFileType DocxFilePickerFileType = new("Microsoft Word")
    {
        Patterns = [FileExtensions.DocxPattern],
        MimeTypes = [MimeTypes.Docx],
    };

    public static readonly FilePickerFileType JpegFilePickerFileType = new(
        "Joint Photographic Experts Group"
    )
    {
        Patterns = [FileExtensions.JpgPattern, FileExtensions.JpegPattern],
        MimeTypes = [MimeTypes.Jpeg],
    };

    public static readonly FilePickerFileType PngFilePickerFileType = new(
        "Portable Network Graphics"
    )
    {
        Patterns = [FileExtensions.PngPattern],
        MimeTypes = [MimeTypes.Png],
    };
}
