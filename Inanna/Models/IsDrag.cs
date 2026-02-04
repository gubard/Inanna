using Gaia.Models;

namespace Inanna.Models;

public interface IIsDrag : IId<Guid>
{
    bool IsDrag { get; set; }
}
