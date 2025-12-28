import React, { useState } from 'react';
import { useRawMaterials } from '../../hooks/useRawMaterials';
import { Plus, Trash2 } from 'lucide-react';

const RawMaterial = () => {
    const { materials, loading, addMaterial } = useRawMaterials();
    console.log(materials);
    // Form state match backend DTO if possible, but UOM is mostly handled by select
    const [formData, setFormData] = useState({ name: '', sku: '', uom: 'Kg' });
    const deleteMaterial = async () => {
       return "Delete function to be implemented";
    }

    const handleSubmit = async (e) => {
        e.preventDefault();
        const success = await addMaterial(formData);
        if (success) {
            setFormData({ name: '', sku: "", uom: 'Kg' }); 
        }
    };

    const handleDelete = async (id) => {
        const success = await deleteMaterial(id);
        if (success) {
            setFormData({ name: '', sku: "", uom: 'Kg' }); 
        }
    };

    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <h1 className="text-2xl font-bold text-slate-800">Raw Material Management</h1>
            </div>

            {/* Quick Add Form */}
            <div className="bg-white p-6 rounded-lg shadow border border-gray-200">
                <h3 className="text-lg font-semibold mb-4 text-slate-700">Add New Material</h3>
                <form onSubmit={handleSubmit} className="flex flex-wrap gap-4 items-end">
                    <div className="flex-1 min-w-[200px]">
                        <label className="block text-sm font-medium text-gray-700 mb-1">Material Name</label>
                        <input 
                            type="text" required
                            className="w-full border border-gray-300 rounded px-3 py-2 outline-none focus:border-blue-500"
                            placeholder="e.g. Teak Wood"
                            value={formData.name}
                            onChange={(e) => setFormData({...formData, name: e.target.value})}
                        />
                    </div>
                    <div className="w-72">
                        <label className="block text-sm font-medium text-gray-700 mb-1">SKU Code</label>
                        <input 
                            type="text" required
                            placeholder="e.g. FG-Cell-A03"
                            className="w-full border border-gray-300 rounded px-3 py-2 outline-none focus:border-blue-500"
                            value={formData.sku}
                            onChange={(e) => setFormData({...formData, sku: e.target.value})}
                        />
                    </div>
                    <div className="w-32">
                        <label className="block text-sm font-medium text-gray-700 mb-1">Unit</label>
                        <select 
                            className="w-full border border-gray-300 rounded px-3 py-2 outline-none focus:border-blue-500"
                            value={formData.uom}
                            onChange={(e) => setFormData({...formData, uom: e.target.value})}
                        >
                            <option value="Kg">Kg</option>
                            <option value="Lts">Lts</option>
                            <option value="Pcs">Pcs</option>
                            <option value="m">m</option>
                        </select>
                    </div>
                    <button type="submit" className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded flex items-center gap-2">
                        <Plus size={18} /> Add
                    </button>
                </form>
            </div>

            {/* List Table */}
            <div className="bg-white rounded-lg shadow overflow-hidden border border-gray-200">
                <table className="w-full text-left border-collapse">
                    <thead className="bg-slate-50 border-b">
                        <tr>
                            <th className="p-4 font-semibold text-slate-600">Name</th>
                            <th className="p-4 font-semibold text-slate-600">SKU</th>
                            <th className="p-4 font-semibold text-slate-600">Current Stock</th> {/* New Column */}
                            <th className="p-4 font-semibold text-slate-600">Unit (UOM)</th>
                            <th className="p-4 font-semibold text-slate-600">CreatedBy</th>
                            <th className="p-4 font-semibold text-slate-600">CreatedAt</th>
                            <th className="p-4 font-semibold text-slate-600">UpdatedAt</th>
                            <th className="p-4 font-semibold text-slate-600">UpdatedBy </th>
                            <th className="p-4 font-semibold text-slate-600 text-right">Action</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100">
                        {loading ? (
                            <tr><td colSpan="5" className="p-6 text-center text-gray-500">Loading...</td></tr>
                        ) : materials.length === 0 ? (
                            <tr><td colSpan="5" className="p-6 text-center text-gray-500">No materials found.</td></tr>
                        ) : (
                            materials.map((rm) => {
                                // Logic to extract stock quantity safely
                                const stockQty = rm.inventories && rm.inventories.length > 0 
                                    ? rm.inventories[0].availableQuantity 
                                    : 0;

                                return (
                                    <tr key={rm.id} className="hover:bg-slate-50">
                                        <td className="p-4 font-medium text-slate-800">{rm.name}</td>
                                        <td className="p-4 text-slate-600 font-mono text-sm">{rm.sku}</td>
                                        <td className="p-4">{stockQty}</td>
                                        <td className="p-4 text-slate-600">{rm.uom}</td>
                                        <td className="p-4 text-slate-600">{rm.createdByUserId}</td>
                                        <td className="p-4 text-slate-600">{rm.createdAt ? new Date(rm.createdAt).toLocaleDateString() : '-'}</td>
                                        <td className="p-4 text-slate-600">{rm.updatedAt ? new Date(rm.updatedAt).toLocaleDateString() : '-'}</td>
                                        <td className="p-4 text-slate-600">{rm.updatedByUserId? rm.updatedByUserId : '-'}</td>
                                        
                                        <td className="p-4 text-right">
                                            <button onClick={() => handleDelete(rm.id)} className="text-red-500 hover:text-red-700 p-1 transition-colors">
                                                <Trash2 size={18} />
                                            </button>
                                        </td>
                                    </tr>
                                );
                            })
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default RawMaterial;