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
    public TipoTransacao Tipo { get; }

    public long QuantidadeSatoshis { get; }

    public decimal QuantidadeBtc => (decimal)QuantidadeSatoshis / Carteira.SatoshisPorBitcoin;

    public decimal CotacaoBrlPorBtc { get; }

    public decimal ValorTotalBrl { get; }

    public DateTime DataHora { get; }

    public Transacao(
        TipoTransacao tipo,
        long quantidadeSatoshis,
        decimal cotacaoBrlPorBtc,
        decimal valorTotalBrl,
        DateTime dataHora)
    {
        if (!Enum.IsDefined(tipo))
        {
            throw new ArgumentOutOfRangeException(nameof(tipo), "Tipo de transação inválido.");
        }

        if (quantidadeSatoshis <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantidadeSatoshis),
                "A quantidade de satoshis deve ser maior que zero.");
        }

        if (cotacaoBrlPorBtc <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cotacaoBrlPorBtc),
                "A cotação do Bitcoin deve ser maior que zero.");
        }

        if (valorTotalBrl <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(valorTotalBrl),
                "O valor total da transação deve ser maior que zero.");
        }

        Tipo = tipo;
        QuantidadeSatoshis = quantidadeSatoshis;
        CotacaoBrlPorBtc = cotacaoBrlPorBtc;
        ValorTotalBrl = valorTotalBrl;
        DataHora = dataHora;
    }
}
