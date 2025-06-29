import { useEffect, useState } from "react";
import axios from "axios";
import ProductAdminCard from "../components/ProductAdminCard";
import ProductForm from "../components/ProductForm";
import CategoryForm from "../components/CategoryForm";
import './AdminPage.css';

function AdminPage() {
  const [products, setProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [editingProduct, setEditingProduct] = useState(null);
  const [showForm, setShowForm] = useState(false);
  const [showCategoryForm, setShowCategoryForm] = useState(false);
  const token = localStorage.getItem("token");

  useEffect(() => {
    fetchProducts();
    fetchCategories();
  }, []);

  const fetchProducts = () => {
    axios.get("http://localhost:5134/api/Products")
      .then(res => setProducts(res.data))
      .catch(err => console.error("Ошибка при загрузке товаров", err));
  };

  const fetchCategories = () => {
    axios.get("http://localhost:5134/api/categories")
      .then(res => setCategories(res.data))
      .catch(err => console.error("Ошибка при загрузке категорий", err));
  };

  const handleDelete = (id) => {
    axios.delete(`http://localhost:5134/api/Products/${id}`)
      .then(fetchProducts)
      .catch(err => console.error("Ошибка при удалении", err));
  };

  const handleDeleteCategory = async (id) => {
  try {
    await axios.delete(`http://localhost:5134/api/categories/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    fetchCategories();
  } catch (err) {
    console.error("Ошибка при удалении категории", err);
    alert("Не удалось удалить категорию");
  }
};

const handleEditCategory = async (category) => {
  const newName = prompt("Введите новое имя категории", category.Name);
  if (!newName || newName.trim() === category.Name) return;

  try {
    await axios.put(`http://localhost:5134/api/categories/${category.Id}`, 
      { name: newName.trim() }, 
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    );
    fetchCategories();
  } catch (err) {
    console.error("Ошибка при редактировании категории", err);
    alert("Не удалось отредактировать категорию");
  }
};

  const handleEdit = (product) => {
    setEditingProduct(product);
    setShowForm(true);
  };

  const handleFormSubmit = () => {
    fetchProducts();
    setEditingProduct(null);
    setShowForm(false);
  };

  return (
    <div>
      <h1>Админ-панель</h1>

      <div className="admin-buttons">
        <button onClick={() => { setEditingProduct(null); setShowForm(true); }}>
          ➕ Добавить товар
        </button>
        <button onClick={() => setShowCategoryForm(true)}>
          📁 Добавить категорию
        </button>
      </div>

      {showForm && (
        <div className="modal">
          <div className="modal-content">
            <button className="close-btn" onClick={() => setShowForm(false)}>✖</button>
            <ProductForm
              product={editingProduct}
              onSubmit={handleFormSubmit}
              categories={categories}
            />
          </div>
        </div>
      )}

      {showCategoryForm && (
        <div className="modal">
          <div className="modal-content">
            <button className="close-btn" onClick={() => setShowCategoryForm(false)}>✖</button>
            <CategoryForm onSubmit={() => { fetchCategories(); setShowCategoryForm(false); }} />
          </div>
        </div>
      )}

      <h2>Категории</h2>
      <ul>
        {categories.map(cat => (
          <li key={cat.Id}>
            {cat.Name}
            <button onClick={() => handleEditCategory(cat)}>✏️</button>
            <button onClick={() => handleDeleteCategory(cat.Id)}>🗑️</button>
          </li>
        ))}
      </ul>

      <h2>Товары</h2>
      <div className="product-grid">
        {products.map(product => (
          <ProductAdminCard
            key={product.id}
            product={product}
            onDelete={handleDelete}
            onEdit={handleEdit}
          />
        ))}
      </div>
    </div>
  );
}

export default AdminPage;
