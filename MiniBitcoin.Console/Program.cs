using System.Globalization;
using MiniBitcoin.Console.Domain;
using MiniBitcoin.Console.Services;

namespace MiniBitcoin.Console;

public static class Program
{
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");
    private const decimal SaldoInicialBrl = 10_000m;
    private const decimal SaldoInicialBtc = 0.01m;

    public static void Main()
    {
        var carteira = new Carteira(SaldoInicialBrl, SaldoInicialBtc);
        var cotacaoService = new CotacaoService();
        var corretora = new CorretoraService();
        var extrato = new Extrato();

        while (true)
        {
            ExibirMenu(cotacaoService);
            var opcao = LerTexto("Escolha uma opção: ");

            switch (opcao)
            {
                case "1":
                    ExibirCotacao(cotacaoService);
                    break;
                case "2":
                    ExibirSaldos(carteira);
                    break;
                case "3":
                    ComprarBitcoin(carteira, corretora, cotacaoService, extrato);
                    break;
                case "4":
                    VenderBitcoin(carteira, corretora, cotacaoService, extrato);
                    break;
                case "5":
                    ExibirExtrato(carteira, cotacaoService, extrato);
                    break;
                case "6":
                    AtualizarCotacao(cotacaoService);
                    break;
                case "7":
                    DepositarBrl(carteira);
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

    private static void ExibirMenu(CotacaoService cotacaoService)
    {
        LimparTela();
        System.Console.WriteLine("========================================");
        System.Console.WriteLine("        MiniBitcoin - Carteira BTC");
        System.Console.WriteLine("========================================");
        System.Console.WriteLine($"Cotação atual: {FormatarBrl(cotacaoService.CotacaoAtualBrlPorBtc)} por BTC");
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

    private static void ExibirCotacao(CotacaoService cotacaoService)
    {
        System.Console.WriteLine();
        System.Console.WriteLine($"Cotação atual do Bitcoin: {FormatarBrl(cotacaoService.CotacaoAtualBrlPorBtc)}");
    }

    private static void ExibirSaldos(Carteira carteira)
    {
        System.Console.WriteLine();
        System.Console.WriteLine("--- Saldos da carteira ---");
        System.Console.WriteLine($"BRL: {FormatarBrl(carteira.SaldoBrl)}");
        System.Console.WriteLine($"BTC: {carteira.SaldoBtc.ToString("N8", Cultura)} BTC");
        System.Console.WriteLine($"Satoshis: {carteira.SaldoSatoshis:N0}");
    }

    private static void ComprarBitcoin(
        Carteira carteira,
        CorretoraService corretora,
        CotacaoService cotacaoService,
        Extrato extrato)
    {
        System.Console.WriteLine();
        System.Console.WriteLine("--- Compra de Bitcoin ---");
        System.Console.WriteLine($"Saldo disponível: {FormatarBrl(carteira.SaldoBrl)}");
        var valorBrl = LerDecimal("Valor da compra em BRL: ");

        try
        {
            var transacao = corretora.Comprar(carteira, valorBrl, cotacaoService);
            extrato.Registrar(transacao);
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

    private static void VenderBitcoin(
        Carteira carteira,
        CorretoraService corretora,
        CotacaoService cotacaoService,
        Extrato extrato)
    {
        System.Console.WriteLine();
        System.Console.WriteLine("--- Venda de Bitcoin ---");
        System.Console.WriteLine($"Saldo disponível: {carteira.SaldoBtc.ToString("N8", Cultura)} BTC");
        var quantidadeBtc = LerDecimal("Quantidade de BTC a vender: ");

        try
        {
            var transacao = corretora.Vender(carteira, quantidadeBtc, cotacaoService);
            extrato.Registrar(transacao);
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

    private static void ExibirExtrato(Carteira carteira, CotacaoService cotacaoService, Extrato extrato)
    {
        System.Console.WriteLine();
        System.Console.WriteLine("--- Extrato de transações ---");

        if (extrato.Transacoes.Count == 0)
        {
            System.Console.WriteLine("Nenhuma operação realizada até o momento.");
        }
        else
        {
            for (var indice = 0; indice < extrato.Transacoes.Count; indice++)
            {
                var transacao = extrato.Transacoes[indice];
                var tipo = transacao.Tipo == TipoTransacao.Compra ? "Compra" : "Venda";
                System.Console.WriteLine(
                    $"{indice + 1}. {transacao.DataHora.ToString("dd/MM/yyyy HH:mm:ss", Cultura)} | " +
                    $"{tipo} | {transacao.QuantidadeBtc.ToString("N8", Cultura)} BTC | " +
                    $"Cotação: {FormatarBrl(transacao.CotacaoBrlPorBtc)} | " +
                    $"Total: {FormatarBrl(transacao.ValorTotalBrl)}");
            }
        }

        System.Console.WriteLine();
        System.Console.WriteLine($"Total comprado: {FormatarBrl(extrato.CalcularTotalComprasBrl())}");
        System.Console.WriteLine($"Total vendido: {FormatarBrl(extrato.CalcularTotalVendasBrl())}");

        var patrimonio = extrato.CalcularPatrimonioTotal(carteira, cotacaoService.CotacaoAtualBrlPorBtc);
        System.Console.WriteLine();
        System.Console.WriteLine("--- Patrimônio consolidado ---");
        System.Console.WriteLine($"BRL: {FormatarBrl(carteira.SaldoBrl)}");
        System.Console.WriteLine($"BTC: {carteira.SaldoBtc.ToString("N8", Cultura)} BTC");
        System.Console.WriteLine($"Total: {FormatarBrl(patrimonio)}");
    }

    private static void AtualizarCotacao(CotacaoService cotacaoService)
    {
        System.Console.WriteLine();
        System.Console.WriteLine("--- Atualização da cotação ---");
        System.Console.WriteLine($"Cotação atual: {FormatarBrl(cotacaoService.CotacaoAtualBrlPorBtc)}");
        var novaCotacao = LerDecimal("Nova cotação do BTC em BRL: ");

        try
        {
            cotacaoService.AtualizarCotacao(novaCotacao);
            System.Console.WriteLine();
            System.Console.WriteLine($"Cotação atualizada para {FormatarBrl(cotacaoService.CotacaoAtualBrlPorBtc)}.");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            System.Console.WriteLine();
            System.Console.WriteLine($"Não foi possível atualizar: {ex.Message}");
        }
    }

    private static void DepositarBrl(Carteira carteira)
    {
        System.Console.WriteLine();
        System.Console.WriteLine("--- Depósito em BRL ---");
        var valor = LerDecimal("Valor a depositar: ");

        try
        {
            carteira.Depositar(valor);
            System.Console.WriteLine();
            System.Console.WriteLine($"Depósito realizado. Novo saldo: {FormatarBrl(carteira.SaldoBrl)}");
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
