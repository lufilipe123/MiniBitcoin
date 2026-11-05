namespace MiniBitcoin.Console.Domain;

/// <summary>
/// Mantém o histórico de operações e calcula o patrimônio consolidado da carteira.
/// </summary>
public class Extrato
{
    public IReadOnlyList<Transacao> Transacoes => throw new NotImplementedException();

    public void Registrar(Transacao transacao) => throw new NotImplementedException();

    public decimal CalcularPatrimonioTotal(Carteira carteira, decimal cotacaoBrlPorBtc)
        => throw new NotImplementedException();

    public decimal CalcularTotalComprasBrl() => throw new NotImplementedException();

    public decimal CalcularTotalVendasBrl() => throw new NotImplementedException();
}
