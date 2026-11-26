using Boomerang.Domain.Editions.Shared;

namespace Boomerang.Application.Interfaces;

public interface IEditionResolver
{
    IGameEdition Resolve(string edition);
}
