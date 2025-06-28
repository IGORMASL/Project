import { useNavigate, useLocation } from 'react-router-dom';
import { useEffect, useState } from 'react';
import axios from 'axios';
import './Header.css';

function Header({ user, setUser }) {
  const navigate = useNavigate();
  const location = useLocation();
  const [isAdmin, setIsAdmin] = useState(false);

  const isLoginPage = location.pathname === '/login';

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) return;

    // Проверка авторизации
    axios.get('http://localhost:5134/api/Auth/test-auth', {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then(res => {
        if (!user) {
          const { Id, Email } = res.data;
          setUser({ Email: Email, Id: Id });
        }
      })
      .catch(() => {
        setUser(null);
        localStorage.removeItem('token');
      });
    axios.get('http://localhost:5134/api/Auth/test-admin', {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then(() => setIsAdmin(true))
      .catch(() => setIsAdmin(false));
  }, [setUser]);

  const handleLogout = () => {
    setUser(null);
    setIsAdmin(false);
    localStorage.removeItem('token');
    navigate('/login');
  };

  return (
    <header className="header">
      <div className="logo" onClick={() => navigate('/')}>Мой Магазин</div>
      <div className="nav-buttons">
        {user ? (
          <>
            <span>{user.FullName}</span>
            {isAdmin && (
              <button className="nav-button" onClick={() => navigate('/admin')}>Админка</button>
            )}
            <button className="nav-button" onClick={handleLogout}>Выйти</button>
          </>
        ) : (
          !isLoginPage && (
            <button className="nav-button" onClick={() => navigate('/login')}>Вход</button>
          )
        )}
      </div>
    </header>
  );
}

export default Header;
