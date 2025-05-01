# Documentação Técnica – Camada Domain

## 1. Visão Geral da Camada

### Propósito
A camada **Domain** representa o núcleo central da aplicação Vrumm.
Seu papel é encapsular alógica de negócio de forma isolada e independente de tecnologia,
frameworks e infraestrutura externa.
Ela expressa de forma explícita as regras do domínio, garantindo integridade,
consistência e validação desde a raiz do sistema.

### Comunicação com outras camadas
- **Entradas:** Recebe instruções da camada de Application (casos de uso).
- **Saídas:** Expõe modelos de domínio (Entidades e Objetos de Valor),
contratos (interfaces de repositórios) e eventos de domínio que podem ser
consumidos por outras camadas, como Application e Infrastructure.

### Padrões e princípios aplicados
- **Domain-Driven Design (DDD)**: Foco em modelar o domínio com conceitos reais do negócio.
- **Clean Architecture**: A camada é independente de UI e infraestrutura.
- **SOLID**: Princípios aplicados como:
  - SRP (Single Responsibility Principle): Cada classe tem um papel bem definido.
  - OCP (Open/Closed Principle): Comportamentos podem ser estendidos sem alterar classes existentes.
  - DIP (Dependency Inversion Principle): Comunicação via interfaces, não implementações.
- **Separation of Concerns**: Domínio desacoplado de infraestrutura e lógica de aplicação.

---

## 2. Componentes Principais

### a. Entidades (Entities)
- **`Rental`**: Regras de locação de motos, cálculo de valores, status, geração de eventos.
- **`Driver`**: Representa o entregador, com CNH, CNPJ, idade e imagem da habilitação.
- **`Motorcycle`**: Moto cadastrada, com status, modelo, ano e placa.
- **`Plan`**: Condições e regras financeiras do aluguel (dias, tarifas, multas).
- **`User`**: Dados de autenticação e controle de acesso.
- **`MotorcycleRegistrationEvent`**: Histórico de registro da moto.

### b. Objetos de Valor (Value Objects)
- **`Cnpj`**, **`LicenseNumber`**, **`LicensePlate`**, **`BirthDate`**, **`ManufactureYear`**,
**`MotorcycleModel`**, **`LicenseTypeValue`**: Garantem imutabilidade e validação encapsulada.

### c. Eventos de Domínio
- **`RentalCreated`**, **`RentalFinalized`**, **`MotorcycleRegistered`**: Representam fatos
importantes do negócio, permitindo reações desacopladas.

### d. Repositórios (Interfaces)
- Contratos como `IRentalRepository`, `IDriverRepository`, `IMotorcycleRepository` etc.
permitem abstrair o acesso ao armazenamento de dados.

### e. Exceções Especializadas
- Agrupadas por contexto (Drivers, Motorcycles, Auth). Garantem clareza e controle de erros
de negócio.

### f. Opções de Configuração
- `JwtOptions`, `PlanOptions`, `MotorcycleRegistrationOptions`, entre outros, encapsulam
configurações externas consumidas pelas camadas superiores.

---

## 3. Fluxo de Execução

### Exemplo: Criação de uma Locação
1. A camada de Application solicita a criação via `Rental.CreateNextDayRental(...)`.
2. A entidade `Rental` aplica validações (ex: datas, IDs válidos).
3. Um evento `RentalCreated` pode ser gerado com os dados da nova locação.
4. A camada de Application salva a entidade via `IRentalRepository.AddAsync()`.
5. Eventos são publicados e controlados em camadas superiores.

### Transições de Estado
- `Motorcycle` altera seu status via `Rent`, `Return`, `SetUnderMaintenance` ou
`SetInactive`, sempre validando a operação com regras explícitas.

---

## 4. Dependências

### Bibliotecas Internas
- `System`, `System.Linq`, `System.Text.RegularExpressions`, `System.Collections.Generic`:
Operações básicas, manipulação de coleções e expressões regulares.

### Nenhuma dependência externa
- A camada Domain é completamente isolada de infraestrutura, frameworks de ORM ou serviços
de terceiros.

---

## 5. Boas Práticas e Decisões de Projeto

### Encapsulamento forte
- Toda lógica de negócio reside nas entidades e objetos de valor.
- Regras de consistência não estão espalhadas, evitando duplicidade.

### Eventos de Domínio
- Favorecem desacoplamento e reatividade.
- Ex: `Rental.GenerateFinalizedEvent()` só ocorre se regras forem atendidas.

### Value Objects com validações internas
- Ex: `BirthDate.Create(value)` valida idade mínima de 18 anos e limites históricos.

### Repositórios via interfaces
- Isolam persistência, facilitando testes e substituição da implementação.

---

## 6. Pontos de Extensão e Manutenção

### Extensões
- Novos tipos de planos: adicionar ao `PlanOptions` e criar nova fábrica.
- Regras de elegibilidade para motoristas: extensão do método `CanRentMotorcycle()`.
- Novos eventos de domínio: ampliar integração com serviços externos.

### Riscos
- Validações muito acopladas podem dificultar reuso.
- Evite lógica de aplicação (como envio de email) dentro do domínio.

---

## 7. Exemplos

### Exemplo: Validação e Criação de CNH
```csharp
var license = LicenseNumber.Create("1234567890");
```

### Exemplo: Criação de uma Locação com Plano
```csharp
var rental = Rental.CreateNextDayRental(motoId, driverId, plan.Id, DateTime.UtcNow, plan);
```

### Exemplo: Evento de Finalização
```csharp
var finalizedEvent = rental.GenerateFinalizedEvent();
```

---

## Conclusão
A camada Domain do sistema Vrumm está sólida, bem estruturada e aderente aos princípios da
Clean Architecture e DDD. Sua independência, foco nas regras de negócio e clareza nas
responsabilidades facilitam manutenção, testes e evolução do sistema com segurança e coesão.