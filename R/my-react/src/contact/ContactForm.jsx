import { useState } from "react";

const initialData = {
  name: "",
  message: "",
};

export default function ContactForm() {
  const [contact, setContact] = useState(initialData);

  function handleNameChange(e) {
    setContact({ ...contact, name: e.target.value });
  }
  function handleMessageChange(e) {
    setContact({ ...contact, message: e.target.value });
  }

  return (
    <div>
      <h1>Contact Form</h1>
      <form>
        <input type="text" value={contact.name} onChange={handleNameChange} />
        <br />
        <input value={contact.message} onChange={handleMessageChange} />
      </form>
      <h1>Contact Detail</h1>
      <p>Name: {contact.name}</p>
      <p>Message: {contact.message}</p>
    </div>
  );
}
