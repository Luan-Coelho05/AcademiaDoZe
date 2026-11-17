// Luan Coelho
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class ControleAcessoViewModel : BaseViewModel
{
    private readonly IAlunoService _alunoService;
    private readonly IColaboradorService _colaboradorService;
    private readonly IMatriculaService _matriculaService;
    private readonly IAcessoService _acessoService;

    public ObservableCollection<string> TiposPessoa { get; } = ["Aluno", "Colaborador"];

    [ObservableProperty]
    private string _selectedTipoPessoa = "Aluno";

    [ObservableProperty]
    private string _cpfBusca = string.Empty;

    [ObservableProperty]
    private bool _hasPessoa;

    [ObservableProperty]
    private bool _isDentroDaAcademia;

    [ObservableProperty]
    private bool _isForaDaAcademia = true;

    [ObservableProperty]
    private string _nomePessoa = string.Empty;

    [ObservableProperty]
    private byte[]? _fotoPessoa;

    [ObservableProperty]
    private string _infoExtra = string.Empty;

    public ObservableCollection<AcessoDto> AcessosHoje { get; } = [];

    private AlunoDto? _alunoSelecionado;
    private ColaboradorDto? _colaboradorSelecionado;
    private AcessoDto? _acessoAberto;

    public ControleAcessoViewModel(
        IAlunoService alunoService,
        IColaboradorService colaboradorService,
        IMatriculaService matriculaService,
        IAcessoService acessoService)
    {
        _alunoService = alunoService;
        _colaboradorService = colaboradorService;
        _matriculaService = matriculaService;
        _acessoService = acessoService;
        Title = "Controle de Acesso / Catraca";
    }

    public async Task InitializeAsync() => await LoadAcessosHojeAsync();

    [RelayCommand]
    private async Task LoadAcessosHojeAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var todosAcessos = await _acessoService.ObterTodosAsync(cts.Token);

            var acessosHoje = todosAcessos?.Where(a => a.DataEntrada.Date == DateTime.Today)
                                           .OrderByDescending(a => a.DataEntrada).ToList() ?? [];

            AcessosHoje.Clear();
            foreach (var acesso in acessosHoje) AcessosHoje.Add(acesso);
        }
        catch { /* Ignorar falhas na listagem inicial */ }
    }

    [RelayCommand]
    private async Task BuscarPessoaAsync()
    {
        if (string.IsNullOrWhiteSpace(CpfBusca))
        {
            await Shell.Current.DisplayAlert("Aviso", "Informe o CPF para buscar.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var cpfLimpo = new string(CpfBusca.Where(char.IsDigit).ToArray());

            _alunoSelecionado = null;
            _colaboradorSelecionado = null;
            _acessoAberto = null;
            HasPessoa = false;

            if (SelectedTipoPessoa == "Aluno")
            {
                var aluno = await _alunoService.ObterPorCpfAsync(cpfLimpo, cts.Token);
                if (aluno != null)
                {
                    _alunoSelecionado = aluno;
                    NomePessoa = aluno.Nome;
                    FotoPessoa = aluno.Foto?.Conteudo;
                    InfoExtra = $"CPF: {aluno.Cpf}";
                    HasPessoa = true;
                }
            }
            else
            {
                var colab = await _colaboradorService.ObterPorCpfAsync(cpfLimpo, cts.Token);
                if (colab != null)
                {
                    _colaboradorSelecionado = colab;
                    NomePessoa = colab.Nome;
                    FotoPessoa = colab.Foto?.Conteudo;
                    InfoExtra = $"Cargo: {colab.Tipo}";
                    HasPessoa = true;
                }
            }

            if (!HasPessoa)
            {
                await Shell.Current.DisplayAlert("Aviso", "Pessoa não encontrada.", "OK");
                return;
            }

            var todosAcessos = await _acessoService.ObterTodosAsync(cts.Token) ?? [];
            if (SelectedTipoPessoa == "Aluno")
                _acessoAberto = todosAcessos.FirstOrDefault(a => a.AlunoId == _alunoSelecionado!.Id && a.DataSaida == null);
            else
                _acessoAberto = todosAcessos.FirstOrDefault(a => a.ColaboradorId == _colaboradorSelecionado!.Id && a.DataSaida == null);

            IsDentroDaAcademia = _acessoAberto != null;
            IsForaDaAcademia = !IsDentroDaAcademia;
        }
        catch (Exception ex) { await Shell.Current.DisplayAlert("Erro", $"Erro na busca: {ex.Message}", "OK"); }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task RegistrarAcessoAsync()
    {
        if (!HasPessoa) return;

        try
        {
            IsBusy = true;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

            if (IsForaDaAcademia)
            {
                // REGRA DE ENTRADA
                if (SelectedTipoPessoa == "Aluno")
                {
                    var matriculasAtivas = await _matriculaService.ObterAtivasAsync(_alunoSelecionado!.Id, cts.Token);
                    if (matriculasAtivas == null || !matriculasAtivas.Any())
                    {
                        await Shell.Current.DisplayAlert("Acesso Negado", "Acesso negado. Aluno não possui matrícula ativa.", "OK");
                        return;
                    }

                    var matricula = matriculasAtivas.First();
                    var diasRestantes = (matricula.DataFim.ToDateTime(new TimeOnly(0, 0)) - DateTime.Today).Days;

                    var novoAcesso = new AcessoDto { AlunoId = _alunoSelecionado.Id, PessoaNome = _alunoSelecionado.Nome, DataEntrada = DateTime.Now };
                    await _acessoService.AdicionarAsync(novoAcesso, cts.Token);

                    await Shell.Current.DisplayAlert("Entrada Autorizada", $"Entrada autorizada. Dias restantes de plano: {diasRestantes} dia(s).", "OK");
                }
                else
                {
                    var todosAcessos = await _acessoService.ObterTodosAsync(cts.Token) ?? [];
                    var acessosHojeColab = todosAcessos.Where(a => a.ColaboradorId == _colaboradorSelecionado!.Id && a.DataEntrada.Date == DateTime.Today).ToList();

                    double horasTrabalhadasHoje = acessosHojeColab.Where(a => a.DataSaida.HasValue).Sum(a => (a.DataSaida!.Value - a.DataEntrada).TotalHours);
                    double limiteHoras = _colaboradorSelecionado!.Vinculo == AppColaboradorVinculo.CLT ? 8.0 : 6.0;

                    if (horasTrabalhadasHoje >= limiteHoras)
                    {
                        await Shell.Current.DisplayAlert("Acesso Negado", $"Limite atingido! Você já possui {horasTrabalhadasHoje:F1}h trabalhadas hoje (Limite: {limiteHoras}h).", "OK");
                        return;
                    }

                    var novoAcesso = new AcessoDto { ColaboradorId = _colaboradorSelecionado.Id, PessoaNome = _colaboradorSelecionado.Nome, DataEntrada = DateTime.Now };
                    await _acessoService.AdicionarAsync(novoAcesso, cts.Token);

                    await Shell.Current.DisplayAlert("Entrada Autorizada", "Entrada registrada com sucesso.", "OK");
                }
            }
            else
            {
                // REGRA DE SAÍDA
                if (_acessoAberto != null)
                {
                    _acessoAberto.DataSaida = DateTime.Now;
                    await _acessoService.AtualizarAsync(_acessoAberto, cts.Token);

                    var tempoPerm = _acessoAberto.DataSaida.Value - _acessoAberto.DataEntrada;

                    if (SelectedTipoPessoa == "Aluno")
                    {
                        await Shell.Current.DisplayAlert("Saída Registrada", $"Saída registrada. Tempo de permanência: {tempoPerm.Hours:D2}:{tempoPerm.Minutes:D2}:{tempoPerm.Seconds:D2}", "OK");
                    }
                    else
                    {
                        var todosAcessos = await _acessoService.ObterTodosAsync(cts.Token) ?? [];
                        var acessosHojeColab = todosAcessos.Where(a => a.ColaboradorId == _colaboradorSelecionado!.Id && a.DataEntrada.Date == DateTime.Today && a.DataSaida.HasValue).ToList();
                        var tempoTotalHoje = TimeSpan.FromHours(acessosHojeColab.Sum(a => (a.DataSaida!.Value - a.DataEntrada).TotalHours));

                        await Shell.Current.DisplayAlert("Saída Registrada", $"Saída registrada.\nPermanência neste acesso: {tempoPerm.Hours:D2}h {tempoPerm.Minutes:D2}m.\nTempo total trabalhado hoje: {tempoTotalHoje.Hours:D2}h {tempoTotalHoje.Minutes:D2}m.", "OK");
                    }
                }
            }

            CpfBusca = string.Empty;
            HasPessoa = false;
            await LoadAcessosHojeAsync();
        }
        catch (Exception ex) { await Shell.Current.DisplayAlert("Erro", $"Erro ao registrar acesso: {ex.Message}", "OK"); }
        finally { IsBusy = false; }
    }
}