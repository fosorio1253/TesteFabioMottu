# Arquitetura Detalhada do Sistema Vrumm

## 1. Visão Geral da Arquitetura

A arquitetura proposta segue princípios de Clean Architecture e Domain-Driven Design (DDD), com uma infraestrutura que
permite desenvolvimento local em Docker e deployment em produção no Google Kubernetes Engine (GKE). A estrutura é 
dividida em camadas bem definidas, cada uma com responsabilidades específicas:

```
┌───────────────────────────────────────────────────────────────┐
│                         API Layer                             │
│  ┌───────────┐  ┌───────────┐  ┌───────────┐  ┌───────────┐   │
│  │ Motorcycle│  │  Driver   │  │  Rental   │  │ Healthcheck│  │
│  │ Controller│  │ Controller│  │Controller │  │ Controller │  │
│  └───────────┘  └───────────┘  └───────────┘  └───────────┘   │
└───────────────────────────────────────────────────────────────┘
                         │
┌───────────────────────────────────────────────────────────────┐
│                     Application Layer                         │
│  ┌───────────┐  ┌───────────┐  ┌───────────┐                  │
│  │  Commands │  │  Queries  │  │ Mediator  │                  │
│  │  Handlers │  │  Handlers │  │Implementation                │
│  └───────────┘  └───────────┘  └───────────┘                  │
└───────────────────────────────────────────────────────────────┘
                         │
┌───────────────────────────────────────────────────────────────┐
│                      Domain Layer                             │
│  ┌───────────┐  ┌───────────┐  ┌───────────┐  ┌───────────┐   │
│  │ Motorcycle│  │  Driver   │  │  Rental   │  │   Plan    │  │
│  │   Entity  │  │   Entity  │  │  Entity   │  │   Entity  │  │
│  └───────────┘  └───────────┘  └───────────┘  └───────────┘   │
└───────────────────────────────────────────────────────────────┘
                         │
┌───────────────────────────────────────────────────────────────┐
│                  Infrastructure Layer                         │
│ ┌─────────────┐ ┌────────────┐ ┌───────────────┐              │
│ │  PostgreSQL │ │   Pub/Sub  │ │  Cloud Storage│              │
│ │ Repositories│ │ Integration│ │  Integration  │              │
│ └─────────────┘ └────────────┘ └───────────────┘              │
└───────────────────────────────────────────────────────────────┘
```

## 2. Componentes e Responsabilidades por Camada

### 2.1. API Layer (Camada de Apresentação)

**Responsabilidades:**
- Expor endpoints REST conforme especificação do Swagger fornecido
- Processar requisições HTTP e produzir respostas apropriadas
- Validar entrada de dados inicial (formato, tipos)
- Transformar DTOs em comandos/consultas para a camada de aplicação
- Fornecer documentação via Swagger/OpenAPI
- Implementar tratamento global de exceções

**Componentes Principais:**
- **MotorcycleController**: Endpoints CRUD para motos
- **DriverController**: Endpoints para gerenciamento de entregadores e upload de CNH
- **RentalController**: Endpoints para gestão de locações e cálculos financeiros
- **HealthcheckController**: Endpoints para monitoramento de saúde do sistema
  - Endpoint `/health` ou `/healthz` para verificação geral do sistema
  - Endpoint `/ready` para verificar prontidão para receber tráfego 
  - Endpoint `/live` para verificar se a aplicação está ativa
  - Verificações de conectividade com PostgreSQL, Cloud Storage e Pub/Sub
  - Suporte a probes de Kubernetes para liveness e readiness

### 2.2. Application Layer (Camada de Aplicação)

**Responsabilidades:**
- Orquestrar fluxos de negócio utilizando o padrão Mediator
- Implementar lógica de aplicação (casos de uso)
- Validar regras de negócio interdependentes entre entidades
- Publicar eventos no sistema de mensageria para processamento assíncrono
- Implementar tratamento de transações

**Componentes Principais:**
- **Implementação do Mediator**: Substituto customizado para o MediatR
  - Interface `IMediator` com métodos `Send` e `Publish`
  - Implementação do Mediator utilizando o container de DI
  
- **Command Handlers**:
  - `CreateMotorcycleCommandHandler`: Cadastro de motos e publicação de eventos
  - `UpdateMotorcycleCommandHandler`: Atualização de placas
  - `DeleteMotorcycleCommandHandler`: Remoção de motos com validação
  - `CreateDriverCommandHandler`: Cadastro de entregadores
  - `UploadLicenseCommandHandler`: Upload de CNH para armazenamento
  - `CreateRentalCommandHandler`: Criação de locação com validações
  - `FinalizeRentalCommandHandler`: Finalização de locação com cálculos

- **Query Handlers**:
  - `GetMotorcyclesQueryHandler`: Listagem de motos com filtros
  - `GetMotorcycleByIdQueryHandler`: Busca de moto por ID
  - `GetDriverQueryHandler`: Busca de entregadores
  - `GetRentalsQueryHandler`: Listagem de locações
  - `CalculateReturnValueQueryHandler`: Cálculo de valor para devolução

### 2.3. Domain Layer (Camada de Domínio)

**Responsabilidades:**
- Definir entidades, agregados e objetos de valor
- Implementar lógica de negócio específica de cada entidade
- Definir regras de validação e comportamentos de domínio
- Especificar interfaces de repositórios
- Definir eventos de domínio

**Entidades e Agregados:**
- **Motorcycle**:
  - Atributos: Id, Year, Model, LicensePlate, Status, CreationDate, UpdateDate
  - Comportamentos: UpdateLicensePlate, RentMotorcycle, ReturnMotorcycle, CanBeRemoved
  - Validações: Unicidade da placa, formato da placa, campos obrigatórios

- **Driver**:
  - Atributos: Id, Name, TaxId, BirthDate, LicenseNumber, LicenseType, LicenseImagePath
  - Comportamentos: UpdateLicenseImage, CanRentMotorcycle
  - Validações: TaxId único, LicenseNumber único, maioridade, tipos válidos de CNH

- **Plan**:
  - Atributos: Id, DayCount, DailyRate, PenaltyPercentage
  - Comportamentos: CalculateTotalValue, CalculateEarlyReturnPenalty, CalculateAdditionalDaysValue
  - Entidades pré-definidas: Planos de 7, 15, 30, 45 e 50 dias

- **Rental**:
  - Atributos: Id, MotorcycleId, DriverId, PlanId, StartDate, ExpectedEndDate, EndDate, TotalValue, Status
  - Comportamentos: CalculateReturnValue, FinalizeRental
  - Validações: Habilitação do entregador, disponibilidade da moto

**Eventos de Domínio:**
- `MotorcycleRegistered`: Disparado quando uma nova moto é registrada
- `RentalCreated`: Disparado quando uma nova locação é iniciada
- `RentalFinalized`: Disparado quando uma locação é encerrada

### 2.4. Infrastructure Layer (Camada de Infraestrutura)

**Responsabilidades:**
- Implementar persistência de dados
- Gerenciar integração com serviços da nuvem
- Fornecer serviços de armazenamento de arquivos
- Configurar logging e telemetria
- Permitir adaptabilidade entre ambiente local e GKE

**Componentes Principais:**

#### 2.4.1. Persistência

- **PostgreSQL Repositories**:
  - `MotorcycleRepository`: Implementação de repositório para Motos
  - `DriverRepository`: Implementação de repositório para Entregadores
  - `RentalRepository`: Implementação de repositório para Locações
  - `PlanRepository`: Implementação de repositório para Planos
  - Implementação de Unit of Work para transações
  - Configuração com EntityFramework Core
  - Migrations para evolução do banco de dados
  - Suporte a PostgreSQL em Docker local e Cloud SQL no GKE

#### 2.4.2. Integração com Serviços de Nuvem

- **Messaging Integration**:
  - Abstração para serviços de mensageria através de interfaces
  - Implementação para ambiente local usando RabbitMQ em Docker
  - Implementação para produção usando Google Cloud Pub/Sub
  - Suporte a publicação de eventos para notificações externas

- **Storage Integration**:
  - Abstração para serviços de armazenamento através de interfaces
  - Implementação para ambiente local usando volume Docker ou MinIO
  - Implementação para produção usando Google Cloud Storage
  - Upload, download e remoção de imagens de CNH
  - Geração de URLs pré-assinadas para acesso temporário

#### 2.4.3. Logging e Telemetria

- Implementação de logging estruturado com Serilog
- Configuração para logs em console no ambiente Docker
- Integração com Cloud Logging no ambiente GKE
- Rastreamento de requests com correlationId
- Métricas de performance e uso

## 3. Configuração de Ambientes

### 3.1. Ambiente Docker Local

**Docker Compose**:
```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__PostgreSQL=Host=postgres;Database=vrumm;Username=postgres;Password=postgres
      - Storage__Type=Local
      - Storage__Path=/app/storage
      - Messaging__Type=RabbitMQ
      - Messaging__Host=rabbitmq
    volumes:
      - ./storage:/app/storage
    depends_on:
      - postgres
      - rabbitmq
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:80/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  postgres:
    image: postgres:14
    ports:
      - "5432:5432"
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
      - POSTGRES_DB=vrumm
    volumes:
      - postgres-data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 5s
      timeout: 5s
      retries: 5

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      - RABBITMQ_DEFAULT_USER=guest
      - RABBITMQ_DEFAULT_PASS=guest
    volumes:
      - rabbitmq-data:/var/lib/rabbitmq
    healthcheck:
      test: ["CMD", "rabbitmqctl", "status"]
      interval: 30s
      timeout: 10s
      retries: 3

volumes:
  postgres-data:
  rabbitmq-data:
```

### 3.2. Arquivos YAML para GKE

```
/kubernetes
  dev.yaml     # Configurações completas para ambiente de desenvolvimento
  uat.yaml     # Configurações completas para ambiente de homologação
  prod.yaml    # Configurações completas para ambiente de produção
```

Cada arquivo YAML contém todas as definições necessárias para seu respectivo ambiente, incluindo:

- Deployments para a API e consumidores de mensagens
- Services para expor endpoints
- ConfigMaps para configurações não-sensíveis
- Secrets para dados sensíveis
- Definições de recursos de computação
- Configurações de saúde (health checks)

Exemplo de seção de um arquivo YAML:

```yaml
# Trecho do arquivo dev.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: vrumm-api
  namespace: vrumm-dev
spec:
  replicas: 2
  selector:
    matchLabels:
      app: vrumm-api
  template:
    metadata:
      labels:
        app: vrumm-api
    spec:
      containers:
      - name: api
        image: gcr.io/vrumm-project/vrumm-api:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: Development
        - name: ConnectionStrings__PostgreSQL
          valueFrom:
            secretKeyRef:
              name: vrumm-db-credentials
              key: connection-string
        - name: Storage__Type
          value: GCS
        - name: Storage__BucketName
          value: vrumm-dev-storage
        - name: Messaging__Type
          value: PubSub
        - name: Messaging__ProjectId
          value: vrumm-project
        livenessProbe:
          httpGet:
            path: /live
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /ready
            port: 80
          initialDelaySeconds: 15
          periodSeconds: 5
```

## 4. Fluxos e Integrações Principais

### 4.1. Fluxo de Cadastro de Moto

1. API recebe requisição POST em `/api/motorcycles`
2. Controller valida formato dos dados e cria um `CreateMotorcycleCommand`
3. Mediator encaminha o comando para `CreateMotorcycleCommandHandler`
4. Handler valida regras de negócio (unicidade da placa)
5. Moto é persistida no PostgreSQL
6. Evento `MotorcycleRegistered` é publicado no sistema de mensageria (RabbitMQ local ou Pub/Sub em produção)
7. O sistema de mensageria gerencia a distribuição da notificação para assinantes externos:
   - Notificações por e-mail
   - Notificações por SMS
   - Funções Cloud para processamento adicional (como armazenamento de motos de 2024)
8. API retorna o ID da moto criada com status 201 Created

### 4.2. Fluxo de Upload de CNH

1. API recebe requisição POST em `/api/drivers/{id}/license` com arquivo multipart
2. Controller valida formato do arquivo (png/bmp) e cria um `UploadLicenseCommand`
3. Handler faz upload do arquivo para o armazenamento (local ou Cloud Storage)
4. Caminho do arquivo é armazenado no registro do entregador
5. API retorna status 200 OK com informações do upload

### 4.3. Fluxo de Criação de Locação

1. API recebe requisição POST em `/api/rentals`
2. Controller valida formato dos dados e cria um `CreateRentalCommand`
3. Handler executa validações:
   - Verifica se entregador possui CNH tipo A
   - Verifica se moto está disponível
   - Calcula data de término com base no plano selecionado
4. Locação é persistida no PostgreSQL
5. Status da moto é atualizado para "Alugada"
6. API retorna dados da locação com status 201 Created

### 4.4. Fluxo de Finalização de Locação

1. API recebe requisição PATCH em `/api/rentals/{id}/finalize`
2. Controller valida formato dos dados e cria um `FinalizeRentalCommand`
3. Handler calcula o valor total considerando:
   - Devolução antecipada (com multa)
   - Devolução no prazo (valor normal)
   - Devolução com atraso (valor adicional)
4. Locação é atualizada com valor e status "Finalizada"
5. Status da moto é atualizado para "Disponível"
6. API retorna dados da locação finalizada

## 5. Modelo de Dados

### 5.1. Modelo Relacional (PostgreSQL)

**Tabela Motorcycles**
```
Id (GUID) - PK
Year (INT)
Model (VARCHAR)
LicensePlate (VARCHAR) - UK
Status (ENUM)
CreationDate (TIMESTAMP)
UpdateDate (TIMESTAMP)
```

**Tabela Drivers**
```
Id (GUID) - PK
Name (VARCHAR)
TaxId (VARCHAR) - UK
BirthDate (DATE)
LicenseNumber (VARCHAR) - UK
LicenseType (ENUM)
LicenseImagePath (VARCHAR)
CreationDate (TIMESTAMP)
UpdateDate (TIMESTAMP)
```

**Tabela Plans**
```
Id (INT) - PK
DayCount (INT)
DailyRate (DECIMAL)
PenaltyPercentage (DECIMAL)
```

**Tabela Rentals**
```
Id (GUID) - PK
MotorcycleId (GUID) - FK
DriverId (GUID) - FK
PlanId (INT) - FK
StartDate (DATE)
ExpectedEndDate (DATE)
EndDate (DATE NULL)
TotalValue (DECIMAL NULL)
Status (ENUM)
CreationDate (TIMESTAMP)
UpdateDate (TIMESTAMP)
```

## 6. Padrão de Adaptadores para Ambientes

Para facilitar a transição entre ambiente local e GKE, adotamos o padrão de adaptadores. Este padrão nos permite:

1. **Abstrair Integrações**: Interfaces definem contratos comuns
2. **Implementar Variantes**: Diferentes implementações para cada ambiente
3. **Injetar Implementações**: Baseado na configuração do ambiente

### 6.1. Exemplo: Armazenamento de Arquivos

```
IStorageService (interface)
  ┌─ LocalStorageService (Docker)
  └─ CloudStorageService (GKE)
```

### 6.2. Exemplo: Serviço de Mensageria

```
IMessagePublisher (interface)
  ┌─ RabbitMQPublisher (Docker)
  └─ PubSubPublisher (GKE)
```

A seleção da implementação correta é feita através da configuração de variáveis de ambiente, permitindo que o código 
funcione sem alterações em ambos os ambientes.

## 7. Estratégia de Escalabilidade e Resiliência

### 7.1. Escalabilidade Horizontal

- Aplicação API sem estado, permitindo replicação horizontal
- Auto-scaling baseado em métricas de uso no GKE
- Configuração de limites de recursos por pod

### 7.2. Resiliência

- Retry policies para operações de I/O
- Circuit breaker para integrações externas
- Uso de filas de mensagens para desacoplamento
- Fallbacks para funcionalidades críticas

### 7.3. Monitoramento e Observabilidade

- Logging estruturado com níveis de severidade
- Métricas de saúde e performance
- Rastreamento distribuído (tracing)
- Dashboards operacionais e alertas

## 8. Considerações de Segurança

### 8.1. Segurança de Dados

- Sanitização de entradas
- Parametrização de consultas SQL
- Validação de tipos e formatos
- Criptografia de dados sensíveis

### 8.2. Segurança de Infraestrutura

- IAM Roles com privilégio mínimo
- Segregação de ambientes
- Network policies restritivas
- Scan de imagens Docker

### 8.3. Segurança de Armazenamento

- Políticas de acesso restritivas para armazenamento
- Criptografia em repouso para dados sensíveis
- Acesso temporário via URLs pré-assinadas

## 9. Estratégia de CI/CD

### 9.1. Pipeline de Integração Contínua

- Build e testes automatizados
- Análise de código estático
- Verificação de vulnerabilidades
- Construção de imagens Docker

### 9.2. Pipeline de Deployment Contínuo

- Promoção gradual entre ambientes (dev → uat → prod)
- Deployments blue/green
- Rollback automatizado em caso de falha
- Testes pós-deployment

### 9.3. Ferramentas Sugeridas

- GitHub Actions ou Google Cloud Build
- SonarQube para análise de código
- Trivy para scan de imagens
- Terraform para IaC

## 10. Estratégia de Testes

### 10.1. Testes Unitários

- Testes de domínio focados em regras de negócio
- Testes de serviços de aplicação com mocks
- Cobertura mínima de 80%

### 10.2. Testes de Integração

- Testes de repositórios com banco de dados em container
- Testes de endpoints de API (contract testing)
- Testes de comunicação com serviços externos

### 10.3. Testes de Carga e Performance

- Simulação de carga para picos de uso
- Medição de latência e throughput
- Identificação de gargalos

## 11. Recomendações de Implementação

### 11.1. Tecnologias e Frameworks

- ASP.NET Core 8.0 para API Web
- Entity Framework Core para ORM
- Google Cloud Libraries para .NET
- FluentValidation para validações
- AutoMapper para mapeamentos
- Serilog para logging

### 11.2. Abordagens de Desenvolvimento

- Desenvolvimento orientado a testes (TDD)
- Metodologia ágil com incrementos pequenos
- Documentação contínua (como código)
- Code reviews e pair programming

### 11.3. Padrões de Código

- Código em inglês
- Nomenclatura clara e significativa
- Arquivos e classes menores e focados
- Design patterns consistentes
- Documentação de interfaces públicas