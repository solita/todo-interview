export interface Todo {
  id: number
  title: string
  description: string | null
  isDone: boolean
  dueDate: string | null // ISO date string
  createdAt: string
  updatedAt: string | null
}

export interface CreateTodoInput {
  title: string
  description?: string
  dueDate?: string
}

// Shape the frontend sends when editing a ToDo. There is no guarantee yet
// that the backend accepts exactly this - see api.ts -> updateTodo.
export interface UpdateTodoInput {
  title: string
  description?: string
  isDone: boolean
  dueDate?: string
}
