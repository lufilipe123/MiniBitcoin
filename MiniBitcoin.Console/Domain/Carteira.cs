namespace MiniBitcoin.Console.Domain;

/// <summary>
/// Representa a carteira do usuário, com saldo em reais (BRL) e em satoshis (BTC).
/// </summary>
public class Carteira
{
    public const int SatoshisPorBitcoin = 100_000_000;

    public decimal SaldoBrl { get; private set; }

    public long SaldoSatoshis { get; private set; }

    public decimal SaldoBtc => (decimal)SaldoSatoshis / SatoshisPorBitcoin;

    public Carteira(decimal saldoBrlInicial, decimal saldoBtcInicial)
    {
        if (saldoBrlInicial < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(saldoBrlInicial),
                "O saldo inicial em BRL não pode ser negativo.");
        }

        if (saldoBtcInicial < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(saldoBtcInicial),
                "O saldo inicial em Bitcoin não pode ser negativo.");
        }

        SaldoBrl = saldoBrlInicial;
        SaldoSatoshis = BtcParaSatoshis(saldoBtcInicial);
    }

    public void Depositar(decimal valorBrl) => CreditarBrl(valorBrl);

    public void Sacar(decimal valorBrl) => DebitarBrl(valorBrl);

    public void CreditarBrl(decimal valorBrl)
    {
        ValidarValorPositivo(valorBrl, nameof(valorBrl), "O valor em BRL deve ser maior que zero.");
        SaldoBrl += valorBrl;
    }

    public void DebitarBrl(decimal valorBrl)
    {
        ValidarValorPositivo(valorBrl, nameof(valorBrl), "O valor em BRL deve ser maior que zero.");

        if (valorBrl > SaldoBrl)
        {
            throw new InvalidOperationException("Saldo em BRL insuficiente para a operação.");
        }

        SaldoBrl -= valorBrl;
    }

    public void CreditarBtc(long satoshis)
    {
        ValidarQuantidadePositiva(satoshis);
        SaldoSatoshis = checked(SaldoSatoshis + satoshis);
    }

    public void DebitarBtc(long satoshis)
    {
        ValidarQuantidadePositiva(satoshis);

        if (satoshis > SaldoSatoshis)
        {
            throw new InvalidOperationException("Saldo em Bitcoin insuficiente para a operação.");
        }

        SaldoSatoshis -= satoshis;
    }

    private static long BtcParaSatoshis(decimal btc)
        => checked((long)decimal.Round(btc * SatoshisPorBitcoin, 0, MidpointRounding.ToZero));

    private static void ValidarValorPositivo(decimal valor, string nomeParametro, string mensagem)
    {
        if (valor <= 0)
        {
            throw new ArgumentOutOfRangeException(nomeParametro, mensagem);
        }
    }

    private static void ValidarQuantidadePositiva(long satoshis)
    {
        if (satoshis <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(satoshis), "A quantidade de satoshis deve ser maior que zero.");
        }
    }
}
