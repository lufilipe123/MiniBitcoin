namespace MiniBitcoin.Console.Services;

/// <summary>
/// Mantém e consulta a cotação simulada do Bitcoin em BRL.
/// </summary>
public class CotacaoService
{
    public const decimal CotacaoInicialPadrao = 500_000m;

    public decimal CotacaoAtualBrlPorBtc { get; private set; }

    public CotacaoService(decimal cotacaoInicial = CotacaoInicialPadrao)
    {
        ValidarCotacao(cotacaoInicial);
        CotacaoAtualBrlPorBtc = cotacaoInicial;
    }

    public void AtualizarCotacao(decimal novaCotacao)
    {
        ValidarCotacao(novaCotacao);
        CotacaoAtualBrlPorBtc = novaCotacao;
    }

    private static void ValidarCotacao(decimal cotacao)
    {
        if (cotacao <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cotacao),
                "A cotação do Bitcoin deve ser maior que zero.");
        }
    }
}
