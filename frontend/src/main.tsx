import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
import { BrowserRouter } from "react-router-dom"
import "./index.css"
import App from "./App"

// Hinter dem Proxy läuft die App unter einem Unterpfad (import.meta.env.BASE_URL).
// React Router muss diesen Prefix als basename kennen, sonst matchen die Routen nicht.
const basename = import.meta.env.BASE_URL.replace(/\/$/, "") || "/"

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <BrowserRouter basename={basename}>
      <App />
    </BrowserRouter>
  </StrictMode>,
)
