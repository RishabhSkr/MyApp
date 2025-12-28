import { useEffect, useState } from 'react';
import { getProducts, 
        createProduct, 
        deleteProduct } from '../../api/master/product';
import { Trash2, Plus, Package, Calendar, User } from 'lucide-react';
import toast from 'react-hot-toast';

const Products = () => {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(false);
    const [isModalOpen, setIsModalOpen] = useState(false);
    
    const [formData, setFormData] = useState({ 
        name: '', 
        maxDailyCapacity: 100,  
    });

    const loadData = async () => {
        setLoading(true);
        try {
            const data = await getProducts();
            setProducts(data);
        } catch (error) {
            console.error("Failed to fetch products", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadData();
    }, []);

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await createProduct({
                name: formData.name,
                maxDailyCapacity: parseInt(formData.maxDailyCapacity)
            });
            toast.success("Product Created!");
            setIsModalOpen(false);
            setFormData({ name: '', maxDailyCapacity: 100 });
            loadData();
        } catch (error) {
            console.error("Error creating product", error);
            alert("Failed to save product.");
        }
    };

    const handleDelete = async (id) => {
        if(confirm("Are you sure you want to delete this product?")) {
            try {
                await deleteProduct(id);
                loadData();
            } catch (error) {
                console.error("Delete failed", error);
                alert("Failed to delete product");
            }
        }
    };

    return (
        <div className="p-6">
            <div className="flex justify-between items-center mb-6">
                <h2 className="text-2xl font-bold flex items-center gap-2">
                    <Package className="text-blue-600" />
                    Product Master (Finished Goods)
                </h2>
                <button 
                    onClick={() => setIsModalOpen(true)}
                    className="bg-blue-600 text-white px-4 py-2 rounded flex items-center gap-2 hover:bg-blue-700"
                >
                    <Plus size={18} /> Add Product
                </button>
            </div>

            <div className="bg-white rounded shadow text-sm">
                <table className="w-full text-left border-collapse">
                    <thead className="bg-gray-50 border-b">
                        <tr>
                            <th className="p-4 font-medium text-gray-500">ID</th>
                            <th className="p-4 font-medium text-gray-500">Name</th>
                            <th className="p-4 font-medium text-gray-500">Max Capacity / Day</th>
                            <th className="p-4 font-medium text-gray-500">Created At</th>
                            <th className="p-4 font-medium text-gray-500">Updated At</th>
                            <th className="p-4 font-medium text-gray-500">Updated By</th>
                            <th className="p-4 font-medium text-gray-500">Action</th>
                        </tr>
                    </thead>
                    <tbody>
                        {loading ? (
                            <tr><td colSpan="5" className="p-4 text-center">Loading...</td></tr>
                        ) : products.length === 0 ? (
                            <tr><td colSpan="5" className="p-4 text-center text-gray-400">No products found</td></tr>
                        ) : (
                            products.map((p) => (
                                <tr key={p.id} className="hover:bg-gray-50 border-b last:border-0">
                                    <td className="p-4 text-gray-600">#{p.id}</td>
                                    <td className="p-4 font-medium text-gray-900">{p.name}</td>
                                    <td className="p-4 text-gray-900 font-semibold">{p.maxDailyCapacity} units</td>
                                    <td className="p-4 text-gray-500 text-xs">
                                        <div className="flex items-center gap-1">
                                            <Calendar size={12} />
                                            {p.createdAt ? new Date(p.createdAt).toLocaleDateString() : '-'}
                                        </div>
                                    </td>
                                    <td className="p-4 text-gray-500 text-xs">
                                        <div className="flex items-center gap-1">
                                            <Calendar size={12} />
                                            {p.updatedAt ? new Date(p.updatedAt).toLocaleDateString() : '-'}
                                        </div>
                                    </td>
                                    <td className="p-4 text-gray-500 text-xs">
                                        <div className="flex items-center gap-1">
                                            <User size={12} />
                                            {p.updatedByUserId ? p.updatedByUserId : '-'}
                                        </div>
                                    </td>
                                    <td className="p-4">
                                        <button 
                                            onClick={() => handleDelete(p.id)}
                                            className="text-red-400 hover:text-red-600 p-1 rounded hover:bg-red-50"
                                        >
                                            <Trash2 size={18} />
                                        </button>
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>

            {/* Modal */}
            {isModalOpen && (
                <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
                    <div className="bg-white p-6 rounded-lg shadow-xl w-96 animate-in slide-in-from-bottom-5">
                        <h3 className="text-xl font-bold mb-4">Add New Product</h3>
                        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                            <div>
                                <label className="block text-sm font-medium mb-1">Product Name</label>
                                <input 
                                    className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none" 
                                    placeholder="e.g. FG-CHAIR-001"
                                    value={formData.name}
                                    onChange={e => setFormData({...formData, name: e.target.value})}
                                    required
                                />
                            </div>
                            
                            <div>
                                <label className="block text-sm font-medium mb-1">Max Daily Capacity</label>
                                <input 
                                    type="number"
                                    className="w-full border p-2 rounded focus:ring-2 focus:ring-blue-500 outline-none" 
                                    value={formData.maxDailyCapacity}
                                    onChange={e => setFormData({...formData, maxDailyCapacity: e.target.value})}
                                    required
                                />
                            </div>
                            
                            <div className="flex justify-end gap-2 mt-4">
                                <button 
                                    type="button" 
                                    onClick={() => setIsModalOpen(false)}
                                    className="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded"
                                >
                                    Cancel
                                </button>
                                <button 
                                    type="submit" 
                                    className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
                                >
                                    Save Product
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
};

export default Products;
