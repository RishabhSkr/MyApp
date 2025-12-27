import  { useEffect, useState } from 'react';
import { getRawMaterials, 
    createRawMaterial, 
    deleteRawMaterial } from '../../api/master';

const RawMaterial = () => {
    const [materials, setMaterials] = useState([]);
    const [loading, setLoading] = useState(false);
    
    // Form State
    const [formData, setFormData] = useState({ name: '', unit: 'kg', cost: '' });

    // 1. Load Data
    const loadData = async () => {
        setLoading(true);
        try {
            const data = await getRawMaterials();
            setMaterials(data);
        } catch (error) {
            console.error("Failed to fetch materials", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadData();
    }, []);

    // 2. Handle Submit
    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!formData.name || !formData.cost) return;

        try {
            await createRawMaterial({
                name: formData.name,
                unit: formData.unit,
                cost: parseFloat(formData.cost)
            });
            alert("Material Added!");
            setFormData({ name: '', unit: 'kg', cost: '' }); // Reset form
            loadData(); // Refresh table
        } catch (error) {
            console.error("Error creating material", error);
            alert("Failed to save.");
        }
    };

    // 3. Handle Delete
    const handleDelete = async (id) => {
        if (confirm("Delete this material?")) {
            await deleteRawMaterial(id);
            loadData();
        }
    };

    return (
        <div className="p-6">
            <h2 className="text-2xl font-bold mb-4">Raw Material Master</h2>

            {/* Simple Form Section */}
            <div className="bg-white p-4 rounded shadow mb-6">
                <h3 className="text-lg font-semibold mb-2">Add New Material</h3>
                <form onSubmit={handleSubmit} className="flex gap-4 items-end">
                    <div>
                        <label className="block text-sm">Name</label>
                        <input 
                            type="text" 
                            className="border p-2 rounded" 
                            placeholder="e.g. Steel"
                            value={formData.name}
                            onChange={(e) => setFormData({...formData, name: e.target.value})}
                        />
                    </div>
                    <div>
                        <label className="block text-sm">Unit</label>
                        <select 
                            className="border p-2 rounded w-24"
                            value={formData.unit}
                            onChange={(e) => setFormData({...formData, unit: e.target.value})}
                        >
                            <option value="kg">kg</option>
                            <option value="ltr">ltr</option>
                            <option value="pcs">pcs</option>
                            <option value="mtr">mtr</option>
                        </select>
                    </div>
                    <div>
                        <label className="block text-sm">Cost</label>
                        <input 
                            type="number" 
                            className="border p-2 rounded w-24" 
                            placeholder="0.00"
                            value={formData.cost}
                            onChange={(e) => setFormData({...formData, cost: e.target.value})}
                        />
                    </div>
                    <button type="submit" className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700">
                        Save
                    </button>
                </form>
            </div>

            {/* Table Section */}
            <div className="bg-white rounded shadow overflow-hidden">
                <table className="w-full text-left border-collapse">
                    <thead className="bg-gray-100">
                        <tr>
                            <th className="p-3 border-b">ID</th>
                            <th className="p-3 border-b">Name</th>
                            <th className="p-3 border-b">Unit</th>
                            <th className="p-3 border-b">Cost</th>
                            <th className="p-3 border-b">Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {loading ? <tr><td colSpan="5" className="p-4 text-center">Loading...</td></tr> : 
                         materials.map((rm) => (
                            <tr key={rm.id} className="hover:bg-gray-50">
                                <td className="p-3 border-b">#{rm.id}</td>
                                <td className="p-3 border-b">{rm.name}</td>
                                <td className="p-3 border-b">{rm.unit}</td>
                                <td className="p-3 border-b">${rm.cost}</td>
                                <td className="p-3 border-b">
                                    <button 
                                        onClick={() => handleDelete(rm.id)}
                                        className="text-red-500 hover:text-red-700"
                                    >
                                        Delete
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default RawMaterial;