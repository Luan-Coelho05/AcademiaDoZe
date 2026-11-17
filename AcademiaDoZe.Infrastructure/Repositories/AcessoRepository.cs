
// Luan Coelho

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class AcessoRepository : BaseRepository, IAcessoRepository
{
    public AcessoRepository(string connectionString, DatabaseType databaseType)
        : base(connectionString, databaseType)
    {
    }

    private static string BaseSelectQuery =>
        @"SELECT 
            ac.id_acesso, 
            ac.aluno_id, 
            ac.colaborador_id, 
            ac.data_entrada, 
            ac.data_saida,
            COALESCE(al.nome, c.nome) AS pessoa_nome
        FROM tb_acesso ac
        LEFT JOIN tb_aluno al ON ac.aluno_id = al.id_aluno
        LEFT JOIN tb_colaborador c ON ac.colaborador_id = c.id_colaborador";

    public async Task<Acesso?> ObterPorId(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = $"{BaseSelectQuery} WHERE ac.id_acesso = @Id";
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@Id", id, DbType.Int32);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_POR_ID", $"Erro ao obter acesso: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<Acesso>> ObterTodos(CancellationToken cancellationToken = default)
    {
        try
        {
            string query = $"{BaseSelectQuery} ORDER BY ac.data_entrada DESC";
            await using var command = await CreateCommandAsync(query, cancellationToken);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            var acessos = new List<Acesso>();
            while (await reader.ReadAsync(cancellationToken))
            {
                acessos.Add(Map(reader));
            }
            return acessos;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_OBTER_TODOS", $"Erro ao obter acessos: {ex.Message}", ex);
        }
    }

    public static Acesso Map(DbDataReader reader)
    {
        try
        {
            int id = reader.GetInt32Value("id_acesso");

            int? alunoId = reader.IsDBNull(reader.GetOrdinal("aluno_id")) ? null : reader.GetInt32(reader.GetOrdinal("aluno_id"));
            int? colaboradorId = reader.IsDBNull(reader.GetOrdinal("colaborador_id")) ? null : reader.GetInt32(reader.GetOrdinal("colaborador_id"));

            DateTime dataEntrada = reader.GetDateTime(reader.GetOrdinal("data_entrada"));
            DateTime? dataSaida = reader.IsDBNull(reader.GetOrdinal("data_saida")) ? null : reader.GetDateTime(reader.GetOrdinal("data_saida"));

            string pessoaNome = reader.GetStringValue("pessoa_nome");

            var result = Acesso.Criar(id, alunoId, colaboradorId, dataEntrada, dataSaida, pessoaNome);

            if (result.IsFailure)
            {
                throw new InfrastructureException("ERRO_DOMINIO_MAPEAMENTO",
                    $"Erro de domínio ao mapear acesso ID {id}: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");
            }

            return result.Value!;
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException("ERRO_MAPEAMENTO_ACESSO", $"Erro ao mapear acesso: {ex.Message}", ex);
        }
    }

    public async Task<Acesso> Adicionar(Acesso entity, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = FormatInsertQuery(
                "INSERT INTO tb_acesso (aluno_id, colaborador_id, data_entrada, data_saida) VALUES (@AlunoId, @ColaboradorId, @DataEntrada, @DataSaida)");

            await using var command = await CreateCommandAsync(query, cancellationToken);

            command.AddParameter("@AlunoId", (object?)entity.AlunoId ?? DBNull.Value, DbType.Int32);
            command.AddParameter("@ColaboradorId", (object?)entity.ColaboradorId ?? DBNull.Value, DbType.Int32);
            command.AddParameter("@DataEntrada", entity.DataEntrada, DbType.DateTime);
            command.AddParameter("@DataSaida", (object?)entity.DataSaida ?? DBNull.Value, DbType.DateTime);

            int id = await command.ExecuteScalarIdAsync("ERRO_ADICIONAR_ACESSO", "Falha ao obter ID inserido.", cancellationToken);

            var idProperty = typeof(Entity).GetProperty("Id");
            idProperty?.SetValue(entity, id);

            return entity;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ADICIONAR_ACESSO", $"Erro ao adicionar acesso: {ex.Message}", ex);
        }
    }

    public async Task<Acesso> Atualizar(Acesso entity, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = "UPDATE tb_acesso SET data_saida = @DataSaida WHERE id_acesso = @Id";
            await using var command = await CreateCommandAsync(query, cancellationToken);

            command.AddParameter("@Id", entity.Id, DbType.Int32);
            command.AddParameter("@DataSaida", (object?)entity.DataSaida ?? DBNull.Value, DbType.DateTime);

            int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);

            if (rowsAffected == 0)
                throw new InfrastructureException("REGISTRO_NAO_ENCONTRADO", $"Nenhum acesso encontrado com ID {entity.Id}.");

            return entity;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ATUALIZAR_ACESSO", $"Erro ao atualizar acesso: {ex.Message}", ex);
        }
    }
}