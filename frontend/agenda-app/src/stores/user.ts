import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/api';

export const useUserStore = defineStore('user', () => {
  const isAuthenticated = ref(false);
  const isLoading = ref(false);
  const authToken = ref<string | null>(null);
  const user = ref<any | null>(null);

const login = async (credentials: any): Promise<boolean> => {
  isLoading.value = true;
  try {
    const response = await api.post('/User/login', credentials);
    if (response.data?.isSuccess === true && response.data?.data) {
      setToken(response.data.data);
      console.log('Token armazenado. isAuthenticated:', isAuthenticated.value);
      return true;
    } else {
      clearToken();
      return false;
    }
  } catch (error) {
    clearToken();
    throw error;
  } finally {
    isLoading.value = false;
  }
};

const setToken = (token: string) => {
  authToken.value = token;
  localStorage.setItem('authToken', token);
  isAuthenticated.value = true;
};

  function clearToken() {
    authToken.value = null;
    localStorage.removeItem('authToken');
    isAuthenticated.value = false;
    user.value = null;
  }

  function logout() {
    isAuthenticated.value = false;
    authToken.value = null;
    user.value = null;
    localStorage.removeItem('authToken');
  }

  function checkAuth() {
    const storedToken = localStorage.getItem('authToken');
    if (storedToken) {
      authToken.value = storedToken;
      isAuthenticated.value = true;
    } else {
      isAuthenticated.value = false;
      authToken.value = null;
      user.value = null;
    }
  }

  return {
    isAuthenticated,
    isLoading,
    authToken,
    user,
    login,
    logout,
    checkAuth,
  };
});