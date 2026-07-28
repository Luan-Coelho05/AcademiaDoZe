
// Luan Coelho 

using System;

namespace AcademiaDoZe.Domain.Entities
{
    public class AcessoAluno : Entity
    {
        public Guid AlunoId { get; private set; }
        public DateTime DataAcesso { get; private set; }
        public string Local { get; private set; }

        public AcessoAluno(Guid alunoId, DateTime dataAcesso, string local)
        {
            AlunoId = alunoId;
            DataAcesso = dataAcesso;
            Local = local;
        }
    }
}