
// Luan Coelho

using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Mappings;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration;

public static class ConfigurationHelper
{
    public static void ConfigureServices(IServiceCollection services)
    {
        // 1. Provedor de Banco padrão (SQLite para execução local segura)
        var databaseType = AppDatabaseType.Sqlite;

        // 2. Caminho do banco SQLite de acordo com a plataforma
        string connectionString;
        if (databaseType == AppDatabaseType.Sqlite)
        {
            var dbPath = DeviceInfo.Platform == DevicePlatform.WinUI
                ? @"C:\DEV\AcademiaDoZe\db_academia_do_ze.db"
                : Path.Combine(FileSystem.AppDataDirectory, "db_academia_do_ze.db");

            // Garante que o diretório C:\DEV\AcademiaDoZe exista se estiver no Windows
            var dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            connectionString = $"Data Source={dbPath};Default Timeout=5;";
        }
        else
        {
            const string dbServer = "10.30.21.16";
            const string dbDatabase = "db_academia_do_ze";
            const string dbUser = "root";
            const string dbPassword = "abcBolinhas12345";

            string dbComplemento = string.Empty;
            if (databaseType == AppDatabaseType.SqlServer)
            {
                dbComplemento = "TrustServerCertificate=True;Encrypt=True;Connect Timeout=5;Connection Timeout=5;";
            }
            else if (databaseType == AppDatabaseType.MySql)
            {
                dbComplemento = "Connection Timeout=5;Default Command Timeout=30;";
            }

            connectionString = $"Server={dbServer};Database={dbDatabase};User Id={dbUser};Password={dbPassword};{dbComplemento}";
        }

        // 3. Registra a configuração de repositórios
        services.AddSingleton(new RepositoryConfig
        {
            ConnectionString = connectionString,
            DatabaseType = databaseType.ToInfrastructure()
        });

        // 4. Ativa as injeções de serviços da camada Application
        services.AddApplicationServices();
    }
}