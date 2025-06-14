// Types for Create Secret API

export interface CreateSecretRequest {
  secretText: string;
  ttl?: number; // Time to live in seconds (optional)
}

export interface CreateSecretResponse {
  uiLink: string; // UI link to view the secret
  serviceShareableLink: string; // API link to fetch the secret
  expiresAt?: string; // ISO date string or undefined if no expiration
}
