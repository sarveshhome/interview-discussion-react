import { useState } from 'react'

import './App.css'

function App() {
  const [value, setValue] = useState(1);
  const [multipliedValue, setMultipliedValue] = useState(1);
  const multiplybyfive =()=>{
    setMultipliedValue(value * 5);
    setValue(value + 1);
  }

  return (
    <>
      <h1>Main value {value}</h1>
      <button onClick={multiplybyfive}> Multiply by 5s</button>
      <h1>Multiplied value {multipliedValue}</h1>
    </>
  )
}

export default App
