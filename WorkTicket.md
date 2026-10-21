# Todo API Work Ticket

## Summary

Improve the existing Todo API solution by completing two tasks:

1. Fix a reported bug.
2. Implement an update endpoint for todo items.

The solution already exists, and all work should be done within the current codebase.

## Background

The sample solution consists of a React frontend and a .NET backend. The backend exposes API endpoints for managing todo items.

The API is missing an endpoint for updating an existing todo item. There is also an unrelated bug in the current solution that needs to be resolved.

## Task 1: Fix the Existing Bug

### Problem

Identify and fix the bug in the current solution.

### Expectations

- Identify the root cause of the issue.
- Implement a fix in the existing codebase.
- Ensure the fix works as expected.
- Be prepared to explain your debugging process and reasoning.

### Notes

- Solve this part without AI assistance.
- Use the existing code, debugger, logs, tests, and documentation or other resources, as you would in a real development setting.

## Task 2: Implement a Todo Update Endpoint

### Problem

The backend API does not currently support updating an existing todo item. Implement an endpoint with this route:

`api/todos/update/{id}`

### Expectations

- Preferably use a test-driven development (TDD) approach.
- Update an existing todo item by ID.
- Perform appropriate validation and return the updated item when the request succeeds.
- Add appropriate test cases for the new endpoint.
- Be prepared to explain your design decisions and reasoning.

### Notes

- Use AI for this part, and produce at least part of the solution by prompting an agent.
- Be prepared to discuss your prompts and explain your API design choices.

## Deliverables

Update the existing backend solution so that:

1. The bug is resolved.
2. Todo items can be updated through a backend API endpoint.

Be prepared to discuss:

- What you changed and why.
- Any assumptions you made.
- Improvements you would make with more time.
- Backend design choices you would make to prepare this demo application for production.

