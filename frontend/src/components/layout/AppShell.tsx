import { useState } from "react"
import { NavLink, Outlet } from "react-router-dom"
import {
  Armchair,
  CalendarCheck,
  DoorOpen,
  LayoutDashboard,
  Menu,
  X,
} from "lucide-react"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { standorte } from "@/lib/mock-data"
import { cn } from "@/lib/utils"

const navItems = [
  { to: "/", label: "Übersicht", icon: LayoutDashboard, end: true },
  { to: "/raeume", label: "Raum buchen", icon: DoorOpen, end: false },
  {
    to: "/arbeitsplatz-buchen",
    label: "Arbeitsplatz buchen",
    icon: Armchair,
    end: false,
  },
  {
    to: "/meine-buchungen",
    label: "Meine Buchungen",
    icon: CalendarCheck,
    end: false,
  },
]

function SidebarNav({ onNavigate }: { onNavigate?: () => void }) {
  return (
    <nav className="flex flex-col gap-1 p-3">
      {navItems.map((item) => (
        <NavLink
          key={item.to}
          to={item.to}
          end={item.end}
          onClick={onNavigate}
          className={({ isActive }) =>
            cn(
              "flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors",
              isActive
                ? "bg-sidebar-accent text-sidebar-accent-foreground"
                : "text-sidebar-foreground/70 hover:bg-sidebar-accent/50 hover:text-sidebar-foreground",
            )
          }
        >
          <item.icon className="size-4 shrink-0" />
          {item.label}
        </NavLink>
      ))}
    </nav>
  )
}

export function AppShell() {
  const [mobileOpen, setMobileOpen] = useState(false)

  return (
    <div className="min-h-svh bg-background">
      {/* Header */}
      <header className="sticky top-0 z-30 flex h-14 items-center gap-3 border-b bg-card px-4">
        <button
          type="button"
          aria-label="Menü öffnen"
          onClick={() => setMobileOpen(true)}
          className="inline-flex size-9 items-center justify-center rounded-md hover:bg-accent md:hidden"
        >
          <Menu className="size-5" />
        </button>

        <div className="flex items-center gap-2 font-semibold">
          <span className="grid size-7 place-items-center rounded-md bg-primary text-primary-foreground">
            C
          </span>
          <span>Calvin</span>
        </div>

        <div className="ml-auto flex items-center gap-3">
          <div className="hidden items-center gap-2 sm:flex">
            <span className="text-sm text-muted-foreground">Standort</span>
            <Select
              defaultValue="koeln"
              items={standorte.map((s) => ({ value: s.id, label: s.name }))}
            >
              <SelectTrigger className="w-[140px]" size="sm">
                <SelectValue placeholder="Standort" />
              </SelectTrigger>
              <SelectContent>
                {standorte.map((s) => (
                  <SelectItem key={s.id} value={s.id}>
                    {s.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          <div className="flex items-center gap-2">
            <Avatar className="size-8">
              <AvatarFallback>AB</AvatarFallback>
            </Avatar>
            <span className="hidden text-sm font-medium md:inline">
              Alex Berger
            </span>
          </div>
        </div>
      </header>

      <div className="flex">
        {/* Sidebar — Desktop */}
        <aside className="sticky top-14 hidden h-[calc(100svh-3.5rem)] w-60 shrink-0 border-r bg-sidebar md:block">
          <SidebarNav />
        </aside>

        {/* Sidebar — Mobile (Overlay) */}
        {mobileOpen && (
          <div className="fixed inset-0 z-40 md:hidden">
            <div
              className="absolute inset-0 bg-black/50"
              onClick={() => setMobileOpen(false)}
            />
            <div className="absolute left-0 top-0 h-full w-64 bg-sidebar shadow-lg">
              <div className="flex h-14 items-center justify-between border-b px-4 font-semibold">
                <span>Calvin</span>
                <button
                  type="button"
                  aria-label="Menü schließen"
                  onClick={() => setMobileOpen(false)}
                  className="inline-flex size-9 items-center justify-center rounded-md hover:bg-accent"
                >
                  <X className="size-5" />
                </button>
              </div>
              <SidebarNav onNavigate={() => setMobileOpen(false)} />
            </div>
          </div>
        )}

        {/* Inhalt */}
        <main className="min-w-0 flex-1 p-4 md:p-8">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
