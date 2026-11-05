namespace MiniBitcoin.Console.Domain;

/// <summary>
/// Mantém o histórico de operações e calcula o patrimônio consolidado da carteira.
/// </summary>
public class Extrato
{
    private readonly List<Transacao> _transacoes = new();

    public IReadOnlyList<Transacao> Transacoes => _transacoes;

    public void Registrar(Transacao transacao)
    {
        ArgumentNullException.ThrowIfNull(transacao);
        _transacoes.Add(transacao);
    }

    public decimal CalcularPatrimonioTotal(Carteira carteira, decimal cotacaoBrlPorBtc)
    {
        ArgumentNullException.ThrowIfNull(carteira);

        if (cotacaoBrlPorBtc <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cotacaoBrlPorBtc),
                "A cotação do Bitcoin deve ser maior que zero.");
        }

        return carteira.SaldoBrl + carteira.SaldoBtc * cotacaoBrlPorBtc;
    }

    public decimal CalcularTotalComprasBrl()
        => _transacoes
            .Where(transacao => transacao.Tipo == TipoTransacao.Compra)
            .Sum(transacao => transacao.ValorTotalBrl);

    public decimal CalcularTotalVendasBrl()
        => _transacoes
            .Where(transacao => transacao.Tipo == TipoTransacao.Venda)
            .Sum(transacao => transacao.ValorTotalBrl);
}
