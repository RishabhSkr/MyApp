import React, { useState, useEffect, useCallback } from 'react';
import { Package, Search, RefreshCw, Box, AlertCircle } from 'lucide-react';
import useApi from '../..//hooks/useApi'; 
import { getFinishedGoodsStock } from '../../api/finishedGoodsInventoryServices';

const FinishedGoodStock = () => {
    const [stockData, setStockData] = useState([]);
    const [searchTerm, setSearchTerm] = useState('');
    const { loading, requestHandlerFunction } = useApi();
    
    // Fetch Data Logic
    const fetchStock = useCallback(async () => {
        const response = await requestHandlerFunction(
            () => getFinishedGoodsStock()
        );

        if (response.success) {
            const backendData = response.data; 
            const safeData = Array.isArray(backendData) ? backendData : [];
            setStockData(safeData);
        }
    }, [requestHandlerFunction]);

    //  Initial Load
    useEffect(() => {
        fetchStock();
    }, [fetchStock]);

    //  Filtering Logic (Client Side)
    const filteredStock = stockData.filter(item => 
        item.productName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        item.inventoryId.toString().includes(searchTerm)
    );

    // Helper
    const formatDate = (dateString) => {
        if (!dateString) return '-';
        return new Date(dateString).toLocaleDateString('en-US', {
            year: 'numeric', month: 'short', day: 'numeric',
            hour: '2-digit', minute: '2-digit'
        });
    };

    // Helper: Stock Status Color
    const getStockStatusColor = (qty) => {
        if (qty === 0) return 'bg-red-100 text-red-700 border-red-200';
        if (qty < 10) return 'bg-yellow-100 text-yellow-700 border-yellow-200'; // Low Stock
        return 'bg-green-100 text-green-700 border-green-200'; // Good Stock
    };

    return (
        <div className="p-6 max-w-7xl mx-auto space-y-6">
            
            {/* Header Section */}
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
                <div>
                    <h1 className="text-2xl font-bold text-slate-800 flex items-center gap-2">
                        <Box className="text-blue-600" /> Finished Goods Stock
                    </h1>
                    <p className="text-sm text-gray-500 mt-1">Real-time inventory of manufactured products</p>
                </div>

                <div className="flex gap-2 w-full md:w-auto">
                    {/* Search Bar */}
                    <div className="relative flex-1 md:w-64">
                        <Search className="absolute left-3 top-2.5 text-gray-400" size={18} />
                        <input 
                            type="text" 
                            placeholder="Search Product..." 
                            className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none transition-all"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>

                    {/* Refresh Button */}
                    <button 
                        onClick={fetchStock} 
                        disabled={loading}
                        className="p-2 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 text-gray-600 transition-colors disabled:opacity-50"
                        title="Refresh Data"
                    >
                        <RefreshCw size={20} className={loading ? "animate-spin" : ""} />
                    </button>
                </div>
            </div>

            {/* Table Section */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
                <div className="overflow-x-auto">
                    <table className="w-full text-left border-collapse">
                        <thead className="bg-slate-50 text-slate-600 uppercase text-xs font-bold border-b border-gray-200">
                            <tr>
                                <th className="p-4">Product Details</th>
                                <th className="p-4 text-center">Inventory ID</th>
                                <th className="p-4 text-center">Available Qty</th>
                                <th className="p-4 text-center">CreatedAt</th>
                                <th className="p-4">Last Updated</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-100 text-sm">
                            {loading && stockData.length === 0 ? (
                                <tr>
                                    <td colSpan="4" className="p-8 text-center text-gray-500">
                                        <div className="flex justify-center items-center gap-2">
                                            <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-blue-500"></div>
                                            Loading inventory...
                                        </div>
                                    </td>
                                </tr>
                            ) : filteredStock.length === 0 ? (
                                <tr>
                                    <td colSpan="4" className="p-10 text-center text-gray-400">
                                        <div className="flex flex-col items-center gap-2">
                                            <Package size={40} className="opacity-20" />
                                            <p>No products found in stock.</p>
                                        </div>
                                    </td>
                                </tr>
                            ) : (
                                filteredStock.map((item) => (
                                    <tr key={item.inventoryId} className="hover:bg-slate-50 transition-colors group">
                                        
                                        {/* Product Name */}
                                        <td className="p-4">
                                            <div className="font-bold text-slate-800 text-base">
                                                {item.productName}
                                            </div>
                                            <div className="text-xs text-gray-400 mt-0.5">
                                                PID: #{item.productId}
                                            </div>
                                        </td>

                                        {/* Inventory ID */}
                                        <td className="p-4 text-center font-mono text-gray-500">
                                            INV-{item.inventoryId}
                                        </td>

                                        {/* Quantity Badge */}
                                        <td className="p-4 text-center">
                                            <span className={`px-4 py-1.5 rounded-full font-bold text-sm border ${getStockStatusColor(item.availableQuantity)}`}>
                                                {item.availableQuantity.toLocaleString()}
                                            </span>
                                        </td>

                                        {/*  Created */}
                                        <td className="p-4 text-gray-500">
                                            <div className="flex flex-col">
                                                <span className="font-medium text-slate-700">
                                                    {formatDate(item.createdAt)}
                                                </span>
                                                <span className="text-xs opacity-70">
                                                    Created: {new Date(item.createdAt).toLocaleDateString()}
                                                </span>
                                            </div>
                                        </td>
                                        {/* Last Updated */}
                                        <td className="p-4 text-gray-500">
                                            <div className="flex flex-col">
                                                <span className="font-medium text-slate-700">
                                                    {formatDate(item.lastUpdated)}
                                                </span>
                                                {/* <span className="text-xs opacity-70">
                                                    Created: {new Date(item.createdAt).toLocaleDateString()}
                                                </span> */}
                                            </div>
                                        </td>
                                    </tr>
                                ))
                            )}
                        </tbody>
                    </table>
                </div>
                
                {/* Footer / Info */}
                <div className="bg-gray-50 p-3 border-t border-gray-200 text-xs text-gray-500 flex justify-between items-center">
                    <span>Showing {filteredStock.length} records</span>
                    {filteredStock.length > 0 && (
                        <span className="flex items-center gap-1">
                            <AlertCircle size={12} /> Stock levels update automatically on production completion.
                        </span>
                    )}
                </div>
            </div>
        </div>
    );
};

export default FinishedGoodStock;