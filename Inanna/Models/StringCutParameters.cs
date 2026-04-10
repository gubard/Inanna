using Gaia.Helpers;

namespace Inanna.Models;

public sealed class StringCutParameters
{
    public StringCutParameters(byte maxLineCount, ushort maxLineLength)
    {
        MaxLineCount = maxLineCount;
        MaxLineLength = maxLineLength;
    }

    public byte MaxLineCount { get; }
    public ushort MaxLineLength { get; }

    public string Cut(string str)
    {
        var lines = str.Split(Environment.NewLine);

        return lines
            .Take(MaxLineCount)
            .Select(x => x.Length > MaxLineLength ? x.Substring(0, MaxLineLength) + "..." : x)
            .Concat(lines.Length > MaxLineCount ? ["..."] : [])
            .JoinString(Environment.NewLine);
    }
}
