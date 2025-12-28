import { useEffect, useState } from 'react';
import { Play } from 'lucide-react';
import PlanningModal from '../../components/production/PlanningModal';
import useApi from '../../hooks/useApi'; // 1. Import Hook
import { 
    getPendingOrders,
    getPlanningInfo,
    createPlan
} from '../../api/productionService';

const Dashboard = () => {
    // Data States
    const [orders, setOrders] = useState([]);
    
    // Modal & Selection States
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [planningData, setPlanningData] = useState(null);
    const [selectedOrderId, setSelectedOrderId] = useState(null);

    const { loading: isLoading, requestHandlerFunction } = useApi();

    const loadDashboardData = async () => {
        // Arrow function for API call
        const response = await requestHandlerFunction(
            () => getPendingOrders()
        );
        
        if(response.success) {
            setOrders(response.data || []);
        } else {
            setOrders([]); 
        }
    };
    //  Initial Load
    useEffect(() => {
        const loadDashboardData = async () => await loadDashboardData();
        loadDashboardData();
    }, []);


    //  Plan Button Click (Fetch Planning Info)
    const handlePlanClick = async (soId) => {
        setSelectedOrderId(soId);
        setIsModalOpen(true);
        setPlanningData(null);

        const response = await requestHandlerFunction(
            () => getPlanningInfo(soId)
        );

        if(response.success) {
            setPlanningData(response.data);
        } else {
            // Agar fail hua to modal band kar do
            setIsModalOpen(false);
        }
    };

    // 5. Create Batch (Submit Plan)
    const handleCreateBatch = async (quantity, startDate, endDate) => {
        const payload = {
            salesOrderId: selectedOrderId,
            quantityToProduce: parseInt(quantity),
            plannedStartDate: new Date(startDate).toISOString(),
            plannedEndDate: new Date(endDate).toISOString(),
        };

        const response = await requestHandlerFunction(
            () => createPlan(payload),
            'Batch created successfully!' // Success Message
        );

        if(response.success) {
            setIsModalOpen(false);
            loadDashboardData(); // Refresh Dashboard
        }
    };

    // Helper for Status Badge Color
    const getStatusColor = status => {
        switch (status) {
            case 'Producing': return 'bg-blue-100 text-blue-800';
            case 'Partially Planned': return 'bg-yellow-100 text-yellow-800';
            case 'New': return 'bg-gray-100 text-gray-800';
            default: return 'bg-gray-100 text-gray-800';
        }
    };

    // Loading State from Hook
    if (isLoading && orders.length === 0) 
        return <div className="p-8 text-center text-blue-600">Loading Dashboard...</div>;

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="p-8 text-center">
                <h1 className=" text-3xl font-bold text-slate-800">
                    Production Dashboard
                </h1>
            </div>
            <div className="flex justify-between items-center">
                <h1 className="text-2xl font-bold text-slate-800">
                    Production Planning
                </h1>
                <div className="text-sm text-gray-500">
                    Total Pending Orders:{' '}
                    <span className="font-bold text-slate-900">
                        {orders.length}
                    </span>
                </div>
            </div>

            {/* Empty State */}
            {orders.length === 0 && !isLoading && (
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
                            {orders.map(order => (
                                <tr key={order.salesOrderId} className="hover:bg-slate-50 transition-colors">
                                    <td className="p-4">
                                        <div className="font-bold text-slate-700">#{order.salesOrderId}</div>
                                        <div className="text-xs text-gray-400">
                                            {new Date(order.orderDate).toLocaleDateString()}
                                        </div>
                                    </td>
                                    <td className="p-4">
                                        <div className="font-medium text-slate-900">{order.customerName}</div>
                                        <div className="text-sm text-slate-500">{order.productName}</div>
                                    </td>
                                    {/* ... Progress & Breakdown Columns (Same as before) ... */}
                                    <td className="p-4 w-1/4">
                                        <div className="flex justify-between text-xs mb-1">
                                            <span className="text-gray-500">Total: {order.totalQuantity}</span>
                                            <span className="font-bold text-blue-600">{order.progressPercentage}%</span>
                                        </div>
                                        <div className="w-full bg-gray-200 rounded-full h-2.5">
                                            <div className="bg-blue-600 h-2.5 rounded-full transition-all duration-500" style={{ width: `${order.progressPercentage}%` }}></div>
                                        </div>
                                    </td>
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

                                    <td className="p-4">
                                        <span className={`px-3 py-1 rounded-full text-xs font-medium ${getStatusColor(order.status)}`}>
                                            {order.status}
                                        </span>
                                    </td>
                                    <td className="p-4 text-right">
                                        <button
                                            className="bg-slate-900 hover:bg-slate-700 text-white px-4 py-2 rounded-md text-sm flex items-center gap-2 ml-auto transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                                            disabled={order.unplannedQuantity === 0 || isLoading}
                                            onClick={() => handlePlanClick(order.salesOrderId)}
                                        >
                                            <Play size={16} /> Plan Batch
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
            
            <PlanningModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                data={planningData}
                isLoading={isLoading} // Modal apna loading dikhayega hook state se
                onConfirm={handleCreateBatch}
            />
        </div>
    );
};

export default Dashboard;