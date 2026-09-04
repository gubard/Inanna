using Avalonia.Markup.Xaml;
using Gaia.Helpers;

namespace Inanna.Helpers;

public sealed class EnumValuesExtension : MarkupExtension
{
    public Type EnumType { get; set; }

    public EnumValuesExtension() { }

    public EnumValuesExtension(Type enumType)
    {
        EnumType = enumType;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (EnumType == null)
        {
            throw new InvalidOperationException("EnumType must be specified.");
        }

        var actualEnumType = Nullable.GetUnderlyingType(EnumType) ?? EnumType;

        if (!actualEnumType.IsEnum)
        {
            throw new ArgumentException("Type must be an Enum.");
        }

        return EnumHelper.GetValues(actualEnumType);
    }
}
