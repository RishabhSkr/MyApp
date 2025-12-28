import { useState, useEffect, useCallback } from 'react';
import { 
    getBOMsService, 
    createBOMService, 
    updateBOMService, 
    deleteBOMService,
    getBOMByProductIdService
} from '../api/master/bom';

import useApi from './useApi';

export const useBom = () => {
    const [boms, setBoms] = useState([]); 
    const {loading,requestHandlerFunction} = useApi();

    //  Get All BOMs
    const fetchBoms = useCallback(async () => {
        const result = await requestHandlerFunction(getBOMsService); 
        if (result.success) setBoms(result.data || []);
        return result.success;
    }, [requestHandlerFunction]);

    //  Get BOM By Product Id (Refactored to return data)
    const getBOMByProductId = useCallback(async (id) => {
        try {
            const data = await getBOMByProductIdService(id);
            return data; // Return the actual DTO
        } catch (error) {
            console.error("Error fetching BOM:", error);
            return null;
        }
    }, []);


    //  Create BOM
    const createBOM = async (data) => {
        const result = await requestHandlerFunction(
            () => createBOMService(data), 
            "BOM Created Successfully!"
        );
        if (result.success) fetchBoms(); // Refresh List
        return result.success;
    };

    // Delete BOM
    const deleteBOM = async (id) => {
        const result = await requestHandlerFunction(
            () => deleteBOMService(id), 
            "BOM Deleted"
        );
        
        if (result.success) fetchBoms();
        return result.success;
    };

    // Update BOM
    const updateBOM = async (id, data) => {
        const result = await requestHandlerFunction(
            () => updateBOMService(id, data), 
            "BOM Updated Successfully!"
        );
        if (result.success) fetchBoms();
        return result.success;
    }
     // Initial Load
    useEffect(() => {
        const loadingBom = async () => await fetchBoms();
        loadingBom();
    }, [fetchBoms]);

    return { 
        boms, 
        loading, 
        createBOM, 
        deleteBOM,
        updateBOM,
        fetchBoms,
        getBOMByProductId 
    };
};