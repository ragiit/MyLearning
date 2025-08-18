import Todo from "./Todo";

export default function TodoList() {
  const data = [
    { text: "Learn React", isCompleted: false, isDeleted: true },
    { text: "Build a Todo App", isCompleted: true },
    { text: "Deploy the App", isCompleted: false },
    { text: "Share with Friends", isCompleted: true },
  ];
  const todos = data.map((todo, index) => <Todo key={index} {...todo} />);
  return <ul>{todos}</ul>;
  // <ul>
  //   <Todo text="Learn React" isCompleted={false} isDeleted={true} />
  //   <Todo text="Build a Todo App" isCompleted={true} />
  //   <Todo text="Deploy the App" isCompleted={false} />
  //   <Todo text="Share with Friends" isCompleted={true} />
  // </ul>
}
