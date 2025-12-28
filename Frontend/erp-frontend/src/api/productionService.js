import api from './axios';

export const getPendingOrders = () => api.get('/production/dashboard/pending-orders');

export const getPlanningInfo = (soId) => api.get(`/production/planning-info/${soId}`);

export const createPlan = (data) => api.post('/production/create-plan', data);

export const startBatch = (poId) => api.put(`/production/start/${poId}`);

export const completeBatch = (data) => api.put('/production/complete', data);

export const getAllBatches = (soId = null) => {
    const url = soId ? `Production/production-orders?salesOrderId=${soId}` : 'Production/production-orders';
    return api.get(url);
};

export const cancelBatch = (poId) => api.put(`/production/cancel/${poId}`);

