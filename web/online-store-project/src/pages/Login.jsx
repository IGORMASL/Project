import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Link } from 'react-router-dom';
import './Login.css';

function LoginForm({setUser}) {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });

  const [errors, setErrors] = useState({});

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const validate = () => {
    const newErrors = {};
    if (!formData.email) newErrors.email = 'Введите email';
    else if (!/\S+@\S+\.\S+/.test(formData.email)) newErrors.email = 'Неверный формат email';
    if (!formData.password) newErrors.password = 'Введите пароль';
    return newErrors;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const validationErrors = validate();
    if (Object.keys(validationErrors).length === 0) {
try {
      const data = await loginUser({
        email: formData.email,
        password: formData.password,
      });
      localStorage.setItem('token', data.Token);

      alert('Вход выполнен успешно!');
      setFormData({ email: '', password: '' });
      setErrors({});
      setUser(data.User);
      navigate('/');
    } catch (error) {
      const friendlyMessage = error.message === 'Failed to fetch'
      ? 'Сервер недоступен. Попробуйте позже.'
      : error.message;
      setErrors({ apiError: friendlyMessage });
    }
    } else {
      setErrors(validationErrors);
    }
  };

  return (
    <div>
    <form className="login-form" onSubmit={handleSubmit}>
      <h2>Вход</h2>
      {errors.apiError && <p className="error-message">{errors.apiError}</p>}
      <label>
        Email
        <input
          type="email"
          name="email"
          value={formData.email}
          onChange={handleChange}
          placeholder="example@mail.com"
          className={errors.email ? 'error' : ''}
        />
        {errors.email && <span className="error-message">{errors.email}</span>}
      </label>

      <label>
        Пароль
        <input
          type="password"
          name="password"
          value={formData.password}
          onChange={handleChange}
          placeholder="Введите пароль"
          className={errors.password ? 'error' : ''}
        />
        {errors.password && <span className="error-message">{errors.password}</span>}
      </label>

      <button type="submit">Войти</button>
    </form>
    <label className='registrationLabel'>
    Нет аккаунта? <Link to="/register" className="nav-button nav-button--primary">Зарегистрируйтесь</Link>
    </label>
    </div>
  );
}

export async function loginUser(credentials) {
  const response = await fetch('http://localhost:5134/api/Auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(credentials),
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.detail);
  }

  return response.json();
}

export default LoginForm;
