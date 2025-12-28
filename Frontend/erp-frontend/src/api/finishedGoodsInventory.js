import api from './axios';

export const getFinishedGoodsStock = async () => {
    const response = await api.get('/FinishedGoods');
    return response.data;
};
