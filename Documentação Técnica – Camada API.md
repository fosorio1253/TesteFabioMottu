# Documentação Técnica da Camada API

## 1. Visão Geral da Camada

A camada API é responsável por expor os endpoints da aplicação, permitindo a comunicação com clientes externos (front-end, outros sistemas, etc.) através de requisições HTTP. Ela recebe as requisições, as traduz para comandos e queries da camada `Application`, e retorna as respostas formatadas.

-   **Propósito:** O propósito principal desta camada é definir a interface de comunicação da aplicação, expondo a lógica de negócio para o mundo externo.
-   **Comunicação:** A camada API recebe requisições HTTP (GET, POST, PUT, DELETE) e as transforma em chamadas para a camada `Application`. Ela utiliza a camada `Application` para executar a lógica de negócio e retorna as respostas formatadas (JSON, XML, etc.) para o cliente.
-   **Padrões e Princípios:**
    -   **Arquitetura RESTful:** A API segue princípios RESTful para organizar os recursos e definir as operações (GET para obter dados, POST para criar, PUT para atualizar, DELETE para excluir).
    -   **Controladores (Controllers):** Utiliza Controllers para organizar os endpoints e definir a lógica de tratamento das requisições.
    -   **Models:** Utiliza Models (Requests e Responses) para estruturar os dados de entrada e saída da API, separando a representação externa dos objetos de domínio internos.
    -   **Princípio de Camadas:** Mantém a responsabilidade de roteamento e serialização/desserialização, delegando a lógica de negócio para a camada `Application`.

## 2. Componentes Principais

A camada API é composta principalmente pelos seguintes componentes:

-   **Controllers:**
    -   São responsáveis por receber as requisições HTTP, roteá-las para a ação correta e retornar as respostas.
    -   Cada Controller representa um conjunto de funcionalidades relacionadas (ex: `AuthController` para autenticação, `EntregadoresController` para motoristas, `LocacaoController` para locações).
    -   Utilizam o `ICommandDispatcher` para enviar comandos para a camada `Application`.
-   **Models (Requests e Responses):**
    -   **Requests:** Classes que representam os dados de entrada das requisições (ex: `LoginRequest`, `CreateEntregadorRequest`).
    -   **Responses:** Classes que representam os dados de saída das requisições (ex: `LoginResponse`, `ErrorResponse`).
    -   São usados para mapear os dados da requisição para os comandos/queries da camada `Application` e para formatar as respostas.
-   **Middleware:**
    -   Implementa funcionalidades transversais, como tratamento de erros (`ErrorHandlingMiddleware`), segurança, etc.
    -   É executado no pipeline de requisições do ASP.NET Core.
-   **Program.cs:**
    -   É o ponto de entrada da aplicação.
    -   Configura os serviços, o pipeline de requisições, a autenticação e autorização, e outras configurações globais.

## 3. Fluxo de Execução

Um caso de uso típico na camada API segue o seguinte fluxo:

1.  O cliente faz uma requisição HTTP para um endpoint da API (ex: `POST /auth/login`).
2.  O ASP.NET Core roteia a requisição para o Controller e a Action correspondente (`AuthController.Login`).
3.  O Controller recebe os dados da requisição e os mapeia para um `Command` ou `Query` da camada `Application` (ex: `LoginCommand`).
4.  O Controller utiliza o `ICommandDispatcher` para enviar o `Command` ou `Query` para a camada `Application`.
5.  A camada `Application` processa o `Command` ou `Query` e retorna o resultado.
6.  O Controller recebe o resultado da camada `Application` e o formata em um `Response` (se necessário).
7.  O Controller retorna o `Response` (ou um código de status HTTP) para o cliente.
8.  O ASP.NET Core serializa a resposta (geralmente em JSON) e a envia de volta para o cliente.

## 4. Dependências

A camada API possui as seguintes dependências externas relevantes:

-   **Microsoft.AspNetCore.Mvc:** Framework do ASP.NET Core para construir APIs RESTful.
-   **Microsoft.AspNetCore.Authorization:** Framework do ASP.NET Core para autenticação e autorização.
-   **Microsoft.AspNetCore.Authentication.JwtBearer:** Middleware para autenticação com JWT (JSON Web Tokens).
-   **Swashbuckle.AspNetCore (OpenAPI):** Utilizado para gerar a documentação da API no formato OpenAPI (Swagger).
-   **Vrumm.Application:** Dependência da camada `Application` para acessar a lógica de negócio.
-   **Vrumm.Domain:** Dependência da camada `Domain` para acessar as entidades de domínio.
-   **Vrumm.Infrastructure:** Dependência da camada `Infrastructure` para acessar serviços externos e dados.

## 5. Boas Práticas e Decisões de Projeto

-   **Uso de Controllers:** Os Controllers são usados para organizar o código da API em termos de funcionalidades e recursos, seguindo o padrão MVC (Model-View-Controller).
-   **Models para DTOs:** Os Models (Requests e Responses) são usados como DTOs (Data Transfer Objects) para isolar a API das entidades de domínio e definir claramente o contrato de entrada e saída da API.
-   **Autenticação e Autorização com JWT:** A autenticação e autorização são implementadas usando JWT, permitindo proteger os endpoints da API e controlar o acesso aos recursos.
-   **Documentação com OpenAPI (Swagger):** O Swagger é utilizado para gerar automaticamente a documentação da API, facilitando o consumo e a integração por parte de clientes externos.
-   **Tratamento de Erros Centralizado:** O `ErrorHandlingMiddleware` centraliza o tratamento de erros, garantindo uma resposta consistente e informativa para o cliente em caso de falha.
-   **Injeção de Dependência:** A API utiliza a injeção de dependência do ASP.NET Core para configurar e fornecer as dependências necessárias para os Controllers e outros componentes.

## 6. Pontos de Extensão e Manutenção

-   **Adicionar novos endpoints:** Novos endpoints podem ser adicionados criando novos Controllers e Actions.
-   **Modificar a lógica de um endpoint:** A lógica de um endpoint pode ser modificada no Controller correspondente.
-   **Adicionar novos Models:** Novos Models podem ser adicionados para representar novos dados de entrada ou saída.
-   **Adicionar novas funcionalidades transversais:** Novas funcionalidades transversais (ex: logging, cache) podem ser adicionadas através de Middlewares.
-   **Modificar a autenticação/autorização:** As configurações de autenticação e autorização podem ser modificadas no `Program.cs`.
-   **Riscos:**
    -   Acoplamento excessivo dos Controllers com a camada `Application` pode dificultar a manutenção e os testes.
    -   Complexidade crescente se muitos Middlewares forem adicionados ao pipeline.
    -   Falta de padronização na formatação de Requests e Responses pode levar a inconsistências.

## 7. Exemplos

**Exemplo: Login de um usuário**

1.  O cliente faz uma requisição `POST` para `/auth/login` com um `LoginRequest` contendo `Username` e `Password`.
2.  O ASP.NET Core roteia a requisição para o `AuthController.Login` action.
3.  O `AuthController.Login` cria um `LoginCommand` com os dados do `LoginRequest`.
4.  O `AuthController.Login` utiliza o `_commandDispatcher` para enviar o `LoginCommand` para a camada `Application`.
5.  A camada `Application` autentica o usuário e gera um token JWT.
6.  A camada `Application` retorna o token para o `AuthController.Login`.
7.  O `AuthController.Login` cria um `LoginResponse` com o token.
8.  O `AuthController.Login` retorna um `Ok` (200 OK) com o `LoginResponse` para o cliente.

Este documento fornece uma visão geral detalhada da camada API, seus componentes, fluxo de execução e decisões de projeto. Ele deve ser útil para entender, manter e estender essa camada no futuro.