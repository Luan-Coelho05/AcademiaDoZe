
// Luan Coelho 

using System;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public record Cpf
    {
        public string Numero { get; init; }

        public Cpf(string numero)
        {
            Numero = numero;
        }
    }
}