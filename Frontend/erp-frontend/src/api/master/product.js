import api from '../axios';

export const getProducts = async () => {
    const response = await api.get('/mock/inventory/products');
    return response.data;
};

export const getProductById = async (id) => {
    const response = await api.get(`/mock/inventory/products/${id}`);
    return response.data;
};

export const createProduct = async (data) => {
    const response = await api.post('/mock/inventory/products', data);
    return response.data;
};

export const updateProduct = async (id, data) => {
    const response = await api.put(`/mock/inventory/products/${id}`, data);
    return response.data;
};

export const deleteProduct = async (id) => {
    const response = await api.delete(`/mock/inventory/products/${id}`);
    return response.data;
};
