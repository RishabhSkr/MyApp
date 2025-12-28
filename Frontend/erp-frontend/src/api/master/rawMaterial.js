import api from '../axios';

export const getRawMaterials = async () => {
    const response = await api.get('/RawMaterial');
    return response.data;
};

export const createRawMaterial = async data => {
    const response = await api.post('/RawMaterial', data);
    return response.data;
};

export const updateRawMaterial = async (id, data) => {
    const response = await api.put(`/RawMaterial/${id}`, data);
    return response.data;
};

// TODO : Delete RM need to be implemented from Backened side
export const deleteRawMaterial = async id => {
    const response = await api.delete(`/RawMaterial/${id}`);
    return response.data;
};

export const addStock = async data => {
    const response = await api.post('/RawMaterial/add-stock', data);
    return response.data;
};
