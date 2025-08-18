export default function Container({ children }) {
  return (
    <div>
      <h2>Container Component</h2>
      {children}
      <p>This is a container for the Hello World component.</p>
      <footer>
        <p>Footer content goes here.</p>
        <p>© 2023 My React App</p>
      </footer>
    </div>
  );
}
