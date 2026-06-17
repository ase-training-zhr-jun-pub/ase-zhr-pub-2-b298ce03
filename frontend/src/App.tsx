import { Route, Routes } from "react-router-dom"
import { AppShell } from "@/components/layout/AppShell"
import { Uebersicht } from "@/pages/Uebersicht"
import { RaumBuchen } from "@/pages/RaumBuchen"
import { ArbeitsplatzBuchen } from "@/pages/ArbeitsplatzBuchen"
import { MeineBuchungen } from "@/pages/MeineBuchungen"

function App() {
  return (
    <Routes>
      <Route element={<AppShell />}>
        <Route index element={<Uebersicht />} />
        <Route path="raum-buchen" element={<RaumBuchen />} />
        <Route path="arbeitsplatz-buchen" element={<ArbeitsplatzBuchen />} />
        <Route path="meine-buchungen" element={<MeineBuchungen />} />
      </Route>
    </Routes>
  )
}

export default App
