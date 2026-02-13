namespace Inanna.Generator;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class ViewPair : Attribute
{
    public ViewPair(Type view, Type viewModel) { }
}
