# Solu��o Backend da Ag�ncia de Viagens

## Vis�o Geral

Este � um projeto backend para uma aplica��o de ag�ncia de viagens desenvolvido com **.NET 8** e **SQL Server**. Ele fornece APIs RESTful para gerenciar autentica��o de usu�rios, pacotes de viagem e integra��es com servi�os de e-mail. O c�digo segue uma arquitetura limpa e est� sendo desenvolvido na branch **dev**.

Este README � a primeira vers�o da documenta��o do projeto, contendo detalhes sobre a estrutura, funcionamento da API e instru��es de configura��o.

## Estrutura do Projeto

O projeto � organizado nas seguintes pastas e arquivos, cada um com uma fun��o espec�fica:

- **Controllers/**: Cont�m os controladores da API que lidam com requisi��es HTTP.
  - `AuthController.cs`: Gerencia endpoints de autentica��o (e.g., `POST /login`, `POST /create-user`).
  - `PackageController.cs`: Gerencia endpoints de busca de pacotes (e.g., `GET /search-packages`).
- **Data/**: Inclui o contexto do banco de dados e configura��es.
  - `ApplicationDBContext.cs`: Contexto do Entity Framework Core para conectar ao banco de dados.
- **DTOs/**: Objetos de Transfer�ncia de Dados (DTOs) para contratos da API.
  - **Requests/**: DTOs para requisi��es.
    - `CreateUserRequestDTO.cs`: Dados para criar um novo usu�rio.
    - `ForgotPasswordRequestDTO.cs`: Dados para solicita��o de redefini��o de senha.
    - `LoginUserRequestDTO.cs`: Dados para login de usu�rio.
    - `NewPasswordRequestDTO.cs`: Dados para nova senha.
    - `PackageSearchRequestDTO.cs`: Dados para busca de pacotes.
  - **Responses/**: DTOs para respostas.
    - `JsonResponseDTO.cs`: Resposta padr�o em JSON com sucesso/mensagem.
- **Migrations/**: Arquivos de migra��o do Entity Framework Core.
  - `20250709125714_InitialIdentityMigration.cs`: Migra��o inicial para tabelas de identidade.
  - `ApplicationDBContextModelSnapshot.cs`: Snapshot do modelo de dados.
- **Models/**: Modelos de entidades que representam tabelas do banco.
  - `User.cs`: Representa um usu�rio com `Id`, `Email` e `PasswordHash`.
  - `AvailablePackages.cs`: Representa pacotes dispon�veis com `Id`, `Destination` e `Price`.
  - `UserPackages.cs`: Representa a rela��o entre usu�rios e pacotes.
- **Repositories/**: Camada de acesso a dados.
  - **Interfaces/**: Defini��es de interfaces.
    - `UserRepository.cs`: Interface para opera��es de reposit�rio de usu�rios.
  - **Services/**: Implementa��es de reposit�rios.
    - `UserRepository.cs`: Implementa opera��es de banco de dados para usu�rios.
- **Services/**: Camada de l�gica de neg�cios.
  - **Interfaces/**: Defini��es de interfaces de servi�os.
    - `IAuthService.cs`: Interface para servi�os de autentica��o.
    - `IUserService.cs`: Interface para servi�os de usu�rio.
  - **Services/**: Implementa��es de servi�os.
    - `AuthService.cs`: L�gica de autentica��o, usando `UserRepository`.
    - `UserService.cs`: L�gica de gerenciamento de usu�rios.
- **Third-party/**: Integra��es com servi�os externos.
  - **Mail/**: Servi�o de e-mail.
    - `EmailService.cs`: Envia e-mails ass�ncronos.
- **Utils/**: Classes utilit�rias reutiliz�veis.
  - `BCryptPasswordHashGenerator.cs`: Gera hashes de senhas usando BCrypt.
- **Properties/**: Configura��es do projeto.
  - `launchSettings.json`: Configura��es de lan�amento local.
- **appsettings.json**: Arquivo de configura��o para strings de conex�o e chaves de API.
- **Program.cs**: Ponto de entrada da aplica��o.
- **README.md**: Este arquivo, com documenta��o do projeto.
- **.gitignore**: Arquivos e diret�rios ignorados pelo controle de vers�o (e.g., `bin/`, `obj/`).

## Como Funciona a API

A API � constru�da com **ASP.NET Core Web API** e segue princ�pios RESTful.

- **Endpoints Exemplo**:
  - `POST /login`: Autentica um usu�rio com `LoginUserRequestDTO`.
  - `POST /register`: Cria um novo usu�rio com `CreateUserRequestDTO`.
  - `GET /search-packages`: Busca pacotes com `PackageSearchRequestDTO`.
- **Fluxo de Requisi��o**:
  1. **Cliente**: Envia uma requisi��o HTTP (e.g., `POST /login` com `LoginUserRequestDTO`).
  2. **Controller**: Valida a requisi��o e chama o servi�o correspondente (e.g., `AuthService`).
  3. **Service**: Executa a l�gica de neg�cios, usando reposit�rios ou servi�os externos (e.g., `EmailService`).
  4. **Repository**: Realiza opera��es no banco via `ApplicationDBContext`.
  5. **Resposta**: Retorna um `JsonResponseDTO` com o resultado ou erro.
- **Inje��o de Depend�ncia**: Gerencia depend�ncias entre controladores, servi�os e reposit�rios.
- **Banco de Dados**: SQL Server, acessado via Entity Framework Core com migra��es.
- **Tratamento de Erros**: Middleware centralizado para respostas consistentes.

## Tecnologias Utilizadas

- **Framework**: .NET 8
- **Banco de Dados**: SQL Server
- **ORM**: Entity Framework Core
- **Controle de Vers�o**: Git (branch dev)
- **IDE**: Visual Studio 2022 ou Rider
- **Ferramentas**: Postman ou Swagger para testes

## Pr�-requisitos

- **SDK .NET 8**: Download
- **SQL Server**: Inst�ncia local (e.g., SQL Server Express) ou na nuvem
- **Git**: Para clonagem e controle de vers�o
- **IDE**: Visual Studio 2022, Rider ou VS Code com extens�es C#

## Instru��es de Configura��o

Para configurar e executar o projeto localmente:

1. **Clone o Reposit�rio**:

   ```bash
   git clone https://github.com/ava-agenciados/travel-agency-back.git
   git checkout dev
   ```

2. **Restaure Depend�ncias**:

   ```bash
   dotnet restore
   ```

3. **Configure o Banco de Dados**:

   - Atualize o `appsettings.json` com a string de conex�o:

     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "Server={server};Database={database};Trusted_Connection=True;"
       }
     }
     ```
   - Aplique as migra��es para criar o esquema:

     ```bash
     dotnet ef database update
     ```

4. **Compile o Projeto**:

   ```bash
   dotnet build
   ```

5. **Execute a Aplica��o**:

   ```bash
   dotnet run
   ```

   A API estar� dispon�vel em `https://localhost:5001` (verifique o `launchSettings.json`).

6. **Teste a API**:

   - Use **Swagger** em `/swagger` (quando a aplica��o estiver rodando).
   - Ou use **Postman** para enviar requisi��es (e.g., `POST https://localhost:5001/login`).

## Fluxo de Desenvolvimento

- **Branch**: Use a branch `dev` para desenvolvimento.
- **Commits**: Use mensagens claras (e.g., `feat: adicionar endpoint de login`, `fix: corrigir erro em AuthService`).
- **Pull Requests**: Envie PRs para `dev` para revis�o.
- **Mudan�as no Banco**: Crie novas migra��es com:

  ```bash
  dotnet ef migrations add <NomeDaMigra��o>
  ```

## Implanta��o

- **Configura��es**: Use arquivos espec�ficos por ambiente (e.g., `appsettings.Production.json`).

## Contribui��o

- Escreva testes unit�rios para servi�os e reposit�rios (usando xUnit).
- Atualize este README para novas funcionalidades.

## Solu��o de Problemas

- **Erros de Banco**: Verifique a string de conex�o e se o SQL Server est� ativo.
- **Problemas de Migra��o**: Confirme as permiss�es do usu�rio no banco.
- **Falhas na API**: Revise os logs no console ou implemente uma classe de logging.

## Pr�ximos Passos

- Adicionar autentica��o JWT para endpoints seguros.
- Implementar mais endpoints (e.g., gerenciamento de pacotes).