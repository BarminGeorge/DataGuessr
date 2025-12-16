import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import './Animations.css'
import App from './App.tsx'
import { PageProvider } from "./PageContext.tsx";

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <PageProvider>
    <App />
    </PageProvider>
  </StrictMode>
)
