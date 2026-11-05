using MiniBitcoin.Console.Domain;

namespace MiniBitcoin.Tests;

public class CarteiraTests
{
    [Fact]
    public void Construtor_DeveIniciarComOsSaldosInformados()
    {
        var carteira = new Carteira(10_000m, 0.5m);

        Assert.Equal(10_000m, carteira.SaldoBrl);
        Assert.Equal(0.5m, carteira.SaldoBtc);
    }

    [Fact]
    public void Construtor_DeveConverterBitcoinParaSatoshis()
    {
        var carteira = new Carteira(0m, 0.00000001m);

        Assert.Equal(1, carteira.SaldoSatoshis);
    }

    [Fact]
    public void Construtor_DeveRejeitarSaldoBrlNegativo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Carteira(-0.01m, 0m));
    }

    [Fact]
    public void Construtor_DeveRejeitarSaldoBitcoinNegativo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Carteira(0m, -0.00000001m));
    }

    [Fact]
    public void Depositar_DeveAumentarSaldoBrl()
    {
        var carteira = new Carteira(100m, 0m);

        carteira.Depositar(50.25m);

        Assert.Equal(150.25m, carteira.SaldoBrl);
    }

    [Fact]
    public void Depositar_DeveRejeitarValorZero()
    {
        var carteira = new Carteira(100m, 0m);

        Assert.Throws<ArgumentOutOfRangeException>(() => carteira.Depositar(0m));
    }

    [Fact]
    public void Depositar_DeveRejeitarValorNegativo()
    {
        var carteira = new Carteira(100m, 0m);

        Assert.Throws<ArgumentOutOfRangeException>(() => carteira.Depositar(-10m));
    }

    [Fact]
    public void Sacar_DeveReduzirSaldoBrl()
    {
        var carteira = new Carteira(100m, 0m);

        carteira.Sacar(40m);

        Assert.Equal(60m, carteira.SaldoBrl);
    }

    [Fact]
    public void Sacar_DeveRejeitarValorMaiorQueOSaldo()
    {
        var carteira = new Carteira(100m, 0m);

        Assert.Throws<InvalidOperationException>(() => carteira.Sacar(100.01m));
        Assert.Equal(100m, carteira.SaldoBrl);
    }

    [Fact]
    public void DebitarBrl_DeveRejeitarSaldoInsuficiente()
    {
        var carteira = new Carteira(100m, 0m);

        Assert.Throws<InvalidOperationException>(() => carteira.DebitarBrl(101m));
    }

    [Fact]
    public void CreditarBrl_DeveAumentarSaldo()
    {
        var carteira = new Carteira(100m, 0m);

        carteira.CreditarBrl(25.50m);

        Assert.Equal(125.50m, carteira.SaldoBrl);
    }

    [Fact]
    public void CreditarBtc_DeveAumentarSaldoEmSatoshis()
    {
        var carteira = new Carteira(0m, 0m);

        carteira.CreditarBtc(150_000_000);

        Assert.Equal(1.5m, carteira.SaldoBtc);
        Assert.Equal(150_000_000, carteira.SaldoSatoshis);
    }

    [Fact]
    public void DebitarBtc_DeveRejeitarSaldoInsuficiente()
    {
        var carteira = new Carteira(0m, 0.5m);

        Assert.Throws<InvalidOperationException>(() => carteira.DebitarBtc(50_000_001));
        Assert.Equal(0.5m, carteira.SaldoBtc);
    }

    [Fact]
    public void DebitarBtc_DeveRejeitarQuantidadeInvalida()
    {
        var carteira = new Carteira(0m, 0.5m);

        Assert.Throws<ArgumentOutOfRangeException>(() => carteira.DebitarBtc(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => carteira.DebitarBtc(-1));
    }

    [Fact]
    public void CreditarBtc_DeveRejeitarQuantidadeInvalida()
    {
        var carteira = new Carteira(0m, 0m);

        Assert.Throws<ArgumentOutOfRangeException>(() => carteira.CreditarBtc(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => carteira.CreditarBtc(-1));
    }
}
