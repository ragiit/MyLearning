import { useState } from "react";

export default function Counter() {
  let [counter, setCounter] = useState(0);

  function handleClick(e) {
    e.preventDefault();
    // setCounter(counter + 1);
    // setCounter(counter + 1);
    // setCounter(counter + 1);
    setCounter((c) => c + 1);
    setCounter((c) => c + 1);
    setCounter((c) => c + 1);
    console.log(counter);
  }

  return (
    <div>
      <form>
        <button onClick={handleClick}>Increment</button>
      </form>
      <h1>Counter: {counter}</h1>
    </div>
  );
}
