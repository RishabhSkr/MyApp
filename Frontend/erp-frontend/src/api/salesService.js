import api from './axios';

export const getPendingSalesOrders = () => api.get('/Sales/pending-sales');