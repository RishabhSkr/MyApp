import React, { useEffect, useState } from 'react';
import { getPendingSalesOrders } from '../../api/salesService';
import { createPlan } from '../../api/productionService'; 
import useApi from '../../hooks/useApi';
import { Save, ArrowLeft, Info, Calendar } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-hot-toast';

const CreateOrder = () => {
    const navigate = useNavigate();
    const { loading, requestHandlerFunction } = useApi();

    // Data State
    const [pendingOrders, setPendingOrders] = useState([]);
    const [selectedOrder, setSelectedOrder] = useState(null);

    // Form State
    const [formData, setFormData] = useState({
        salesOrderId: '',
        quantityToProduce: '',
        startDate: '',
        endDate: ''
    });

    //  Load Pending Sales Orders 
    useEffect(() => {
        const loadOrders = async () => {
            const response = await requestHandlerFunction(
                () => getPendingSalesOrders()
            );
            if (response.success) {
                setPendingOrders(response.data?.data|| []);
            }
        };
        loadOrders();
    }, []);

    // Handle Dropdown Change 
    const handleOrderSelect = (e) => {
        const orderId = e.target.value;
        
        if (!orderId) {
            setSelectedOrder(null);
            setFormData({ ...formData, salesOrderId: '', quantityToProduce: '' });
            return;
        }

        // Find full object from array to show details
        const order = pendingOrders.find(o => o.id == orderId);
        setSelectedOrder(order);

        setFormData({
            ...formData,
            salesOrderId: orderId,
            quantityToProduce: order.quantity 
        });
    };

    //  Submit Handler
    const handleSubmit = async (e) => {
        e.preventDefault();

        // Validation
        if (!selectedOrder || !formData.startDate || !formData.endDate) {
            toast.error("Please fill all required fields");
            return;
        }

        const payload = {
            salesOrderId: parseInt(formData.salesOrderId),
            quantityToProduce: parseInt(formData.quantityToProduce),
            plannedStartDate: new Date(formData.startDate).toISOString(),
            plannedEndDate: new Date(formData.endDate).toISOString()
        };

        const response = await requestHandlerFunction(
            () => createPlan(payload),
            "Production Plan Created Successfully!"
        );

        if (response.success) {
            navigate('/production-plan'); 
        }
    };

    return (
        <div className="max-w-4xl mx-auto p-6">
            
            {/* Page Header */}
            <div className="flex items-center gap-4 mb-6">
                <button onClick={() => navigate(-1)} className="p-2 hover:bg-gray-100 rounded-full">
                    <ArrowLeft size={24} className="text-gray-600" />
                </button>
                <div>
                    <h1 className="text-2xl font-bold text-slate-800">Create Production Order</h1>
                    <p className="text-sm text-gray-500">Plan a new batch from pending sales orders</p>
                </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                
                {/* LEFT: The Form */}
                <div className="md:col-span-2 bg-white p-6 rounded-lg shadow border border-gray-200">
                    <form onSubmit={handleSubmit} className="space-y-6">
                        
                        {/* 1. Sales Order Selection */}
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">Select Pending Order</label>
                            <select
                                className="w-full border border-gray-300 rounded-lg p-2.5 focus:ring-2 focus:ring-blue-500 outline-none bg-white"
                                value={formData.salesOrderId}
                                onChange={handleOrderSelect}
                                disabled={loading}
                            >
                                <option value="">-- Select a Sales Order --</option>
                                {pendingOrders.map(order => (
                                    <option key={order.id} value={order.id}>
                                        #{order.id} - {order.customerName} (Qty: {order.quantity})
                                    </option>
                                ))}
                            </select>
                            {pendingOrders.length === 0 && !loading && (
                                <p className="text-xs text-amber-600 mt-1">No pending orders found.</p>
                            )}
                        </div>

                        {/* 2. Quantity Input */}
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">Production Quantity</label>
                            <input 
                                type="number"
                                className="w-full border border-gray-300 rounded-lg p-2.5 outline-none focus:border-blue-500"
                                value={formData.quantityToProduce}
                                onChange={(e) => setFormData({...formData, quantityToProduce: e.target.value})}
                                placeholder="Enter Quantity"
                                required
                            />
                        </div>

                        {/* 3. Dates */}
                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">Start Date</label>
                                <input 
                                    type="date"
                                    className="w-full border border-gray-300 rounded-lg p-2.5 outline-none focus:border-blue-500"
                                    value={formData.startDate}
                                    onChange={(e) => setFormData({...formData, startDate: e.target.value})}
                                    required
                                />
                            </div>
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">End Date</label>
                                <input 
                                    type="date"
                                    className="w-full border border-gray-300 rounded-lg p-2.5 outline-none focus:border-blue-500"
                                    value={formData.endDate}
                                    min={formData.startDate} // End date cannot be before start
                                    onChange={(e) => setFormData({...formData, endDate: e.target.value})}
                                    required
                                />
                            </div>
                        </div>

                        {/* Submit Button */}
                        <div className="pt-4">
                            <button 
                                type="submit" 
                                disabled={loading || !selectedOrder}
                                className="w-full bg-blue-600 hover:bg-blue-700 text-white font-medium py-2.5 rounded-lg flex justify-center items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                            >
                                {loading ? 'Creating...' : <><Save size={18} /> Create Plan</>}
                            </button>
                        </div>
                    </form>
                </div>

                {/* RIGHT: Order Summary Card (Visual Confirmation) */}
                <div className="md:col-span-1">
                    {selectedOrder ? (
                        <div className="bg-blue-50 p-5 rounded-lg border border-blue-200 sticky top-6">
                            <h3 className="font-bold text-blue-800 flex items-center gap-2 mb-4">
                                <Info size={18} /> Order Summary
                            </h3>
                            
                            <div className="space-y-4 text-sm">
                                <div className="bg-white p-3 rounded border border-blue-100">
                                    <span className="block text-gray-500 text-xs uppercase">Customer</span>
                                    <span className="font-semibold text-gray-800 text-lg">{selectedOrder.customerName}</span>
                                </div>

                                <div className="bg-white p-3 rounded border border-blue-100">
                                    <span className="block text-gray-500 text-xs uppercase">Product</span>
                                    <span className="font-semibold text-gray-800">{selectedOrder.productName}</span>
                                </div>

                                <div className="flex gap-2">
                                    <div className="bg-white p-3 rounded border border-blue-100 flex-1">
                                        <span className="block text-gray-500 text-xs uppercase">Order Qty</span>
                                        <span className="font-bold text-slate-700">{selectedOrder.quantity}</span>
                                    </div>
                                    <div className="bg-white p-3 rounded border border-blue-100 flex-1">
                                        <span className="block text-gray-500 text-xs uppercase">Order Date</span>
                                        <span className="font-bold text-slate-700">
                                            {new Date(selectedOrder.orderDate).toLocaleDateString()}
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    ) : (
                        // Placeholder State
                        <div className="bg-gray-50 p-8 rounded-lg border border-dashed border-gray-300 text-center h-full flex flex-col justify-center items-center text-gray-400">
                            <Calendar size={48} className="mb-2 opacity-20" />
                            <p>Select a Sales Order to see details here.</p>
                        </div>
                    )}
                </div>

            </div>
        </div>
    );
};

export default CreateOrder;