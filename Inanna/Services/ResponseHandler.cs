using System.Runtime.CompilerServices;
using Nestor.Db.Models;

namespace Inanna.Services;

public interface IResponseHandler
{
    ConfiguredValueTaskAwaitable HandleResponseAsync<TResponse>(
        TResponse response,
        CancellationToken ct
    )
        where TResponse : IResponse;
}
