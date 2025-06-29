import { useNavigate, useLocation } from 'react-router-dom';
import './Header.css';

function Header({ user, setUser }) {
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    setUser(null);
    localStorage.removeItem('token');
    navigate('/');
  };

  return (
    <header className="header">
      <div className="logo" onClick={() => navigate('/')}>
        Мой Магазин
      </div>
      <div className="nav-buttons">
        {user ? (
          <>
            <span>{user.FullName}</span>
            {user.Role === 'Admin' && (
              <button className="nav-button" onClick={() => navigate('/admin')}>
                Админка
              </button>
            )}
            <button className="nav-button" onClick={handleLogout}>Выйти</button>
          </>
        ) : (
          location.pathname !== '/login' && (
            <button className="nav-button" onClick={() => navigate('/login')}>
              Вход
            </button>
          )
        )}
      </div>
    </header>
  );
}

export default Header;
