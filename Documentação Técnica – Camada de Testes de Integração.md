# Documentação Técnica da Camada de Testes de Integração

## 1.   Visão Geral da Camada

A camada de Testes de Integração é responsável por verificar a interação entre diferentes partes do sistema. Ao contrário dos testes unitários, que testam componentes isoladamente, os testes de integração avaliam como os componentes funcionam em conjunto.

-   **Propósito:** O propósito principal desta camada é garantir que os diferentes módulos e serviços da aplicação funcionem corretamente quando integrados. Isso inclui testar a comunicação entre a API e a camada `Application`, a interação com o banco de dados e outros serviços externos.
-   **Comunicação:** A camada de Testes de Integração interage com a aplicação em um nível mais alto do que os testes unitários. Ela envia requisições HTTP para a API, simula o comportamento de clientes externos e verifica as respostas da API.
-   **Padrões e Princípios:**
    -   **Testes de Integração:** Implementa testes de integração para verificar a interação entre diferentes componentes da aplicação.
    -   **Testes de API:** Foca em testar a API da aplicação, enviando requisições HTTP e verificando as respostas.
    -   **Setup e Teardown:** Utiliza mecanismos de setup e teardown para configurar o ambiente de teste antes da execução dos testes e limpar o ambiente após a execução.
    -   **Fixtures:** Utiliza fixtures para compartilhar o contexto entre os testes.

## 2.   Componentes Principais

A camada de Testes de Integração é organizada em projetos de teste que focam em diferentes aspectos da integração da aplicação. Os componentes principais são:

-   **Projetos de Teste:**
    -   `Vrumm.Test.Integration`: Contém os testes de integração para a aplicação.
-   **Classes de Teste:**
    -   Cada classe de teste é responsável por testar um conjunto de funcionalidades relacionadas da API (ex: `AuthControllerTests`, `EntregadoresControllerTests`).
-   **Métodos de Teste:**
    -   Cada método de teste verifica um cenário específico de integração (ex: `Login_Should_Return_Token_When_Credentials_Are_Valid`, `Register_Should_Create_User_When_Admin_Token_Provided`).
-   **HttpClient:**
    -   Utilizado para enviar requisições HTTP para a API da aplicação.
-   **CustomWebApplicationFactory:**
    -   Classe que configura um ambiente de teste para a API, permitindo que os testes sejam executados em um contexto controlado.
-   **AuthHelper:**
    -   Classe auxiliar para obter tokens de autenticação para os testes que requerem autorização.
-   **Models (Requests e Responses):**
    -   Classes que representam os dados de entrada e saída das requisições da API, utilizadas para serializar e desserializar os dados nos testes.
-   **Assertions:**
    -   Verificações que confirmam se o resultado da integração é o esperado, utilizando a biblioteca FluentAssertions (ex: `response.StatusCode.Should().Be(HttpStatusCode.OK)`, `result!.Token.Should().NotBeNullOrEmpty()`).

## 3.   Fluxo de Execução

O fluxo de execução dos testes de integração é o seguinte:

1.  O desenvolvedor executa os testes utilizando um runner de testes (ex: Visual Studio Test Explorer, dotnet test).
2.  O runner de testes descobre e executa todas as classes e métodos de teste no projeto de teste de integração.
3.  Para cada classe de teste:
    -   Uma instância do `CustomWebApplicationFactory` é criada para configurar o ambiente de teste da API.
    -   Um `HttpClient` é criado para enviar requisições para a API.
4.  Para cada método de teste:
    -   Ocorre a configuração do cenário de teste, incluindo a preparação dos dados de entrada e a configuração do ambiente de teste.
    -   Requisições HTTP são enviadas para a API utilizando o `HttpClient`.
    -   As respostas da API são verificadas utilizando asserções.
5.  O runner de testes reporta os resultados dos testes (sucesso ou falha).
6.  Após a execução dos testes, o `CustomWebApplicationFactory` limpa o ambiente de teste.

## 4.   Dependências

A camada de Testes de Integração possui as seguintes dependências externas relevantes:

-   **Microsoft.AspNetCore.Mvc.Testing:** Fornece classes para testar aplicações ASP.NET Core.
-   **Microsoft.NET.Test.Sdk:** SDK do .NET para testes.
-   **xUnit:** Framework de testes unitários para .NET (utilizado também para testes de integração).
-   **FluentAssertions:** Biblioteca de asserções para .NET, utilizada para escrever asserções mais claras e expressivas.
-   **System.Net.Http.Json:** Fornece métodos de extensão para trabalhar com JSON em `HttpClient`.

## 5.   Boas Práticas e Decisões de Projeto

-   **Testes de API:** Os testes são focados em testar a API da aplicação, garantindo que ela funcione corretamente como um todo.
-   **Ambiente de Teste Controlado:** O `CustomWebApplicationFactory` é utilizado para configurar um ambiente de teste isolado, garantindo que os testes sejam determinísticos e não afetem o ambiente de produção.
-   **Reutilização de Código:** Classes auxiliares como `AuthHelper` são utilizadas para reutilizar código comum entre os testes, como a obtenção de tokens de autenticação.
-   **Nomeclatura Clara:** Os testes são nomeados de forma clara e descritiva, indicando o cenário que está sendo testado (ex: `Login_Should_Return_Token_When_Credentials_Are_Valid`).
-   **Organização por Funcionalidade:** Os testes são organizados em classes que focam em funcionalidades específicas da API, facilitando a localização e manutenção dos testes.

## 6.   Pontos de Extensão e Manutenção

-   **Adicionar novos testes:** Novos testes podem ser adicionados para testar novas funcionalidades da API ou corrigir bugs.
-   **Modificar testes existentes:** Testes existentes podem ser modificados para refletir mudanças na API da aplicação.
-   **Refatorar testes:** Os testes podem ser refatorados para melhorar a legibilidade, a manutenção ou o desempenho.
-   **Adicionar novos contextos de teste:** Novos contextos de teste podem ser adicionados para testar diferentes configurações ou cenários da aplicação.
-   **Riscos:**
    -   Testes de integração podem ser mais lentos do que testes unitários, pois envolvem a execução de várias partes da aplicação.
    -   Testes de integração podem ser mais complexos de configurar e manter do que testes unitários.
    -   Testes de integração podem ser mais propensos a falhar devido a problemas de ambiente ou configuração.

## 7.   Exemplos

**Exemplo: Teste do Login da API**

```csharp
[Fact]
public async Task Login_Should_Return_Token_When_Credentials_Are_Valid()
{
    // Arrange
    var credentials = new { username = "admin", password = "Admin123!" }; // Dados de login válidos

    // Act
    var response = await _client.PostAsJsonAsync("/auth/login", credentials); // Envia a requisição de login

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK); // Verifica se o status code é 200 OK
    var result = await response.Content.ReadFromJsonAsync<LoginResponse>(); // Desserializa a resposta JSON
    result!.Token.Should().NotBeNullOrEmpty(); // Verifica se o token não é nulo ou vazio
}