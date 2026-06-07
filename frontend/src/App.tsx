import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login      from './pages/Login';
import Dashboard  from './pages/Dashboard';
import Products   from './pages/Products';
import Facilities from './pages/Facilities';
import Categories from './pages/Categories';
import Inventory  from './pages/Inventory';
import Orders     from './pages/Orders';
import Returns    from './pages/Returns';
import Layout     from './components/Layout';

// User Panel
import UserLayout        from './components/UserLayout';
import UserHome          from './pages/user/UserHome';
import UserCatalog       from './pages/user/UserCatalog';
import UserProductDetail from './pages/user/UserProductDetail';
import UserCart          from './pages/user/UserCart';
import UserCheckout      from './pages/user/UserCheckout';
import UserOrderSuccess  from './pages/user/UserOrderSuccess';
import UserProfile       from './pages/user/UserProfile';

function App() {
  const isAuthenticated = !!localStorage.getItem('token');

  return (
    <Router>
      <Routes>
        {/* Public */}
        <Route path="/login" element={<Login />} />

        {/* Admin Panel (Protected) */}
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
          <Route path="orders"          element={<Orders />} />
          <Route path="returns"         element={<Returns />} />
        </Route>

        {/* User Panel (Public — no auth required) */}
        <Route path="/user" element={<UserLayout />}>
          <Route index                         element={<UserHome />} />
          <Route path="catalog"                element={<UserCatalog />} />
          <Route path="catalog/:id"            element={<UserProductDetail />} />
          <Route path="cart"                   element={<UserCart />} />
          <Route path="checkout"               element={<UserCheckout />} />
          <Route path="order-success"          element={<UserOrderSuccess />} />
          <Route path="profile"               element={<UserProfile />} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;
