namespace MiniBitcoin.Console.Domain;

/// <summary>
/// Representa a carteira do usuário, com saldo em reais (BRL) e em satoshis (BTC).
/// </summary>
public class Carteira
{
    public const int SatoshisPorBitcoin = 100_000_000;

    public decimal SaldoBrl => throw new NotImplementedException();

    public long SaldoSatoshis => throw new NotImplementedException();

    public decimal SaldoBtc => throw new NotImplementedException();

    public Carteira(decimal saldoBrlInicial, decimal saldoBtcInicial)
    {
        throw new NotImplementedException();
    }

    public void Depositar(decimal valorBrl) => throw new NotImplementedException();

    public void Sacar(decimal valorBrl) => throw new NotImplementedException();

    public void CreditarBrl(decimal valorBrl) => throw new NotImplementedException();

    public void DebitarBrl(decimal valorBrl) => throw new NotImplementedException();

    public void CreditarBtc(long satoshis) => throw new NotImplementedException();

    public void DebitarBtc(long satoshis) => throw new NotImplementedException();
}
