import { useState, useEffect } from "react";
import './ProductForm.css';

function ProductForm({ product, onSubmit, categories }) {
  const [form, setForm] = useState({
    name: "",
    description: "",
    price: "",
    imageFile: null,
  });
  const token = localStorage.getItem('token');
  const [preview, setPreview] = useState(null);

  useEffect(() => {
    if (product) {
      setForm({
        name: product.name || "",
        description: product.description || "",
        price: product.price || "",
        category: product.categoryId || "",
        imageFile: null,
      });
      setPreview(`http://localhost:5134/${product.imageUrl}`);
    } else {
      setForm({ name: "", description: "", price: "", imageFile: null });
      setPreview(null);
    }
  }, [product]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm(prev => ({ ...prev, [name]: value }));
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    setForm(prev => ({ ...prev, imageFile: file }));
    if (file) {
      setPreview(URL.createObjectURL(file));
    } else {
      setPreview(null);
    }
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    const formData = new FormData();
    formData.append("name", form.name);
    formData.append("description", form.description);
    formData.append("price", form.price);
    if (form.imageFile) {
      formData.append("image", form.imageFile);
    }

    const method = product ? "PUT" : "POST";
    const url = product
      ? `http://localhost:5134/api/Products/${product.id}`
      : `http://localhost:5134/api/Products`;

    fetch(url, {
      method,
      body: formData,
      headers: {
        Authorization: token
      }
    })
      .then(res => {
        if (!res.ok) throw new Error("Ошибка при сохранении товара");
        return res.json();
      })
      .then(() => {
        onSubmit();
        setForm({ name: "", description: "", price: "", imageFile: null });
        setPreview(null);
      })
      .catch(err => alert(err.message));
  };

  return (
    <form className="product-form" onSubmit={handleSubmit}>
      <h2>{product ? "Редактировать товар" : "Добавить товар"}</h2>

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
        type="number"
        value={form.price}
        onChange={handleChange}
        placeholder="Цена"
        required
      />

      <input
        type="file"
        accept="image/*"
        onChange={handleFileChange}
      />

      <select
        name="categoryId"
        value={form.categoryId}
        onChange={handleChange}
        required
      >
      <option value="">Выберите категорию</option>
      {categories.map(cat => (
        <option key={cat.Id} value={cat.Id}>{cat.Name}</option>
      ))}
</select>

      {preview && (
        <img
          src={preview}
          alt="Превью изображения"
          className="image-preview"
        />
      )}

      <button type="submit">
        {product ? "Сохранить изменения" : "Добавить товар"}
      </button>
    </form>
  );
}

export default ProductForm;
