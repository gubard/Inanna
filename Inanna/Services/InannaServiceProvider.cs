using Inanna.Helpers;
using Inanna.Models;
using Jab;

namespace Inanna.Services;

[ServiceProviderModule]
[Transient(typeof(IInannaViewModelFactory), typeof(InannaViewModelFactory))]
[Transient(typeof(IItemMutationService), typeof(ItemMutationService))]
[Transient(typeof(ViewModelServices))]
[Singleton(typeof(InannaCommands))]
[Singleton(typeof(IDialogService), Factory = nameof(GetDialogService))]
public interface IInannaServiceProvider : IServiceProvider
{
    public static IDialogService GetDialogService(
        ViewModelServices services,
        ICommandFactory commandFactory,
        IInannaViewModelFactory factory
    )
    {
        return new DialogService("MessageBox", commandFactory, factory, services);
    }
}
