import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      // Forward API calls to the ASP.NET Core backend during development
      // so the frontend can just call fetch('/api/...') with no CORS setup
      // needed on this side (backend still has CORS enabled as a fallback).
      '/api': {
        target: 'http://localhost:5203',
        changeOrigin: true
      }
    }
  }
})
