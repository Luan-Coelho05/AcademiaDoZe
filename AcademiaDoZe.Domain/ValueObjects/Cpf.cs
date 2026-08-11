// Luan Coelho

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cpf
{
    public string Valor { get; }

    private Cpf(string valor)
    {
        Valor = valor;
    }

    public static Result<Cpf> Criar(string valor)
    {
        if (NormalizadoService.TextoVazioOuNulo(valor))
            return Result<Cpf>.Failure("Cpf", "CPF_OBRIGATORIO");

        var textoLimpo = NormalizadoService.LimparEDigitos(valor);

        return Result<Cpf>.Success(new Cpf(textoLimpo));
    }

    public override string ToString() => Valor;
}