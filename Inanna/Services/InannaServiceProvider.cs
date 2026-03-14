using Jab;

namespace Inanna.Services;

[ServiceProviderModule]
[Transient(typeof(IInannaViewModelFactory), typeof(InannaViewModelFactory))]
[Singleton(typeof(InannaCommands))]
public interface IInannaServiceProvider : IServiceProvider;
