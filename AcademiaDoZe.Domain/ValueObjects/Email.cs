
// Luan Coelho 

using System;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public record Email
    {
        public string Endereco { get; init; }

        public Email(string endereco)
        {
            Endereco = endereco;
        }
    }
}