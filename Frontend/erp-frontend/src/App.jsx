import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Layout from './components/layout/Layout';
import Dashboard from './pages/production/Dashboard';
import RawMaterial from './pages/masters/rawMaterial';
import Products from './pages/masters/products';
import BOM from './pages/masters/BOM';
import Production from './pages/production/AllOrders';
import OrderManagement from './pages/production/OrderManagement';
import CreateOrder from './pages/production/CreateOrder';
import AddRawMaterialStock from './pages/inventory/AddRawMaterialStock';
import FinishedGoodStock from './pages/inventory/FinishedGoodStock';


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
          <Route path="/production-create-order" element={<CreateOrder />} />
          {/* Inventory Routes */}
          <Route path="/inventory/raw-material" element={<AddRawMaterialStock />} />
          <Route path="/inventory/finished-goods" element={<FinishedGoodStock />} />

          {/* Production */}
          <Route path="/production-orders" element={<Production />} />


        </Routes>
      </Layout>
    </BrowserRouter>
  );
}

export default App;