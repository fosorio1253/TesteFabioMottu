# Documentação Técnica - Sistema Vrumm

## Índice

1. [Visão Geral da Arquitetura](#1-visão-geral-da-arquitetura)
2. [Camada de Domínio (Domain)](#2-camada-de-domínio-domain)
   - [Propósito e Comunicação](#21-propósito-e-comunicação)
   - [Componentes Principais](#22-componentes-principais)
   - [Fluxo de Execução](#23-fluxo-de-execução)
   - [Boas Práticas e Decisões de Projeto](#24-boas-práticas-e-decisões-de-projeto)
   - [Pontos de Extensão e Manutenção](#25-pontos-de-extensão-e-manutenção)
3. [Camada de Aplicação (Application)](#3-camada-de-aplicação-application)
   - [Propósito e Comunicação](#31-propósito-e-comunicação)
   - [Componentes Principais](#32-componentes-principais)
   - [Fluxo de Execução](#33-fluxo-de-execução)
   - [Dependências](#34-dependências)
   - [Boas Práticas e Decisões de Projeto](#35-boas-práticas-e-decisões-de-projeto)
   - [Pontos de Extensão e Manutenção](#36-pontos-de-extensão-e-manutenção)
4. [Camada de API](#4-camada-de-api)
   - [Propósito e Comunicação](#41-propósito-e-comunicação)
   - [Componentes Principais](#42-componentes-principais)
   - [Fluxo de Execução](#43-fluxo-de-execução)
   - [Dependências](#44-dependências)
   - [Boas Práticas e Decisões de Projeto](#45-boas-práticas-e-decisões-de-projeto)
   - [Pontos de Extensão e Manutenção](#46-pontos-de-extensão-e-manutenção)
5. [Camada de Infraestrutura (Infrastructure)](#5-camada-de-infraestrutura-infrastructure)
   - [Propósito e Comunicação](#51-propósito-e-comunicação)
   - [Componentes Principais](#52-componentes-principais)
   - [Fluxo de Execução](#53-fluxo-de-execução)
   - [Dependências](#54-dependências)
   - [Boas Práticas e Decisões de Projeto](#55-boas-práticas-e-decisões-de-projeto)
   - [Pontos de Extensão e Manutenção](#56-pontos-de-extensão-e-manutenção)
6. [Camada de Testes](#6-camada-de-testes)
   - [Testes Unitários](#61-testes-unitários)
   - [Testes de Integração](#62-testes-de-integração)
   - [Boas Práticas e Decisões de Projeto](#63-boas-práticas-e-decisões-de-projeto)
7. [Exemplos de Implementação](#7-exemplos-de-implementação)
   - [Domain](#71-domain)
   - [Application](#72-application)
   - [API](#73-api)
   - [Infrastructure](#74-infrastructure)
   - [Testes](#75-testes)

## 1. Visão Geral da Arquitetura

O sistema Vrumm é construído seguindo os princípios de Clean Architecture e Domain-Driven Design (DDD), organizando-se em camadas bem definidas com responsabilidades específicas. Essa estrutura garante um alto nível de separação de preocupações, testabilidade e manutenibilidade.

As principais camadas da arquitetura são:

- **Domain**: O núcleo do sistema, contendo entidades, regras de negócio e interfaces de repositórios.
- **Application**: Orquestra os casos de uso da aplicação, implementando a lógica de negócio através de Commands, Queries e seus respectivos Handlers.
- **API**: Expõe endpoints REST para clientes externos, convertendo requisições HTTP em comandos e queries para a camada Application.
- **Infrastructure**: Implementa os detalhes técnicos como acesso a banco de dados, serviços de mensageria e armazenamento em nuvem.

Esta arquitetura segue os seguintes princípios:

- **Dependência Unidirecional**: Camadas externas dependem de camadas internas, nunca o contrário.
- **Inversão de Dependência**: Interfaces definidas em camadas internas, implementadas em camadas externas.
- **Separação de Responsabilidades**: Cada camada tem um propósito claro e bem definido.
- **Encapsulamento**: Detalhes de implementação são escondidos através de abstrações.

## 2. Camada de Domínio (Domain)

### 2.1 Propósito e Comunicação

A camada Domain representa o núcleo central da aplicação Vrumm. Seu papel é encapsular a lógica de negócio de forma isolada e independente de tecnologia, frameworks e infraestrutura externa. Ela expressa de forma explícita as regras do domínio, garantindo integridade, consistência e validação desde a raiz do sistema.

**Comunicação com outras camadas**:
- **Entradas**: Recebe instruções da camada de Application (casos de uso).
- **Saídas**: Expõe modelos de domínio (Entidades e Objetos de Valor), contratos (interfaces de repositórios) e eventos de domínio que podem ser consumidos por outras camadas.

### 2.2 Componentes Principais

#### Entidades (Entities)
- **`Rental`**: Regras de locação de motos, cálculo de valores, status, geração de eventos.
- **`Driver`**: Representa o entregador, com CNH, CNPJ, idade e imagem da habilitação.
- **`Motorcycle`**: Moto cadastrada, com status, modelo, ano e placa.
- **`Plan`**: Condições e regras financeiras do aluguel (dias, tarifas, multas).
- **`User`**: Dados de autenticação e controle de acesso.
- **`MotorcycleRegistrationEvent`**: Histórico de registro da moto.

#### Objetos de Valor (Value Objects)
- **`Cnpj`**, **`LicenseNumber`**, **`LicensePlate`**, **`BirthDate`**, **`ManufactureYear`**, **`MotorcycleModel`**, **`LicenseTypeValue`**: Garantem imutabilidade e validação encapsulada.

#### Eventos de Domínio
- **`RentalCreated`**, **`RentalFinalized`**, **`MotorcycleRegistered`**: Representam fatos importantes do negócio, permitindo reações desacopladas.

#### Repositórios (Interfaces)
- Contratos como `IRentalRepository`, `IDriverRepository`, `IMotorcycleRepository` etc. permitem abstrair o acesso ao armazenamento de dados.

#### Exceções Especializadas
- Agrupadas por contexto (Drivers, Motorcycles, Auth). Garantem clareza e controle de erros de negócio.

#### Opções de Configuração
- `JwtOptions`, `PlanOptions`, `MotorcycleRegistrationOptions`, entre outros, encapsulam configurações externas consumidas pelas camadas superiores.

### 2.3 Fluxo de Execução

**Exemplo: Criação de uma Locação**
1. A camada de Application solicita a criação via `Rental.CreateNextDayRental(...)`.
2. A entidade `Rental` aplica validações (ex: datas, IDs válidos).
3. Um evento `RentalCreated` pode ser gerado com os dados da nova locação.
4. A camada de Application salva a entidade via `IRentalRepository.AddAsync()`.
5. Eventos são publicados e controlados em camadas superiores.

**Transições de Estado**
- `Motorcycle` altera seu status via `Rent`, `Return`, `SetUnderMaintenance` ou `SetInactive`, sempre validando a operação com regras explícitas.

### 2.4 Boas Práticas e Decisões de Projeto

- **Encapsulamento forte**: Toda lógica de negócio reside nas entidades e objetos de valor. Regras de consistência não estão espalhadas, evitando duplicidade.
- **Eventos de Domínio**: Favorecem desacoplamento e reatividade. Ex: `Rental.GenerateFinalizedEvent()` só ocorre se regras forem atendidas.
- **Value Objects com validações internas**: Ex: `BirthDate.Create(value)` valida idade mínima de 18 anos e limites históricos.
- **Repositórios via interfaces**: Isolam persistência, facilitando testes e substituição da implementação.

### 2.5 Pontos de Extensão e Manutenção

**Extensões**:
- Novos tipos de planos: adicionar ao `PlanOptions` e criar nova fábrica.
- Regras de elegibilidade para motoristas: extensão do método `CanRentMotorcycle()`.
- Novos eventos de domínio: ampliar integração com serviços externos.

**Riscos**:
- Validações muito acopladas podem dificultar reuso.
- Evite lógica de aplicação (como envio de email) dentro do domínio.

## 3. Camada de Aplicação (Application)

### 3.1 Propósito e Comunicação

A camada Application é o coração da arquitetura, responsável por implementar a lógica de negócio da aplicação. Ela orquestra a interação entre diferentes partes do sistema, define os casos de uso e valida as requisições.

**Comunicação**:
- Recebe requisições da camada de API e retorna respostas para ela.
- Comunica-se com a camada Domain para acessar e manipular entidades de domínio.
- Interage com a camada Infrastructure para acessar dados e serviços externos.

### 3.2 Componentes Principais

#### Commands e Handlers
- **Commands**: Representam as ações que o sistema pode executar (ex: `CreateDriverCommand`, `CreateRentalCommand`). São objetos que encapsulam os dados necessários para realizar a ação.
- **Handlers**: Responsáveis por executar a lógica associada a um Command. Cada Command tem um Handler correspondente (ex: `CreateDriverCommandHandler`, `CreateRentalCommandHandler`).

#### Queries e Handlers
- **Queries**: Representam as solicitações de dados do sistema (ex: `GetDriversQuery`, `GetRentalsQuery`).
- **Handlers**: Responsáveis por recuperar os dados solicitados pelas Queries.

#### DTOs (Data Transfer Objects)
- Objetos simples usados para transferir dados entre a camada Application e outras camadas. Evitam expor as entidades de domínio diretamente.

#### Interfaces
- Definem contratos para os componentes da camada, promovendo o baixo acoplamento e a possibilidade de substituir implementações.  
- Exemplos: `ICommandDispatcher`, `IQueryDispatcher`, `ITokenService`, `IUserService`.

#### Validators
- Responsáveis por validar os Commands e Queries antes de serem processados, garantindo a integridade dos dados de entrada.

#### Behaviors
- Implementam comportamentos transversais, como logging, validação, tratamento de transações e performance. 
- São aplicados através de um pipeline de execução.

#### CommandBus e Dispatchers
- **CommandBus**: Atua como um mediador para enviar Commands e Queries para seus respectivos Handlers.
- **CommandDispatcher e QueryDispatcher**: Responsáveis por localizar e executar os Handlers corretos.

#### Events e Handlers
- **Events**: Representam algo que aconteceu no domínio (ex: `MotorcycleRegisteredEvent`, `RentalCreatedEvent`).
- **Handlers**: Executam ações em resposta aos Events (ex: enviar notificações, atualizar outros agregados).

#### Policies
- Definem regras de negócio e restrições (ex: `MotorcycleRegistrationPolicy`).

#### Factories
- Criam instâncias de objetos complexos, encapsulando a lógica de criação (ex: `PlanFactory`).

#### Services
- Implementam lógicas específicas, como autenticação e geração de tokens (ex: `JwtTokenService`, `UserService`).

### 3.3 Fluxo de Execução

1. A camada API recebe uma requisição e a transforma em um `Command` ou `Query`.
2. O `Command` ou `Query` é enviado para o `CommandBus` ou `QueryDispatcher`.
3. O `CommandBus` ou `QueryDispatcher` localiza o `Handler` correspondente ao `Command` ou `Query`.
4. Antes de executar o `Handler`, um pipeline de `Behaviors` é executado. Esse pipeline pode incluir:
   - **Validação**: O `Command` ou `Query` é validado usando os `Validators` configurados.
   - **Logging**: A requisição é registrada para fins de auditoria e diagnóstico.
   - **Transação**: Uma transação é iniciada (se necessário) para garantir a consistência dos dados.
   - **Performance**: O tempo de execução da requisição é medido para monitorar o desempenho.
   - **Autorização**: Verifica se o usuário tem permissão para executar a ação.
5. O `Handler` é executado, realizando a lógica de negócio:
   - Interage com o `Domain` para manipular as entidades.
   - Acessa a `Infrastructure` para persistir os dados ou interagir com serviços externos.
   - Publica `Events` se necessário.
6. O `Handler` retorna o resultado para o `CommandBus` ou `QueryDispatcher`.
7. Se uma transação foi iniciada, ela é comitada. Em caso de erro, é feito rollback.
8. O resultado é retornado para a camada API.

### 3.4 Dependências

- **FluentValidation**: Utilizada para definir regras de validação para Commands e Queries.
- **Microsoft.Extensions.DependencyInjection**: Utilizada para Injeção de Dependência (DI).
- **Microsoft.Extensions.Logging**: Utilizada para registrar logs e informações de diagnóstico.
- **Microsoft.Extensions.Options**: Utilizada para acessar configurações da aplicação.
- **Microsoft.AspNetCore.Identity**: Utilizada para recursos de identidade, como hash de senhas.

### 3.5 Boas Práticas e Decisões de Projeto

- **Injeção de Dependência (DI)**: A camada utiliza DI extensivamente para promover o baixo acoplamento e a testabilidade. Os componentes são injetados via construtor, permitindo a substituição fácil de implementações.
- **Uso de Interfaces**: Interfaces são amplamente utilizadas para definir contratos entre os componentes, facilitando a manutenção e a extensão do sistema.
- **Pipeline de Behaviors**: A implementação de comportamentos transversais através de um pipeline permite adicionar ou remover funcionalidades sem modificar o código dos Handlers.
- **Exceções Customizadas**: A camada define exceções customizadas para representar erros específicos da aplicação, facilitando o tratamento de erros pelas camadas superiores.
- **CQRS (Command and Query Responsibility Segregation)**: Embora não implementado de forma estrita, a separação entre Commands e Queries indica uma intenção de aderir aos princípios do CQRS, melhorando a escalabilidade e a performance.

### 3.6 Pontos de Extensão e Manutenção

**Extensões**:
- Adicionar novos casos de uso: Novos casos de uso podem ser adicionados criando novos `Commands` e `Handlers`.
- Estender a validação: Novas regras de validação podem ser adicionadas aos `Validators`.
- Adicionar novos comportamentos: Novos `Behaviors` podem ser adicionados ao pipeline para implementar funcionalidades transversais adicionais.
- Modificar a lógica de negócio: A lógica de negócio pode ser modificada nos `Handlers`.

**Riscos**:
- Aumento da complexidade se o pipeline de `Behaviors` se tornar muito grande.
- Acoplamento excessivo dos `Handlers` com a camada `Infrastructure` pode dificultar os testes e a manutenção.

## 4. Camada de API

### 4.1 Propósito e Comunicação

A camada API é responsável por expor os endpoints da aplicação, permitindo a comunicação com clientes externos (front-end, outros sistemas, etc.) através de requisições HTTP. Ela recebe as requisições, as traduz para comandos e queries da camada Application, e retorna as respostas formatadas.

**Comunicação**:
- Recebe requisições HTTP (GET, POST, PUT, DELETE) e as transforma em chamadas para a camada Application.
- Utiliza a camada Application para executar a lógica de negócio e retorna as respostas formatadas (JSON, XML, etc.) para o cliente.

### 4.2 Componentes Principais

#### Controllers
- Responsáveis por receber as requisições HTTP, roteá-las para a ação correta e retornar as respostas.
- Cada Controller representa um conjunto de funcionalidades relacionadas (ex: `AuthController` para autenticação, `EntregadoresController` para motoristas, `LocacaoController` para locações).
- Utilizam o `ICommandDispatcher` para enviar comandos para a camada Application.

#### Models (Requests e Responses)
- **Requests**: Classes que representam os dados de entrada das requisições (ex: `LoginRequest`, `CreateEntregadorRequest`).
- **Responses**: Classes que representam os dados de saída das requisições (ex: `LoginResponse`, `ErrorResponse`).
- São usados para mapear os dados da requisição para os comandos/queries da camada Application e para formatar as respostas.

#### Middleware
- Implementa funcionalidades transversais, como tratamento de erros (`ErrorHandlingMiddleware`), segurança, etc.
- É executado no pipeline de requisições do ASP.NET Core.

#### Program.cs
- É o ponto de entrada da aplicação.
- Configura os serviços, o pipeline de requisições, a autenticação e autorização, e outras configurações globais.

### 4.3 Fluxo de Execução

1. O cliente faz uma requisição HTTP para um endpoint da API (ex: `POST /auth/login`).
2. O ASP.NET Core roteia a requisição para o Controller e a Action correspondente (`AuthController.Login`).
3. O Controller recebe os dados da requisição e os mapeia para um `Command` ou `Query` da camada Application (ex: `LoginCommand`).
4. O Controller utiliza o `ICommandDispatcher` para enviar o `Command` ou `Query` para a camada Application.
5. A camada Application processa o `Command` ou `Query` e retorna o resultado.
6. O Controller recebe o resultado da camada Application e o formata em um `Response` (se necessário).
7. O Controller retorna o `Response` (ou um código de status HTTP) para o cliente.
8. O ASP.NET Core serializa a resposta (geralmente em JSON) e a envia de volta para o cliente.

### 4.4 Dependências

- **Microsoft.AspNetCore.Mvc**: Framework do ASP.NET Core para construir APIs RESTful.
- **Microsoft.AspNetCore.Authorization**: Framework do ASP.NET Core para autenticação e autorização.
- **Microsoft.AspNetCore.Authentication.JwtBearer**: Middleware para autenticação com JWT (JSON Web Tokens).
- **Swashbuckle.AspNetCore (OpenAPI)**: Utilizado para gerar a documentação da API no formato OpenAPI (Swagger).
- **Vrumm.Application**: Dependência da camada Application para acessar a lógica de negócio.
- **Vrumm.Domain**: Dependência da camada Domain para acessar as entidades de domínio.
- **Vrumm.Infrastructure**: Dependência da camada Infrastructure para acessar serviços externos e dados.

### 4.5 Boas Práticas e Decisões de Projeto

- **Uso de Controllers**: Os Controllers são usados para organizar o código da API em termos de funcionalidades e recursos, seguindo o padrão MVC (Model-View-Controller).
- **Models para DTOs**: Os Models (Requests e Responses) são usados como DTOs (Data Transfer Objects) para isolar a API das entidades de domínio e definir claramente o contrato de entrada e saída da API.
- **Autenticação e Autorização com JWT**: A autenticação e autorização são implementadas usando JWT, permitindo proteger os endpoints da API e controlar o acesso aos recursos.
- **Documentação com OpenAPI (Swagger)**: O Swagger é utilizado para gerar automaticamente a documentação da API, facilitando o consumo e a integração por parte de clientes externos.
- **Tratamento de Erros Centralizado**: O `ErrorHandlingMiddleware` centraliza o tratamento de erros, garantindo uma resposta consistente e informativa para o cliente em caso de falha.
- **Injeção de Dependência**: A API utiliza a injeção de dependência do ASP.NET Core para configurar e fornecer as dependências necessárias para os Controllers e outros componentes.

### 4.6 Pontos de Extensão e Manutenção

**Extensões**:
- Adicionar novos endpoints: Novos endpoints podem ser adicionados criando novos Controllers e Actions.
- Modificar a lógica de um endpoint: A lógica de um endpoint pode ser modificada no Controller correspondente.
- Adicionar novos Models: Novos Models podem ser adicionados para representar novos dados de entrada ou saída.
- Adicionar novas funcionalidades transversais: Novas funcionalidades transversais (ex: logging, cache) podem ser adicionadas através de Middlewares.
- Modificar a autenticação/autorização: As configurações de autenticação e autorização podem ser modificadas no `Program.cs`.

**Riscos**:
- Acoplamento excessivo dos Controllers com a camada Application pode dificultar a manutenção e os testes.
- Complexidade crescente se muitos Middlewares forem adicionados ao pipeline.
- Falta de padronização na formatação de Requests e Responses pode levar a inconsistências.

## 5. Camada de Infraestrutura (Infrastructure)

### 5.1 Propósito e Comunicação

A camada Infrastructure é responsável por fornecer implementações técnicas de baixo nível que dão suporte à aplicação. Ela conecta o domínio a recursos externos, como banco de dados, serviços de mensageria, armazenamento de arquivos e logging, mantendo o domínio isolado de dependências técnicas.

**Comunicação com outras camadas**:
- **Entradas**: Chamadas via interfaces definidas na camada de Domain.
- **Saídas**: Persistência de dados, publicação/consumo de mensagens, armazenamento em nuvem, registro de logs e health checks.

### 5.2 Componentes Principais

#### Persistência com EF Core
- **Contexto**: `VrummDbContext`, configura todas as entidades e aplica os mapeamentos com `ApplyConfigurationsFromAssembly`.
- **Repositórios**: Implementações específicas como `DriverRepository`, `MotorcycleRepository`, `RentalRepository`, `PlanRepository`, `UserRepository`, etc.
- **Configurations**: Arquivos que definem o mapeamento via Fluent API para cada entidade.

#### Unit of Work
- Interface `IUnitOfWork` e implementação `UnitOfWork` que centralizam acesso e transações.

#### Mensageria (Google Pub/Sub)
- Interfaces: `IMessagePublisher`, `IMessageConsumer`.
- Implementações: `PubSubPublisher`, `PubSubConsumer`, `PubSubHealthCheck`.
- Envelope de mensagens com metadados: `MessageEnvelope<T>`.

#### Armazenamento (Google Cloud Storage)
- Interface: `IStorageService`.
- Implementação: `GoogleCloudStorageService`.
- Suporte a upload, download, exclusão e geração de URL assinada.

#### Middlewares
- `GlobalExceptionHandlingMiddleware`: tratamento centralizado de exceções do domínio.
- `CorrelationIdMiddleware`: insere header `X-Correlation-ID` em todas as requisições e logs.

#### Logging com Serilog
- Configurado via `SerilogConfiguration`.
- Suporte a console, Google Cloud Logging e enrichments personalizados.

#### Health Checks
- Extensões para `/health`, `/live` e `/ready`.
- Health check customizado para Pub/Sub e GCS.

### 5.3 Fluxo de Execução

**Exemplo: Persistência de entidade Driver**
1. A camada Application injeta `IUnitOfWork`.
2. A chamada `Drivers.AddAsync(driver)` é delegada ao `DriverRepository`, que usa o `VrummDbContext`.
3. Ao final, `UnitOfWork.SaveChangesAsync()` persiste os dados.
4. Em caso de transações complexas, `BeginTransactionAsync()` e `CommitTransactionAsync()` são utilizados.

**Exemplo: Publicação de mensagem**
1. Evento de domínio é emitido no Domain.
2. A Application utiliza `IMessagePublisher.PublishAsync()`.
3. `PubSubPublisher` serializa e publica a mensagem no tópico configurado.

### 5.4 Dependências

**NuGet e bibliotecas externas**:
- **Entity Framework Core** (`Microsoft.EntityFrameworkCore.*`): ORM.
- **Serilog**: Logging extensível com suporte a sinks customizados.
- **Google.Cloud.\***:
  - `Storage.V1`: Acesso a buckets e objetos.
  - `PubSub.V1`: Mensageria com tópicos e subscrições.
- **HealthChecks**: Monitoramento de serviços essenciais.

### 5.5 Boas Práticas e Decisões de Projeto

- **Isolamento de responsabilidades**: Camada Infrastructure nunca contém lógica de negócio. Cada responsabilidade técnica está encapsulada em componentes específicos (Ex: PubSubPublisher, StorageService).
- **Modularidade via DependencyInjection**: Arquivos `DatabaseDependencyInjection`, `MessagingDependencyInjection`, `StorageDependencyInjection` segregam os módulos.
- **Middleware reusável e padronizado**: Logging e rastreamento de requisições centralizados. Erros expostos em JSON com status adequado.
- **Extensibilidade**: Interfaces abstratas permitem substituição de implementações. Health Checks baseados em tags permitem compor endpoints de readiness e liveness.

### 5.6 Pontos de Extensão e Manutenção

**Extensões**:
- Novos repositórios ou entidades podem ser adicionados facilmente com EF.
- Nova estratégia de logging: adicionar novo provider em `SerilogConfiguration`.
- Novo broker de mensagens: implementar `IMessagePublisher` e `IMessageConsumer`.

**Riscos**:
- Mudanças em nuvem (Pub/Sub, GCS) podem quebrar contratos se não forem validadas.
- Configurações mal definidas podem afetar health checks e inicialização da aplicação.

## 6. Camada de Testes

### 6.1 Testes Unitários

#### Visão Geral
A camada de Testes Unitários é responsável por garantir o correto funcionamento dos componentes individuais da aplicação. Ela contém testes automatizados que verificam se cada unidade de código (método, classe, etc.) se comporta conforme o esperado.

#### Componentes Principais
- **Projetos de Teste**:
  - `Vrumm.Test.Unit.Application`: Contém testes unitários para a camada Application, testando os Handlers, Validators e outros componentes.
  - `Vrumm.Test.Unit.Domain`: Contém testes unitários para a camada Domain, testando as entidades, Value Objects e regras de negócio.
- **Classes de Teste**:
  - Cada classe de teste é responsável por testar um componente específico da aplicação (ex: `LoginCommandHandlerTests`, `BirthDateTests`).
- **Métodos de Teste**:
  - Cada método de teste verifica um cenário específico de comportamento do componente sob teste (ex: `Handle_ValidCredentials_ReturnsAuthToken`, `Create_EmptyBirthDate_ThrowsDomainException`).
- **Mocks**:
  - Objetos simulados criados com a biblioteca Moq para substituir dependências reais (ex: `_userServiceMock`, `_jwtTokenServiceMock`).
- **Assertions**:
  - Verificações que confirmam se o resultado do código sob teste é o esperado, utilizando a biblioteca FluentAssertions (ex: `result.Should().Be(token)`, `act.Should().Throw<DomainException>()`).

#### Fluxo de Execução
1. O desenvolvedor executa os testes utilizando um runner de testes (ex: Visual Studio Test Explorer, dotnet test).
2. O runner de testes descobre e executa todas as classes e métodos de teste nos projetos de teste.
3. Para cada método de teste:
   - **Arrange**: Ocorre a configuração do cenário de teste, incluindo a criação de objetos de teste e a configuração de mocks.
   - **Act**: O código sob teste é executado.
   - **Assert**: As asserções verificam se o resultado da execução é o esperado.
4. O runner de testes reporta os resultados dos testes (sucesso ou falha).

#### Dependências
- **xUnit**: Framework de testes unitários para .NET.
- **Moq**: Biblioteca de mocking para .NET, utilizada para criar objetos simulacros de dependências.
- **FluentAssertions**: Biblioteca de asserções para .NET, utilizada para escrever asserções mais claras e expressivas.
- **Microsoft.NET.Test.Sdk**: SDK do .NET para testes.

### 6.2 Testes de Integração

#### Visão Geral
A camada de Testes de Integração é responsável por verificar a interação entre diferentes partes do sistema. Ao contrário dos testes unitários, que testam componentes isoladamente, os testes de integração avaliam como os componentes funcionam em conjunto.

#### Componentes Principais
- **Projetos de Teste**:
  - `Vrumm.Test.Integration`: Contém os testes de integração para