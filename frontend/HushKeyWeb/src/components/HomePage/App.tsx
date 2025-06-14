import { useEffect, useState } from 'react';
import ReactMarkdown from 'react-markdown';

function App() {
  const [readme, setReadme] = useState<string | null>(null);

  useEffect(() => {
    fetch('https://raw.githubusercontent.com/Shikhar03Stark/HushKey/master/README.md')
      .then(res => res.ok ? res.text() : Promise.reject('Not found'))
      .then(text => setReadme(text))
      .catch(() => setReadme(null));
  }, []);

  return (
    <div className="prose prose-invert max-w-4xl mx-auto py-16 px-8 bg-gray-950 rounded-3xl shadow-2xl border border-blue-900/40 mt-14">
      {readme ? (
        <div className="font-sans text-[1.15rem] leading-relaxed tracking-wide text-blue-100">
          <div className="mb-8 p-4 rounded-xl bg-blue-900/40 border border-blue-800 text-blue-200 text-base italic flex items-center gap-2">
            <svg width="20" height="20" fill="currentColor" className="text-blue-400 flex-shrink-0" viewBox="0 0 20 20"><path d="M10 2a8 8 0 100 16 8 8 0 000-16zm1 12H9v-2h2v2zm0-4H9V6h2v4z"/></svg>
            <span>
              <b>Disclaimer:</b> The content below is fetched live from the project's GitHub repository README. For the latest version, visit
              {' '}<a href="https://github.com/Shikhar03Stark/HushKey/blob/master/README.md" target="_blank" rel="noopener noreferrer" className="underline hover:text-blue-400">GitHub README</a>.
            </span>
          </div>
          <ReactMarkdown
            components={{
              h1: ({node, ...props}) => <h1 className="text-4xl font-extrabold text-blue-300 mb-6 mt-10 drop-shadow-lg" {...props} />,
              h2: ({node, ...props}) => <h2 className="text-2xl font-bold text-blue-200 mt-8 mb-4" {...props} />,
              h3: ({node, ...props}) => <h3 className="text-xl font-semibold text-blue-200 mt-6 mb-3" {...props} />,
              p: ({node, ...props}) => <p className="mb-5 text-blue-100 text-lg" {...props} />,
              ul: ({node, ...props}) => <ul className="mb-6 space-y-2 pl-7 list-disc text-blue-200" {...props} />,
              li: ({node, ...props}) => <li className="text-blue-100 text-base" {...props} />,
              a: ({node, ...props}) => <a className="underline hover:text-blue-400 transition-colors" target="_blank" rel="noopener noreferrer" {...props} />,
              code: ({node, ...props}) => <code className="bg-blue-900/60 px-1.5 py-0.5 rounded text-blue-300 font-mono text-base" {...props} />,
              pre: ({node, ...props}) => <pre className="bg-blue-950/80 border border-blue-800 rounded-xl p-4 my-4 overflow-x-auto text-blue-200" {...props} />,
              blockquote: ({node, ...props}) => <blockquote className="border-l-4 border-blue-500 pl-4 italic text-blue-300 my-4" {...props} />,
              table: ({node, ...props}) => <table className="min-w-full my-4 border border-blue-800" {...props} />,
              th: ({node, ...props}) => <th className="border border-blue-800 px-3 py-2 bg-blue-900/40" {...props} />,
              td: ({node, ...props}) => <td className="border border-blue-800 px-3 py-2" {...props} />,
            }}
          >
            {readme}
          </ReactMarkdown>
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
