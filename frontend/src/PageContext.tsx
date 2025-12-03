
import { createContext, useContext, useState } from "react";

type Page = "home" | "login" | "room" | "registration" | "profile";

interface PageContextValue {
  page: Page;
  setPage: (p: Page) => void;
}

const PageContext = createContext<PageContextValue | null>(null);

export function PageProvider({ children }: { children: React.ReactNode }) {
  const [page, setPage] = useState<Page>("home");

  return (
    <PageContext.Provider value={{ page, setPage }}>
      {children}
    </PageContext.Provider>
  );
}

export function usePage() {
  const ctx = useContext(PageContext);
  if (!ctx) throw new Error("usePage must be used inside <PageProvider />");
  return ctx;
}
