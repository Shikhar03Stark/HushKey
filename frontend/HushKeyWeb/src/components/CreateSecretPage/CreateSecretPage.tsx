import { useState } from 'react';
import { API_ENDPOINTS, MAX_SECRET_LENGTH } from '../../constants';
import type { CreateSecretRequest, CreateSecretResponse } from './types';
import Toast from '../common/Toast';

function SharePopup({ url, onClose }: { url: string; onClose: () => void }) {
	const [copied, setCopied] = useState(false);
	const handleCopy = async () => {
		try {
			await navigator.clipboard.writeText(url);
			setCopied(true);
			setTimeout(() => setCopied(false), 2000);
		} catch {
			setCopied(false);
		}
	};
	return (
		<div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-60">
			<div className="bg-gray-900 border border-gray-700 rounded-lg shadow-xl p-6 w-full max-w-md">
				<h2 className="text-lg font-bold text-blue-300 mb-4 text-center">Share this Secret Link</h2>
				<div className="flex items-center bg-gray-800 rounded px-3 py-2 mb-4">
					<input
						className="flex-1 bg-transparent text-blue-200 text-sm outline-none"
						value={url}
						readOnly
						onFocus={e => e.target.select()}
					/>
					<button
						onClick={handleCopy}
						className="ml-2 px-3 py-1 bg-blue-700 hover:bg-blue-800 text-white rounded text-sm font-semibold transition-colors"
					>
						{copied ? 'Copied!' : 'Copy'}
					</button>
				</div>
				<div className="mb-4 text-blue-200 text-sm text-center italic">
					Paste this link in your browser's address bar to view the secret.
				</div>
				<button
					onClick={onClose}
					className="w-full mt-2 py-2 bg-gray-700 hover:bg-gray-800 text-gray-200 rounded font-semibold"
				>
					Close
				</button>
			</div>
		</div>
	);
}

export default function CreateSecretPage() {
	const [secret, setSecret] = useState('');
	const [ttl, setTtl] = useState(3600); // default 1 hour in seconds
	const [submitted, setSubmitted] = useState(false);
	const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);
	const [shareUrl, setShareUrl] = useState<string | null>(null);

	const MIN_TTL = 3600; // 1 hour in seconds
	const MAX_TTL = 86400; // 24 hours in seconds

	const showToast = (message: string, type: 'success' | 'error') => {
		setToast({ message, type });
		setTimeout(() => setToast(null), 3500);
	};

	const ensureUIDomain = (url: string): string => {
		const urlObj = new URL(url);
		if(urlObj.hostname !== window.location.hostname){
			const msg = "UI Host does not match uiLink Host"
			console.error(msg);
			throw Error(msg);
		}
		if (urlObj.hostname === "localhost" && urlObj.port !== window.location.port){
			// set to ui port for development scenario
			urlObj.port = window.location.port;
            urlObj.protocol = window.location.protocol;
		}
		return urlObj.toString();
	}

	const handleSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		const requestBody: CreateSecretRequest = {
			secretText: secret,
			ttl: ttl,
		};
		try {
			const endpoint = API_ENDPOINTS.createSecret();
			const response = await fetch(endpoint, {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify(requestBody),
			});
			if (response.status !== 201) {
				console.error(`Response status does not indicate Create: ${response.status}`);
				throw new Error('Response status is not success');
			}
			const data = await response.json() as CreateSecretResponse;
			const shareableLink = ensureUIDomain(data.uiLink);
			setShareUrl(shareableLink);
			showToast('Secret submitted successfully!', 'success');
			setSubmitted(true);

		} catch (error) {
			showToast('Failed to submit secret.', 'error');
		}
	};

	return (
		<div className="min-h-screen flex items-center justify-center bg-gray-950">
			{shareUrl !== null && <SharePopup url={shareUrl} onClose={() => setShareUrl(null)} />}
			{toast && <Toast message={toast.message} type={toast.type} onClose={() => setToast(null)} />}
			<div className="bg-gray-900 shadow-2xl rounded-lg p-8 w-full max-w-md border border-gray-800">
				<h1 className="text-2xl font-bold mb-6 text-center text-blue-300">Create a Secret</h1>
				<form onSubmit={handleSubmit} className="space-y-4">
					<label className="block text-gray-300 font-medium">Secret</label>
					<textarea
						className="w-full p-3 border border-gray-800 bg-gray-800 text-blue-100 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none min-h-[100px] placeholder:text-gray-500"
						value={secret}
						onChange={e => {
							if (e.target.value.length <= MAX_SECRET_LENGTH) setSecret(e.target.value);
						}}
						placeholder="Enter your secret here..."
						required
						maxLength={MAX_SECRET_LENGTH}
					/>
					<div className="text-right text-xs text-gray-400">
						{secret.length} / {MAX_SECRET_LENGTH} characters
					</div>
					<label className="block text-gray-300 font-medium mt-4">Time to Live (TTL)</label>
					<input
						type="range"
						min={MIN_TTL}
						max={MAX_TTL}
						step={3600}
						value={ttl}
						onChange={e => setTtl(Number(e.target.value))}
						className="w-full accent-blue-700"
					/>
					<div className="text-xs text-gray-400 text-right mb-2">
						{Math.floor(ttl / 3600)} hour{Math.floor(ttl / 3600) > 1 ? 's' : ''}
					</div>
					<button
						type="submit"
						className="w-full bg-blue-700 hover:bg-blue-800 text-white font-semibold py-2 px-4 rounded-lg transition-colors"
					>
						Save Secret
					</button>
				</form>
				{submitted && (
					<div className="mt-4 text-green-400 text-center font-medium">Secret submitted!</div>
				)}
			</div>
		</div>
	);
}
