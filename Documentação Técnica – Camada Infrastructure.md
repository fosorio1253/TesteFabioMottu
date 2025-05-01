# Documentação Técnica – Camada Infrastructure

## 1. Visão Geral da Camada

### Propósito

A camada **Infrastructure** é responsável por fornecer implementações técnicas
de baixo nível que dão suporte à aplicação. Ela conecta o domínio a recursos externos, como banco de dados, serviços de mensageria, armazenamento de arquivos e logging, mantendo o domínio isolado de dependências técnicas.

### Comunicação com outras camadas

- **Entradas:** Chamadas via interfaces definidas na camada de Domain.
- **Saídas:** Persistência de dados, publicação/consumo de mensagens, armazenamento em nuvem,
registro de logs e health checks.

### Padrões e princípios aplicados

- **Dependency Injection**: Todos os serviços são registrados por meio de extensões
de `IServiceCollection`.
- **Repository Pattern**: Implementações concretas dos contratos de repositório definidos
na camada Domain.
- **Unit of Work**: Controle transacional e de persistência agrupada.
- **Middleware de infraestrutura**: Logging, correlação de requisições e tratamento
global de exceções.
- **Health Checks**: Avaliação de disponibilidade de serviços externos (DB, PubSub, GCS).

---

## 2. Componentes Principais

### a. **Persistência com EF Core**

- **Contexto:** `VrummDbContext`, configura todas as entidades e aplica os mapeamentos
com `ApplyConfigurationsFromAssembly`.
- **Repositórios:** Implementações específicas como `DriverRepository`, `MotorcycleRepository`,
`RentalRepository`, `PlanRepository`, `UserRepository`, etc.
- **Configurations:** Arquivos que definem o mapeamento via Fluent API para cada entidade.

### b. **Unit of Work**

- Interface `IUnitOfWork` e implementação `UnitOfWork` que centralizam acesso e transações.

### c. **Mensageria (Google Pub/Sub)**

- Interfaces: `IMessagePublisher`, `IMessageConsumer`.
- Implementações: `PubSubPublisher`, `PubSubConsumer`, `PubSubHealthCheck`.
- Envelope de mensagens com metadados: `MessageEnvelope<T>`.

### d. **Armazenamento (Google Cloud Storage)**

- Interface: `IStorageService`.
- Implementação: `GoogleCloudStorageService`.
- Suporte a upload, download, exclusão e geração de URL assinada.

### e. **Middlewares**

- `GlobalExceptionHandlingMiddleware`: tratamento centralizado de exceções do domínio.
- `CorrelationIdMiddleware`: insere header `X-Correlation-ID` em todas as requisições e logs.

### f. **Logging com Serilog**

- Configurado via `SerilogConfiguration`.
- Suporte a console, Google Cloud Logging e enrichments personalizados.

### g. **Health Checks**

- Extensões para `/health`, `/live` e `/ready`.
- Health check customizado para Pub/Sub e GCS.

---

## 3. Fluxo de Execução

### Exemplo: Persistência de entidade Driver

1. A camada Application injeta `IUnitOfWork`.
2. A chamada `Drivers.AddAsync(driver)` é delegada ao `DriverRepository`,
que usa o `VrummDbContext`.
3. Ao final, `UnitOfWork.SaveChangesAsync()` persiste os dados.
4. Em caso de transações complexas, `BeginTransactionAsync()` e
`CommitTransactionAsync()` são utilizados.

### Exemplo: Publicação de mensagem

1. Evento de domínio é emitido no Domain.
2. A Application utiliza `IMessagePublisher.PublishAsync()`.
3. `PubSubPublisher` serializa e publica a mensagem no tópico configurado.

---

## 4. Dependências

### NuGet e bibliotecas externas:

- **Entity Framework Core** (`Microsoft.EntityFrameworkCore.*`): ORM.
- **Serilog**: Logging extensível com suporte a sinks customizados.
- **Google.Cloud.\*:**
  - `Storage.V1`: Acesso a buckets e objetos.
  - `PubSub.V1`: Mensageria com tópicos e subscrições.
- **HealthChecks**: Monitoramento de serviços essenciais.

---

## 5. Boas Práticas e Decisões de Projeto

### Isolamento de responsabilidades

- Camada Infrastructure nunca contém lógica de negócio.
- Cada responsabilidade técnica está encapsulada em componentes específicos
(Ex: PubSubPublisher, StorageService).

### Modularidade via DependencyInjection

- Arquivos `DatabaseDependencyInjection`, `MessagingDependencyInjection`,
- `StorageDependencyInjection` segregam os módulos.

### Middleware reusável e padronizado

- Logging e rastreamento de requisições centralizados.
- Erros expostos em JSON com status adequado.

### Extensibilidade

- Interfaces abstratas permitem substituição de implementações.
- Health Checks baseados em tags permitem compor endpoints de readiness e liveness.

---

## 6. Pontos de Extensão e Manutenção

### Extensões

- Novos repositórios ou entidades podem ser adicionados facilmente com EF.
- Nova estratégia de logging: adicionar novo provider em `SerilogConfiguration`.
- Novo broker de mensagens: implementar `IMessagePublisher` e `IMessageConsumer`.

### Riscos

- Mudanças em nuvem (Pub/Sub, GCS) podem quebrar contratos se não forem validadas.
- Configurações mal definidas podem afetar health checks e inicialização da aplicação.

---

## 7. Exemplos

### Injeção de dependências:

```csharp
services.AddInfrastructure();
```

### Persistência com Unit of Work:

```csharp
await _unitOfWork.Drivers.AddAsync(driver);
await _unitOfWork.SaveChangesAsync();
```

### Publicação de evento:

```csharp
await _messagePublisher.PublishAsync(new RentalCreatedEvent(...));
```

---

## Conclusão

A camada Infrastructure do sistema Vrumm está bem segmentada, baseada em boas práticas de
engenharia e alinhada à Clean Architecture. Ela oferece suporte robusto para persistência,
mensageria, observabilidade e resiliência, sendo facilmente extensível e testável.
A separação clara entre infraestrutura e domínio garante alta manutenibilidade e aderência
a princípios modernos de arquitetura de software.

