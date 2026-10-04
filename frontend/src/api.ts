import type { CreateTodoInput, Todo, UpdateTodoInput } from './types'

const BASE_URL = '/api/todos'

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const body = await res.text().catch(() => '')
    throw new Error(`Request failed (${res.status}): ${body || res.statusText}`)
  }
  // 204 No Content has no body to parse
  if (res.status === 204) {
    return undefined as T
  }
  return res.json() as Promise<T>
}

export function getTodos(includeCompleted: boolean = true): Promise<Todo[]> {
  const url = `${BASE_URL}?includeCompleted=${includeCompleted}`
  return fetch(url).then((res) => handleResponse<Todo[]>(res))
}

export function addTodo(input: CreateTodoInput): Promise<Todo> {
  return fetch(BASE_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(input)
  }).then((res) => handleResponse<Todo>(res))
}

export function deleteTodo(id: number): Promise<void> {
  return fetch(`${BASE_URL}/${id}`, { method: 'DELETE' }).then((res) =>
    handleResponse<void>(res)
  )
}

// NOTE: this calls PUT /api/todos/{id}, which does not exist on the backend
// yet - implementing it is the interview task. This is a reasonable default
// contract (verb + body shape) but it's fine for the candidate to change it,
// as long as this call is updated to match.
export function updateTodo(id: number, input: UpdateTodoInput): Promise<Todo> {
  return fetch(`${BASE_URL}/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(input)
  }).then((res) => handleResponse<Todo>(res))
}
