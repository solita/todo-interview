import { useEffect, useState, type FormEvent } from 'react'
import { addTodo, deleteTodo, getTodos, updateTodo } from './api'
import type { Todo } from './types'
import './App.css'

function formatDate(value: string | null): string {
  if (!value) return '—'
  const d = new Date(value)
  return Number.isNaN(d.getTime()) ? '—' : d.toLocaleString()
}

function toDateInputValue(value: string | null): string {
  if (!value) return ''
  const d = new Date(value)
  if (Number.isNaN(d.getTime())) return ''
  return d.toISOString().slice(0, 10)
}

export default function App() {
  const [todos, setTodos] = useState<Todo[]>([])
  const [loading, setLoading] = useState(true)
  const [loadError, setLoadError] = useState<string | null>(null)
  const [showCompleted, setShowCompleted] = useState(true)

  // Add form state
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [dueDate, setDueDate] = useState('')
  const [addError, setAddError] = useState<string | null>(null)
  const [adding, setAdding] = useState(false)

  // Edit state
  const [editingId, setEditingId] = useState<number | null>(null)
  const [editTitle, setEditTitle] = useState('')
  const [editDescription, setEditDescription] = useState('')
  const [editIsDone, setEditIsDone] = useState(false)
  const [editDueDate, setEditDueDate] = useState('')
  const [editError, setEditError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    refreshTodos()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [showCompleted])

  function refreshTodos() {
    setLoading(true)
    setLoadError(null)
    getTodos(showCompleted)
      .then(setTodos)
      .catch((err: Error) => setLoadError(err.message))
      .finally(() => setLoading(false))
  }

  async function handleAddSubmit(e: FormEvent) {
    e.preventDefault()
    if (!title.trim()) {
      setAddError('Title is required')
      return
    }

    setAdding(true)
    setAddError(null)
    try {
      const created = await addTodo({
        title: title.trim(),
        description: description.trim() || undefined,
        dueDate: dueDate || undefined
      })
      setTodos((prev) => [...prev, created])
      setTitle('')
      setDescription('')
      setDueDate('')
    } catch (err) {
      setAddError(err instanceof Error ? err.message : 'Failed to add ToDo')
    } finally {
      setAdding(false)
    }
  }

  async function handleDelete(id: number) {
    try {
      await deleteTodo(id)
      setTodos((prev) => prev.filter((t) => t.id !== id))
    } catch (err) {
      setLoadError(err instanceof Error ? err.message : 'Failed to delete ToDo')
    }
  }

  async function handleToggleDone(todo: Todo) {
    try {
      const updated = await updateTodo(todo.id, {
        title: todo.title,
        description: todo.description ?? undefined,
        isDone: !todo.isDone,
        dueDate: todo.dueDate ?? undefined
      })
      setTodos((prev) =>
        showCompleted || !updated.isDone
          ? prev.map((t) => (t.id === todo.id ? updated : t))
          : prev.filter((t) => t.id !== todo.id)
      )
    } catch (err) {
      setLoadError(err instanceof Error ? err.message : 'Failed to update ToDo')
    }
  }

  function startEdit(todo: Todo) {
    setEditingId(todo.id)
    setEditTitle(todo.title)
    setEditDescription(todo.description ?? '')
    setEditIsDone(todo.isDone)
    setEditDueDate(toDateInputValue(todo.dueDate))
    setEditError(null)
  }

  function cancelEdit() {
    setEditingId(null)
    setEditError(null)
  }

  async function handleEditSubmit(e: FormEvent, id: number) {
    e.preventDefault()
    if (!editTitle.trim()) {
      setEditError('Title is required')
      return
    }

    setSaving(true)
    setEditError(null)
    try {
      const updated = await updateTodo(id, {
        title: editTitle.trim(),
        description: editDescription.trim() || undefined,
        isDone: editIsDone,
        dueDate: editDueDate || undefined
      })
      setTodos((prev) => prev.map((t) => (t.id === id ? updated : t)))
      setEditingId(null)
    } catch (err) {
      // Expected to fail with a 404/405 until the Update endpoint exists.
      setEditError(
        err instanceof Error
          ? `${err.message} (the Update endpoint may not be implemented yet)`
          : 'Failed to save changes'
      )
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="app">
      <h1>ToDo</h1>

      <form className="add-form" onSubmit={handleAddSubmit}>
        <div className="field-row">
          <input
            type="text"
            placeholder="Title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
          />
          <input
            type="text"
            placeholder="Description (optional)"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
          <input
            type="date"
            value={dueDate}
            onChange={(e) => setDueDate(e.target.value)}
          />
          <button type="submit" disabled={adding}>
            {adding ? 'Adding…' : 'Add'}
          </button>
        </div>
        {addError && <p className="error">{addError}</p>}
      </form>

      {loading && <p>Loading…</p>}
      {loadError && <p className="error">{loadError}</p>}

      <div className="filter-row">
        <label className="checkbox-label">
          <input
            type="checkbox"
            checked={showCompleted}
            onChange={(e) => setShowCompleted(e.target.checked)}
          />
          Show completed
        </label>
      </div>

      <ul className="todo-list">
        {todos.map((todo) => (
          <li key={todo.id} className="todo-item">
            {editingId === todo.id ? (
              <form className="edit-form" onSubmit={(e) => handleEditSubmit(e, todo.id)}>
                <div className="field-row">
                  <input
                    type="text"
                    value={editTitle}
                    onChange={(e) => setEditTitle(e.target.value)}
                  />
                  <input
                    type="text"
                    value={editDescription}
                    onChange={(e) => setEditDescription(e.target.value)}
                  />
                  <input
                    type="date"
                    value={editDueDate}
                    onChange={(e) => setEditDueDate(e.target.value)}
                  />
                  <label className="checkbox-label">
                    <input
                      type="checkbox"
                      checked={editIsDone}
                      onChange={(e) => setEditIsDone(e.target.checked)}
                    />
                    Done
                  </label>
                </div>
                <div className="button-row">
                  <button type="submit" disabled={saving}>
                    {saving ? 'Saving…' : 'Save'}
                  </button>
                  <button type="button" onClick={cancelEdit} disabled={saving}>
                    Cancel
                  </button>
                </div>
                {editError && <p className="error">{editError}</p>}
              </form>
            ) : (
              <>
                <div className="todo-main">
                  <input
                    type="checkbox"
                    checked={todo.isDone}
                    onChange={() => handleToggleDone(todo)}
                  />
                  <div className="todo-text">
                    <span className={todo.isDone ? 'title done' : 'title'}>
                      {todo.title}
                    </span>
                    {todo.description && (
                      <span className="description">{todo.description}</span>
                    )}
                    <span className="meta">
                      Due: {formatDate(todo.dueDate)} · Created: {formatDate(todo.createdAt)}
                      {todo.updatedAt && <> · Updated: {formatDate(todo.updatedAt)}</>}
                    </span>
                  </div>
                </div>
                <div className="button-row">
                  <button type="button" onClick={() => startEdit(todo)}>
                    Edit
                  </button>
                  <button type="button" onClick={() => handleDelete(todo.id)}>
                    Delete
                  </button>
                </div>
              </>
            )}
          </li>
        ))}
      </ul>

      {!loading && todos.length === 0 && <p>No ToDos yet — add one above.</p>}
    </div>
  )
}
