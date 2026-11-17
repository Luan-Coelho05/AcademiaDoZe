
// Luan Coelho

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public class AcessoService : IAcessoService
{
    private readonly Func<IAcessoRepository> _repoFactory;

    public AcessoService(Func<IAcessoRepository> repoFactory)
    {
        _repoFactory = repoFactory ?? throw new ArgumentNullException(nameof(repoFactory));
    }

    public async Task<AcessoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var acesso = await _repoFactory().ObterPorId(id, cancellationToken);
        return acesso != null ? MapearParaDto(acesso) : null;
    }

    public async Task<IEnumerable<AcessoDto>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        var acessos = await _repoFactory().ObterTodos(cancellationToken);
        return acessos.Select(MapearParaDto).ToList();
    }

    public async Task<AcessoDto> AdicionarAsync(AcessoDto acessoDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(acessoDto);

        var result = Acesso.Criar(acessoDto.Id, acessoDto.AlunoId, acessoDto.ColaboradorId, acessoDto.DataEntrada, acessoDto.DataSaida, acessoDto.PessoaNome);
        if (result.IsFailure)
        {
            throw new ArgumentException($"Erro ao criar Acesso: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");
        }

        var adicionado = await _repoFactory().Adicionar(result.Value!, cancellationToken);
        return MapearParaDto(adicionado);
    }

    public async Task<AcessoDto> AtualizarAsync(AcessoDto acessoDto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(acessoDto);

        var acessoExistente = await _repoFactory().ObterPorId(acessoDto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Acesso com ID {acessoDto.Id} não encontrado.");

        acessoExistente.RegistrarSaida(acessoDto.DataSaida ?? DateTime.Now);

        var atualizado = await _repoFactory().Atualizar(acessoExistente, cancellationToken);
        return MapearParaDto(atualizado);
    }

    private static AcessoDto MapearParaDto(Acesso acesso)
    {
        return new AcessoDto
        {
            Id = acesso.Id,
            AlunoId = acesso.AlunoId,
            ColaboradorId = acesso.ColaboradorId,
            PessoaNome = acesso.PessoaNome,
            DataEntrada = acesso.DataEntrada,
            DataSaida = acesso.DataSaida
        };
    }
}