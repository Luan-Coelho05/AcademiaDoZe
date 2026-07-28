
// Luan Coelho 

using System;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public record Arquivo
    {
        public string Nome { get; init; }
        public byte[] Conteudo { get; init; }
        public string ContentType { get; init; }
        public long Tamanho { get; init; }

        public Arquivo(string nome, byte[] conteudo, string contentType, long tamanho)
        {
            Nome = nome;
            Conteudo = conteudo;
            ContentType = contentType;
            Tamanho = tamanho;
        }
    }
}