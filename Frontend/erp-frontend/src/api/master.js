import axiosClient from './axios'; // Tumhara base axios configuration

// Raw Material Endpoints
export const getRawMaterials = async () => {
    const response = await axiosClient.get('/rawmaterial');
    return response.data;
};

export const createRawMaterial = async (data) => {
    const response = await axiosClient.post('/rawmaterials', data);
    return response.data;
};

export const deleteRawMaterial = async (id) => {
    const response = await axiosClient.delete(`/rawmaterials/${id}`);
    return response.data;
};