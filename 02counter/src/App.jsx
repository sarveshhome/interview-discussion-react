import { useState } from 'react'
import './App.css'

function App() {
  const [counter, setCounter] = useState(1)

  const addHandler =()=>{
    setCounter(counter + 1)
    setCounter(counter + 1)
    setCounter(counter + 1)
    setCounter(counter + 1)
  }

  return (
    <>
      <h1>Counter: {counter}</h1>
      <button onClick={addHandler}>Increment</button>
    </>
  )
}

export default App
