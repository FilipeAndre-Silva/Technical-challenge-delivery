import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7213/api',
  headers: {
    'Content-Type': 'application/json',
    'accept': '*/*'
  }
});

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('authToken');
      console.error('Sessão expirada ou não autorizado.');
    }
    return Promise.reject(error);
  }
);

export default api;