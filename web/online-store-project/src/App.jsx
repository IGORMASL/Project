import { BrowserRouter as Router, Routes, Route,} from 'react-router-dom';
import { useState } from 'react';
import ProductList from './components/ProductList';
import Header from './components/Header'
import Login from './pages/Login';
import Register from './pages/Register';
import AdminPage from './pages/AdminPage';
import './App.css'
import ProductDetails from './pages/ProductDetails';

function App() {
  const [user, setUser] = useState(null);


  return (
    <Router>
      <Header user={user} setUser={setUser} /> 
      <main style={{ padding: '20px' }}>
        <Routes>
          <Route path="/login" element={<Login setUser={setUser} />} />
          <Route path="/register" element={<Register />} />
          <Route path="/" element={<Home />} />
          <Route path="/product/:id" element={<ProductDetails />} />
          <Route path="/admin" element={<AdminPage />} />
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
