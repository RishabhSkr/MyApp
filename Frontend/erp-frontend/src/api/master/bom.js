import api from '../axios';

export const getBOMsService = async () => {
    const response = await api.get('/Bom');
    return response.data;
};

export const getBOMByProductIdService = async (productId) => {
    const response = await api.get(`/Bom/${productId}`);
    return response.data;
};

export const createBOMService = async (data) => {
    const response = await api.post('/Bom', data);
    return response.data;
};

export const updateBOMService = async (productId, data) => {
    const response = await api.put(`/Bom/${productId}`, data);
    return response.data;
};

export const deleteBOMService = async (productId) => {
    const response = await api.delete(`/Bom/${productId}`);
    return response.data;
};
