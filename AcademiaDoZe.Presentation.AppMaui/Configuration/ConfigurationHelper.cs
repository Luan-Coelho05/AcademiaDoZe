// Luan Coelho 
using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration;

public static class ConfigurationHelper
{
    public static void ConfigureServices(IServiceCollection services)
    {
        var (connectionString, databaseType) = ObterConfiguracaoAtual();
        var repoConfig = new RepositoryConfig
        {
            ConnectionString = connectionString,
            DatabaseType = databaseType.ToInfrastructure()
        };

        services.AddSingleton(repoConfig);

        WeakReferenceMessenger.Default.Register<RepositoryConfig, BancoPreferencesUpdatedMessage>(repoConfig, (r, m) =>
        {
            var (novaConnStr, novoDbType) = ObterConfiguracaoAtual();
            r.ConnectionString = novaConnStr;
            r.DatabaseType = novoDbType.ToInfrastructure();
        });

        services.AddApplicationServices();
    }

    public static (string ConnectionString, AppDatabaseType DatabaseType) ObterConfiguracaoAtual()
    {
        // 1. FORÇA O USO DO SQLITE (Ignora completamente o cache do Windows)
        var databaseType = AppDatabaseType.Sqlite;

        // 2. APONTA DIRETAMENTE PARA O SEU BANCO COM OS 85 ALUNOS
        string dbPath = @"C:\DEV\AcademiaDoZe\db_academia_do_ze.db";

        // 3. O 'Mode=ReadWrite' impede que o MAUI crie um banco vazio caso se perca
        string connectionString = $"Data Source={dbPath};Mode=ReadWrite;";

        return (connectionString, databaseType);
    }
}