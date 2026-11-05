using MiniBitcoin.Console.Domain;
using MiniBitcoin.Console.Services;
using MiniBitcoin.Console.UI;

namespace MiniBitcoin.Console;

public static class Program
{
    private const decimal SaldoInicialBrl = 10_000m;
    private const decimal SaldoInicialBtc = 0.01m;

    public static void Main()
    {
        var menu = new MenuConsole(
            new Carteira(SaldoInicialBrl, SaldoInicialBtc),
            new CotacaoService(),
            new CorretoraService(),
            new Extrato());

        menu.Executar();
    }
}
