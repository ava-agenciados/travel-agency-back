# Solução Backend da Agência de Viagens

## Visão Geral

Este é um projeto backend para uma aplicação de agência de viagens desenvolvido com **.NET 8** e **SQL Server**. Ele fornece APIs RESTful para gerenciar autenticação de usuários, pacotes de viagem e integrações com serviços de e-mail. O código segue uma arquitetura limpa e está sendo desenvolvido na branch **dev**.

Este README é a primeira versão da documentação do projeto, contendo detalhes sobre a estrutura, funcionamento da API e instruções de configuração.

## Estrutura do Projeto

O projeto é organizado nas seguintes pastas e arquivos, cada um com uma função específica:

- **Controllers/**: Contém os controladores da API que lidam com requisições HTTP.
  - `AuthController.cs`: Gerencia endpoints de autenticação (e.g., `POST /login`, `POST /create-user`).
  - `PackageController.cs`: Gerencia endpoints de busca de pacotes (e.g., `GET /search-packages`).
- **Data/**: Inclui o contexto do banco de dados e configurações.
  - `ApplicationDBContext.cs`: Contexto do Entity Framework Core para conectar ao banco de dados.
- **DTOs/**: Objetos de Transferência de Dados (DTOs) para contratos da API.
  - **Requests/**: DTOs para requisições.
    - `CreateUserRequestDTO.cs`: Dados para criar um novo usuário.
    - `ForgotPasswordRequestDTO.cs`: Dados para solicitação de redefinição de senha.
    - `LoginUserRequestDTO.cs`: Dados para login de usuário.
    - `NewPasswordRequestDTO.cs`: Dados para nova senha.
    - `PackageSearchRequestDTO.cs`: Dados para busca de pacotes.
  - **Responses/**: DTOs para respostas.
    - `JsonResponseDTO.cs`: Resposta padrão em JSON com sucesso/mensagem.
- **Migrations/**: Arquivos de migração do Entity Framework Core.
  - `20250709125714_InitialIdentityMigration.cs`: Migração inicial para tabelas de identidade.
  - `ApplicationDBContextModelSnapshot.cs`: Snapshot do modelo de dados.
- **Models/**: Modelos de entidades que representam tabelas do banco.
  - `User.cs`: Representa um usuário com `Id`, `Email` e `PasswordHash`.
  - `AvailablePackages.cs`: Representa pacotes disponíveis com `Id`, `Destination` e `Price`.
  - `UserPackages.cs`: Representa a relação entre usuários e pacotes.
- **Repositories/**: Camada de acesso a dados.
  - **Interfaces/**: Definições de interfaces.
    - `UserRepository.cs`: Interface para operações de repositório de usuários.
  - **Services/**: Implementações de repositórios.
    - `UserRepository.cs`: Implementa operações de banco de dados para usuários.
- **Services/**: Camada de lógica de negócios.
  - **Interfaces/**: Definições de interfaces de serviços.
    - `IAuthService.cs`: Interface para serviços de autenticação.
    - `IUserService.cs`: Interface para serviços de usuário.
  - **Services/**: Implementações de serviços.
    - `AuthService.cs`: Lógica de autenticação, usando `UserRepository`.
    - `UserService.cs`: Lógica de gerenciamento de usuários.
- **Third-party/**: Integrações com serviços externos.
  - **Mail/**: Serviço de e-mail.
    - `EmailService.cs`: Envia e-mails assíncronos.
- **Utils/**: Classes utilitárias reutilizáveis.
  - `BCryptPasswordHashGenerator.cs`: Gera hashes de senhas usando BCrypt.
- **Properties/**: Configurações do projeto.
  - `launchSettings.json`: Configurações de lançamento local.
- **appsettings.json**: Arquivo de configuração para strings de conexão e chaves de API.
- **Program.cs**: Ponto de entrada da aplicação.
- **README.md**: Este arquivo, com documentação do projeto.
- **.gitignore**: Arquivos e diretórios ignorados pelo controle de versão (e.g., `bin/`, `obj/`).

## Como Funciona a API

A API é construída com **ASP.NET Core Web API** e segue princípios RESTful.

- **Endpoints Exemplo**:
  - `POST /login`: Autentica um usuário com `LoginUserRequestDTO`.
  - `POST /register`: Cria um novo usuário com `CreateUserRequestDTO`.
  - `GET /search-packages`: Busca pacotes com `PackageSearchRequestDTO`.
- **Fluxo de Requisição**:
  1. **Cliente**: Envia uma requisição HTTP (e.g., `POST /login` com `LoginUserRequestDTO`).
  2. **Controller**: Valida a requisição e chama o serviço correspondente (e.g., `AuthService`).
  3. **Service**: Executa a lógica de negócios, usando repositórios ou serviços externos (e.g., `EmailService`).
  4. **Repository**: Realiza operações no banco via `ApplicationDBContext`.
  5. **Resposta**: Retorna um `JsonResponseDTO` com o resultado ou erro.
- **Injeção de Dependência**: Gerencia dependências entre controladores, serviços e repositórios.
- **Banco de Dados**: SQL Server, acessado via Entity Framework Core com migrações.
- **Tratamento de Erros**: Middleware centralizado para respostas consistentes.

## Tecnologias Utilizadas

- **Framework**: .NET 8
- **Banco de Dados**: SQL Server
- **ORM**: Entity Framework Core
- **Controle de Versão**: Git (branch dev)
- **IDE**: Visual Studio 2022 ou Rider
- **Ferramentas**: Postman ou Swagger para testes

## Pré-requisitos

- **SDK .NET 8**: Download
- **SQL Server**: Instância local (e.g., SQL Server Express) ou na nuvem
- **Git**: Para clonagem e controle de versão
- **IDE**: Visual Studio 2022, Rider ou VS Code com extensões C#

## Instruções de Configuração

Para configurar e executar o projeto localmente:

1. **Clone o Repositório**:

   ```bash
   git clone https://github.com/ava-agenciados/travel-agency-back.git
   git checkout dev
   ```

2. **Restaure Dependências**:

   ```bash
   dotnet restore
   ```

3. **Configure o Banco de Dados**:

   - Atualize o `appsettings.json` com a string de conexão:

     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Server={server};Database={database};Trusted_Connection=True;"
       }
     }
     ```
   - Aplique as migrações para criar o esquema:

     ```bash
     dotnet ef database update
     ```

4. **Compile o Projeto**:

   ```bash
   dotnet build
   ```

5. **Execute a Aplicação**:

   ```bash
   dotnet run
   ```

   A API estará disponível em `https://localhost:5001` (verifique o `launchSettings.json`).

6. **Teste a API**:

   - Use **Swagger** em `/swagger` (quando a aplicação estiver rodando).
   - Ou use **Postman** para enviar requisições (e.g., `POST https://localhost:5001/login`).

## Fluxo de Desenvolvimento

- **Branch**: Use a branch `dev` para desenvolvimento.
- **Commits**: Use mensagens claras (e.g., `feat: adicionar endpoint de login`, `fix: corrigir erro em AuthService`).
- **Pull Requests**: Envie PRs para `dev` para revisão.
- **Mudanças no Banco**: Crie novas migrações com:

  ```bash
  dotnet ef migrations add <NomeDaMigração>
  ```

## Implantação

- **Configurações**: Use arquivos específicos por ambiente (e.g., `appsettings.Production.json`).

## Contribuição

- Escreva testes unitários para serviços e repositórios (usando xUnit).
- Atualize este README para novas funcionalidades.

## Solução de Problemas

- **Erros de Banco**: Verifique a string de conexão e se o SQL Server está ativo.
- **Problemas de Migração**: Confirme as permissões do usuário no banco.
- **Falhas na API**: Revise os logs no console ou implemente uma classe de logging.

## Próximos Passos

- Adicionar autenticação JWT para endpoints seguros.
- Implementar mais endpoints (e.g., gerenciamento de pacotes).