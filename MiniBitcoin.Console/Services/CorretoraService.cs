using MiniBitcoin.Console.Domain;

namespace MiniBitcoin.Console.Services;

/// <summary>
/// Executa ordens de compra e venda de Bitcoin utilizando a cotação atual.
/// </summary>
public class CorretoraService
{
    public Transacao Comprar(Carteira carteira, decimal valorBrl, CotacaoService cotacaoService)
        => throw new NotImplementedException();

    public Transacao Vender(Carteira carteira, decimal quantidadeBtc, CotacaoService cotacaoService)
        => throw new NotImplementedException();
}
