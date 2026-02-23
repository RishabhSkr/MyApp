import { useCallback, useEffect, useState } from 'react';
import { getAllBatches } from '../../api/productionService';
import FilterBar from '../../components/common/FilterBar';
import useApi from '../../hooks/useApi'; // 1. Import Hook
import { Factory, Calendar, CheckCircle, AlertTriangle, PlayCircle, ArrowDownAZ, ArrowLeftToLine, ArrowLeftIcon } from 'lucide-react';

const Production = () => {
    const [orders, setOrders] = useState([]);
    const [filterId, setFilterId] = useState('');
    console.log(orders);
    // hook API Calls
    const { loading: isLoading, requestHandlerFunction } = useApi();

    // Fetch Function 
    const fetchOrders = useCallback(async (soId = null) => {
        const response = await requestHandlerFunction(
            () => getAllBatches(soId)
        );
        // console.count("Order Management: Fetching Batches", response.data);
        const backendData = response.data?.data;
        const orderList = Array.isArray(backendData)?backendData
            : (backendData? [backendData]: []);
        setOrders(orderList);
    },[requestHandlerFunction]);

    //  Initial Load
    useEffect(() => {
        const loadOrders = async () => await fetchOrders();
        loadOrders();
    }, [fetchOrders]);
    
    const filteredOrders = orders.filter(order => {
    if (!filterId) return true;
    const search = filterId.toLowerCase();
    return order.orderNumber.toLowerCase().includes(search) 
        || order.salesOrderId.toLowerCase().includes(search);
    });


    const handleClear = () => {
        setFilterId(''); 
        fetchOrders(null); 
    };

    const getStatusColor = (status) => {
        const safeStatus = String(status || '').toLowerCase();
        switch (safeStatus) {
            case 'completed': return 'bg-green-100 text-green-700 border-green-200';
            case 'cancelled': return 'bg-red-100 text-red-700 border-red-200';
            case 'created': return 'bg-yellow-100 text-yellow-700 border-yellow-200';
            case 'released': return 'bg-purple-100 text-purple-700 border-purple-200';
            case 'in progress': return 'bg-blue-100 text-blue-700 border-blue-200';
            default: return 'bg-gray-100 text-gray-700 border-gray-200';
        }
    };

    const formatDate = (dateString) => {
        if (!dateString) return '-';
        return new Date(dateString).toLocaleString('en-US', {
            month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
        });
    };

    return (
        <div className="p-6 max-w-7xl mx-auto">
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-6 gap-4">
                <h1 className="text-2xl font-bold flex items-center gap-2 text-slate-800">
                    <Factory className="text-blue-600" />
                    Production Orders List
                </h1>
                
                <div className="flex flex-col sm:flex-row gap-4 items-center w-full md:w-auto">
                    <FilterBar
                        value={filterId}           
                        onChange={setFilterId}     
                        onSearch={() => {}}
                        onClear={handleClear}
                        placeholder="Search by PO or SO ID"
                        type="text"
                    />

                    <div className="text-sm text-gray-500 whitespace-nowrap">
                        Total Orders: <span className="font-bold text-gray-800">{orders.length}</span>
                    </div>
                </div>
            </div>

            <div className="bg-white rounded-lg shadow border border-gray-200 overflow-hidden">
                <div className="overflow-x-auto">
                    <table className="w-full text-sm text-left">
                        <thead className="bg-slate-50 text-slate-600 uppercase text-xs font-semibold border-b border-gray-200">
                            <tr>
                                <th className="px-6 py-4">Order ID</th>
                                <th className="px-6 py-4">Product</th>
                                <th className="px-6 py-4 text-center">Batch Qty</th>
                                <th className="px-6 py-4 text-center">Produced </th>
                                <th className="px-6 py-4 text-center">Scrap</th>
                                <th className="px-6 py-4 text-center">Unused Returned Qty</th>
                                <th className="px-6 py-4">Status</th>
                                <th className="px-6 py-4">Timeline</th>
                                <th className="px-6 py-4">CreatedBy</th>
                                {/* <th className="px-6 py-4">UpdatedBy</th> */}
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-100">
                            {/* 5. Use isLoading from Hook */}
                            {isLoading ? (
                                <tr>
                                    <td colSpan="7" className="px-6 py-12 text-center text-gray-400">
                                        <div className="flex justify-center items-center gap-2">
                                            <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-blue-500"></div>
                                            Loading production orders...
                                        </div>
                                    </td>
                                </tr>
                            ) : orders.length === 0 ? (
                                <tr>
                                    <td colSpan="7" className="px-6 py-12 text-center text-gray-400">
                                        <div className="flex flex-col items-center gap-2">
                                            <Factory size={40} className="opacity-20" />
                                            <p>No production orders found.</p>
                                        </div>
                                    </td>
                                </tr>
                            ) : (
                                filteredOrders.map((order) => (
                                    <tr key={order.productionOrderId} className="hover:bg-slate-50 transition-colors">
                                        <td className="px-6 py-4 font-mono text-slate-700">
                                            <span className="font-semibold">{order.orderNumber || '—'}</span>
                                            <div className="text-xs text-gray-400 mt-1">SO #{String(order.salesOrderId).slice(0,8)}...</div>
                                        </td>
                                        <td className="px-6 py-4 font-medium text-slate-800">
                                            {order.productName}
                                            {order.bomNumber && (
                                                <div className="text-xs text-indigo-500 font-mono mt-0.5">📋 {order.bomNumber} v{order.bomVersion}</div>
                                            )}
                                        </td>
                                        <td className="px-6 py-4 text-center font-semibold text-slate-700">
                                            {order.batchQuantity}
                                        </td>
                                        <td className="px-6 py-4 text-center">
                                            <span className="text-green-600 font-medium flex items-center gap-1 justify-center">
                                                <CheckCircle size={12} /> {order.producedQuantity}
                                            </span>
                                        </td>
                                        <td className="px-6 py-4 text-center">
                                            <span className="text-red-600 font-medium flex items-center gap-1 justify-center">
                                                <AlertTriangle size={12} /> {order.scrapQuantity}
                                            </span>
                                        </td>
                                        <td className="px-6 py-4 text-center">
                                            <span className="text-red-600 font-medium flex items-center gap-1 justify-center">
                                                <ArrowLeftIcon size={12} /> {order.unusedReturnedQuantity}
                                            </span>
                                        </td>
                                        <td className="px-4 py-4">
                                            <span className={`px-2.5 py-1 rounded-full text-xs font-semibold border ${getStatusColor(order.status)}`}>
                                                {order.status}
                                            </span>
                                        </td>
                                        <td className="px-6 py-4 text-xs text-gray-500 space-y-1">
                                            <div className="flex items-center gap-1" title="Planned Date">
                                                <Calendar size={12} className="text-gray-400" /> 
                                                <span>PlanStart: {formatDate(order.plannedStartDate)}</span>
                                            </div>
                                            <div className="flex items-center gap-1" title="Planned Date">
                                                <Calendar size={12} className="text-gray-400" /> 
                                                <span>PlanEnd: {formatDate(order.plannedEndDate)}</span>
                                            </div>
                                            {order.actualStartDate && (
                                                <div className="flex items-center gap-1 text-blue-600" title="Actual Start">
                                                    <PlayCircle size={12} /> 
                                                    <span>ActualStart: {formatDate(order.actualStartDate)}</span>
                                                </div>
                                            )}
                                            {order.actualEndDate && (
                                                <div className="flex items-center gap-1 text-green-600" title="Actual End">
                                                    <CheckCircle size={12} /> 
                                                    <span>ActualEnd: {formatDate(order.actualEndDate)}</span>
                                                </div>
                                            )}
                                        </td>
                                        <td className="px-6 py-4 text-xs text-center">{String(order.createdByUserId).slice(0,8)}...</td>
                                        {/* <td className="px-6 py-4 text-xs text-center">{String(order.updatedByUserId).slice(0,8)}...</td> */}
                                    </tr>
                                ))
                            )}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default Production;