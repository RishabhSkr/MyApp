import api from '../axios';

export const getProducts = async () => {
    const response = await api.get('/Products');
    return response.data;
};

export const getProductById = async (id) => {
    const response = await api.get(`/Products/${id}`);
    return response.data;
};

export const createProduct = async (data) => {
    const response = await api.post('/Products', data);
    return response.data;
};

export const updateProduct = async (id, data) => {
    const response = await api.put(`/Products/${id}`, data);
    return response.data;
};

export const deleteProduct = async (id) => {
    const response = await api.delete(`/Products/${id}`);
    return response.data;
};
