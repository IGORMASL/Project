import { useNavigate, useLocation} from 'react-router-dom';
import './Header.css';

function Header({ user, setUser }) {
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    setUser(null);
    localStorage.removeItem('token');
    navigate('/login');
  };
  const isLoginPage = location.pathname === '/login';
  return (
    <header className="header">
      <div className="logo" onClick={() => navigate('/')}>Мой Магазин</div>
      <div className="nav-buttons">
        {user ? (
          <>
            <span>Привет, {user.username}!</span>
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
