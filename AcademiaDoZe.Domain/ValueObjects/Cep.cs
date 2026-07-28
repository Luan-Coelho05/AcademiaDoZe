
// Luan Coelho 

using System;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public record Cep
    {
        public string Codigo { get; init; }

        public Cep(string codigo)
        {
            Codigo = codigo;
        }
    }
}