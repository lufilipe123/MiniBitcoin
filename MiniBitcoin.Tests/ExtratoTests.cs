using MiniBitcoin.Console.Domain;

namespace MiniBitcoin.Tests;

public class ExtratoTests
{
    private static Transacao CriarTransacao(
        TipoTransacao tipo = TipoTransacao.Compra,
        long quantidadeSatoshis = 200_000,
        decimal cotacao = 500_000m,
        decimal valorTotal = 1_000m)
        => new(tipo, quantidadeSatoshis, cotacao, valorTotal, new DateTime(2026, 1, 1, 10, 0, 0));

    [Fact]
    public void NovoExtrato_DeveEstarVazio()
    {
        var extrato = new Extrato();

        Assert.Empty(extrato.Transacoes);
    }

    [Fact]
    public void Registrar_DeveArmazenarTransacao()
    {
        var extrato = new Extrato();
        var transacao = CriarTransacao();

        extrato.Registrar(transacao);

        Assert.Single(extrato.Transacoes);
        Assert.Same(transacao, extrato.Transacoes[0]);
    }

    [Fact]
    public void Registrar_DeveManterAOrdemDasOperacoes()
    {
        var extrato = new Extrato();
        var compra = CriarTransacao();
        var venda = CriarTransacao(TipoTransacao.Venda, 100_000, 500_000m, 500m);

        extrato.Registrar(compra);
        extrato.Registrar(venda);

        Assert.Collection(
            extrato.Transacoes,
            item => Assert.Same(compra, item),
            item => Assert.Same(venda, item));
    }

    [Fact]
    public void Registrar_DeveRejeitarTransacaoNula()
    {
        var extrato = new Extrato();

        Assert.Throws<ArgumentNullException>(() => extrato.Registrar(null!));
    }

    [Fact]
    public void CalcularPatrimonioTotal_SemBitcoin_DeveRetornarSaldoBrl()
    {
        var carteira = new Carteira(1_500m, 0m);
        var extrato = new Extrato();

        var patrimonio = extrato.CalcularPatrimonioTotal(carteira, 500_000m);

        Assert.Equal(1_500m, patrimonio);
    }

    [Fact]
    public void CalcularPatrimonioTotal_DeveSomarSaldoBrlECotacaoDoBitcoin()
    {
        var carteira = new Carteira(1_000m, 0.002m);
        var extrato = new Extrato();

        var patrimonio = extrato.CalcularPatrimonioTotal(carteira, 500_000m);

        Assert.Equal(2_000m, patrimonio);
    }

    [Fact]
    public void CalcularPatrimonioTotal_DeveRejeitarCarteiraNula()
    {
        var extrato = new Extrato();

        Assert.Throws<ArgumentNullException>(() => extrato.CalcularPatrimonioTotal(null!, 500_000m));
    }

    [Fact]
    public void CalcularPatrimonioTotal_DeveRejeitarCotacaoInvalida()
    {
        var carteira = new Carteira(1_000m, 0m);
        var extrato = new Extrato();

        Assert.Throws<ArgumentOutOfRangeException>(() => extrato.CalcularPatrimonioTotal(carteira, 0m));
        Assert.Throws<ArgumentOutOfRangeException>(() => extrato.CalcularPatrimonioTotal(carteira, -1m));
    }

    [Fact]
    public void CalcularTotalCompras_DeveSomarSomenteCompras()
    {
        var extrato = new Extrato();
        extrato.Registrar(CriarTransacao(valorTotal: 1_000m));
        extrato.Registrar(CriarTransacao(valorTotal: 500m));
        extrato.Registrar(CriarTransacao(TipoTransacao.Venda, valorTotal: 300m));

        Assert.Equal(1_500m, extrato.CalcularTotalComprasBrl());
    }

    [Fact]
    public void CalcularTotalVendas_DeveSomarSomenteVendas()
    {
        var extrato = new Extrato();
        extrato.Registrar(CriarTransacao(valorTotal: 1_000m));
        extrato.Registrar(CriarTransacao(TipoTransacao.Venda, valorTotal: 300m));
        extrato.Registrar(CriarTransacao(TipoTransacao.Venda, valorTotal: 200m));

        Assert.Equal(500m, extrato.CalcularTotalVendasBrl());
    }
}
