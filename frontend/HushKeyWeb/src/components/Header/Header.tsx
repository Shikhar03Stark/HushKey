
export default function Header() {
  return (
    <header className="w-full bg-gray-900 border-b border-gray-800 py-4 shadow-sm">
      <div className="container mx-auto px-4 flex items-center justify-between">
        <h1 className="text-2xl font-extrabold text-blue-300 tracking-wide drop-shadow-sm select-none">
          HushKey{' '}
          <span className="text-base font-semibold text-blue-500 align-super ml-2">
            share secrets securely
          </span>
        </h1>
        {/* You can add navigation or user info here if needed */}
      </div>
    </header>
  );
}
