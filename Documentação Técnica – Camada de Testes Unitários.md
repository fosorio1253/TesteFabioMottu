# Documentação Técnica da Camada de Testes Unitários

## 1. Visão Geral da Camada

A camada de Testes Unitários é responsável por garantir o correto funcionamento dos componentes individuais da aplicação. Ela contém testes automatizados que verificam se cada unidade de código (método, classe, etc.) se comporta conforme o esperado.

-   **Propósito:** O propósito principal desta camada é isolar e testar cada parte do código da aplicação, assegurando que ela funcione corretamente de forma independente. Isso ajuda a identificar e corrigir bugs precocemente, além de facilitar a refatoração e a evolução do código.
-   **Comunicação:** A camada de Testes Unitários não se comunica diretamente com outras camadas da aplicação em tempo de execução. Em vez disso, ela utiliza bibliotecas de teste (como xUnit e Moq) para simular o comportamento de dependências e interagir com o código sob teste de forma isolada.
-   **Padrões e Princípios:**
    -   **Testes Unitários:** Implementa testes unitários para verificar o comportamento de unidades individuais de código.
    -   **AAA (Arrange-Act-Assert):** Utiliza o padrão AAA para estruturar os testes, onde Arrange prepara o cenário, Act executa o código sob teste e Assert verifica o resultado.
    -   **Moq (Mocking):** Utiliza a biblioteca Moq para criar objetos mock (simulacros) de dependências, permitindo isolar o código sob teste.
    -   **FluentAssertions:** Utiliza a biblioteca FluentAssertions para escrever asserções mais claras e expressivas.

## 2. Componentes Principais

A camada de Testes Unitários é organizada em projetos de teste que espelham a estrutura da camada `Application` e `Domain`. Os componentes principais são:

-   **Projetos de Teste:**
    -   `Vrumm.Test.Unit.Application`: Contém testes unitários para a camada `Application`, testando os Handlers, Validators e outros componentes.
    -   `Vrumm.Test.Unit.Domain`: Contém testes unitários para a camada `Domain`, testando as entidades, Value Objects e regras de negócio.
-   **Classes de Teste:**
    -   Cada classe de teste é responsável por testar um componente específico da aplicação (ex: `LoginCommandHandlerTests`, `BirthDateTests`).
-   **Métodos de Teste:**
    -   Cada método de teste verifica um cenário específico de comportamento do componente sob teste (ex: `Handle_ValidCredentials_ReturnsAuthToken`, `Create_EmptyBirthDate_ThrowsDomainException`).
-   **Mocks:**
    -   Objetos simulados criados com a biblioteca Moq para substituir dependências reais (ex: `_userServiceMock`, `_jwtTokenServiceMock`).
-   **Assertions:**
    -   Verificações que confirmam se o resultado do código sob teste é o esperado, utilizando a biblioteca FluentAssertions (ex: `result.Should().Be(token)`, `act.Should().Throw<DomainException>()`).

## 3. Fluxo de Execução

O fluxo de execução dos testes unitários é o seguinte:

1.  O desenvolvedor executa os testes utilizando um runner de testes (ex: Visual Studio Test Explorer, dotnet test).
2.  O runner de testes descobre e executa todas as classes e métodos de teste nos projetos de teste.
3.  Para cada método de teste:
    -   **Arrange:** Ocorre a configuração do cenário de teste, incluindo a criação de objetos de teste e a configuração de mocks.
    -   **Act:** O código sob teste é executado.
    -   **Assert:** As asserções verificam se o resultado da execução é o esperado.
4.  O runner de testes reporta os resultados dos testes (sucesso ou falha).

## 4. Dependências

A camada de Testes Unitários possui as seguintes dependências externas relevantes:

-   **xUnit:** Framework de testes unitários para .NET.
-   **Moq:** Biblioteca de mocking para .NET, utilizada para criar objetos simulacros de dependências.
-   **FluentAssertions:** Biblioteca de asserções para .NET, utilizada para escrever asserções mais claras e expressivas.
-   **Microsoft.NET.Test.Sdk:** SDK do .NET para testes.

## 5. Boas Práticas e Decisões de Projeto

-   **Nomeclatura Clara:** Os testes são nomeados de forma clara e descritiva, indicando o cenário que está sendo testado (ex: `Handle_ValidCredentials_ReturnsAuthToken`).
-   **Organização por Camada e Componente:** Os testes são organizados em projetos e classes que espelham a estrutura da camada `Application` e `Domain`, facilitando a localização e manutenção dos testes.
-   **Isolamento das Dependências:** Os mocks são utilizados para isolar o código sob teste das suas dependências, garantindo que os testes sejam rápidos e determinísticos.
-   **Cobertura de Código:** Busca-se manter uma boa cobertura de código, garantindo que a maior parte do código da aplicação seja testada.
-   **Testes Automatizados:** Os testes são automatizados e podem ser executados facilmente pelo runner de testes, garantindo que eles sejam executados frequentemente.

## 6. Pontos de Extensão e Manutenção

-   **Adicionar novos testes:** Novos testes podem ser adicionados para testar novas funcionalidades ou corrigir bugs.
-   **Modificar testes existentes:** Testes existentes podem ser modificados para refletir mudanças no código da aplicação.
-   **Refatorar testes:** Os testes podem ser refatorados para melhorar a legibilidade, a manutenção ou o desempenho.
-   **Riscos:**
    -   Testes frágeis que quebram facilmente com pequenas mudanças no código podem dificultar a refatoração.
    -   Testes lentos podem aumentar o tempo de build e feedback.
    -   Falta de testes pode levar a bugs e dificultar a manutenção do código.

## 7. Exemplos

**Exemplo: Teste do LoginCommandHandler**

```csharp
[Fact]
public async Task Handle_ValidCredentials_ReturnsAuthToken()
{
    // Arrange
    var command = new LoginCommand
    {
        Username = "user@example.com",
        Password = "Password123"
    };
    // Configurar mocks para simular o comportamento do UserService e JwtTokenService
    _userServiceMock.Setup(u => u.CheckPasswordAsync(It.IsAny<User>(), command.Password, It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);
    _jwtTokenServiceMock.Setup(j => j.GenerateTokenAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync("jwt-token");

    // Act
    var result = await _handler.Handle(command, CancellationToken.None); // Executar o código sob teste

    // Assert
    result.Should().Be("jwt-token"); // Verificar se o resultado é o esperado
    _userServiceMock.Verify(u => u.CheckPasswordAsync(It.IsAny<User>(), command.Password, It.IsAny<CancellationToken>()), Times.Once); // Verificar se o método foi chamado
    _jwtTokenServiceMock.Verify(j => j.GenerateTokenAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once); // Verificar se o método foi chamado
}