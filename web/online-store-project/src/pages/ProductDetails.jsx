import { useParams, useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import './ProductDetails.css';

function ProductDetails() {
  const { id } = useParams();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    fetch(`http://localhost:5134/api/Products/${id}`)
      .then(res => {
        if (!res.ok) throw new Error('Ошибка загрузки товара');
        return res.json();
      })
      .then(data => {
        setProduct(data);
        setLoading(false);
      })
      .catch(err => {
        setError(err.message);
        setLoading(false);
      });
  }, [id]);

  if (loading) return <p>Загрузка...</p>;
  if (error) return <p>Ошибка: {error}</p>;

  return (
    <div className="product-details">
      <h2>{product.Name}</h2>
      <img
        src={`http://localhost:5134/${product.ImageUrl}`}
        alt={product.Name}
      />
      <p>{product.Description}</p>
      <p className="price">Цена: {product.Price}₽</p>

      <button className="back-button" onClick={() => navigate(-1)}>← Назад</button>
    </div>
  );
}

export default ProductDetails;
