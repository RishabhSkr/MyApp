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

// NEW: Get next auto-generated Order Number (for pre-filling form)
export const getNextOrderNumber = () => api.get('/production/next-order-number');

// NEW: Release order (Created → Released, reserves materials)
export const releaseOrder = (poId) => api.put(`/production/release/${poId}`);

// NEW: Update production order (quantity, dates)
export const updateOrder = (data) => api.put('/production/update', data);
