using Inanna.Helpers;
using Jab;

namespace Inanna.Services;

[ServiceProviderModule]
[Transient(typeof(IInannaViewModelFactory), typeof(InannaViewModelFactory))]
[Transient(typeof(IItemMutationService), typeof(ItemMutationService))]
[Singleton(typeof(InannaCommands))]
public interface IInannaServiceProvider : IServiceProvider;
