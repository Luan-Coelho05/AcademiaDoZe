
// Luan Coelho

using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface IAcessoRepository
{
    Task<Acesso?> ObterPorId(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Acesso>> ObterTodos(CancellationToken cancellationToken = default);
    Task<Acesso> Adicionar(Acesso entity, CancellationToken cancellationToken = default);
    Task<Acesso> Atualizar(Acesso entity, CancellationToken cancellationToken = default);
}