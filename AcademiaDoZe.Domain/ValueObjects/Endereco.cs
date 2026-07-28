// Luan Coelho

namespace AcademiaDoZe.Domain.ValueObjects;

public record Endereco
{
    public Entities.Logradouro Logradouro { get; init; }

    public string Numero { get; init; }

    public string Complemento { get; init; }

    public Endereco(
        Entities.Logradouro logradouro,
        string numero,
        string complemento)
    {
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
    }
}