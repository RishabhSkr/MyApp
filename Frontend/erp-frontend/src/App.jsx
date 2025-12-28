import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Layout from './components/layout/Layout';
import Dashboard from './pages/production/Dashboard';
import RawMaterial from './pages/masters/rawMaterial';
import Products from './pages/masters/products';
import BOM from './pages/masters/BOM';
import Production from './pages/production/AllOrders';
import OrderManagement from './pages/production/OrderManagement';

// Import other pages as placeholder for now
const RMStock = () => <div className="p-8"><h1>Raw Material Inventory</h1></div>;
const FGStock = () => <div className="p-8"><h1>Finished Goods Inventory</h1></div>;

function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          {/* Dashboard */}
          <Route path="/" element={<Dashboard />} />


          {/* Masters Routes */}
          <Route path="/masters/raw-materials" element={<RawMaterial />} />
          <Route path="/masters/products" element={<Products />} />
          <Route path="/masters/bom" element={<BOM />} />
         
          <Route path="/production-plan" element={<OrderManagement />} />

          {/* Inventory Routes */}
          <Route path="/inventory/raw-material" element={<RMStock />} />
          <Route path="/inventory/finished-goods" element={<FGStock />} />

          {/* Production */}
          <Route path="/production-orders" element={<Production />} />


        </Routes>
      </Layout>
    </BrowserRouter>
  );
}

export default App;