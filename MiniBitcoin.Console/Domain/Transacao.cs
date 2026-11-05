namespace MiniBitcoin.Console.Domain;

public enum TipoTransacao
{
    Compra,
    Venda
}

/// <summary>
/// Registra uma operação de compra ou venda de Bitcoin.
/// </summary>
public sealed class Transacao
{
    public TipoTransacao Tipo => throw new NotImplementedException();

    public long QuantidadeSatoshis => throw new NotImplementedException();

    public decimal QuantidadeBtc => throw new NotImplementedException();

    public decimal CotacaoBrlPorBtc => throw new NotImplementedException();

    public decimal ValorTotalBrl => throw new NotImplementedException();

    public DateTime DataHora => throw new NotImplementedException();

    public Transacao(
        TipoTransacao tipo,
        long quantidadeSatoshis,
        decimal cotacaoBrlPorBtc,
        decimal valorTotalBrl,
        DateTime dataHora)
    {
        throw new NotImplementedException();
    }
}
