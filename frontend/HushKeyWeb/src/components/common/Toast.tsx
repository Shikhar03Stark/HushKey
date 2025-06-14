export default function Toast({ message, type, onClose }: { message: string; type: 'success' | 'error'; onClose: () => void }) {
  return (
    <div className={`fixed top-6 right-6 z-99 px-6 py-3 rounded shadow-lg text-white transition-all
      ${type === 'success' ? 'bg-green-600' : 'bg-red-600'}`}
    >
      <div className="flex items-center gap-2">
        <span>{message}</span>
        <button onClick={onClose} className="ml-4 text-lg font-bold">×</button>
      </div>
    </div>
  );
}
