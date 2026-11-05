namespace MiniBitcoin.Console.Services;

/// <summary>
/// Mantém e consulta a cotação simulada do Bitcoin em BRL.
/// </summary>
public class CotacaoService
{
    public const decimal CotacaoInicialPadrao = 500_000m;

    public decimal CotacaoAtualBrlPorBtc => throw new NotImplementedException();

    public CotacaoService(decimal cotacaoInicial = CotacaoInicialPadrao)
    {
        throw new NotImplementedException();
    }

    public void AtualizarCotacao(decimal novaCotacao) => throw new NotImplementedException();
}
