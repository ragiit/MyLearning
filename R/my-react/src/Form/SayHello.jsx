export default function SayHello() {
  function handleClick(e) {
    e.preventDefault();
    const name = document.getElementById("input_name").value;
    document.getElementById("text_hello").innerText = `Hello, ${name}!`;
  }
  return (
    <div> 
      <form>
        <input id="input_name" />
        <button onClick={handleClick}>Say Hello</button>
      </form>
      <h1 id="text_hello">Hello World!</h1>
    </div>
  );
}
