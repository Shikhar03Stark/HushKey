// src/constants.ts
// Centralized API endpoint constants for HushKey frontend

// Use NODE_ENV to determine if in development mode
const isDev =   import.meta.env.DEV;
export const API_BASE_URL = isDev ? "https://localhost:5001/api" : "https://hushkeyapi.devitvish.in/api";

export const MAX_SECRET_LENGTH = 512;

export const API_ENDPOINTS = {
  createSecret: () => `${API_BASE_URL}/secrets/public`,
  getSecret: (secretId: string) => `${API_BASE_URL}/secrets/public/${secretId}`,
  // Add more endpoints as needed, always as functions
};

export interface NavigationItem {
  path: string;
  component: React.ComponentType<any>;
  label: string;
  showInHeader: boolean;
}

import App from './components/HomePage/App';
import CreateSecretPage from './components/CreateSecretPage/CreateSecretPage';
import ViewSecretPage from './components/ViewSecretPage/ViewSecretPage';

export const NAVIGATION: NavigationItem[] = [
  {
    path: '/',
    component: App,
    label: 'Home',
    showInHeader: true,
  },
  {
    path: '/secrets/public',
    component: CreateSecretPage,
    label: 'Create Secret',
    showInHeader: true,
  },
  {
    path: '/secrets/public/:secretId',
    component: ViewSecretPage,
    label: 'View Secret',
    showInHeader: false,
  },
];
