import api from '../axios';

export const getRawMaterials = async () => {
    const response = await api.get('/mock/inventory/raw-materials');
    return response.data;
};

export const createRawMaterial = async data => {
    const response = await api.post('/mock/inventory/raw-materials', data);
    return response.data;   
};

export const updateRawMaterial = async (id, data) => {
    const response = await api.put(`/mock/inventory/raw-materials/${id}`, data);
    return response.data;
};

// TODO : Delete RM need to be implemented from Backened side
export const deleteRawMaterial = async id => {
    const response = await api.delete(`/mock/inventory/raw-materials/${id}`);
    return response.data;
};

export const addStock = async data => {
    const response = await api.post('/mock/inventory/raw-materials/add-stock', data);
    return response.data;
};
