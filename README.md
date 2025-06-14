# HushKey Web

[![Backend CI](https://github.com/Shikhar03Stark/HushKey/actions/workflows/backend.yml/badge.svg)](https://github.com/Shikhar03Stark/HushKey/actions/workflows/backend.yml) [![Frontend CI](https://github.com/Shikhar03Stark/HushKey/actions/workflows/frontend.yml/badge.svg)](https://github.com/Shikhar03Stark/HushKey/actions/workflows/frontend.yml)

**HushKey** is a secure, privacy-first platform for sharing secrets using modern cryptography and a zero-knowledge approach. All encryption and decryption happen in your browser—**the server never sees or stores your secret keys**.

---

## 🚀 Features

- **Zero Knowledge Proof**: The server never has access to your secret key. Only encrypted data is stored.
- **Ephemeral Key Encryption**: The server generates a temporary encryption key to encrypt your secret, destroys the key in memory after encryption, and only stores the encrypted secret.
- **AES-GCM Encryption**: Uses the industry-standard AES-GCM algorithm for strong, authenticated encryption.
- **Fragment-based Key Delivery**: The encryption key (`keyString`) is appended as a fragment in the shareable URL (after `#`). By browser design, this fragment is **never** sent to the server in any HTTP request.
- **One-Time View**: Secrets can be set to self-destruct after being viewed once.
- **Modern Stack**:  
  - Frontend: React + TypeScript + Vite + TailwindCSS  
  - Backend: ASP.NET Core Web API
- **Open Source**: [GitHub Repo](https://github.com/Shikhar03Stark/HushKey/tree/master)

---

## 🛡️ How It Works

1. **Secret Creation**
   - Your browser sends the secret securely to the server over HTTPS.
   - The server generates an **ephemeral encryption key** (using AES-GCM), encrypts your secret, destroys the key in memory, and stores only the encrypted secret.
   - The server returns a reference ID to the encrypted secret, and the `keyString` is appended as a fragment in the shareable URL (e.g.,  
     `https://hushkey.app/secrets/public/abc123#keyString`).

2. **Secret Retrieval**
   - When someone opens the link, their browser extracts the key from the fragment and sends the reference ID to the server to fetch the encrypted secret.
   - The browser decrypts the secret locally using the `keyString`.
   - **The server never sees or stores the keyString**—privacy is preserved by design.

---

## ⚠️ Security Considerations

- **Treat the full URL as sensitive**: Anyone with the full URL (including the fragment) can access and decrypt the secret.
- **No server-side revocation**: If the URL is leaked, there is no way to revoke access (except for one-time view).
- **Client security matters**: If the recipient's device is compromised, secrets can be exposed.
- **HTTPS is required**: Always use the service over a secure connection.

---

## 🤝 Contributing

Contributions, issues, and feature requests are welcome!  
Feel free to open an [issue](https://github.com/Shikhar03Stark/HushKey/issues) or submit a pull request.

---

## 📄 License

This project is [GNU GPLv3 licensed](https://github.com/Shikhar03Stark/HushKey/blob/master/LICENSE).

---