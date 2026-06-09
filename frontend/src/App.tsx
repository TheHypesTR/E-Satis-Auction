import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login      from './pages/Login';
import Dashboard  from './pages/Dashboard';
import Products   from './pages/Products';
import Facilities from './pages/Facilities';
import Categories from './pages/Categories';
import Inventory  from './pages/Inventory';
import Orders     from './pages/Orders';
import Returns    from './pages/Returns';
import ProductListings from './pages/ProductListings';
import Campaigns from './pages/Campaigns';
import AdminUserSaleRequests from './pages/AdminUserSaleRequests';
import AdminAuctions from './pages/AdminAuctions';
import PartSales from './pages/PartSales';
import Dispatches from './pages/Dispatches';
import Layout     from './components/Layout';

import UserLayout        from './components/UserLayout';
import UserHome          from './pages/user/UserHome';
import UserCatalog       from './pages/user/UserCatalog';
import UserProductDetail from './pages/user/UserProductDetail';
import UserCart          from './pages/user/UserCart';
import UserCheckout      from './pages/user/UserCheckout';
import UserOrderSuccess  from './pages/user/UserOrderSuccess';
import UserProfile       from './pages/user/UserProfile';
import UserAuctions      from './pages/user/UserAuctions';
import UserAuctionDetail from './pages/user/UserAuctionDetail';
import UserSellRequest   from './pages/user/UserSellRequest';

function App() {
  const isAuthenticated = !!localStorage.getItem('token');

  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/" element={isAuthenticated ? <Layout /> : <Navigate to="/login" replace />}>
          <Route index element={<Navigate to="/dashboard" replace />} />
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="products" element={<Products />} />
          <Route path="facilities" element={<Facilities />} />
          <Route path="categories" element={<Categories />} />
          <Route path="inventory" element={<Inventory />} />
          <Route path="orders" element={<Orders />} />
          <Route path="returns" element={<Returns />} />
          <Route path="listings" element={<ProductListings />} />
          <Route path="campaigns" element={<Campaigns />} />
          <Route path="user-sale-requests" element={<AdminUserSaleRequests />} />
          <Route path="auctions" element={<AdminAuctions />} />
          <Route path="part-sales" element={<PartSales />} />
          <Route path="dispatches" element={<Dispatches />} />
        </Route>
        <Route path="/user" element={<UserLayout />}>
          <Route index element={<UserHome />} />
          <Route path="catalog" element={<UserCatalog />} />
          <Route path="catalog/:id" element={<UserProductDetail />} />
          <Route path="cart" element={<UserCart />} />
          <Route path="checkout" element={<UserCheckout />} />
          <Route path="order-success" element={<UserOrderSuccess />} />
          <Route path="profile" element={<UserProfile />} />
          <Route path="auctions" element={<UserAuctions />} />
          <Route path="auctions/:id" element={<UserAuctionDetail />} />
          <Route path="sell" element={<UserSellRequest />} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;
