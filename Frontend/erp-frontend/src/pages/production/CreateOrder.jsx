import React, { useEffect, useState } from 'react';
import { createPlan, getNextOrderNumber, getPendingOrders, getPlanningInfo } from '../../api/productionService'; 
import useApi from '../../hooks/useApi';
import { Save, ArrowLeft, Info, Calendar } from 'lucide-react';
import { useNavigate,useLocation } from 'react-router-dom';
import { toast } from 'react-hot-toast';
import SmartBatchCard from '../../components/production/SmartBatchCard';
import { getEfficiencyWarning } from '../../utility/batchUtils';

const CreateOrder = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { loading, requestHandlerFunction } = useApi();
    // Data State
    const [pendingOrders, setPendingOrders] = useState([]);
    const [selectedOrder, setSelectedOrder] = useState(null);
    
    
    const [planningInfo, setPlanningInfo] = useState(null);
    // Form State
    const [formData, setFormData] = useState({
        salesOrderId: '',
        quantityToProduce: '',
        startDate: '',
        endDate: '',
        customOrderNumber: '',
        forceCreate: false,
    });

    const efficiencyWarning = getEfficiencyWarning(planningInfo, formData.quantityToProduce);
    console.log(efficiencyWarning)
    //  Load Pending Sales Orders    
    useEffect(() => {
        const loadOrders = async () => {
            const response = await requestHandlerFunction(
                () => getPendingOrders()
            );
            if (response.success) {
                setPendingOrders(response.data?.data|| []);
            }
        };
        loadOrders();
    }, [location.key]);
    
    const handleOrderSelect = async (e) => {
        const orderId = e.target.value;
        if (!orderId) {
            setSelectedOrder(null);
            setPlanningInfo(null);
            setFormData({ ...formData, salesOrderId: '', quantityToProduce: '', customOrderNumber: '' });
            return;
        }
        
        // salesOrderId se match karo (production dashboard data)
        const order = pendingOrders.find(o => o.salesOrderId == orderId);
        setSelectedOrder(order);
        
        // Auto-fetch next order number
        let nextNum = '';
        try {
            const res = await getNextOrderNumber();
            nextNum = res.data?.orderNumber || '';
        } catch(err) { console.error('Could not fetch order number'); }
        
        setFormData({
            ...formData,
            salesOrderId: orderId,
            quantityToProduce: order.unplannedQuantity,  // Only unplanned qty
            customOrderNumber: nextNum
        });

        // Fetch Smart Batch Suggestion
        try {
            const planRes = await getPlanningInfo(orderId);
            setPlanningInfo(planRes.data);
        } catch(err) { 
            console.error('Could not fetch planning info', err);
            setPlanningInfo(null);
        }
    };

    //  Submit Handler
    const handleSubmit = async (e) => {
        e.preventDefault();

        // Validation
        if (!selectedOrder || !formData.startDate || !formData.endDate) {
            toast.error("Please fill all required fields");
            return;
        }
        if(formData.quantityToProduce <= 0) {
            toast.error("Quantity must be greater than 0");
            return;
        }
        const payload = {
            salesOrderId: formData.salesOrderId,
            quantityToProduce: parseFloat(formData.quantityToProduce),
            plannedStartDate: new Date(formData.startDate).toISOString(),
            plannedEndDate: new Date(formData.endDate).toISOString(),
            forceCreate: formData.forceCreate,
            customOrderNumber: formData.customOrderNumber || null
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
        <div className="max-w-6xl mx-auto p-6">
            
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
                                    <option key={order.salesOrderId} value={order.salesOrderId}>
                                       SO#{order.salesOrderId.substring(0,8)}... 
                                       | {order.productName} 
                                       - Unplanned: {order.unplannedQuantity} / {order.totalQuantity}
                                    </option>
                                ))}
                            </select>
                                {selectedOrder && selectedOrder.unplannedQuantity === 0 && (
                                    <p className="text-xs text-red-500 mt-1">
                                        ⚠️ This order is fully planned (unplanned=0). Cancel existing batches to plan more.
                                    </p>
                                )}
                            {pendingOrders.length === 0 && !loading && (
                                <p className="text-xs text-amber-600 mt-1">No pending orders found.</p>
                            )}
                        </div>

                        {/* 1.5 Order Number (Auto-filled, editable) */}
                        {formData.customOrderNumber && (
                            <div>
                                <label className="block text-sm font-medium text-gray-700 mb-2">
                                    Order Number 
                                    <span className="text-xs text-gray-400 ml-1">(auto-generated, editable)</span>
                                </label>
                                <input 
                                    type="text"
                                    className="w-full border border-gray-300 rounded-lg p-2.5 outline-none focus:border-blue-500 font-mono"
                                    value={formData.customOrderNumber}
                                    onChange={(e) => setFormData({...formData, customOrderNumber: e.target.value})}
                                    placeholder="PO-YYYYMMDD-XXXX"
                                />
                            </div>
                        )}

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
                            {/* {check box show } */}
                           {efficiencyWarning && (
                            <div className="bg-amber-50 p-3 rounded-lg border border-amber-300 mt-2">
                                {efficiencyWarning?.type === "lowEfficiency" ? (
                                    <p className="text-amber-800 text-sm font-medium">
                                        ⚠️ Last batch efficiency: {efficiencyWarning.efficiency}% 
                                        ({efficiencyWarning.lastBatch}/{efficiencyWarning.capacity} per day)
                                    </p>
                                ) : (
                                    <p className="text-red-700 text-sm font-medium">
                                        ⚠️ Cannot produce more than remainingQuantity or DailyCapacity: {efficiencyWarning.availableQtyToProduce || efficiencyWarning.machineCapacity}
                                    </p>
                                )}
                            </div>
                        )}
                        {/* Force Create — always visible for efficiency + buffer cap */}
                        <label className="flex items-center gap-2 mt-2 cursor-pointer">
                            <input 
                                type="checkbox"
                                checked={formData.forceCreate}
                                onChange={(e) => setFormData({...formData, forceCreate: e.target.checked})}
                                className="w-4 h-4 accent-amber-600"
                            />
                            <span className="text-sm text-gray-600">Force Create (ignore batch efficiency warning)</span>
                        </label>
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
                                {loading ? 'Creating...' : <><Save size={18} /> Create Order</>}
                            </button>
                        </div>
                    </form>
                </div>

                {/* RIGHT: Order Summary Card (Visual Confirmation) */}
                <div className="md:col-span-1">
                    {selectedOrder ? (
                        <>
                        <div className="bg-blue-50 p-5 rounded-lg border border-blue-200 sticky top-6">
                            <h3 className="font-bold text-blue-800 flex items-center gap-2 mb-4">
                                <Info size={18} /> Order Summary
                            </h3>
                            
                            <div className="space-y-4 text-sm">
                                <div className="bg-white p-3 rounded border border-blue-100">
                                    <span className="block text-gray-500 text-xs uppercase">Product</span>
                                    <span className="font-semibold text-gray-800 text-lg">{selectedOrder.productName}</span>
                                </div>

                                <div className="flex gap-2">
                                    <div className="bg-white p-3 rounded border border-blue-100 flex-1">
                                        <span className="block text-gray-500 text-xs uppercase">Total Qty</span>
                                        <span className="font-bold text-slate-700">{selectedOrder.totalQuantity}</span>
                                    </div>
                                    <div className="bg-white p-3 rounded border border-blue-100 flex-1">
                                        <span className="block text-gray-500 text-xs uppercase">Produced</span>
                                        <span className="font-bold text-green-600">{selectedOrder.producedQuantity}</span>
                                    </div>
                                </div>

                                <div className="flex gap-2">
                                    <div className="bg-white p-3 rounded border border-purple-100 flex-1">
                                        <span className="block text-gray-500 text-xs uppercase">In Pipeline</span>
                                        <span className="font-bold text-purple-600">{selectedOrder.inPipelineQuantity}</span>
                                    </div>
                                    <div className="bg-orange-50 p-3 rounded border border-orange-200 flex-1">
                                        <span className="block text-orange-600 text-xs uppercase font-semibold">Unplanned ⬅️</span>
                                        <span className="font-bold text-orange-700 text-lg">{selectedOrder.unplannedQuantity}</span>
                                    </div>
                                </div>

                                {/* Progress Bar */}
                                <div>
                                    <div className="flex justify-between text-xs text-gray-500 mb-1">
                                        <span>Progress</span>
                                        <span>{selectedOrder.progressPercentage}%</span>
                                    </div>
                                    <div className="w-full bg-gray-200 rounded-full h-2">
                                        <div 
                                            className="bg-blue-600 h-2 rounded-full transition-all" 
                                            style={{width: `${selectedOrder.progressPercentage}%`}}
                                        ></div>
                                    </div>
                                </div>
                                {/* Smart Batch Suggestion */}
                                
                                <div className="mt-4">
                                    <SmartBatchCard 
                                        planningInfo={planningInfo}
                                        onUseSuggestion={(qty) => setFormData({...formData, quantityToProduce: qty})}
                                    />
                                </div>
                            </div>
                        </div>


                        </>
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