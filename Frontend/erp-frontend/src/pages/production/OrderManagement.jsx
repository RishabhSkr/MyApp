import { useCallback, useEffect, useState } from 'react';
import { PlayCircle, CheckCircle, XCircle } from 'lucide-react';
import CompleteOrderModal from '../../components/production/CompleteOrderModal';
import FilterBar from '../../components/common/FilterBar';
import useApi from '../../hooks/useApi';
import {
    getAllBatches,
    startBatch,
    cancelBatch,
    completeBatch
} from '../../api/productionService';

const OrderManagement = () => {
    // Data States
    const [orders, setOrders] = useState([]);
    const [filteredOrders, setFilteredOrders] = useState([]); 
    
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

    useEffect(() => {
        const loadOrders = async () => await fetchOrders();

        loadOrders();
    }, [fetchOrders]);

    
    //  Filter Logic
    useEffect(() => {
        const applyStatusFilter = () => {
            if (filterStatus === 'All') {
                setFilteredOrders(orders);
            } else {
                setFilteredOrders(orders.filter(o => o.status === filterStatus));
            }
        };
        applyStatusFilter();
    }, [orders, filterStatus]);



    const handleSearch = () => {
        if(filterId) fetchOrders(filterId);
    };

    const handleClear = () => {
        setFilterId(''); 
        setFilterStatus('All'); 
        fetchOrders(null); 
    };

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
            'Completed': 'bg-green-100 text-green-800',
            'Cancelled': 'bg-red-100 text-red-800',
            'Pending': 'bg-yellow-100 text-yellow-800',
            'Planned': 'bg-yellow-100 text-yellow-800',
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
                    onSearch={handleSearch}
                    onClear={handleClear}
                    placeholder="Search by Sales Order ID"
                    type="number"             
                />
                
                {/* Status Tabs */}
                <div className="flex bg-white rounded-lg shadow-sm p-1 border">
                    {['All', 'Planned', 'In Progress', 'Completed', 'Cancelled'].map(status => (
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
                            <th className="p-4 text-center">Batch Qty</th>
                            <th className="p-4">Dates</th>
                            <th className="p-4">Status</th>
                            <th className="p-4 text-right">Actions</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100 text-sm">
                        {isLoading ? ( // Using Hook Loading State
                             <tr><td colSpan="6" className="p-8 text-center">
                                 <div className="flex justify-center gap-2 text-blue-600 font-medium">
                                     Loading...
                                 </div>
                             </td></tr>
                        ) : filteredOrders.length === 0 ? (
                            <tr><td colSpan="6" className="p-8 text-center text-gray-400">No orders found.</td></tr>
                        ) : (
                            filteredOrders.map((order,index) => (
                                <tr key={order.productionOrderId || index} className="hover:bg-slate-50">
                                    <td className="p-4 font-mono text-gray-600">#{order.productionOrderId}
                                        <div className="text-xs text-blue-400">SO #{order.salesOrderId}</div>
                                    </td>
                                    <td className="p-4 font-medium">{order.productName}</td>
                                    <td className="p-4 text-center">{order.batchQuantity}</td>
                                    <td className="p-4 text-xs text-gray-500">
                                        <div>Plan: {new Date(order.plannedDate).toLocaleDateString()}</div>
                                        {order.actualStartDate && <div className="text-blue-600">Start: {new Date(order.actualStartDate).toLocaleDateString()}</div>}
                                    </td>
                                    <td className="p-4">{getStatusBadge(order.status)}</td>
                                    
                                    <td className="p-4 text-right">
                                        <div className="flex justify-end gap-2">
                                            {/* Logic Buttons same as your code, just disabled={isLoading} */}
                                            {order.status === 'Planned' && (
                                                <>
                                                    <button onClick={() => handleStart(order.productionOrderId)} disabled={isLoading} className="p-1.5 text-blue-600 hover:bg-blue-50 rounded border border-blue-200" title="Start Production">
                                                       <PlayCircle size={16} />
                                                    </button>
                                                    <button onClick={() => handleCancel(order.productionOrderId)} disabled={isLoading} className="p-1.5 text-red-600 hover:bg-red-50 rounded border border-red-200" title="Cancel Order">
                                                       <XCircle size={16} />
                                                    </button>
                                                </>
                                            )}

                                            {order.status === 'In Progress' && (
                                                <button onClick={() => openCompleteModal(order)} disabled={isLoading} className="px-3 py-1 bg-green-500 text-white text-xs rounded hover:bg-green-700 flex items-center gap-1">
                                                    <span>Mark Complete</span> <CheckCircle size={16} />
                                                </button>
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