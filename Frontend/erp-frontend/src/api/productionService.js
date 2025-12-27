import api from './axios';

export const productionService = {
    // Dashboard
    getPendingOrders: () => api.get('/production/dashboard/pending-orders'),
    
    // Planning Info
    getPlanningInfo: (soId) => api.get(`/production/planning-info/${soId}`),
    
    // Actions
    createPlan: (data) => api.post('/production/plan', data),
    startBatch: (poId) => api.put(`/production/start/${poId}`),
    
    // Worker Complete (With Scrap)
    completeBatch: (data) => api.put('/production/complete', data),
    
    // List & Cancel
    getAllBatches: (soId = null) => {
        const url = soId ? `/production/orders?salesOrderId=${soId}` : '/production/orders';
        return api.get(url);
    },
    cancelBatch: (poId) => api.put(`/production/cancel/${poId}`),
};