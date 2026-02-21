```markdown
# IoT Concurseiro Web Backend

Backend responsável por disponibilizar dados de **Pause Mirror** para consumo da aplicação Web do módulo Concurseiro.

API desenvolvida em **.NET (ASP.NET Core)** com foco em:

- Leitura de dados (read-only)
- Alta performance
- Baixa complexidade arquitetural
- Código simples e direto
- SQL otimizado com Dapper
- Paginação para uso em produção

---

# 📌 Arquitetura

O projeto segue uma arquitetura simplificada inspirada em Clean Architecture, porém reduzida para o cenário atual (read-only).

Estrutura:

```

Peneluc.Concurseiro.Web.Backend
│
├── Api
│   ├── Controllers
│   ├── HealthChecks
│   └── Program.cs
│
├── Domain
│   └── Entities
│
└── Infrastructure
├── Common
├── Interfaces
├── Data
└── Repositories

```

---

# 🧱 Camadas

## 1️⃣ Api

Responsável por:

- Exposição dos endpoints HTTP
- Validação de parâmetros
- Paginação
- HealthCheck
- Injeção de dependência

### Controller principal

`PauseMirrorController`

Fluxo:

```

HTTP Request
↓
Controller
↓
Repository
↓
PostgreSQL

```

Não utiliza MediatR.
Não possui camada Application.
Comunicação direta Controller → Repository.

---

## 2️⃣ Domain

Contém apenas:

- Entidades do domínio (modelo de leitura)

Exemplo:

```

PauseMirrorEntity

````

Sem regras de negócio complexas.
Sem agregados.
Sem eventos.
Sem commands.

---

## 3️⃣ Infrastructure

Responsável por:

- Acesso ao banco
- SQL otimizado
- Paginação
- Conexão PostgreSQL

### Componentes principais

- `PostgresConnectionFactory`
- `IPauseMirrorReadRepository`
- `PauseMirrorReadRepository`
- `PagedResult<T>`

Utiliza **Dapper** para performance máxima.

---

# 🗄 Banco de Dados

- PostgreSQL
- Consulta baseada em:
  - asset_id
  - intervalo de datas (formato inteiro YYYYMMDD)
- Ordenação otimizada
- Paginação via OFFSET / LIMIT

Exemplo lógico da consulta:

```sql
SELECT ...
FROM pause_mirror
WHERE asset_id = @assetId
  AND data BETWEEN @startDate AND @endDate
ORDER BY data DESC
LIMIT @pageSize OFFSET @offset;
````

---

# 📅 Filtro de Datas

O endpoint permite filtros flexíveis:

| Parâmetro | Descrição                            |
| --------- | ------------------------------------ |
| year      | Ano obrigatório para filtro temporal |
| month     | Opcional                             |
| day       | Opcional                             |

Casos suportados:

* Apenas ano → filtra o ano inteiro
* Ano + mês → filtra o mês inteiro
* Ano + mês + dia → filtra dia específico

Validações aplicadas:

* Ano entre 2000 e 2100
* Mês entre 1 e 12
* Dia válido conforme calendário
* Datas convertidas para formato inteiro YYYYMMDD

---

# 📄 Paginação

Paginação padrão:

```
?page=1&pageSize=50
```

Resposta:

```json
{
  "data": [],
  "page": 1,
  "pageSize": 50,
  "totalRecords": 1234,
  "totalPages": 25
}
```

Implementada através de `PagedResult<T>`.

Recomendado para ambiente produtivo.

---

# ❤️ Health Check

Endpoint de verificação de saúde do banco:

```
/health
```

Valida conectividade com PostgreSQL.

---

# 🚀 Tecnologias Utilizadas

* .NET 8/9/10
* ASP.NET Core
* Dapper
* PostgreSQL
* Dependency Injection nativa
* HealthChecks

---

# 🔐 Características Arquiteturais

✔ Read-only
✔ Sem MediatR
✔ Sem Application Layer
✔ Sem Behaviors
✔ Sem Commands
✔ Sem CQRS completo
✔ Simples e performático

Projetado para:

* BFF (Backend for Frontend)
* APIs de consulta
* Alta velocidade
* Baixa complexidade

---

# 📦 Como Executar

1. Configurar `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=...;Port=...;Database=...;Username=...;Password=..."
  }
}
```

2. Rodar:

```bash
dotnet restore
dotnet build
dotnet run --project Peneluc.Concurseiro.Web.Backend.Api
```

---

# 📈 Evolução Futura

Caso o projeto cresça, pode-se evoluir para:

* Reintroduzir camada Application
* Adicionar Commands
* Adicionar MediatR
* Implementar CQRS completo
* Introduzir testes unitários por camada
* Adicionar cache (Redis)
* Adicionar observabilidade (OpenTelemetry)

---

# 🎯 Objetivo do Projeto

Disponibilizar dados de Pause Mirror de forma:

* Rápida
* Escalável
* Simples de manter
* Fácil de evoluir

---

# 📌 Padrões Aplicados

* Repository Pattern
* Separation of Concerns
* Dependency Injection
* Clean layering simplificada

---

# 🧠 Decisão Arquitetural

Optou-se por remover:

* MediatR
* Behaviors
* Application Layer

Motivo:

O sistema é exclusivamente read-only com apenas uma entidade e uma tabela.

Complexidade adicional não traria ganho arquitetural real neste cenário.

---

# 🏁 Status

Projeto pronto para produção em cenário read-only de média/alta carga.

```
```
