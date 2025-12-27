import axios from 'axios';

// Backend URL yahan set karein
const api = axios.create({
    baseURL: 'http://localhost:5243/api', // backend port se replace karein
    headers: {
        'Content-Type': 'application/json',
    },
});

// Response Interceptor (Errors handle karne ke liye)
api.interceptors.response.use(
    (response) => response,
    (error) => {
        console.error("API Error:", error.response?.data?.message || error.message);
        return Promise.reject(error);
    }
);

export default api;