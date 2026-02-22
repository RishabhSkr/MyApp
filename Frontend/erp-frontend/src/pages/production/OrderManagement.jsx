import { useCallback, useEffect, useState } from 'react';
import { PlayCircle, CheckCircle, XCircle } from 'lucide-react';
import CompleteOrderModal from '../../components/production/CompleteOrderModal';
import FilterBar from '../../components/common/FilterBar';
import useApi from '../../hooks/useApi';
import {
    getAllBatches,
    releaseOrder,
    startBatch,
    cancelBatch,
    completeBatch
} from '../../api/productionService';

const OrderManagement = () => {
    // Data States
    const [orders, setOrders] = useState([]);
    
    // Filter States
    const [filterStatus, setFilterStatus] = useState('All');
    const [filterId, setFilterId] = useState('');
    
    // UI States
    const [selectedOrder, setSelectedOrder] = useState(null);
    const [isCompleteModalOpen, setIsCompleteModalOpen] = useState(false);

   
    const { loading: isLoading, requestHandlerFunction } = useApi();
    // API Calls
  const fetchOrders = useCallback(async(searchId = null) => {
        const response = await requestHandlerFunction(
            () => getAllBatches(searchId)
        );

        if (response.success) {
            //  check structure
            // console.log("Hook Data:", response.data); 
            const backendData = response.data?.data; 
            // console.log("Backend Data:", backendData);
            const orderList = Array.isArray(backendData) 
                ? backendData 
                : (backendData ? [backendData] : []);

            setOrders(orderList);
        } else {
            setOrders([]); 
        }
    }, [requestHandlerFunction]);

    const filteredOrders = orders.filter(order => {
        // Status filter
        const statusMatch = filterStatus === 'All' || order.status === filterStatus;
        // Search filter  
        const searchMatch = !filterId 
            || order.orderNumber?.toLowerCase().includes(filterId.toLowerCase())
            || order.salesOrderId?.toLowerCase().includes(filterId.toLowerCase());
        
        return statusMatch && searchMatch;
    });

    useEffect(() => {
        const loadOrders = async () => await fetchOrders();

        loadOrders();
    }, [fetchOrders]);



    const handleStart = async (id) => {
        if(!confirm("Are you sure you want to START this production batch?")) return;
        
        const response = await requestHandlerFunction(
            () => startBatch(id),
            'Batch started successfully!' 
        );

        if (response.success) {
            fetchOrders(); 
        }
    };

    const handleCancel = async (id) => {
        if(!confirm("Are you sure you want to CANCEL this order?")) return;

        const response = await requestHandlerFunction(
            () => cancelBatch(id),
            'Order Cancelled Successfully.'
        );

        if (response.success) {
            fetchOrders();
        }
    };

    const handleRelease = async (id) => {
        if(!confirm("Are you sure you want to RELEASE this order?")) return;

        const response = await requestHandlerFunction(
            () => releaseOrder(id),
            'Order Released Successfully.'
        );

        if (response.success) {
            fetchOrders();
        }
    };

    const handleCompleteConfirm = async (id, producedQty, scrapQty) => {
        const payload = {
            productionOrderId: id,
            actualProducedQuantity: parseFloat(producedQty),
            scrapQuantity: parseFloat(scrapQty)
        };

        const response = await requestHandlerFunction(
            () => completeBatch(payload),
            'Order Completed! Inventory Updated.'
        );

        if (response.success) {
            setIsCompleteModalOpen(false);
            fetchOrders();
        }
    };

    const openCompleteModal = (order) => {
        setSelectedOrder(order);
        setIsCompleteModalOpen(true);
    };
    
    // Helper Functions
    const getStatusBadge = (status) => {
        const styles = {
            'Completed': 'bg-blue-200 text-blue-800',
            'Released': 'bg-blue-100 text-green-800',
            'Cancelled': 'bg-red-100 text-red-800',
            'Pending': 'bg-yellow-100 text-yellow-800',
            'Created': 'bg-yellow-100 text-yellow-800',
            'In Progress': 'bg-blue-100 text-blue-800'
        };
        return (
            <span className={`px-2 py-1 rounded-full text-xs font-semibold ${styles[status] || 'bg-gray-100'}`}>
                {status}
            </span>
        );
    };

    return (
        <div className="p-6 max-w-7xl mx-auto">
             <div className="mb-6 flex flex-col md:flex-row justify-between items-center gap-4">
                <h1 className="text-2xl font-bold text-slate-800">Order Management</h1>
                
                <FilterBar
                    value={filterId} 
                    onChange={setFilterId}  
                    onSearch={() => {}}
                    onClear={() => setFilterId('')}
                    placeholder="Search by PO or SO ID"
                    type="text"             
                />
                
                {/* Status Tabs */}
                <div className="flex bg-white rounded-lg shadow-sm p-1 border">
                    {['All', 'Planned', 'Released','In Progress', 'Completed', 'Cancelled'].map(status => (
                        <button
                            key={status}
                            onClick={() => setFilterStatus(status)}
                            className={`px-4 py-2 text-sm rounded-md transition-all ${
                                filterStatus === status 
                                ? 'bg-blue-600 text-white shadow' 
                                : 'text-gray-600 hover:bg-gray-50'
                            }`}
                        >
                            {status}
                        </button>
                    ))}
                </div>
            </div>

            {/* Table */}
            <div className="bg-white rounded-lg shadow border border-gray-200 overflow-hidden">
                <table className="w-full text-left">
                    <thead className="bg-slate-50 border-b text-xs uppercase text-slate-500">
                        <tr>
                            <th className="p-4">Order ID</th>
                            <th className="p-4">Product</th>
                            <th className="p-4 text-center">Batch Qty Details</th>
                            <th className="p-4">Start Dates</th>
                            <th className="p-4">End Dates</th>
                            <th className="p-4">Status</th>
                            <th className="p-4 text-right">Actions</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100 text-sm">
                        {isLoading ? ( // Using Hook Loading State
                             <tr><td colSpan="7" className="p-8 text-center">
                                 <div className="flex justify-center gap-2 text-blue-600 font-medium">
                                     Loading...
                                 </div>
                             </td></tr>
                        ) : filteredOrders.length === 0 ? (
                            <tr><td colSpan="7" className="p-8 text-center text-gray-400">No orders found.</td></tr>
                        ) : (
                            filteredOrders.map((order,index) => (
                                <tr key={order.productionOrderId || index} className="hover:bg-slate-50">
                                    <td className="p-4 font-mono text-gray-600">#{order.orderNumber}
                                        <div className="text-xs text-blue-400">SO #{String(order.salesOrderId).slice(0, 8)}</div>
                                    </td>
                                    <td className="p-4 font-medium">{order.productName}</td>
                                    <td className="p-4 text-center">
                                        <div className="font-bold">{order.batchQuantity}</div>
                                        {order.status === 'Completed' && (
                                            <div className="text-xs space-y-0.5 mt-1">
                                                <div className="text-green-600">✅ Produced: {order.producedQuantity}</div>
                                                <div className="text-red-500">🔴 Scrap: {order.scrapQuantity}</div>
                                                <div className="text-amber-600">🔁 Unused: {order.unusedReturnedQuantity}</div>
                                            </div>
                                        )}
                                    </td>
                                    <td className="p-4 text-xs text-gray-500">
                                        <div>PlanStart: {new Date(order.plannedStartDate).toLocaleDateString()}</div>
                                        {order.actualStartDate && <div className="text-blue-600">ActualStart: {new Date(order.actualStartDate).toLocaleDateString()}</div>}
                                    </td>
                                    <td className="p-4 text-xs text-gray-500">
                                        <div>PlanEnd: {new Date(order.plannedEndDate).toLocaleDateString()}</div>
                                        {order.actualEndDate && <div className="text-blue-600">ActualEnd: {new Date(order.actualEndDate).toLocaleDateString()}</div>}
                                    </td>
                                    <td className="p-4">{getStatusBadge(order.status)}</td>
                                    
                                    <td className="p-4 text-right">
                                        <div className="flex justify-end gap-2">
                                            {/* Logic Buttons same as your code, just disabled={isLoading} */}
                                            {order.status === 'Created' && (
                                                <>
                                                    <button onClick={() => handleRelease(order.productionOrderId)} 
                                                            disabled={isLoading} 
                                                            className="p-1.5 bg-green-500 text-white hover:bg-green-700 rounded border border-blue-200" 
                                                            title="Release Order">
                                                      Release
                                                    </button>
                                                    <button onClick={() => handleCancel(order.productionOrderId)} disabled={isLoading} 
                                                    className="p-1.5 bg-red-500 text-white hover:bg-red-700 rounded border border-red-200" title="Cancel Order">
                                                        Cancel
                                                    </button>
                                                </>
                                            )}

                                            {order.status === 'Released' && (
                                                <>
                                                    <button onClick={() => handleStart(order.productionOrderId)} 
                                                        disabled={isLoading}
                                                        className="px-3 py-1 bg-blue-500 text-white text-xs rounded hover:bg-blue-700 flex items-center gap-1">
                                                        <PlayCircle size={16} /> Start
                                                    </button>
                                                    <button onClick={() => handleCancel(order.productionOrderId)} disabled={isLoading} 
                                                    className="p-1.5 bg-red-500 text-white hover:bg-red-700 rounded border border-red-200" title="Cancel Order">
                                                      Cancel
                                                    </button>   
                                                </>
                                                
                                            )}
                                            {order.status === 'In Progress' && (
                                                <>
                                                    <button onClick={() => openCompleteModal(order)} 
                                                        disabled={isLoading}
                                                        className="px-3 py-1 bg-green-500 text-white text-xs rounded ...">
                                                        Complete
                                                    </button>                                                   
                                                </>
                                            )}
                                            
                                        </div>
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>

            <CompleteOrderModal 
                isOpen={isCompleteModalOpen} 
                onClose={() => setIsCompleteModalOpen(false)}
                order={selectedOrder}
                isLoading={isLoading} // Pass Hook Loading here
                onConfirm={handleCompleteConfirm}
            />
        </div>
    );
}

export default OrderManagement;