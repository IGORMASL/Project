import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import './ProductList.css';

function ProductList() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetch('http://localhost:5134/api/Products')
      .then(res => {
        if (!res.ok) throw new Error('Ошибка загрузки товаров');
        return res.json();
      })
      .then(data => {
        setProducts(data);
        setLoading(false);
      })
      .catch(err => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

  if (loading) return <p>Загрузка товаров...</p>;
  if (error) return <p>Ошибка: {error}</p>;

  return (
    <div className="product-list">
      <h2>Все товары</h2>
      <div className="products">
        {products.map(product => (
          <Link to={`/product/${product.Id}`} key={product.Id} className="product-link">
          <div key={product.Id} className="product-card">
            <img src={`http://localhost:5134/${product.ImageUrl}`} alt={product.Name} />
            <h3>{product.Name}</h3>
            <p>{product.Description}</p>
            <p>Цена: {product.Price}₽</p>
          </div>
          </Link>
        ))}
      </div>
    </div>
  );
}

export default ProductList;
