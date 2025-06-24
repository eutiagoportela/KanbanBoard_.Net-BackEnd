# 🚀 KANBAN BOARD - BACKEND API

Uma API RESTful completa para gerenciamento de tarefas no estilo Kanban, construída com **.NET 8**, **Entity Framework Core** e **PostgreSQL**. Projetada com **Clean Architecture**, autenticação JWT e pronta para produção.

---

## 📋 FUNCIONALIDADES

* ✅ CRUD completo de tarefas
* ✅ Kanban com 3 colunas (A Fazer, Em Progresso, Concluído)
* ✅ Sistema de autenticação JWT
* ✅ Controle de permissões por usuário
* ✅ Endpoint específico para drag & drop
* ✅ Busca e filtros de tarefas
* ✅ Data de vencimento com flag de vencida
* ✅ Sistema de ordenação das tarefas
* ✅ Clean Architecture completa
* ✅ Documentação Swagger automática

---

## 🏗️ ARQUITETURA DO PROJETO

```
Kanban/
├── Dominio/          # Entidades, Enums, Interfaces
├── Aplicacao/        # Use Cases, AutoMapper
├── Comunicacao/      # DTOs (Requests/Responses)
├── Infraestrutura/   # Repositórios, DbContext, Security
├── Exceptions/       # Exceções customizadas
└── Api/              # Controllers, Extensions, Startup
```

---

## 🎯 ENDPOINTS

### 🔐 Autenticação

* `POST /api/auth/login` → Login
* `POST /api/usuario` → Registro

### 📋 Tarefas (Kanban)

* `GET /api/tarefa` → Listar tarefas
* `GET /api/tarefa/kanban` → Board organizado
* `GET /api/tarefa/{id}` → Buscar por ID
* `POST /api/tarefa` → Criar tarefa
* `PUT /api/tarefa/{id}` → Atualizar tarefa
* `PATCH /api/tarefa/{id}/mover` → Mover (Drag & Drop)
* `DELETE /api/tarefa/{id}` → Excluir tarefa

### 🔍 Filtros

* `GET /api/tarefa?termoBusca=texto` → Buscar por texto
* `GET /api/tarefa?status=1` → Filtrar por status

---

## 🛠️ TECNOLOGIAS UTILIZADAS

* .NET 8
* Entity Framework Core
* PostgreSQL
* AutoMapper
* BCrypt
* JWT Bearer
* Swagger
* Docker

---

## 🚀 COMO EXECUTAR LOCALMENTE

### 1. Pré-requisitos

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Docker](https://docs.docker.com/get-docker/)

### 2. Clonar o projeto

```bash
git clone <repo-url>
cd kanban-backend
```

### 3. Subir banco de dados PostgreSQL

```bash
cd src/Kanban.Api
docker-compose up -d
```

### 4. Aplicar Migrações

```bash
dotnet ef migrations add InitialCreate --project ../Kanban.Infraestrutura --startup-project .
dotnet ef database update --project ../Kanban.Infraestrutura --startup-project .
```

### 5. Rodar o projeto

```bash
dotnet run
```

### 6. Acessar Aplicação

* Swagger UI: `http://localhost:5000`
* API Base: `http://localhost:5000/api`
* PostgreSQL: `localhost:5432`

---

## 🔑 CREDENCIAIS DE TESTE

| Email                                       | Senha  | Nome          |
| ------------------------------------------- | ------ | ------------- |
| [admin@kanban.com](mailto:admin@kanban.com) | 123456 | Administrador |
| [joao@kanban.com](mailto:joao@kanban.com)   | 123456 | João Silva    |

---

## 📱 EXEMPLOS DE USO VIA CURL

### 1. Login

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "admin@kanban.com", "senha": "123456"}'
```

### 2. Obter Kanban Board

```bash
curl -X GET http://localhost:5000/api/tarefa/kanban \
  -H "Authorization: Bearer SEU_TOKEN"
```

### 3. Criar Nova Tarefa

```bash
curl -X POST http://localhost:5000/api/tarefa \
  -H "Authorization: Bearer SEU_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"titulo": "Nova tarefa", "descricao": "Descrição", "status": 1, "dataVencimento": "2024-12-31T10:00:00Z"}'
```

### 4. Mover Tarefa

```bash
curl -X PATCH http://localhost:5000/api/tarefa/1/mover \
  -H "Authorization: Bearer SEU_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"novoStatus": 2, "novaOrdem": 1}'
```

---

## 🐳 DOCKER

### Subir apenas o banco de dados PostgreSQL

```bash
docker-compose up -d
```

## 📁 ESTRUTURA DA SOLUÇÃO COMPLETA (6 PROJETOS) - COM USE CASE USUÁRIO

```plaintext
📦 Solution 'Solution' (6 de 6 projetos)
├── 📁 src/
│   ├── 📁 Kanban.Api/                          # 🎯 Camada de Apresentação (API)
│   │   ├── 📁 Controllers/
│   │   │   ├── 📄 AuthController.cs            # POST /api/auth/login (com validação ModelState)
│   │   │   ├── 📄 TarefaController.cs          # CRUD + Kanban + Drag&Drop (com validação ModelState)
│   │   │   └── 📄 UsuarioController.cs         # ✅ POST /api/usuario (FUNCIONANDO - Use Case implementado)
│   │   ├── 📁 Extensions/
│   │   │   └── 📄 DependencyInjectionExtensions.cs # ✅ DI (COM CriarUsuarioUseCase registrado)
│   │   ├── 📁 Middleware/
│   │   │   └── 📄 GlobalExceptionMiddleware.cs # ✅ Tratamento global (COM lista de erros)
│   │   ├── 📁 Properties/
│   │   │   └── 📄 launchSettings.json
│   │   ├── 📄 Program.cs
│   │   ├── 📄 Kanban.Api.csproj
│   │   ├── 📄 appsettings.json
│   │   ├── 📄 appsettings.Development.json
│   │   └── 📄 docker-compose.yml
│
│   ├── 📁 Kanban.Aplicacao/                    # ⚙️ Camada de Aplicação
│   │   ├── 📁 UseCases/
│   │   │   ├── 📁 Tarefa/
│   │   │   ├── 📁 Usuario/
│   │   │   │   └── 📁 CriarUsuario/
│   │   │   │       ├── 📄 ICriarUsuarioUseCase.cs
│   │   │   │       └── 📄 CriarUsuarioUseCase.cs
│   │   │   └── 📁 Login/
│   │   ├── 📁 AutoMapper/
│   │   │   └── 📄 MappingProfile.cs
│   │   └── 📄 Kanban.Aplicacao.csproj
│
│   ├── 📁 Kanban.Comunicacao/                  # 📨 Camada de Comunicação
│   │   ├── 📁 Requests/
│   │   ├── 📁 Responses/
│   │   ├── 📁 DTOs/
│   │   │   └── 📄 RespostaPadrao.cs
│   │   └── 📄 Kanban.Comunicacao.csproj
│
│   ├── 📁 Kanban.Dominio/                      # 🧠 Camada de Domínio
│   │   ├── 📁 Entidades/
│   │   │   ├── 📄 Usuarios.cs
│   │   │   └── 📄 Tarefas.cs
│   │   ├── 📁 Enum/
│   │   │   └── 📄 StatusTarefa.cs
│   │   ├── 📁 Repositorios/
│   │   │   └── 📁 Interfaces/
│   │   │       ├── 📄 IUsuarioRepository.cs
│   │   │       └── 📄 ITarefaRepository.cs
│   │   └── 📄 Kanban.Dominio.csproj
│
│   ├── 📁 Kanban.Exceptions/                   # ❌ Exceções Customizadas
│   │   ├── 📄 EmailJaExisteException.cs
│   │   ├── 📄 ValidacaoException.cs
│   │   ├── 📄 DatabaseException.cs
│   │   └── 📄 Kanban.Exceptions.csproj
│
│   └── 📁 Kanban.Infraestrutura/               # 🔧 Camada de Infraestrutura
│       ├── 📁 Repositorios/
│       ├── 📁 Security/
│       ├── 📄 KanbanDbContext.cs
│       ├── 📄 PostgreSqlConfig.cs
│       └── 📄 Kanban.Infraestrutura.csproj
│
├── 📁 tests/
│   └── 📁 Kanban.tests/
│       ├── 📁 UseCases/
│       │   └── 📁 Usuario/
│       │       └── 📄 CriarUsuarioUseCaseTests.cs
│       └── 📄 Kanban.tests.csproj
│
├── 📄 Solution.sln
├── 📄 .gitignore
├── 📄 .env
├── 📄 README.md
├── 📄 setup.ps1
└── 📄 setup.sh




🎯 Projeto pronto para integração com qualquer frontend React, Vue, Angular ou mobile!
