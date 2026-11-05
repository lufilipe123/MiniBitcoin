using System.Globalization;
using MiniBitcoin.Console.Domain;
using MiniBitcoin.Console.Services;

namespace MiniBitcoin.Console.UI;

/// <summary>
/// Interface interativa de linha de comando da carteira MiniBitcoin.
/// </summary>
public class MenuConsole
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");

    private readonly Carteira _carteira;
    private readonly CotacaoService _cotacaoService;
    private readonly CorretoraService _corretora;
    private readonly Extrato _extrato;

    public MenuConsole(
        Carteira carteira,
        CotacaoService cotacaoService,
        CorretoraService corretora,
        Extrato extrato)
    {
        ArgumentNullException.ThrowIfNull(carteira);
        ArgumentNullException.ThrowIfNull(cotacaoService);
        ArgumentNullException.ThrowIfNull(corretora);
        ArgumentNullException.ThrowIfNull(extrato);

        _carteira = carteira;
        _cotacaoService = cotacaoService;
        _corretora = corretora;
        _extrato = extrato;
    }

    public void Executar()
    {
        while (true)
        {
            ExibirMenu();
            var opcao = LerTexto("Escolha uma opção: ");

            switch (opcao)
            {
                case "1":
                    ExibirCotacao();
                    break;
                case "2":
                    ExibirSaldos();
                    break;
                case "3":
                    ComprarBitcoin();
                    break;
                case "4":
                    VenderBitcoin();
                    break;
                case "5":
                    ExibirExtrato();
                    break;
                case "6":
                    AtualizarCotacao();
                    break;
                case "7":
                    DepositarBrl();
                    break;
                case "0":
                    System.Console.WriteLine();
                    System.Console.WriteLine("Obrigado por usar o MiniBitcoin. Até logo!");
                    return;
                default:
                    System.Console.WriteLine();
                    System.Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

            Pausar();
        }
    }

    private void ExibirMenu()
    {
        LimparTela();
        System.Console.WriteLine("========================================");
        System.Console.WriteLine("        MiniBitcoin - Carteira BTC");
        System.Console.WriteLine("========================================");
        System.Console.WriteLine($"Cotação atual: {FormatarBrl(_cotacaoService.CotacaoAtualBrlPorBtc)} por BTC");
        System.Console.WriteLine();
        System.Console.WriteLine("1 - Consultar cotação");
        System.Console.WriteLine("2 - Ver saldos");
        System.Console.WriteLine("3 - Comprar Bitcoin");
        System.Console.WriteLine("4 - Vender Bitcoin");
        System.Console.WriteLine("5 - Exibir extrato e patrimônio");
        System.Console.WriteLine("6 - Atualizar cotação");
        System.Console.WriteLine("7 - Depositar BRL");
        System.Console.WriteLine("0 - Sair");
        System.Console.WriteLine();
    }

    private void ExibirCotacao()
    {
        System.Console.WriteLine();
        System.Console.WriteLine($"Cotação atual do Bitcoin: {FormatarBrl(_cotacaoService.CotacaoAtualBrlPorBtc)}");
    }

    private void ExibirSaldos()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("--- Saldos da carteira ---");
        System.Console.WriteLine($"BRL: {FormatarBrl(_carteira.SaldoBrl)}");
        System.Console.WriteLine($"BTC: {_carteira.SaldoBtc.ToString("N8", Cultura)} BTC");
        System.Console.WriteLine($"Satoshis: {_carteira.SaldoSatoshis:N0}");
    }

    private void ComprarBitcoin()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("--- Compra de Bitcoin ---");
        System.Console.WriteLine($"Saldo disponível: {FormatarBrl(_carteira.SaldoBrl)}");
        var valorBrl = LerDecimal("Valor da compra em BRL: ");

        try
        {
            var transacao = _corretora.Comprar(_carteira, valorBrl, _cotacaoService);
            _extrato.Registrar(transacao);
            System.Console.WriteLine();
            System.Console.WriteLine("Compra realizada com sucesso!");
            ExibirResumoTransacao(transacao);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            System.Console.WriteLine();
            System.Console.WriteLine($"Não foi possível comprar: {ex.Message}");
        }
    }

    private void VenderBitcoin()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("--- Venda de Bitcoin ---");
        System.Console.WriteLine($"Saldo disponível: {_carteira.SaldoBtc.ToString("N8", Cultura)} BTC");
        var quantidadeBtc = LerDecimal("Quantidade de BTC a vender: ");

        try
        {
            var transacao = _corretora.Vender(_carteira, quantidadeBtc, _cotacaoService);
            _extrato.Registrar(transacao);
            System.Console.WriteLine();
            System.Console.WriteLine("Venda realizada com sucesso!");
            ExibirResumoTransacao(transacao);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            System.Console.WriteLine();
            System.Console.WriteLine($"Não foi possível vender: {ex.Message}");
        }
    }

    private void ExibirExtrato()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("--- Extrato de transações ---");

        if (_extrato.Transacoes.Count == 0)
        {
            System.Console.WriteLine("Nenhuma operação realizada até o momento.");
        }
        else
        {
            for (var indice = 0; indice < _extrato.Transacoes.Count; indice++)
            {
                var transacao = _extrato.Transacoes[indice];
                var tipo = transacao.Tipo == TipoTransacao.Compra ? "Compra" : "Venda";
                System.Console.WriteLine(
                    $"{indice + 1}. {transacao.DataHora.ToString("dd/MM/yyyy HH:mm:ss", Cultura)} | " +
                    $"{tipo} | {transacao.QuantidadeBtc.ToString("N8", Cultura)} BTC | " +
                    $"Cotação: {FormatarBrl(transacao.CotacaoBrlPorBtc)} | " +
                    $"Total: {FormatarBrl(transacao.ValorTotalBrl)}");
            }
        }

        System.Console.WriteLine();
        System.Console.WriteLine($"Total comprado: {FormatarBrl(_extrato.CalcularTotalComprasBrl())}");
        System.Console.WriteLine($"Total vendido: {FormatarBrl(_extrato.CalcularTotalVendasBrl())}");

        var patrimonio = _extrato.CalcularPatrimonioTotal(_carteira, _cotacaoService.CotacaoAtualBrlPorBtc);
        System.Console.WriteLine();
        System.Console.WriteLine("--- Patrimônio consolidado ---");
        System.Console.WriteLine($"BRL: {FormatarBrl(_carteira.SaldoBrl)}");
        System.Console.WriteLine($"BTC: {_carteira.SaldoBtc.ToString("N8", Cultura)} BTC");
        System.Console.WriteLine($"Total: {FormatarBrl(patrimonio)}");
    }

    private void AtualizarCotacao()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("--- Atualização da cotação ---");
        System.Console.WriteLine($"Cotação atual: {FormatarBrl(_cotacaoService.CotacaoAtualBrlPorBtc)}");
        var novaCotacao = LerDecimal("Nova cotação do BTC em BRL: ");

        try
        {
            _cotacaoService.AtualizarCotacao(novaCotacao);
            System.Console.WriteLine();
            System.Console.WriteLine($"Cotação atualizada para {FormatarBrl(_cotacaoService.CotacaoAtualBrlPorBtc)}.");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            System.Console.WriteLine();
            System.Console.WriteLine($"Não foi possível atualizar: {ex.Message}");
        }
    }

    private void DepositarBrl()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("--- Depósito em BRL ---");
        var valor = LerDecimal("Valor a depositar: ");

        try
        {
            _carteira.Depositar(valor);
            System.Console.WriteLine();
            System.Console.WriteLine($"Depósito realizado. Novo saldo: {FormatarBrl(_carteira.SaldoBrl)}");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            System.Console.WriteLine();
            System.Console.WriteLine($"Não foi possível depositar: {ex.Message}");
        }
    }

    private static void ExibirResumoTransacao(Transacao transacao)
    {
        System.Console.WriteLine($"Quantidade: {transacao.QuantidadeBtc.ToString("N8", Cultura)} BTC");
        System.Console.WriteLine($"Cotação: {FormatarBrl(transacao.CotacaoBrlPorBtc)}");
        System.Console.WriteLine($"Total: {FormatarBrl(transacao.ValorTotalBrl)}");
    }

    private static void LimparTela()
    {
        if (System.Console.IsOutputRedirected)
        {
            return;
        }

        try
        {
            System.Console.Clear();
        }
        catch (IOException)
        {
            // Alguns terminais não suportam limpeza de tela; o menu continua funcional.
        }
    }

    private static void Pausar()
    {
        System.Console.WriteLine();
        System.Console.WriteLine("Pressione Enter para continuar...");
        System.Console.ReadLine();
    }

    private static string LerTexto(string mensagem)
    {
        System.Console.Write(mensagem);
        return System.Console.ReadLine()?.Trim() ?? string.Empty;
    }

    private static decimal LerDecimal(string mensagem)
    {
        while (true)
        {
            var texto = LerTexto(mensagem);
            if (decimal.TryParse(texto, NumberStyles.Number, Cultura, out var valor))
            {
                return valor;
            }

            System.Console.WriteLine("Valor inválido. Use o formato brasileiro, por exemplo: 1000,50");
        }
    }

    private static string FormatarBrl(decimal valor) => valor.ToString("C", Cultura);
}
