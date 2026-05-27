import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login      from './pages/Login';
import Dashboard  from './pages/Dashboard';
import Products   from './pages/Products';
import Facilities from './pages/Facilities';
import Categories from './pages/Categories';
import Inventory  from './pages/Inventory';
import Layout     from './components/Layout';

function App() {
  const isAuthenticated = !!localStorage.getItem('token');

  return (
    <Router>
      <Routes>
        {/* Public */}
        <Route path="/login" element={<Login />} />

        {/* Protected */}
        <Route
          path="/"
          element={isAuthenticated ? <Layout /> : <Navigate to="/login" replace />}
        >
          <Route index                  element={<Navigate to="/dashboard" replace />} />
          <Route path="dashboard"       element={<Dashboard />} />
          <Route path="products"        element={<Products />} />
          <Route path="facilities"      element={<Facilities />} />
          <Route path="categories"      element={<Categories />} />
          <Route path="inventory"       element={<Inventory />} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;
