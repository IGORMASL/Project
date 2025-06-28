import { useState, useEffect } from "react";
import './ProductForm.css';

function ProductForm({ product, onSubmit }) {
  const [form, setForm] = useState({
    name: "",
    description: "",
    price: "",
    imageUrl: ""
  });

  useEffect(() => {
    if (product) {
      setForm(product);
    } else {
      setForm({ name: "", description: "", price: "", imageUrl: "" });
    }
  }, [product]);

  const handleChange = e => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = e => {
    e.preventDefault();

    const method = product ? "put" : "post";
    const url = product
      ? `http://localhost:5134/api/Products/${product.id}`
      : "http://localhost:5134/api/Products";

    fetch(url, {
      method: method.toUpperCase(),
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(form),
    })
      .then(res => {
        if (!res.ok) throw new Error("Ошибка при сохранении");
        return res.json();
      })
      .then(() => {
        onSubmit();
        setForm({ name: "", description: "", price: "", imageUrl: "" });
      })
      .catch(err => alert(err.message));
  };

  return (
    <form className="product-form" onSubmit={handleSubmit}>
      <h2>{product ? "Редактировать товар" : "Добавить новый товар"}</h2>
      <input
        name="name"
        value={form.name}
        onChange={handleChange}
        placeholder="Название товара"
        required
      />
      <textarea
        name="description"
        value={form.description}
        onChange={handleChange}
        placeholder="Описание"
        required
      />
      <input
        name="price"
        value={form.price}
        onChange={handleChange}
        type="number"
        placeholder="Цена"
        required
      />
      <input
        name="imageUrl"
        value={form.imageUrl}
        onChange={handleChange}
        placeholder="Ссылка на изображение"
      />
      <button type="submit">{product ? "Сохранить" : "Добавить"}</button>
    </form>
  );
}

export default ProductForm;
