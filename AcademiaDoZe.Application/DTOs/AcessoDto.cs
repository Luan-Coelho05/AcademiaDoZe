
// Luan Coelho

namespace AcademiaDoZe.Application.DTOs;

public class AcessoDto
{
    public int Id { get; set; }
    public int? AlunoId { get; set; }
    public int? ColaboradorId { get; set; }
    public string? PessoaNome { get; set; }
    public required DateTime DataEntrada { get; set; }
    public DateTime? DataSaida { get; set; }
}