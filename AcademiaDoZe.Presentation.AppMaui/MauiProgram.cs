// Luan Coelho
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Repositories;
using AcademiaDoZe.Presentation.AppMaui.Configuration;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Views;
using Microsoft.Extensions.Logging;

namespace AcademiaDoZe.Presentation.AppMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            });

        // Configurar serviços da aplicação e repositórios
        ConfigurationHelper.ConfigureServices(builder.Services);

        // Registrar ViewModels
        builder.Services.AddTransient<DashboardListViewModel>();
        builder.Services.AddTransient<LogradouroListViewModel>();
        builder.Services.AddTransient<LogradouroViewModel>();

        // Registrar Views
        builder.Services.AddTransient<DashboardListPage>();
        builder.Services.AddTransient<LogradouroListPage>();
        builder.Services.AddTransient<LogradouroPage>();
        builder.Services.AddTransient<ConfigPage>();

        // Registrar ViewModels
        builder.Services.AddTransient<ColaboradorListViewModel>();
        builder.Services.AddTransient<ColaboradorViewModel>();
        builder.Services.AddTransient<AlunoListViewModel>();
        builder.Services.AddTransient<AlunoViewModel>();

        // Registrar Views
        builder.Services.AddTransient<ColaboradorListPage>();
        builder.Services.AddTransient<ColaboradorPage>();
        builder.Services.AddTransient<AlunoListPage>();
        builder.Services.AddTransient<AlunoPage>();

        builder.Services.AddTransient<MatriculaListViewModel>();
        builder.Services.AddTransient<MatriculaViewModel>();

        builder.Services.AddTransient<MatriculaListPage>();
        builder.Services.AddTransient<MatriculaPage>();
        builder.Services.AddScoped<IAcessoRepository, AcessoRepository>();
        builder.Services.AddScoped<IAcessoService, AcessoService>();

        builder.Services.AddTransient<ControleAcessoViewModel>();
        builder.Services.AddTransient<ControleAcessoPage>();

        //

        // Registos dos Serviços (verifique se não falta o IAcessoService)
        builder.Services.AddScoped<IAlunoService, AlunoService>();
        builder.Services.AddScoped<IColaboradorService, ColaboradorService>();
        builder.Services.AddScoped<IMatriculaService, MatriculaService>();
        builder.Services.AddScoped<IAcessoService, AcessoService>(); // <- TEM DE ESTAR AQUI!

        // Registos da Página e ViewModel
        builder.Services.AddTransient<ControleAcessoViewModel>();
        builder.Services.AddTransient<ControleAcessoPage>();

#if DEBUG



        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}