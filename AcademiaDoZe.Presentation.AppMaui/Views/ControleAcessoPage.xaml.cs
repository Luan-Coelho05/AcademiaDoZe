// Luan Coelho
using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

public partial class ControleAcessoPage : ContentPage
{
    private readonly ControleAcessoViewModel _viewModel;

    public ControleAcessoPage(ControleAcessoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Removemos o Dispatcher.Dispatch e qualquer Task.Run que force a UI.
        // O método "InitializeAsync" será chamado e "esquecido" pela UI (fire-and-forget), 
        // permitindo que a interface seja desenhada imediatamente.
        CarregarDadosSeguros();
    }

    private async void CarregarDadosSeguros()
    {
        try
        {
            // Dá um atraso propositado para o MAUI terminar de desenhar a página
            await Task.Delay(300);

            // Só então vai ao SQLite
            await _viewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            // Se houver um bloqueio mortal de ficheiro no SQLite, o erro será apanhado aqui 
            // em vez de congelar a aplicação para sempre!
            await DisplayAlert("Erro Crítico", $"O SQLite impediu a leitura: {ex.Message}", "OK");
        }
    }
}