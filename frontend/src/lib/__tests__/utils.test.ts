import { describe, it, expect } from "vitest"
import { cn } from "../utils"

describe("cn()", () => {
  it("returns an empty string when called with no arguments", () => {
    expect(cn()).toBe("")
  })

  it("returns a single class unchanged", () => {
    expect(cn("text-red-500")).toBe("text-red-500")
  })

  it("joins multiple classes", () => {
    expect(cn("flex", "items-center", "gap-4")).toBe("flex items-center gap-4")
  })

  it("ignores falsy values (false, null, undefined)", () => {
    expect(cn("block", false, null, undefined, "mt-2")).toBe("block mt-2")
  })

  it("handles conditional object syntax", () => {
    expect(cn({ "font-bold": true, "font-normal": false })).toBe("font-bold")
  })

  it("merges conflicting Tailwind classes — last one wins", () => {
    // tailwind-merge should resolve conflicts: p-4 overrides p-2
    expect(cn("p-2", "p-4")).toBe("p-4")
  })

  it("merges conflicting text colour — last one wins", () => {
    expect(cn("text-red-500", "text-blue-500")).toBe("text-blue-500")
  })

  it("handles mixed conditional and plain classes", () => {
    const isActive = true
    const isDisabled = false
    expect(cn("base-class", { "active-class": isActive, "disabled-class": isDisabled })).toBe(
      "base-class active-class",
    )
  })

  it("handles arrays of classes", () => {
    expect(cn(["flex", "gap-2"], "mt-4")).toBe("flex gap-2 mt-4")
  })

  it("deduplicates identical classes via tailwind-merge", () => {
    // tailwind-merge keeps the last occurrence of a utility group
    const result = cn("mx-auto", "mx-auto")
    expect(result).toBe("mx-auto")
  })
})
