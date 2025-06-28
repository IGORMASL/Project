import { useEffect, useState } from "react";
import axios from "axios";
import ProductAdminCard from "../components/ProductAdminCard";
import ProductForm from "../components/ProductForm";

function AdminPage() {
  const [products, setProducts] = useState([]);
  const [editingProduct, setEditingProduct] = useState(null);

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = () => {
    axios.get("http://localhost:5134/api/Products")
      .then(res => setProducts(res.data))
      .catch(err => console.error("Ошибка при загрузке товаров", err));
  };

  const handleDelete = (id) => {
    axios.delete(`http://localhost:5134/api/Products/${id}`)
      .then(() => fetchProducts())
      .catch(err => console.error("Ошибка при удалении", err));
  };

  const handleEdit = (product) => {
    setEditingProduct(product);
    window.scrollTo(0, 0); // Прокрутка наверх к форме
  };

  const handleFormSubmit = () => {
    fetchProducts();
    setEditingProduct(null);
  };

  return (
    <div>
      <h1>Админ-панель товаров</h1>
      <ProductForm product={editingProduct} onSubmit={handleFormSubmit} />
      <div style={{ display: "grid", gap: "20px", marginTop: "40px" }}>
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
