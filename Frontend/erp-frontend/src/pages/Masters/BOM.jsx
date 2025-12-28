import React, { useEffect, useState } from 'react';
import { getProducts } from '../../api/master/product';       
import { getRawMaterials } from '../../api/master/rawMaterial';
import { Layers, Save, Plus, Trash2, Edit2, AlertCircle, Calendar, User, Info } from 'lucide-react';
import { useBom } from '../../hooks/useBom';

const BOM = () => {

    const [products, setProducts] = useState([]);
    const [materials, setMaterials] = useState([]);
    const [selectedProduct, setSelectedProduct] = useState('');
    
    // Editor State
    const [bomItems, setBomItems] = useState([]); 
    const [bomMetadata, setBomMetadata] = useState(null); 
    const [isEditMode, setIsEditMode] = useState(false); 

    // Hook integration 
    const { loading, createBOM, updateBOM, deleteBOM, getBOMByProductId,boms} = useBom();
    
    //  Load Masters
    useEffect(() => {
        const fetchMasters = async () => {
            try {
                const [pData, mData] = await Promise.all([
                    getProducts(), 
                    getRawMaterials()
                ]);
                setProducts(pData); 
                setMaterials(mData); 
            } catch (error) {
                console.error('Error loading master data', error);
            }
        };
        fetchMasters();
    }, []);

    // 3. Handle Product Selection (Load Recipe)
    const handleProductChange = (e) => {
        const productId = e.target.value;
        loadRecipe(productId);
    };

    const loadRecipe = async (productId) => {
        setSelectedProduct(productId);
        if (!productId) {
            setBomItems([]);
            setBomMetadata(null);
            setIsEditMode(false);
            return;
        }

        try {
            const data = await getBOMByProductId(productId);
            
            // Backend returns { productId, productName, materials: [...] }
            if (data && data.materials && data.materials.length > 0) {
                setIsEditMode(true);
                setBomMetadata({
                    createdAt: data.createdAt,
                    updatedAt: data.updatedAt,
                    createdBy: data.createdByUserId,
                    updatedBy: data.updatedByUserId
                });
                setBomItems(data.materials.map(item => ({
                    rawMaterialId: item.rawMaterialId,
                    rawMaterialName: item.rawMaterialName,
                    quantity: item.quantityRequired,
                    uom: item.uom,  
                    sku: item.sku
                })));
            } else {
                setIsEditMode(false);
                setBomMetadata(null);
                setBomItems([]); 
            }
        } catch (error) {
            console.log('Error or No BOM found', error);
            setBomItems([]);
            setBomMetadata(null);
            setIsEditMode(false);
        }
    };

    //  UI Handlers
    const addRow = () => {
        setBomItems([...bomItems, { rawMaterialId: '', quantity: 1, uom: '-' }]);
    };
    const removeRow = (index) => {
        const newItems = bomItems.filter((_, i) => i !== index);
        setBomItems(newItems);
    };

    const updateRow = (index, field, value) => {
        const newItems = [...bomItems];
        newItems[index][field] = value;
        
        // Auto-populate UOM and SKU if material changes
        if(field === 'rawMaterialId') {
            const mat = materials.find(m => m.id == value);
            if(mat) {
                newItems[index].uom = mat.unit || '-';
                newItems[index].sku = mat.sku || '-'; // Assuming material has SKU
            }
        }
        
        setBomItems(newItems);
    };

    // 5. Save Logic
    const handleSave = async () => {
        if (!selectedProduct) return alert('Select a product first.');

        const validItems = bomItems.filter(i => i.rawMaterialId && i.quantity > 0);
        if (validItems.length === 0) return alert('Add at least one material.');

        const payload = {
            productId: parseInt(selectedProduct),
            bomItems: validItems.map(i => ({
                rawMaterialId: parseInt(i.rawMaterialId),
                quantity: parseFloat(i.quantity),
            })),
        };

        if (isEditMode) {
            await updateBOM(selectedProduct, payload);
        } else {
            await createBOM(payload);
            setIsEditMode(true);
        }
    };

    // 6. Delete Logic
    const handleDelete = async () => {
        if(!selectedProduct) return;
        if(confirm("Are you sure you want to delete this recipe?")) {
            await deleteBOM(selectedProduct);
            // Reset UI
            setSelectedProduct('');
            setBomItems([]);
            setBomMetadata(null);
            setIsEditMode(false);
        }
    };

    return (
        <div className="p-4 lg:p-6 max-w-7xl mx-auto flex flex-col lg:flex-row gap-6 items-start">
            
            {/* Left Column: BOM Editor */}
            <div className="flex-1 w-full min-w-0">
                <h2 className="text-2xl font-bold mb-6 flex items-center gap-2">
                    <Layers className="text-purple-600" />
                    Recipe Manager
                </h2>

                {/* Product Selector */}
                <div className="bg-white p-4 lg:p-6 rounded shadow mb-6">
                    <label className="block text-sm font-medium mb-2">Select Product</label>
                    <select
                        className="w-full border p-2 rounded text-lg font-medium focus:ring-2 focus:ring-purple-500"
                        value={selectedProduct}
                        onChange={handleProductChange} 
                    >
                        <option value="">-- Choose Product --</option>
                        {products.map(p => (
                            <option key={p.id} value={p.id}>{p.name}</option>
                        ))}
                    </select>
                </div>

                {/* Recipe Editor */}
                {selectedProduct && (
                    <div className="bg-white p-4 lg:p-6 rounded shadow animate-in fade-in slide-in-from-bottom-4">
                        
                        {/* Header & Actions */}
                        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6 gap-4">
                            <h3 className="text-lg font-semibold flex items-center gap-2">
                                {isEditMode ? 'Edit Recipe' : 'Create New Recipe'}
                                {loading && <span className="text-xs text-gray-500 animate-pulse">(Saving...)</span>}
                            </h3>
                            
                            <div className="flex gap-2 w-full sm:w-auto">
                                {isEditMode && (
                                    <button 
                                        onClick={handleDelete}
                                        className="text-red-500 hover:bg-red-50 px-3 py-1 rounded flex items-center gap-1 text-sm border border-red-100"
                                    >
                                        <Trash2 size={16} /> Delete
                                    </button>
                                )}
                                <button
                                    onClick={addRow}
                                    className="bg-blue-50 text-blue-600 hover:bg-blue-100 px-4 py-1.5 rounded flex items-center gap-1 text-sm font-medium transition ml-auto sm:ml-0"
                                >
                                    <Plus size={16} /> Add Material
                                </button>
                            </div>
                        </div>

                        {/* Metadata Panel */}
                        {bomMetadata && isEditMode && (
                            <div className="bg-gray-50 p-4 rounded-lg mb-6 border border-gray-100 grid grid-cols-2 md:grid-cols-4 gap-4 text-xs text-gray-600">
                                <div>
                                    <span className="block text-gray-400 mb-1 flex items-center gap-1"><Calendar size={12}/> Created At</span>
                                    <span className="font-medium">{bomMetadata.createdAt ? new Date(bomMetadata.createdAt).toLocaleDateString() : '-'}</span>
                                </div>
                                <div>
                                    <span className="block text-gray-400 mb-1 flex items-center gap-1"><User size={12}/> Created By</span>
                                    <span className="font-medium">User #{bomMetadata.createdBy || '-'}</span>
                                </div>
                                <div>
                                    <span className="block text-gray-400 mb-1 flex items-center gap-1"><Calendar size={12}/> Updated At</span>
                                    <span className="font-medium">{bomMetadata.updatedAt ? new Date(bomMetadata.updatedAt).toLocaleDateString() : '-'}</span>
                                </div>
                                <div>
                                    <span className="block text-gray-400 mb-1 flex items-center gap-1"><User size={12}/> Updated By</span>
                                    <span className="font-medium">User #{bomMetadata.updatedBy || '-'}</span>
                                </div>
                            </div>
                        )}

                        {/* Materials Table */}
                        <div className="overflow-x-auto rounded-lg border border-gray-200 mb-6">
                            <table className="w-full text-sm text-left">
                                <thead className="text-xs text-gray-700 uppercase bg-gray-50 border-b">
                                    <tr>
                                        <th className="px-4 py-3 w-10">#</th>
                                        <th className="px-4 py-3 min-w-[180px]">Material</th>
                                        <th className="px-4 py-3 w-32">SKU</th>
                                        <th className="px-4 py-3 w-32 text-center">Qty</th>
                                        <th className="px-4 py-3 w-16"></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {bomItems.map((item, index) => (
                                        <tr key={index} className="bg-white border-b hover:bg-gray-50 last:border-0">
                                            <td className="px-4 py-3 text-gray-400 font-mono">{index + 1}</td>
                                            <td className="px-4 py-3">
                                                 <select
                                                    className="w-full border p-2 rounded bg-white focus:outline-none focus:ring-1 focus:ring-blue-500"
                                                    value={item.rawMaterialId}
                                                    onChange={e => updateRow(index, 'rawMaterialId', e.target.value)}
                                                >
                                                    <option value="">Select Material</option>
                                                    {materials.map(m => (
                                                        <option key={m.id} value={m.id}>{m.name} ({m.uom})</option>
                                                    ))}
                                                </select>
                                            </td>
                                            <td className="px-4 py-3 text-gray-500 font-mono text-xs">
                                                {item.sku || '-'}
                                            </td>
                                            <td className="px-4 py-3">
                                                 <input
                                                    type="number"
                                                    className="w-full border p-1.5 rounded text-center focus:outline-none focus:ring-1 focus:ring-blue-500"
                                                    placeholder="0"
                                                    min="0"
                                                    value={item.quantity}
                                                    onChange={e => updateRow(index, 'quantity', e.target.value)}
                                                />
                                            </td>
                                            <td className="px-4 py-3 text-right">
                                                 <button 
                                                    onClick={() => removeRow(index)} 
                                                    className="text-gray-400 hover:text-red-500 p-1.5 rounded hover:bg-red-50 transition"
                                                    title="Remove Item"
                                                >
                                                    <Trash2 size={16} />
                                                </button>
                                            </td>
                                        </tr>
                                    ))}
                                    {bomItems.length === 0 && (
                                        <tr>
                                            <td colSpan="6" className="px-4 py-8 text-center text-gray-400 bg-gray-50/30">
                                                <div className="flex flex-col items-center gap-2">
                                                    <Info size={24} className="opacity-20"/>
                                                    <span>No raw materials added yet.</span>
                                                    <button onClick={addRow} className="text-blue-600 hover:underline text-xs">Add First Item</button>
                                                </div>
                                            </td>
                                        </tr>
                                    )}
                                </tbody>
                            </table>
                        </div>

                        <div className="flex justify-end">
                            <button
                                onClick={handleSave}
                                disabled={loading}
                                className={`px-6 py-2.5 rounded flex items-center gap-2 shadow-lg text-white font-medium transition transform active:scale-95
                                    ${loading ? 'bg-purple-400 cursor-not-allowed' : 'bg-purple-600 hover:bg-purple-700'}`}
                            >
                                <Save size={18} /> 
                                {loading ? 'Saving...' : 'Save Recipe'}
                            </button>
                        </div>
                    </div>
                )}

                 {!selectedProduct && (
                     <div className="text-center py-12 text-gray-400 bg-white/50 rounded border-2 border-dashed border-gray-200">
                         <AlertCircle size={48} className="mx-auto mb-2 opacity-20" />
                         <p>Select a product to view or create its recipe.</p>
                     </div>
                 )}
            </div>

            {/* Right Column: Existing Recipes List */}
            <div className="w-full lg:w-80 bg-white rounded shadow h-fit border border-gray-100 flex-shrink-0">
                <div className="p-4 border-b bg-gray-50 flex justify-between items-center">
                    <h3 className="font-semibold text-gray-700">Existing Recipes</h3>
                    <span className="bg-gray-200 text-gray-600 px-2 py-0.5 rounded-full text-xs font-bold">{boms.length}</span>
                </div>
                <div className="max-h-[500px] overflow-y-auto">
                    {boms.length === 0 ? (
                        <div className="p-8 text-center text-gray-400 text-sm">
                            <p>No recipes found.</p>
                            <p className="text-xs mt-1 opacity-70">Create one to get started.</p>
                        </div>
                    ) : (
                        boms.map((bom) => (
                            <button 
                                key={bom.productId}
                                onClick={() => loadRecipe(bom.productId)}
                                className={`w-full text-left p-4 border-b hover:bg-purple-50 transition flex justify-between items-center group
                                    ${selectedProduct == bom.productId ? 'bg-purple-50 border-l-4 border-purple-500' : 'border-l-4 border-transparent'}`}
                            >
                                <div className="min-w-0">
                                    <h4 className="font-medium text-gray-800 truncate">{bom.productName}</h4>
                                    <p className="text-xs text-gray-500 flex items-center gap-1">
                                        <Layers size={10}/> {bom.materials?.length || 0} Ingredients
                                    </p>
                                </div>
                                <Edit2 size={16} className={`text-gray-300 group-hover:text-purple-500 transition ${selectedProduct == bom.productId ? 'text-purple-500' : ''}`} />
                            </button>
                        ))
                    )}
                </div>
            </div>

        </div>
    );
};

export default BOM;