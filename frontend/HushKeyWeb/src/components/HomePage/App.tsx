import { useEffect, useState } from 'react';

function markdownToHtml(md: string): string {
  // Simple markdown to HTML (headings, lists, code, links, paragraphs)
  return md
    .replace(/^# (.*$)/gim, '<h1>$1</h1>')
    .replace(/^## (.*$)/gim, '<h2>$1</h2>')
    .replace(/^### (.*$)/gim, '<h3>$1</h3>')
    .replace(/^- (.*$)/gim, '<li>$1</li>')
    .replace(/\n<li>/gim, '<ul><li>')
    .replace(/<li>(.*?)<\/li>/gim, '<li>$1</li>')
    .replace(/<li>(.*?)<\/li>(?!<li>)/gim, '$&</ul>')
    .replace(/\[(.*?)\]\((.*?)\)/gim, '<a href="$2" target="_blank" rel="noopener noreferrer">$1</a>')
    .replace(/```[\s\S]*?```/gim, match => `<pre>${match.replace(/```/g, '')}</pre>`)
    .replace(/\n/g, '<br/>');
}

function App() {
  const [readme, setReadme] = useState<string | null>(null);

  useEffect(() => {
    fetch('https://raw.githubusercontent.com/Shikhar03Stark/HushKey/master/README.md')
      .then(res => res.ok ? res.text() : Promise.reject('Not found'))
      .then(text => setReadme(text))
      .catch(() => setReadme(null));
  }, []);

  return (
    <div className="prose prose-invert max-w-3xl mx-auto py-14 px-6 bg-gray-950 rounded-2xl shadow-2xl border border-blue-900/40 mt-10">
      {readme ? (
        <div className="font-sans text-lg leading-relaxed tracking-wide text-blue-100">
          <div dangerouslySetInnerHTML={{ __html: markdownToHtml(readme) }} />
        </div>
      ) : (
        <div className="font-sans text-lg leading-relaxed tracking-wide text-blue-100">
          <h1 className="text-4xl font-extrabold text-blue-300 mb-4 drop-shadow-lg">HushKey</h1>
          <p className="mb-6 text-blue-200 text-xl font-medium">HushKey lets you share secrets securely and privately using modern cryptography and a zero-knowledge approach. <b>All encryption and decryption happen in your browser</b>—the server never sees or stores your secret keys.</p>
          <ul className="mb-6 space-y-2 pl-6 list-disc text-blue-200">
            <li><b>Zero Knowledge Proof:</b> The server never has access to your secret key. Only the encrypted data is sent and stored.</li>
            <li><b>Ephemeral Key Encryption:</b> The server generates a <b>temporary encryption key</b> to encrypt your secret, destroys the key in memory after encryption, and only stores the encrypted secret.</li>
            <li><b>Key Handling:</b> The encryption key (<b>keyString</b>) is never stored or logged by the server. It is appended as a fragment in the shareable URL and only available to the sender and recipient.</li>
            <li><b>Secret URL Design:</b> The <b>keyString</b> is embedded in the URL <b>fragment</b> (the part after <code>#</code>). By browser design, this fragment is <b>never</b> sent to the server in any HTTP request, so only the recipient with the full URL can decrypt the secret.</li>
            <li><b>One-Time View:</b> Secrets can be set to self-destruct after being viewed once, ensuring maximum privacy.</li>
            <li><b>Modern Stack:</b> <span className="font-mono text-blue-400">React + TypeScript + Vite + TailwindCSS</span> frontend, <span className="font-mono text-blue-400">ASP.NET Core Web API</span> backend.</li>
            <li><b>Open Source:</b> <a className="underline hover:text-blue-400 transition-colors" href="https://github.com/Shikhar03Stark/HushKey/tree/master" target="_blank" rel="noopener noreferrer">GitHub Repo</a></li>
          </ul>
          <div className="bg-blue-950/60 border border-blue-800 rounded-xl p-6 mb-4 shadow-inner">
            <p className="mb-2 font-semibold text-blue-300">How it works:</p>
            <p className="text-blue-100">
              When you create a secret, your browser sends the secret securely to the server over HTTPS. The server generates an <b>ephemeral encryption key</b> to encrypt your secret, immediately destroys the key in memory after encryption, and stores only the encrypted secret. The server then returns a reference ID to the encrypted secret, and the <b>keyString</b> is appended as a fragment in the shareable URL (e.g., <code>https://hushkey.app/secrets/public/abc123#keyString</code>).<br/><br/>
              When someone opens the link, their browser extracts the key from the fragment and sends the reference ID to the server to fetch the encrypted secret. The browser then decrypts the secret locally using the keyString. <b>The server never sees or stores the keyString</b>—privacy is preserved by design.
            </p>
          </div>
        </div>
      )}
    </div>
  );
}

export default App;
