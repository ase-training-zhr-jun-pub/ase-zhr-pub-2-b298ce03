/// <reference types="vitest" />
import path from "path"
import tailwindcss from "@tailwindcss/vite"
import react from "@vitejs/plugin-react"
import { defineConfig } from "vite"

export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    host: "0.0.0.0",
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  define: {
    __BACKEND_URL__: JSON.stringify(""),
  },
  test: {
    environment: "jsdom",
    include: ["src/**/*.test.ts?(x)"],
    globals: true,
    setupFiles: ["src/test-setup.ts"],
  },
})
