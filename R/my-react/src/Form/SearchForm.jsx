export default function SearchForm() {
  //   function handleSubmit(e) {
  //     e.preventDefault();
  //     const query = e.target.elements.query.value;
  //     onSearch(query);
  //   }

  return (
    // <form onSubmit={handleSubmit}>
    <form>
      <input type="text" name="query" placeholder="Search..." />
      {/* <button type="submit">Search</button> */}
      <button
        onClick={(e) => {
        //   e.preventDefault();
          alert("Search button clicked!");
        }}
      >
        Search
      </button>
    </form>
  );
}
