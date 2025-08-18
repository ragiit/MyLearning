export default function Todo({ text, isCompleted, isDeleted = false }) {
  if (isDeleted) {
    return null; // Skip rendering if the todo is deleted
  }
  return <li>{isCompleted ? <del>{text}</del> : text}</li>;
}
