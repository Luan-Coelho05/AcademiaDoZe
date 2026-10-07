
// Luan Coelho 

using AcademiaDoZe.Presentation.AppMaui.Views;

namespace AcademiaDoZe.Presentation.AppMaui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registra a rota da tela de formulário (cadastro e edição)
        Routing.RegisterRoute("logradouro", typeof(LogradouroPage));
    }
}