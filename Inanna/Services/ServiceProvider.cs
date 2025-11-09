namespace Inanna.Services;

public interface IServiceProvider
{
    T GetService<T>();
}