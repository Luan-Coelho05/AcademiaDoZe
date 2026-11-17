
// Luan Coelho

using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces;

public interface IAcessoService
{
    Task<AcessoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AcessoDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<AcessoDto> AdicionarAsync(AcessoDto acessoDto, CancellationToken cancellationToken = default);
    Task<AcessoDto> AtualizarAsync(AcessoDto acessoDto, CancellationToken cancellationToken = default);
}