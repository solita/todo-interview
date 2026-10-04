# ToDo interview project

A small full-stack ToDo app: an ASP.NET Core Web API backend with an
in-memory database, and a React + TypeScript frontend.

Currently, the code contains one bug and one missing feature, which you are expected to fix and implement.

## Structure

```
backend/    ASP.NET Core Web API (.NET 10), EF Core InMemory provider
frontend/   React + TypeScript (Vite)
```

## Getting started

Requires the .NET 10 SDK installed (`dotnet --version` should report a `10.x` SDK).

### Backend

From `backend/TodoApi` (or open `backend/TodoApi.sln` in Rider and run):

```
cd backend/TodoApi
dotnet restore
dotnet run
```

Runs on `http://localhost:5203`, with Swagger UI at `http://localhost:5203/swagger`.
Data is in-memory only — it resets every time the app restarts.

### Frontend

From `frontend`:

```
cd frontend
npm install
npm run dev
```

Runs on `http://localhost:5173` and proxies `/api/*` calls to the backend
(see `frontend/vite.config.ts`), so no CORS setup is needed to use it.
