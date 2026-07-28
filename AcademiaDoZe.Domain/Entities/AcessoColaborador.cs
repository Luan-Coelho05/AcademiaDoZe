
// Luan Coelho 

using System;

namespace AcademiaDoZe.Domain.Entities
{
    public class AcessoColaborador : Entity
    {
        public Guid ColaboradorId { get; private set; }
        public DateTime DataAcesso { get; private set; }
        public string Acao { get; private set; }

        public AcessoColaborador(Guid colaboradorId, DateTime dataAcesso, string acao)
        {
            ColaboradorId = colaboradorId;
            DataAcesso = dataAcesso;
            Acao = acao;
        }
    }
}