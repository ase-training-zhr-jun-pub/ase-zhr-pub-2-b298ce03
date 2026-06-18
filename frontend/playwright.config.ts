import { defineConfig, devices } from "@playwright/test"

export default defineConfig({
  testDir: "./e2e",
  fullyParallel: false,
  retries: 0,
  use: {
    baseURL: "http://localhost:5175",
    screenshot: "on",
    trace: "on",
  },
  reporter: [["html", { open: "never" }]],
  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],
  webServer: {
    command: "npm run dev -- --port 5175",
    url: "http://localhost:5175",
    reuseExistingServer: false,
    timeout: 30_000,
    // Proxy-URI deaktivieren, damit Vite base="/" und __BACKEND_URL__="http://localhost:5000" nutzt
    env: { VSCODE_PROXY_URI: "" },
  },
})
