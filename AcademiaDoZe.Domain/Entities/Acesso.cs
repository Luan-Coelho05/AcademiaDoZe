
// Luan Coelho


using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities;

public class Acesso : Entity, IAggregateRoot
{
    public int? AlunoId { get; private set; }
    public int? ColaboradorId { get; private set; }
    public DateTime DataEntrada { get; private set; }
    public DateTime? DataSaida { get; private set; }

    // Propriedade auxiliar para facilitar a exibição na tela
    public string? PessoaNome { get; private set; }

    private Acesso(int id, int? alunoId, int? colaboradorId, DateTime dataEntrada, DateTime? dataSaida, string? pessoaNome)
    {
        Id = id;
        AlunoId = alunoId;
        ColaboradorId = colaboradorId;
        DataEntrada = dataEntrada;
        DataSaida = dataSaida;
        PessoaNome = pessoaNome;
    }

    public static Result<Acesso> Criar(int id, int? alunoId, int? colaboradorId, DateTime dataEntrada, DateTime? dataSaida = null, string? pessoaNome = null)
    {
        var notifications = new List<Notification>();

        if (alunoId == null && colaboradorId == null)
            notifications.Add(new Notification("Pessoa", "ACESSO_DEVE_TER_ALUNO_OU_COLABORADOR"));

        if (alunoId != null && colaboradorId != null)
            notifications.Add(new Notification("Pessoa", "ACESSO_NAO_PODE_TER_ALUNO_E_COLABORADOR_SIMULTANEAMENTE"));

        if (dataEntrada == default)
            notifications.Add(new Notification("DataEntrada", "DATA_ENTRADA_OBRIGATORIA"));

        if (notifications.Count != 0)
            return Result<Acesso>.Failure(notifications);

        return Result<Acesso>.Success(new Acesso(id, alunoId, colaboradorId, dataEntrada, dataSaida, pessoaNome));
    }

    public void RegistrarSaida(DateTime dataSaida)
    {
        DataSaida = dataSaida;
    }
}