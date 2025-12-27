import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Layout from './components/layout/Layout';
import Dashboard from './pages/Dashboard';
import RawMaterial from './pages/Masters/RawMaterial';

// Placeholder Pages (Inko baad me asli banayenge)
// const Dashboard = () => <h1 className="text-2xl font-bold">Planner Dashboard</h1>;
const Production = () => <h1 className="text-2xl font-bold">Production Floor</h1>;
const Inventory = () => <h1 className="text-2xl font-bold">Inventory View</h1>;

function App() {
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          {/* Masters Routes */}
          <Route path="/masters/raw-materials" element={<RawMaterial />} />
          <Route path="/production" element={<Production />} />
          <Route path="/inventory" element={<Inventory />} />
        </Routes>
      </Layout>
    </BrowserRouter>
  );
}

export default App;