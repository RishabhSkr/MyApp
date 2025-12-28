import React, { useState } from 'react';
import { useRawMaterials } from '../../hooks/useRawMaterials'; // Adjust path as needed
import { Plus, Package, Save } from 'lucide-react';

const AddRawMaterialStock = () => {
    const { materials, loading, addRawMaterialStock } = useRawMaterials();
    // Local State for Form
    const [formData, setFormData] = useState({
        rawMaterialId: '',
        quantity: ''
    });

    // Handle Input Change
    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    //  Submit Handler
    const handleSubmit = async (e) => {
        e.preventDefault();

        // Validation
        if (!formData.rawMaterialId || !formData.quantity) return;

        // Payload to be sent
        const payload = {
            rawMaterialId: parseInt(formData.rawMaterialId),
            quantity: parseFloat(formData.quantity)
        };

        const success = await addRawMaterialStock(payload);

        // Reset
        if (success) {
            setFormData({
                rawMaterialId: '',
                quantity: ''
            });
        }
    };

    return (
        <div className="max-w-xl mx-auto mt-10">
            <div className="bg-white rounded-xl shadow-md border border-gray-100 overflow-hidden">
                
                {/* Header */}
                <div className="bg-slate-50 p-6 border-b border-gray-100 flex items-center gap-3">
                    <div className="bg-blue-100 p-2 rounded-lg text-blue-600">
                        <Package size={24} />
                    </div>
                    <div>
                        <h2 className="text-xl font-bold text-slate-800">Add Stock</h2>
                        <p className="text-sm text-gray-500">Update inventory levels for raw materials</p>
                    </div>
                </div>

                {/* Form */}
                <div className="p-6">
                    <form onSubmit={handleSubmit} className="space-y-6">
                        
                        {/* 1. Select Material Dropdown */}
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Select Raw Material
                            </label>
                            <div className="relative">
                                <select
                                    name="rawMaterialId"
                                    value={formData.rawMaterialId}
                                    onChange={handleChange}
                                    className="w-full border border-gray-300 rounded-lg p-3 bg-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition-all"
                                    required
                                >
                                    <option value="">-- Choose Material --</option>
                                    {materials.length > 0 ? (
                                        materials.map((mat) => (
                                            <option key={mat.id || mat.rawMaterialId} value={mat.id || mat.rawMaterialId}>
                                                {mat.name} (Current Stock: {mat.inventories && mat.inventories.length > 0 
                                            ? mat.inventories[0].availableQuantity 
                                            : 0} {mat.uom})
                                            </option>
                                        ))
                                    ) : (
                                        <option disabled>Loading materials...</option>
                                    )}
                                </select>
                            </div>
                        </div>

                        {/* 2. Quantity Input */}
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">
                                Quantity to Add
                            </label>
                            <div className="relative">
                                <input
                                    type="number"
                                    name="quantity"
                                    step="0.01"
                                    value={formData.quantity}
                                    onChange={handleChange}
                                    placeholder="e.g. 50.5"
                                    className="w-full border border-gray-300 rounded-lg p-3 pl-10 focus:ring-2 focus:ring-blue-500 outline-none transition-all"
                                    required
                                    min="0.01"
                                />
                                <Plus size={18} className="absolute left-3 top-3.5 text-gray-400" />
                            </div>
                            <p className="text-xs text-gray-500 mt-1">
                                Enter the amount to increase. Negative values are not allowed here.
                            </p>
                        </div>

                        {/* Submit Button */}
                        <button
                            type="submit"
                            disabled={loading}
                            className={`w-full py-3 px-4 rounded-lg text-white font-medium flex items-center justify-center gap-2 transition-all ${
                                loading 
                                ? 'bg-blue-400 cursor-not-allowed' 
                                : 'bg-blue-600 hover:bg-blue-700 shadow-lg hover:shadow-blue-500/30'
                            }`}
                        >
                            {loading ? (
                                <span>Updating Inventory...</span>
                            ) : (
                                <>
                                    <Save size={20} />
                                    Update Stock
                                </>
                            )}
                        </button>
                    </form>
                </div>
            </div>
        </div>
    );
};

export default AddRawMaterialStock;