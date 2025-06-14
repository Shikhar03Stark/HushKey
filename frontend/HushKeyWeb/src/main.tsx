import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import './index.css'
import App from './components/HomePage/App.tsx'
import CreateSecretPage from './components/CreateSecretPage/CreateSecretPage.tsx'
import Header from './components/Header/Header.tsx'
import ViewSecretPage from './components/ViewSecretPage/ViewSecretPage.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <Header />
      <Routes>
        <Route path="/" element={<App />} />
        <Route path="/secrets/public" element={<CreateSecretPage />} />
        <Route path='/secrets/public/:secretId' element={<ViewSecretPage />} />
      </Routes>
    </BrowserRouter>
  </StrictMode>,
)
