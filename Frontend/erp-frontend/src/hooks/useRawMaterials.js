import { useState, useEffect, useCallback } from 'react';
import { getRawMaterials, 
        createRawMaterial,
        updateRawMaterial,
        deleteRawMaterial, 
        addStock } from '../api/master/rawMaterial';

import useApi from './useApi';

export const useRawMaterials = () => {
    const [materials, setMaterials] = useState([]);
    const {loading,requestHandlerFunction} = useApi();


    // Fetch Data
    const fetchMaterials = useCallback( async () => {
        const result =await requestHandlerFunction(getRawMaterials); 
        if (result.success) {
            setMaterials(result.data || []);
        }
    }, [requestHandlerFunction]);

    // Add Data
    const addMaterial = async (data) => {
        const result = await requestHandlerFunction(() => createRawMaterial(data),"Material Added Successfully!");
        if (result.success)fetchMaterials(); 
        return result.success
    };

    // Add Stock
    const addRawMaterialStock = async (data) => {
        const result = await requestHandlerFunction(() => addStock(data),"Stock Updated Successfully!");
        if (result.success)fetchMaterials(); 
        return result.success
    }
    // Update
    const updateRM = async (id, data) => {
        const result = await requestHandlerFunction(() => updateRawMaterial(id, data),"Material Updated Successfully!");
        if (result.success)fetchMaterials(); 
        return result.success
    };

    // delete RM
     const deleteRM = async (id) => {
        const result = await requestHandlerFunction(() => deleteRawMaterial(id),"Material Deleted Successfully!");
        if (result.success)fetchMaterials(); 
        return result.success
     }
    // Initial Load
    useEffect(()  =>  {
        const loadInitialData = async () => {
            await fetchMaterials();
        };
        loadInitialData();
    }, [fetchMaterials]);

    return { materials, loading, addMaterial, addRawMaterialStock, updateRM,deleteRM};
};