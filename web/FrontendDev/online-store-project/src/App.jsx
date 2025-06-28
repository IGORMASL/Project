import { BrowserRouter as Router, Routes, Route,} from 'react-router-dom';
import { useState } from 'react';
import ProductList from './components/ProductList';
import Header from './components/Header'
import Login from './pages/Login';
import Register from './pages/Register';
import './App.css'
import ProductDetails from './pages/ProductDetails';

function App() {
  const [user, setUser] = useState(null);

  return (
    <Router>
      <Header user={user}/>
      <main style={{ padding: '20px' }}>
        <Routes>
          <Route path="/login" element={<Login onLogin={setUser}/>} />
          <Route path="/register" element={<Register />} />
          <Route path="/" element={<Home />} />
          <Route path="/product/:id" element={<ProductDetails />} />
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
