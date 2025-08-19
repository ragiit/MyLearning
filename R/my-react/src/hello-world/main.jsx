import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import HelloWorld from "./HelloWorld";
import Container from "./Container";
import TodoList from "../todo-list/TodoList";
import Table from "../table/Table";
import { AlertButton } from "../button/AlertButton";
import { MyButton } from "../button/MyButton";

createRoot(document.getElementById("root")).render(
  <StrictMode>
    <Container>
      <HelloWorld />
      <TodoList />
      <Table />
      <AlertButton message="Hello, World!" text="Click Me!" />
      <MyButton text="Smash!" onSmash={() => alert("Button smashed!")} />
      <MyButton text="Smash!" onSmash={() => alert("Button smashed again!")} />
      <MyButton
        text="Smash!"
        onSmash={() => alert("Button smashed once more!")}
      />
    </Container>
  </StrictMode>
);
