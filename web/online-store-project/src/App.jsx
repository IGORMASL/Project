import { BrowserRouter as Router, Routes, Route,} from 'react-router-dom';
import { useEffect, useState } from 'react';
import axios from 'axios';
import ProtectedRoute from './components/ProtectedRoute';
import ProductList from './components/ProductList';
import Header from './components/Header'
import Login from './pages/Login';
import Register from './pages/Register';
import AdminPage from './pages/AdminPage';
import './App.css'
import ProductDetails from './pages/ProductDetails';

function App() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  useEffect(() => {
    const token = localStorage.getItem('token')
    if(!token){
      setLoading(false);
      return;
    }
    axios.get('http://localhost:5134/api/auth/me', {
      headers: {
        Authorization: `Bearer ${token}`,
      }
    })
    .then(res => {
      setUser(res.data);
    })
    .catch(() => {
      localStorage.removeItem('token');
    })
    .finally(() => {
      setLoading(false);
    }, []);
  })
  if(loading) return <div>Загрузка...</div>
  return (
    <Router>
      <Header user={user} setUser={setUser} /> 
      <main style={{ padding: '20px' }}>
        <Routes>
          <Route path="/login" element={<Login setUser={setUser} />} />
          <Route path="/register" element={<Register />} />
          <Route path="/" element={<Home />} />
          <Route path="/product/:id" element={<ProductDetails />} />
          <Route path="/admin" element={ 
            <ProtectedRoute user={user} requiredRole="Admin">
            <AdminPage />
            </ProtectedRoute>
            } />
        </Routes>
      </main>
    </Router>
  )
}
function Home() {
  return (
    <div>
      <h1>Добро пожаловать в онлайн-магазин!</h1>
      <ProductList />
    </div>
  );
}

export default App
