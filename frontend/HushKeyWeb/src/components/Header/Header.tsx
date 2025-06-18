import { NAVIGATION } from '../../constants';
import { Link, useLocation } from 'react-router-dom';
import { useState } from 'react';

export default function Header() {
  const [menuOpen, setMenuOpen] = useState(false);
  const location = useLocation();

  return (
    <header className="w-full bg-gray-900 border-b border-gray-800 py-4 shadow-sm">
      <div className="container mx-auto px-2 sm:px-4 flex items-center justify-between relative min-h-[48px]">
        {/* Logo on left */}
        <h1 className="text-2xl font-extrabold text-blue-300 tracking-wide drop-shadow-sm select-none flex-shrink-0">
          HushKey{' '}
          <span className="text-base font-semibold text-blue-500 align-super ml-2 hidden xs:inline">
            share secrets securely
          </span>
        </h1>
        {/* GitHub icon in center */}
        <div className="absolute left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 flex items-center">
          <a
            href="https://github.com/Shikhar03Stark/HushKey/tree/master"
            target="_blank"
            rel="noopener noreferrer"
            className="flex items-center gap-2 hover:text-blue-400 transition-colors flex-shrink-0"
            title="View on GitHub"
          >
            <svg height="24" width="24" viewBox="0 0 16 16" fill="#fff" aria-hidden="true">
              <path d="M8 0C3.58 0 0 3.58 0 8c0 3.54 2.29 6.53 5.47 7.59.4.07.55-.17.55-.38 0-.19-.01-.82-.01-1.49-2.01.37-2.53-.49-2.69-.94-.09-.23-.48-.94-.82-1.13-.28-.15-.68-.52-.01-.53.63-.01 1.08.58 1.23.82.72 1.21 1.87.87 2.33.66.07-.52.28-.87.51-1.07-1.78-.2-3.64-.89-3.64-3.95 0-.87.31-1.59.82-2.15-.08-.2-.36-1.02.08-2.12 0 0 .67-.21 2.2.82a7.65 7.65 0 0 1 2-.27c.68 0 1.36.09 2 .27 1.53-1.04 2.2-.82 2.2-.82.44 1.1.16 1.92.08 2.12.51.56.82 1.27.82 2.15 0 3.07-1.87 3.75-3.65 3.95.29.25.54.73.54 1.48 0 1.07-.01 1.93-.01 2.19 0 .21.15.46.55.38A8.013 8.013 0 0 0 16 8c0-4.42-3.58-8-8-8z"/>
            </svg>
            <span className="sr-only">GitHub</span>
          </a>
        </div>
        {/* Mobile menu icon and nav on right */}
        <div className="flex items-center">
          {/* Desktop nav */}
          <nav className="hidden sm:flex items-center gap-4 flex-1 justify-end">
            {NAVIGATION.filter(item => item.showInHeader).map(item => (
              <Link
                key={item.path}
                to={item.path}
                className={
                  'text-blue-200 hover:text-blue-400 font-semibold transition-colors' +
                  (location.pathname === item.path ? ' underline' : '')
                }
              >
                {item.label}
              </Link>
            ))}
          </nav>
          {/* Mobile menu icon */}
          <button
            className="sm:hidden flex items-center justify-center p-2 ml-2 text-blue-200 hover:text-blue-400 focus:outline-none"
            aria-label="Open menu"
            onClick={() => setMenuOpen((v) => !v)}
          >
            <svg width="28" height="28" fill="none" viewBox="0 0 24 24">
              <path stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>
        </div>
        {/* Mobile nav drawer */}
        {menuOpen && (
          <div className="absolute top-full right-0 w-48 bg-gray-900 border-b border-gray-800 z-50 flex flex-col items-stretch sm:hidden animate-fade-in shadow-lg rounded-b-lg">
            {NAVIGATION.filter(item => item.showInHeader).map(item => (
              <Link
                key={item.path}
                to={item.path}
                className={
                  'px-4 py-3 text-blue-200 hover:text-blue-400 font-semibold border-t border-gray-800' +
                  (location.pathname === item.path ? ' underline' : '')
                }
                onClick={() => setMenuOpen(false)}
              >
                {item.label}
              </Link>
            ))}
          </div>
        )}
      </div>
    </header>
  );
}
