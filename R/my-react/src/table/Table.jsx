import { Row } from "./Row";

export default function Table() {
  return (
    <table border="1">
      <tbody>
        <Row text="Row 1" />
        <Row text="Row 2" />
        <Row text="Row 3" />
      </tbody>
    </table>
  );
}
