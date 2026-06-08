import axios from 'axios';

const api = axios.create({
  baseURL: '/api', // Vite proxy üzerinden yönlendirilecek
  headers: {
    'Content-Type': 'application/json',
  },
});

// İstek öncesi interceptor (Örn: JWT token ekleme)
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Yanıt sonrası interceptor (Örn: 401 hatasında logine yönlendirme)
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      // window.location.href = '/login'; 
    }
    return Promise.reject(error);
  }
);

export default api;
