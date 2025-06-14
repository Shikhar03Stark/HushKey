import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { API_ENDPOINTS } from '../../constants';
import Toast from '../common/Toast';

export default function ViewSecretPage() {
	const { secretId } = useParams<{ secretId: string }>();
	const [secret, setSecret] = useState<string | null>(null);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);
	const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

    const validateAndExtractKeys = (): string[] => {
        const location = window.location;
        const fragmet = location.hash;
        if (!fragmet) {
            throw new Error("Secret Url malformed, no key fragments");
        }
        const keyString = fragmet.split('#')[1];
        const [nonce, key, authTag] = keyString.split('.');
        if (!nonce || !key || !authTag) {
            throw new Error(`Secret Url malformed, PartALen: ${nonce ? nonce.length : 0} PartB: ${key ? key.length : 0} PartC: ${authTag ? authTag.length : 0}`);
        }
        return [nonce, key, authTag];
    }

	const base64ToBytes = (base64: string) => Uint8Array.from(atob(base64), c => c.charCodeAt(0));

	const decryptSecret = async (
		encryptedSecretBase64: string,
		nonceB64: string,
		keyB64: string,
		tagB64: string
	): Promise<string> => {
		const nonce = base64ToBytes(nonceB64);
		const key = base64ToBytes(keyB64);
		const tag = base64ToBytes(tagB64);
		const encryptedSecret = base64ToBytes(encryptedSecretBase64);
		const fullCiphertext = new Uint8Array(encryptedSecret.length + tag.length);
		fullCiphertext.set(encryptedSecret);
		fullCiphertext.set(tag, encryptedSecret.length);
		const cryptoKey = await window.crypto.subtle.importKey(
			"raw",
			key,
			"AES-GCM",
			false,
			["decrypt"]
		);
		const decrypted = await window.crypto.subtle.decrypt(
			{
				name: "AES-GCM",
				iv: nonce,
				tagLength: tag.length * 8
			},
			cryptoKey,
			fullCiphertext
		);
		return new TextDecoder().decode(decrypted);
	};

	useEffect(() => {
		async function fetchSecret() {
			if (!secretId) {
				setError('No secret ID provided.');
				setLoading(false);
				return;
			}
			try {
				const [nonce, key, authTag] = validateAndExtractKeys();
				const endpoint = API_ENDPOINTS.getSecret(secretId);
				const response = await fetch(endpoint);
				if (!response.ok) {
					throw new Error('Failed to fetch secret.');
				}
				const data = await response.json();
				if (!data.encryptedSecret) {
					throw new Error('Secret found, but content is empty.');
				}
				const decrypted = await decryptSecret(data.encryptedSecret, nonce, key, authTag);
				setSecret(decrypted);
			} catch (err: any) {
				setError(err.message || 'Unknown error occurred.');
				setToast({ message: 'Failed to fetch or decrypt secret.', type: 'error' });
			} finally {
				setLoading(false);
			}
		}
		fetchSecret();
	}, [secretId]);

	return (
		<div className="min-h-screen flex items-center justify-center bg-gray-950">
			{toast && <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />}
			<div className="bg-gray-900 shadow-2xl rounded-lg p-8 w-full max-w-md border border-gray-800">
				<h1 className="text-2xl font-bold mb-6 text-center text-blue-300">View Secret</h1>
				{loading ? (
					<div className="text-blue-200 text-center">Loading...</div>
				) : error ? (
					<div className="text-red-400 text-center">{error}</div>
				) : (
					<div className="bg-gray-800 text-blue-100 rounded-lg p-4 break-words text-center">
						{secret}
					</div>
				)}
			</div>
		</div>
	);
}
