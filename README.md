# MiniBitcoin

Aplicação de console em .NET para simular uma carteira de Bitcoin, permitindo consultar a cotação, acompanhar saldos em BRL e BTC, comprar e vender Bitcoin e visualizar o extrato com o patrimônio consolidado.

O projeto foi desenvolvido com **TDD básico** (testes antes da implementação), testes unitários em **xUnit** e histórico de commits no padrão **Conventional Commits**.

## Funcionalidades

- **Gestão de carteira**: saldo em reais (BRL) e em Bitcoin (BTC/satoshis), com validações que impedem saldos negativos.
- **Cotação de mercado**: cotação simulada do Bitcoin em BRL, com consulta e atualização pelo menu.
- **Compra e venda**: ordens executadas com a cotação atual, validando saldo em BRL para compra e saldo em BTC para venda.
- **Extrato**: histórico completo das operações, totais comprados/vendidos e patrimônio consolidado.
- **Interface de console**: menu interativo com tratamento de entradas inválidas e mensagens em português.

## Arquitetura

O projeto foi organizado em duas aplicações, conforme o roteiro da atividade:

| Projeto | Responsabilidade |
| --- | --- |
| `MiniBitcoin.Console` | Aplicação console, domínio e serviços |
| `MiniBitcoin.Tests` | Testes unitários com xUnit |

Dentro do projeto console:

| Pasta | Conteúdo |
| --- | --- |
| `Domain/` | `Carteira`, `Transacao`, `Extrato` e `TipoTransacao` |
| `Services/` | `CotacaoService` e `CorretoraService` |
| `UI/` | `MenuConsole`, responsável pela interação com o usuário |
| `Program.cs` | Composição das dependências e ponto de entrada |

```
MiniBitcoin/
├─ MiniBitcoin.sln
├─ MiniBitcoin.Console/
│  ├─ Domain/
│  │  ├─ Carteira.cs
│  │  ├─ Transacao.cs
│  │  └─ Extrato.cs
│  ├─ Services/
│  │  ├─ CotacaoService.cs
│  │  └─ CorretoraService.cs
│  ├─ UI/
│  │  └─ MenuConsole.cs
│  ├─ Program.cs
│  └─ MiniBitcoin.Console.csproj
└─ MiniBitcoin.Tests/
   ├─ CarteiraTests.cs
   ├─ TransacaoTests.cs
   ├─ ExtratoTests.cs
   └─ MiniBitcoin.Tests.csproj
```

## Regras de negócio

- Os saldos em BRL usam `decimal`; o saldo em Bitcoin é armazenado em **satoshis** (`long`), evitando erros de arredondamento.
- `1 BTC = 100.000.000 satoshis` (8 casas decimais).
- Saldos iniciais e depósitos não podem ser negativos.
- Saques e débitos não podem deixar a carteira com saldo negativo.
- A compra debita o valor em BRL e credita a quantidade de satoshis calculada pela cotação atual.
- A venda debita os satoshis e credita o valor em BRL calculado pela cotação atual.
- Quantidades convertidas são arredondadas para baixo, garantindo que o usuário nunca receba mais BTC do que pagou.
- O patrimônio consolidado é calculado como `saldo em BRL + saldo em BTC × cotação atual`.
- A cotação simulada parte de **R$ 500.000,00 por BTC** e pode ser alterada pelo menu.

## Pré-requisitos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) ou superior

## Como executar

Na raiz do repositório:

```bash
dotnet restore
dotnet run --project MiniBitcoin.Console/MiniBitcoin.Console.csproj
```

A carteira inicia com **R$ 10.000,00** e **0,01 BTC**. O menu oferece as opções:

```
1 - Consultar cotação
2 - Ver saldos
3 - Comprar Bitcoin
4 - Vender Bitcoin
5 - Exibir extrato e patrimônio
6 - Atualizar cotação
7 - Depositar BRL
0 - Sair
```

Valores decimais devem ser informados no formato brasileiro, por exemplo `1000,50`.

## Como executar os testes

```bash
dotnet test
```

A suíte cobre:

- **Carteira**: saldos iniciais, conversão para satoshis, depósitos, saques e bloqueio de saldos negativos.
- **Transações**: compra e venda com saldo suficiente, saldo insuficiente, frações de BTC, arredondamento e valores inválidos.
- **Extrato**: registro e ordem das operações, totais de compras/vendas e cálculo do patrimônio.

## Desenvolvimento com TDD

Cada etapa foi conduzida no ciclo **vermelho → verde → refatoração**:

1. Os testes da funcionalidade foram escritos e commitados (falhando).
2. A implementação mínima foi adicionada até todos os testes passarem.
3. O código foi refatorado e documentado.

O histórico segue o padrão Conventional Commits:

```
chore: initial project structure with console and xunit apps
test(carteira): add unit tests for wallet balance validation
feat(carteira): implement wallet entity and initial balance rules
test(transacao): add unit tests for bitcoin buy and sell operations
feat(transacao): implement bitcoin purchase and sale logic with quote validation
test(extrato): add unit tests for transaction ledger and portfolio valuation
feat(extrato): implement transaction history and portfolio summary
feat(console): implement interactive menu for bitcoin wallet application
refactor(core): improve code readability and organize namespaces
docs: add comprehensive instructions in README.md
```

## Decisões e limitações

- A cotação é **simulada** e mantida em memória; não há integração com APIs externas de mercado.
- Os dados da carteira e do extrato existem apenas durante a execução; não há persistência em banco de dados.
- A aplicação não realiza transferências reais de Bitcoin; trata-se de um exercício acadêmico de qualidade de software.
