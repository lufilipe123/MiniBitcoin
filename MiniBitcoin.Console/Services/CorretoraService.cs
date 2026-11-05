using MiniBitcoin.Console.Domain;

namespace MiniBitcoin.Console.Services;

/// <summary>
/// Executa ordens de compra e venda de Bitcoin utilizando a cotação atual.
/// </summary>
public class CorretoraService
{
    public Transacao Comprar(Carteira carteira, decimal valorBrl, CotacaoService cotacaoService)
    {
        ArgumentNullException.ThrowIfNull(carteira);
        ArgumentNullException.ThrowIfNull(cotacaoService);

        if (valorBrl <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valorBrl), "O valor da compra deve ser maior que zero.");
        }

        var cotacao = cotacaoService.CotacaoAtualBrlPorBtc;
        var quantidadeSatoshis = ConverterBrlParaSatoshis(valorBrl, cotacao);

        if (quantidadeSatoshis <= 0)
        {
            throw new InvalidOperationException("O valor informado é insuficiente para comprar 1 satoshi.");
        }

        if (valorBrl > carteira.SaldoBrl)
        {
            throw new InvalidOperationException("Saldo em BRL insuficiente para a compra.");
        }

        carteira.DebitarBrl(valorBrl);
        carteira.CreditarBtc(quantidadeSatoshis);

        return new Transacao(
            TipoTransacao.Compra,
            quantidadeSatoshis,
            cotacao,
            valorBrl,
            DateTime.Now);
    }

    public Transacao Vender(Carteira carteira, decimal quantidadeBtc, CotacaoService cotacaoService)
    {
        ArgumentNullException.ThrowIfNull(carteira);
        ArgumentNullException.ThrowIfNull(cotacaoService);

        if (quantidadeBtc <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantidadeBtc),
                "A quantidade de Bitcoin deve ser maior que zero.");
        }

        var quantidadeSatoshis = ConverterBtcParaSatoshis(quantidadeBtc);

        if (quantidadeSatoshis <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantidadeBtc),
                "A quantidade informada é menor que 1 satoshi.");
        }

        if (quantidadeSatoshis > carteira.SaldoSatoshis)
        {
            throw new InvalidOperationException("Saldo em Bitcoin insuficiente para a venda.");
        }

        var cotacao = cotacaoService.CotacaoAtualBrlPorBtc;
        var valorBrl = decimal.Round(
            (decimal)quantidadeSatoshis / Carteira.SatoshisPorBitcoin * cotacao,
            2,
            MidpointRounding.ToZero);

        if (valorBrl <= 0)
        {
            throw new InvalidOperationException("O valor da venda é insuficiente para crédito em BRL.");
        }

        carteira.DebitarBtc(quantidadeSatoshis);
        carteira.CreditarBrl(valorBrl);

        return new Transacao(
            TipoTransacao.Venda,
            quantidadeSatoshis,
            cotacao,
            valorBrl,
            DateTime.Now);
    }

    private static long ConverterBrlParaSatoshis(decimal valorBrl, decimal cotacao)
        => checked((long)decimal.Round(
            valorBrl / cotacao * Carteira.SatoshisPorBitcoin,
            0,
            MidpointRounding.ToZero));

    private static long ConverterBtcParaSatoshis(decimal quantidadeBtc)
        => checked((long)decimal.Round(
            quantidadeBtc * Carteira.SatoshisPorBitcoin,
            0,
            MidpointRounding.ToZero));
}
