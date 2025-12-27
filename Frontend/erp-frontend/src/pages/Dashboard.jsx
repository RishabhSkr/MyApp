import React, { useEffect, useState } from 'react';
import { productionService } from '../api/productionService';
import { Play, AlertCircle, CheckCircle2, Clock } from 'lucide-react';

const Dashboard = () => {
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);

    
    // 1. Fetch Data on Load
    useEffect(() => {
        const loadDashboardData = async () => {
            try {
                const response = await productionService.getPendingOrders();
                console.log("API Response:", response); // Check this in your browser console!
                
                // SAFETY CHECK: Ensure response.data exists, otherwise default to empty array
                setOrders(response?.data || []); 
                setLoading(false);
            } catch (error) {
                console.error("Failed to load dashboard:", error);
                setOrders([]); // Set empty array on error so app doesn't crash
                setLoading(false);
            }
        };
        loadDashboardData();
    }, []);
    
    // 2. Helper for Status Badge Color
    const getStatusColor = (status) => {
        switch (status) {
            case 'Producing': return 'bg-blue-100 text-blue-800';
            case 'Partially Planned': return 'bg-yellow-100 text-yellow-800';
            case 'New': return 'bg-gray-100 text-gray-800';
            default: return 'bg-gray-100 text-gray-800';
        }
    };

    if (loading) return <div className="p-8 text-center">Loading Dashboard...</div>;

    return (
        <div className="space-y-6">
            <div className="p-8">
                <h1 className="text-2xl font-bold text-slate-800">Dashboard</h1>
            </div>
            {/* Header */}
            <div className="flex justify-between items-center">
                <h1 className="text-2xl font-bold text-slate-800">Production Planning</h1>
                <div className="text-sm text-gray-500">
                    Total Pending Orders: <span className="font-bold text-slate-900">{orders.length}</span>
                </div>
            </div>

            {/* Empty State */}
            {orders.length === 0 && (
                <div className="bg-white p-8 rounded-lg shadow text-center text-gray-500">
                    No pending orders found. Good job! 🎉
                </div>
            )}

            {/* Orders Table */}
            {orders.length > 0 && (
                <div className="bg-white rounded-lg shadow overflow-hidden border border-gray-200">
                    <table className="w-full text-left border-collapse">
                        <thead className="bg-slate-50 text-slate-600 uppercase text-xs font-semibold">
                            <tr>
                                <th className="p-4 border-b">Order ID</th>
                                <th className="p-4 border-b">Customer / Product</th>
                                <th className="p-4 border-b">Progress</th>
                                <th className="p-4 border-b text-center">Breakdown</th>
                                <th className="p-4 border-b">Status</th>
                                <th className="p-4 border-b text-right">Action</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-100">
                            {orders.map((order) => (
                                <tr key={order.salesOrderId} className="hover:bg-slate-50 transition-colors">
                                    {/* ID & Date */}
                                    <td className="p-4">
                                        <div className="font-bold text-slate-700">#{order.salesOrderId}</div>
                                        <div className="text-xs text-gray-400">
                                            {new Date(order.orderDate).toLocaleDateString()}
                                        </div>
                                    </td>

                                    {/* Customer Info */}
                                    <td className="p-4">
                                        <div className="font-medium text-slate-900">{order.customerName}</div>
                                        <div className="text-sm text-slate-500">{order.productName}</div>
                                    </td>

                                    {/* Progress Bar */}
                                    <td className="p-4 w-1/4">
                                        <div className="flex justify-between text-xs mb-1">
                                            <span className="text-gray-500">Total: {order.totalQuantity}</span>
                                            <span className="font-bold text-blue-600">{order.progressPercentage}%</span>
                                        </div>
                                        <div className="w-full bg-gray-200 rounded-full h-2.5">
                                            <div 
                                                className="bg-blue-600 h-2.5 rounded-full transition-all duration-500" 
                                                style={{ width: `${order.progressPercentage}%` }}
                                            ></div>
                                        </div>
                                    </td>

                                    {/* Detailed Stats (Produced / Pipeline / Pending) */}
                                    <td className="p-4 text-xs text-center">
                                        <div className="flex gap-4 justify-center">
                                            <div className="flex flex-col items-center">
                                                <span className="text-green-600 font-bold">{order.producedQuantity}</span>
                                                <span className="text-gray-400">Done</span>
                                            </div>
                                            <div className="flex flex-col items-center">
                                                <span className="text-blue-600 font-bold">{order.inPipelineQuantity}</span>
                                                <span className="text-gray-400">WIP</span>
                                            </div>
                                            <div className="flex flex-col items-center">
                                                <span className="text-red-500 font-bold">{order.unplannedQuantity}</span>
                                                <span className="text-gray-400">Pending</span>
                                            </div>
                                        </div>
                                    </td>

                                    {/* Status Badge */}
                                    <td className="p-4">
                                        <span className={`px-3 py-1 rounded-full text-xs font-medium ${getStatusColor(order.status)}`}>
                                            {order.status}
                                        </span>
                                    </td>

                                    {/* Action Button */}
                                    <td className="p-4 text-right">
                                        <button 
                                            className="bg-slate-900 hover:bg-slate-700 text-white px-4 py-2 rounded-md text-sm flex items-center gap-2 ml-auto transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                                            disabled={order.unplannedQuantity === 0}
                                            onClick={() => alert(`Open Planning Modal for SO #${order.salesOrderId}`)}
                                        >
                                            <Play size={16} />
                                            Plan Batch
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};

export default Dashboard;