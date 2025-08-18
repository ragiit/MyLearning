import Todo from "./Todo";

export default function TodoList() {
  return (
    <ul>
      <Todo text="Learn React" isCompleted={false} isDeleted={true} />
      <Todo text="Build a Todo App" isCompleted={true} />
      <Todo text="Deploy the App" isCompleted={false} />
      <Todo text="Share with Friends" isCompleted={true} />
    </ul>
  );
}
