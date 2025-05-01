# Documentação Técnica – Camada Application

## 1. Visão Geral da Camada

A camada `Application` é o coração da arquitetura, responsável por implementar a lógica de negócio da aplicação. Ela orquestra a interação entre diferentes partes do sistema, define os casos de uso e valida as requisições. [cite: 1, 2, 3, 4, 5]

-   **Propósito:** O propósito principal desta camada é conter a lógica de negócio da aplicação. Ela define como o sistema deve se comportar em resposta a diferentes entradas e eventos. [cite: 1, 2, 3, 4, 5]
-   **Comunicação:** A camada `Application` recebe requisições da camada de `Presentation` (ou API) e retorna respostas para ela. Ela se comunica com a camada `Domain` para acessar e manipular entidades de domínio e com a camada `Infrastructure` para acessar dados e serviços externos. [cite: 1, 2, 3, 4, 5]
-   **Padrões e Princípios:**
    -   **Clean Architecture:** A camada segue os princípios da Clean Architecture, mantendo a lógica de negócio independente de frameworks e detalhes de infraestrutura.
    -   **DDD (Domain-Driven Design):** Utiliza conceitos de DDD para modelar o domínio da aplicação, com foco nas entidades e casos de uso.
    -   **SOLID:** Aplica princípios SOLID como Single Responsibility Principle (SRP), Dependency Inversion Principle (DIP) e outros para promover um código coeso e de baixo acoplamento.
    -   **Separação de Responsabilidades:** Cada componente na camada tem uma responsabilidade bem definida, facilitando a manutenção e evolução do sistema. [cite: 468, 469, 470, 471, 472, 473]

## 2. Componentes Principais

A camada `Application` é composta por vários componentes que colaboram para implementar a lógica de negócio:

-   **Commands e Handlers:**
    -   **Commands:** Representam as ações que o sistema pode executar (ex: `CreateDriverCommand`, `CreateRentalCommand`). Eles são objetos que encapsulam os dados necessários para realizar a ação. [cite: 102, 103, 104, 105, 106, 107]
    -   **Handlers:** São responsáveis por executar a lógica associada a um Command. Cada Command tem um Handler correspondente (ex: `CreateDriverCommandHandler`, `CreateRentalCommandHandler`). Os Handlers interagem com o domínio e a infraestrutura para realizar a ação. [cite: 102, 103, 104, 105, 106, 107]
-   **Queries e Handlers:**
    -   **Queries:** Representam as solicitações de dados do sistema (ex: `GetDriversQuery`, `GetRentalsQuery`). [cite: 112, 113, 114, 115]
    -   **Handlers:** São responsáveis por recuperar os dados solicitados pelas Queries. [cite: 112, 113, 114, 115]
-   **DTOs (Data Transfer Objects):** Objetos simples usados para transferir dados entre a camada `Application` e outras camadas. Eles evitam expor as entidades de domínio diretamente. [cite: 137, 138, 139, 140, 141]
-   **Interfaces:** Definem contratos para os componentes da camada, promovendo o baixo acoplamento e a possibilidade de substituir implementações.  Exemplos: `ICommandDispatcher`, `IQueryDispatcher`, `ITokenService`, `IUserService`. [cite: 103, 104, 105, 106, 107]
-   **Validators:** Responsáveis por validar os Commands e Queries antes de serem processados, garantindo a integridade dos dados de entrada. [cite: 555, 556, 557, 558, 559, 560, 561, 562]
-   **Behaviors:** Implementam comportamentos transversais, como logging, validação, tratamento de transações e performance. Eles são aplicados através de um pipeline de execução. [cite: 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534]
-   **CommandBus e Dispatchers:**
    -   **CommandBus:** Atua como um mediador para enviar Commands e Queries para seus respectivos Handlers. [cite: 562, 563, 564, 565, 566, 567, 568, 569, 570, 571, 572]
    -   **CommandDispatcher e QueryDispatcher:** São responsáveis por localizar e executar os Handlers corretos. [cite: 573, 574, 575, 576, 577, 578, 579, 580, 581, 582, 583, 584, 585, 586]
-   **Events e Handlers:**
    -   **Events:** Representam algo que aconteceu no domínio (ex: `MotorcycleRegisteredEvent`, `RentalCreatedEvent`). [cite: 469, 470, 4, 12, 13]
    -   **Handlers:** Executam ações em resposta aos Events (ex: enviar notificações, atualizar outros agregados). [cite: 476, 477, 478, 479, 480, 481]
-   **Policies:** Definem regras de negócio e restrições (ex: `MotorcycleRegistrationPolicy`). [cite: 477]
-   **Factories:** Criam instâncias de objetos complexos, encapsulando a lógica de criação (ex: `PlanFactory`). [cite: 483]
-   **Services:** Implementam lógicas específicas, como autenticação e geração de tokens (ex: `JwtTokenService`, `UserService`). [cite: 474, 475]

## 3. Fluxo de Execução

Um caso de uso típico na camada `Application` segue o seguinte fluxo:

1.  A camada `Presentation` (ou API) recebe uma requisição e a transforma em um `Command` ou `Query`.
2.  O `Command` ou `Query` é enviado para o `CommandBus` ou `QueryDispatcher`.
3.  O `CommandBus` ou `QueryDispatcher` localiza o `Handler` correspondente ao `Command` ou `Query`.
4.  Antes de executar o `Handler`, um pipeline de `Behaviors` é executado. Esse pipeline pode incluir:
    -   **Validação:** O `Command` ou `Query` é validado usando os `Validators` configurados.
    -   **Logging:** A requisição é registrada para fins de auditoria e diagnóstico.
    -   **Transação:** Uma transação é iniciada (se necessário) para garantir a consistência dos dados.
    -   **Performance:** O tempo de execução da requisição é medido para monitorar o desempenho.
    -   **Autorização:** Verifica se o usuário tem permissão para executar a ação.
5.  O `Handler` é executado, realizando a lógica de negócio:
    -   Interage com o `Domain` para manipular as entidades.
    -   Acessa a `Infrastructure` para persistir os dados ou interagir com serviços externos.
    -   Publica `Events` se necessário.
6.  O `Handler` retorna o resultado para o `CommandBus` ou `QueryDispatcher`.
7.  Se uma transação foi iniciada, ela é comitada. Em caso de erro, é feito rollback.
8.  O resultado é retornado para a camada `Presentation`.

## 4. Dependências

A camada `Application` possui as seguintes dependências externas relevantes:

-   **FluentValidation:** Utilizada para definir regras de validação para Commands e Queries. [cite: 1, 468, 469, 470, 471, 472, 473]
-   **Microsoft.Extensions.DependencyInjection:** Utilizada para Injeção de Dependência (DI), permitindo que os componentes da camada sejam facilmente configurados e desacoplados. [cite: 1, 468, 469, 470, 471, 472, 473]
-   **Microsoft.Extensions.Logging:** Utilizada para registrar logs e informações de diagnóstico. [cite: 1, 468, 469, 470, 471, 472, 473]
-   **Microsoft.Extensions.Options:** Utilizada para acessar configurações da aplicação. [cite: 1, 468, 469, 470, 471, 472, 473]
-   **Microsoft.AspNetCore.Identity:** Utilizada para recursos de identidade, como hash de senhas. [cite: 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47]

## 5. Boas Práticas e Decisões de Projeto

-   **Injeção de Dependência (DI):** A camada utiliza DI extensivamente para promover o baixo acoplamento e a testabilidade. Os componentes são injetados via construtor, permitindo a substituição fácil de implementações. [cite: 468, 469, 470, 471, 472, 473]
-   **Uso de Interfaces:** Interfaces são amplamente utilizadas para definir contratos entre os componentes, facilitando a manutenção e a extensão do sistema. [cite: 103, 104, 105, 106, 107]
-   **Pipeline de Behaviors:** A implementação de comportamentos transversais através de um pipeline permite adicionar ou remover funcionalidades sem modificar o código dos Handlers. [cite: 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534]
-   **Exceções Customizadas:** A camada define exceções customizadas para representar erros específicos da aplicação, facilitando o tratamento de erros pelas camadas superiores. [cite: 586, 587, 588, 589, 590, 99, 100, 101]
-   **CQRS (Command and Query Responsibility Segregation):** Embora não implementado de forma estrita, a separação entre Commands e Queries indica uma intenção de aderir aos princípios do CQRS, melhorando a escalabilidade e a performance. [cite: 102, 103, 104, 105, 106, 107]

## 6. Pontos de Extensão e Manutenção

-   **Adicionar novos casos de uso:** Novos casos de uso podem ser adicionados criando novos `Commands` e `Handlers`.
-   **Estender a validação:** Novas regras de validação podem ser adicionadas aos `Validators`.
-   **Adicionar novos comportamentos:** Novos `Behaviors` podem ser adicionados ao pipeline para implementar funcionalidades transversais adicionais.
-   **Modificar a lógica de negócio:** A lógica de negócio pode ser modificada nos `Handlers`.
-   **Riscos:**
    -   Aumento da complexidade se o pipeline de `Behaviors` se tornar muito grande.
    -   Acoplamento excessivo dos `Handlers` com a camada `Infrastructure` pode dificultar os testes e a manutenção.

## 7. Exemplos

**Exemplo: Criar um novo Driver**

1.  A API recebe uma requisição para criar um novo Driver.
2.  A API cria um `CreateDriverCommand` com os dados da requisição.
3.  A API envia o `CreateDriverCommand` para o `CommandDispatcher`.
4.  O `CommandDispatcher` envia o `Command` para o `CommandBus`.
5.  O `CommandBus` localiza o `CreateDriverCommandHandler`.
6.  O pipeline de `Behaviors` é executado:
    -   O `CreateDriverCommand` é validado usando o `CreateDriverCommandValidator`.
    -   A requisição é logada.
    -   Uma transação é iniciada.
7.  O `CreateDriverCommandHandler` é executado:
    -   Cria uma nova entidade `Driver` no `Domain`.
    -   Persiste o `Driver` no banco de dados usando a `Infrastructure`.
    -   Publica um `DriverCreatedEvent` (se necessário).
8.  A transação é comitada.
9.  O `CreateDriverCommandHandler` retorna o ID do novo Driver para a API.

Este documento fornece uma visão geral detalhada da camada `Application`, seus componentes, fluxo de execução e decisões de projeto. Ele deve ser útil para entender, manter e estender essa camada no futuro.