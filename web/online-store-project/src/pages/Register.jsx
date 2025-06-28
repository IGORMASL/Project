import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './Register.css';

function RegistrationForm() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
  });

  const [errors, setErrors] = useState({});

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const validate = (formData) => {
  const newErrors = {};

  // Проверка имени пользователя
  if (!formData.username.trim()) {
    newErrors.username = 'Введите имя пользователя';
  }

  // Проверка email
  if (!formData.email) {
    newErrors.email = 'Введите email';
  } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
    newErrors.email = 'Неверный формат email';
  } else if (formData.email.length > 100) {
    newErrors.email = 'Email должен быть не длиннее 100 символов';
  }

  // Проверка пароля
  if (!formData.password) {
    newErrors.password = 'Введите пароль';
  } else {
    if (formData.password.length < 8) {
      newErrors.password = 'Пароль должен содержать минимум 8 символов';
    }
    if (!/[A-Z]/.test(formData.password)) {
      newErrors.password = 'Пароль должен содержать заглавную букву';
    }
    if (!/[a-z]/.test(formData.password)) {
      newErrors.password = 'Пароль должен содержать строчную букву';
    }
    if (!/[0-9]/.test(formData.password)) {
      newErrors.password = 'Пароль должен содержать цифру';
    }
  }

  // Проверка подтверждения пароля
  if (formData.confirmPassword !== formData.password) {
    newErrors.confirmPassword = 'Пароли не совпадают';
  }

  // Проверка длины имени пользователя
  if (formData.username.length > 100) {
    newErrors.username = 'Имя пользователя должно быть не длиннее 100 символов';
  }

  return newErrors;
};


  const handleSubmit = async (e) => {
    e.preventDefault();
    const validationErrors = validate(formData);
    if (Object.keys(validationErrors).length === 0) {
      try {
        const data = await registerUser({
          email: formData.email,
          password: formData.password,
          fullName: formData.username,
        });
        alert('Регистрация прошла успешно!');
        setFormData({ username: '', email: '', password: '', confirmPassword: '' });
        setErrors({});
        navigate('/login');
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
    <form className="registration-form" onSubmit={handleSubmit}>
      <h2>Регистрация</h2>
      {errors.apiError && <p className="error-message">{errors.apiError}</p>}

      <label>
        Имя пользователя
        <input
          type="text"
          name="username"
          value={formData.username}
          onChange={handleChange}
          placeholder="Введите имя пользователя"
          className={errors.username ? 'error' : ''}
        />
        {errors.username && <span className="error-message">{errors.username}</span>}
      </label>

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

      <label>
        Подтвердите пароль
        <input
          type="password"
          name="confirmPassword"
          value={formData.confirmPassword}
          onChange={handleChange}
          placeholder="Повторите пароль"
          className={errors.confirmPassword ? 'error' : ''}
        />
        {errors.confirmPassword && <span className="error-message">{errors.confirmPassword}</span>}
      </label>

      <button type="submit">Зарегистрироваться</button>
    </form>
  );
}

async function registerUser(data) {
  const response = await fetch('http://localhost:5134/api/Auth/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.detail);
  }
}

export default RegistrationForm;
