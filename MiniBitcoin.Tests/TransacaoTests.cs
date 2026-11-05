using MiniBitcoin.Console.Domain;
using MiniBitcoin.Console.Services;

namespace MiniBitcoin.Tests;

public class TransacaoTests
{
    private static CotacaoService CriarCotacao(decimal valor = 500_000m) => new(valor);

    [Fact]
    public void CotacaoService_DeveIniciarComCotacaoPadrao()
    {
        var cotacao = new CotacaoService();

        Assert.Equal(CotacaoService.CotacaoInicialPadrao, cotacao.CotacaoAtualBrlPorBtc);
    }

    [Fact]
    public void CotacaoService_DeveAceitarCotacaoInicialPersonalizada()
    {
        var cotacao = new CotacaoService(350_000m);

        Assert.Equal(350_000m, cotacao.CotacaoAtualBrlPorBtc);
    }

    [Fact]
    public void CotacaoService_DeveRejeitarCotacaoZeroOuNegativa()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CotacaoService(0m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CotacaoService(-1m));
    }

    [Fact]
    public void CotacaoService_DeveAtualizarCotacao()
    {
        var cotacao = new CotacaoService(500_000m);

        cotacao.AtualizarCotacao(620_000m);

        Assert.Equal(620_000m, cotacao.CotacaoAtualBrlPorBtc);
    }

    [Fact]
    public void CotacaoService_DeveRejeitarAtualizacaoInvalida()
    {
        var cotacao = new CotacaoService(500_000m);

        Assert.Throws<ArgumentOutOfRangeException>(() => cotacao.AtualizarCotacao(0m));
        Assert.Throws<ArgumentOutOfRangeException>(() => cotacao.AtualizarCotacao(-10m));
        Assert.Equal(500_000m, cotacao.CotacaoAtualBrlPorBtc);
    }

    [Fact]
    public void Comprar_ComSaldoSuficiente_DeveAtualizarSaldos()
    {
        var carteira = new Carteira(10_000m, 0m);
        var corretora = new CorretoraService();

        var transacao = corretora.Comprar(carteira, 1_000m, CriarCotacao());

        Assert.Equal(9_000m, carteira.SaldoBrl);
        Assert.Equal(200_000, carteira.SaldoSatoshis);
        Assert.Equal(0.002m, carteira.SaldoBtc);
        Assert.Equal(TipoTransacao.Compra, transacao.Tipo);
        Assert.Equal(200_000, transacao.QuantidadeSatoshis);
        Assert.Equal(500_000m, transacao.CotacaoBrlPorBtc);
        Assert.Equal(1_000m, transacao.ValorTotalBrl);
    }

    [Fact]
    public void Comprar_FracaoDeBitcoin_DeveSerPermitida()
    {
        var carteira = new Carteira(1_000m, 0m);
        var corretora = new CorretoraService();

        var transacao = corretora.Comprar(carteira, 250m, CriarCotacao());

        Assert.Equal(50_000, transacao.QuantidadeSatoshis);
        Assert.Equal(0.0005m, carteira.SaldoBtc);
        Assert.Equal(750m, carteira.SaldoBrl);
    }

    [Fact]
    public void Comprar_DeveArredondarQuantidadeParaBaixo()
    {
        var carteira = new Carteira(1_000m, 0m);
        var corretora = new CorretoraService();

        var transacao = corretora.Comprar(carteira, 1m, CriarCotacao(300_000m));

        Assert.Equal(333, transacao.QuantidadeSatoshis);
    }

    [Fact]
    public void Comprar_SemSaldoSuficiente_DeveFalharSemAlterarSaldos()
    {
        var carteira = new Carteira(100m, 0m);
        var corretora = new CorretoraService();

        Assert.Throws<InvalidOperationException>(() => corretora.Comprar(carteira, 100.01m, CriarCotacao()));
        Assert.Equal(100m, carteira.SaldoBrl);
        Assert.Equal(0, carteira.SaldoSatoshis);
    }

    [Fact]
    public void Comprar_ValorMenorQueUmSatoshi_DeveFalhar()
    {
        var carteira = new Carteira(1_000m, 0m);
        var corretora = new CorretoraService();

        Assert.Throws<InvalidOperationException>(() => corretora.Comprar(carteira, 0.001m, CriarCotacao()));
        Assert.Equal(1_000m, carteira.SaldoBrl);
    }

    [Fact]
    public void Comprar_DeveRejeitarValoresInvalidos()
    {
        var carteira = new Carteira(1_000m, 0m);
        var corretora = new CorretoraService();

        Assert.Throws<ArgumentOutOfRangeException>(() => corretora.Comprar(carteira, 0m, CriarCotacao()));
        Assert.Throws<ArgumentOutOfRangeException>(() => corretora.Comprar(carteira, -10m, CriarCotacao()));
        Assert.Throws<ArgumentNullException>(() => corretora.Comprar(null!, 10m, CriarCotacao()));
        Assert.Throws<ArgumentNullException>(() => corretora.Comprar(carteira, 10m, null!));
    }

    [Fact]
    public void Vender_ComSaldoSuficiente_DeveAtualizarSaldos()
    {
        var carteira = new Carteira(0m, 0.002m);
        var corretora = new CorretoraService();

        var transacao = corretora.Vender(carteira, 0.001m, CriarCotacao());

        Assert.Equal(500m, carteira.SaldoBrl);
        Assert.Equal(100_000, carteira.SaldoSatoshis);
        Assert.Equal(TipoTransacao.Venda, transacao.Tipo);
        Assert.Equal(100_000, transacao.QuantidadeSatoshis);
        Assert.Equal(500_000m, transacao.CotacaoBrlPorBtc);
        Assert.Equal(500m, transacao.ValorTotalBrl);
    }

    [Fact]
    public void Vender_TodoOSaldo_DeveZerarBitcoin()
    {
        var carteira = new Carteira(0m, 0.5m);
        var corretora = new CorretoraService();

        corretora.Vender(carteira, 0.5m, CriarCotacao());

        Assert.Equal(0, carteira.SaldoSatoshis);
        Assert.Equal(250_000m, carteira.SaldoBrl);
    }

    [Fact]
    public void Vender_SemSaldoSuficiente_DeveFalharSemAlterarSaldos()
    {
        var carteira = new Carteira(0m, 0.001m);
        var corretora = new CorretoraService();

        Assert.Throws<InvalidOperationException>(() => corretora.Vender(carteira, 0.002m, CriarCotacao()));
        Assert.Equal(100_000, carteira.SaldoSatoshis);
        Assert.Equal(0m, carteira.SaldoBrl);
    }

    [Fact]
    public void Vender_DeveRejeitarValoresInvalidos()
    {
        var carteira = new Carteira(0m, 0.001m);
        var corretora = new CorretoraService();

        Assert.Throws<ArgumentOutOfRangeException>(() => corretora.Vender(carteira, 0m, CriarCotacao()));
        Assert.Throws<ArgumentOutOfRangeException>(() => corretora.Vender(carteira, -0.001m, CriarCotacao()));
        Assert.Throws<ArgumentOutOfRangeException>(() => corretora.Vender(carteira, 0.000000001m, CriarCotacao()));
        Assert.Throws<ArgumentNullException>(() => corretora.Vender(null!, 0.001m, CriarCotacao()));
        Assert.Throws<ArgumentNullException>(() => corretora.Vender(carteira, 0.001m, null!));
    }

    [Fact]
    public void CompraEVenda_DevemPreservarOPatrimonioSemSpread()
    {
        var carteira = new Carteira(10_000m, 0m);
        var corretora = new CorretoraService();
        var cotacao = CriarCotacao();

        corretora.Comprar(carteira, 1_000m, cotacao);
        corretora.Vender(carteira, 0.001m, cotacao);

        var patrimonio = carteira.SaldoBrl + carteira.SaldoBtc * cotacao.CotacaoAtualBrlPorBtc;
        Assert.Equal(10_000m, patrimonio);
    }
}
