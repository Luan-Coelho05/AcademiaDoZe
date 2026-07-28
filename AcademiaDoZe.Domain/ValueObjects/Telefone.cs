
// Luan Coelho 

using System;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public record Telefone
    {
        public string Numero { get; init; }
        public string DDD { get; init; }

        public Telefone(string ddd, string numero)
        {
            DDD = ddd;
            Numero = numero;
        }
    }
}