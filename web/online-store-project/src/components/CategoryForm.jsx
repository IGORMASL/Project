import { useState } from "react";
import axios from "axios";

function CategoryForm({ onSubmit }) {
  const [name, setName] = useState("");
  const token = localStorage.getItem("token");

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!name.trim()) return;

    try {
      await axios.post("http://localhost:5134/api/categories", { name },
        {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      onSubmit();
      setName("");
    } catch (err) {
      alert("Ошибка при добавлении категории");
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h3>Добавить категорию</h3>
      <input
        type="text"
        placeholder="Название категории"
        value={name}
        onChange={(e) => setName(e.target.value)}
        required
      />
      <button type="submit">Добавить</button>
    </form>
  );
}

export default CategoryForm;
