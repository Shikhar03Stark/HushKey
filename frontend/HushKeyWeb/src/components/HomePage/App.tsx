import { useState } from 'react'

function App() {
  const [count, _setCount] = useState(0)

  return (
    <>
      <div className='flex'>
        <p>Count: {count}</p>
      </div>
    </>
  )
}

export default App
