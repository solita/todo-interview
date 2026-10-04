# ToDo interview project

A small full-stack ToDo app used as a live coding exercise. Add and Delete
are already implemented; the candidate implements Update.

## Structure

```
backend/    ASP.NET Core Web API (.NET 10), EF Core InMemory provider
frontend/   React + TypeScript (Vite)
```

Requires the .NET 10 SDK installed (`dotnet --version` should report a `10.x` SDK).

## Running it

**Backend** (from `backend/TodoApi`, or open `backend/TodoApi.sln` in Rider and run):

```
cd backend/TodoApi
dotnet restore
dotnet run
```

Runs on `http://localhost:5203`, with Swagger UI at `http://localhost:5203/swagger`.
Data is in-memory only — it resets every time the app restarts.

**Frontend** (from `frontend`):

```
cd frontend
npm install
npm run dev
```

Runs on `http://localhost:5173` and proxies `/api/*` calls to the backend
(see `frontend/vite.config.ts`), so no CORS setup is needed to use it — CORS
is enabled on the backend too, as a fallback (e.g. for calling it directly
from Swagger or a different port).

Start the backend first, then the frontend.

## What's already implemented

- `GET /api/todos` — list all
- `GET /api/todos/{id}` — get one
- `POST /api/todos` — add a ToDo (`{ title, description?, dueDate? }`)
- `DELETE /api/todos/{id}` — delete a ToDo
- Frontend: list view, add form, delete button — all working end to end.
- Frontend: an **Edit** button that opens an inline edit form and calls
  `PUT /api/todos/{id}`. This call will fail (404) until the backend
  endpoint exists — that's expected, and the UI shows a friendly error
  rather than crashing.

## The task

The candidate implements the **Update** endpoint on the backend
(`backend/TodoApi/Controllers/TodosController.cs` has a `CANDIDATE TASK`
comment marking where). It's deliberately open-ended — there's no stub
signature to fill in. Things worth having them talk through while they work,
or asking about afterward:

- HTTP verb and route: `PUT` (full replace) vs `PATCH` (partial update), and why.
- Request DTO: which fields should be editable? Should `Title` still be
  required/validated? Should `CreatedAt` ever be touched (it shouldn't)?
- What should happen to `UpdatedAt`?
- Status codes: 200 with the updated resource, 404 for a missing id, 400/422
  for invalid input — do they cover these deliberately or only the happy path?
- Do they touch EF Core correctly (fetch tracked entity, mutate, `SaveChangesAsync`)
  vs. e.g. trying to `Update()` a detached object incorrectly?
- The frontend already calls `PUT /api/todos/{id}` with
  `{ title, description, isDone, dueDate }` (see `frontend/src/api.ts`,
  `updateTodo`). A reasonable candidate answer is to just implement against
  that; a stronger signal is if they notice this, articulate the trade-off
  vs. PATCH, and either match it deliberately or update the frontend call to
  match their own chosen contract.

## Notes

- EF Core is configured with `UseInMemoryDatabase`, not a real database —
  no connection string or migrations needed, and it's easy to reset by just
  restarting the backend.
- This was scaffolded without network access to actually run `dotnet build`
  / `npm install` in the environment that generated it, so give both a spin
  once before running an interview with it, in case a package version needs
  a small bump. This applies especially to the NuGet package versions in
  `TodoApi.csproj` (`Microsoft.EntityFrameworkCore.InMemory` and
  `Swashbuckle.AspNetCore`, pinned for .NET 10) — I couldn't reach NuGet to
  confirm the exact latest versions, so `dotnet restore` may need you to bump
  them if it can't resolve what's pinned.
