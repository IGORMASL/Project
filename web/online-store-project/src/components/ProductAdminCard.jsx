import './ProductAdminCard.css';

function ProductAdminCard({ product, onDelete, onEdit }) {
  return (
    <div className="product-card">
      <img src={`http://localhost:5134/${product.ImageUrl}`} alt={product.Name} className="product-image" />
      <div className="product-info">
        <h3>{product.Name}</h3>
        <p>{product.Description}</p>
        <p><strong>{product.Price} ₽</strong></p>
        <div className="admin-buttons">
          <button onClick={() => onEdit(product)}>Редактировать</button>
          <button onClick={() => onDelete(product.Id)}>Удалить</button>
        </div>
      </div>
    </div>
  );
}

export default ProductAdminCard;
