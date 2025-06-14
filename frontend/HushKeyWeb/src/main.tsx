import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import './index.css'
import Header from './components/Header/Header.tsx'
import { NAVIGATION } from './constants'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <Header />
      <Routes>
        {NAVIGATION.map(item => (
          <Route key={item.path} path={item.path} element={<item.component />} />
        ))}
      </Routes>
    </BrowserRouter>
  </StrictMode>,
)
