import "./HelloWorld.css";

export default function HelloWorld() {
  const props = {
    text: "Hello Worldsss",
  };
  return (
    <div>
      <HeaderHelloWorld {...props} />
      <ParagraphHelloWorld />
    </div>
  );
}

function HeaderHelloWorld({ text = "Hello World" }) {
  return (
    <header>
      <h1 className="title">{text.toUpperCase()}</h1>
    </header>
  );
}

function ParagraphHelloWorld() {
  return (
    <p className="content">This is a paragraph in the Hello World component.</p>
  );
}