import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/api';
import type { CreateContactCommand, Contact, FetchContactsParams, PagedResult } from '@/interfaces/Contact';

export const useContactStore = defineStore('contact', () => {
  const isLoading = ref(false);
  const contacts = ref<Contact[]>([]);
  const totalRecords = ref(0);
  const contactToEdit = ref<Contact | null>(null);

  const createContact = async (contactData: CreateContactCommand): Promise<void> => {
    isLoading.value = true;
    try {
      const response = await api.post('/Contact', contactData);
      console.log('Resposta da Criação de Contato:', response);
      if (response.status === 201) {
      } else {
        const errorMessage = response.data?.message || 'Erro ao criar o contato.';
        throw new Error(errorMessage);
      }
    } catch (error: any) {
      console.error('Erro na requisição de criação de contato:', error);
      throw error;
    } finally {
      isLoading.value = false;
    }
  };

  const fetchContacts = async (params: FetchContactsParams): Promise<void> => {
    isLoading.value = true;
    try {
      const token = localStorage.getItem('authToken');
      const headers: Record<string, string> = {
        'Content-Type': 'application/json',
      };
      if (token) {
        headers['Authorization'] = `Bearer ${token}`;
      }
      const response = await api.get<PagedResult<Contact>>('Contact/ByFilter', {
        params,
        headers,
      });
      console.log('Resposta da Busca de Contatos:', response);
      if (response.status === 200) {
        contacts.value = response.data.result;
      } else if (response.status === 204) {
        contacts.value = [];
        totalRecords.value = 0;
      }
    } catch (error: any) {
      console.error('Erro na requisição de busca de contatos:', error);
      let errorMessage = 'Erro ao buscar contatos.';
      if (error.response?.data?.message) {
        errorMessage = error.response.data.message;
      } else if (error.message) {
        errorMessage = error.message;
      }
      return Promise.reject(new Error(errorMessage));
    } finally {
      isLoading.value = false;
    }
  };

  const deleteContact = async (id: string): Promise<void> => {
    isLoading.value = true;
    try {
      const token = localStorage.getItem('authToken');
      const headers: Record<string, string> = {
        'Content-Type': 'application/json',
      };
      if (token) {
        headers['Authorization'] = `Bearer ${token}`;
      }
      const response = await api.delete(`/Contact/${id}`, { headers });
      console.log('Resposta da Deleção de Contato:', response);
      if (response.status !== 204) {
        const errorMessage = response.data?.message || `Erro ao deletar o contato com ID ${id}.`;
        throw new Error(errorMessage);
      }
    } catch (error: any) {
      console.error(`Erro na requisição de deleção do contato com ID ${id}:`, error);
      let errorMessage = `Erro ao deletar o contato com ID ${id}.`;
      if (error.response?.data?.message) {
        errorMessage = error.response.data.message;
      } else if (error.message) {
        errorMessage = error.message;
      }
      return Promise.reject(new Error(errorMessage));
    } finally {
      isLoading.value = false;
    }
  };

    const getContactById = async (id: string): Promise<Contact> => {
    isLoading.value = true;
    try {
      const token = localStorage.getItem('authToken');
      const headers: Record<string, string> = {
        'Content-Type': 'application/json',
      };
      if (token) {
        headers['Authorization'] = `Bearer ${token}`;
      }
      const response = await api.get<Contact>(`/Contact/${id}`, { headers });
      console.log('Resposta da Busca de Contato por ID:', response);
      if (response.status === 200) {
        contactToEdit.value = response.data;
        return response.data;
      } else {
        throw new Error();
      }
    } catch (error: any) {
      console.error(`Erro ao buscar o contato com ID ${id}:`, error);
      let errorMessage = `Erro ao buscar o contato com ID ${id}.`;
      if (error.response?.data?.message) {
        errorMessage = error.response.data.message;
      } else if (error.message) {
        errorMessage = error.message;
      }
       throw new Error(errorMessage);
    } finally {
      isLoading.value = false;
    }
  };

  const updateContact = async (id: string, contactData: Contact): Promise<void> => {
    isLoading.value = true;
    try {
      const token = localStorage.getItem('authToken');
            const headers: Record<string, string> = {
                'Content-Type': 'application/json',
            };
            if (token) {
                headers['Authorization'] = `Bearer ${token}`;
            }
      const response = await api.put(`/Contact/${id}`, contactData, {headers});
      console.log('Resposta da Atualização de Contato:', response);
      if (response.status === 204) {
      } else {
        const errorMessage = response.data?.message || `Erro ao atualizar o contato com ID ${id}.`;
        throw new Error(errorMessage);
      }
    } catch (error: any) {
      console.error(`Erro ao atualizar o contato com ID ${id}:`, error);
      let errorMessage = `Erro ao atualizar o contato com ID ${id}.`;
      if (error.response?.data?.message) {
        errorMessage = error.response.data.message;
      } else if (error.message) {
        errorMessage = error.message;
      }
      throw error;
    } finally {
      isLoading.value = false;
    }
  };

  return {
    isLoading,
    contacts,
    totalRecords,
    contactToEdit,
    createContact,
    fetchContacts,
    deleteContact,
    getContactById,
    updateContact
  };
});
