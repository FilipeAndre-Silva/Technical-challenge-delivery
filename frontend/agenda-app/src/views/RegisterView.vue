<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'primevue/usetoast';
import { useUserStore } from '@/stores/user';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import api from '@/api';

const email = ref('');
const password = ref('');
const router = useRouter();
const toast = useToast();
const userStore = useUserStore();

const registerUser = async () => {
  userStore.isLoading = true;
  try {
    const response = await api.post('/User/register', { email: email.value, password: password.value });
    console.log('Resposta do Registro:', response);
    console.log('Status da Resposta:', response.status);
    console.log('response.data:', response.data);
    console.log('isSuccess:', response.data?.isSuccess);
    console.log('Errors:', response.data?.errors);

    if (response.data?.isSuccess === true) {
      toast.add({ severity: 'success', summary: 'Sucesso', detail: 'Registro realizado com sucesso! Você pode fazer login agora.', life: 3000 });
      setTimeout(() => {
        router.push('/login');
      }, 2000);
    } else {
      console.log('Erro no registro (frontend)');
      if (response.data?.errors) {
        Object.entries(response.data.errors).forEach(([key, value]) => {
          (value as string[]).forEach(errorMessage => {
            toast.add({ severity: 'error', summary: `Erro em ${key}`, detail: errorMessage, life: 5000 });
          });
        });
      } else {
        toast.add({ severity: 'error', summary: 'Erro', detail: response.data?.message || 'Erro ao registrar usuário.', life: 3000 });
      }
    }
  } catch (error: any) {
    console.error('Erro na requisição de registro:', error);
    console.log('Error Response Data:', error.response?.data);
    if (error.response?.data) {
      Object.entries(error.response.data).forEach(([key, value]) => {
        if (Array.isArray(value)) {
          value.forEach(errorMessage => {
            toast.add({ severity: 'error', summary: `Erro em ${key}`, detail: errorMessage, life: 5000 });
          });
        } else if (typeof value === 'string') {
          toast.add({ severity: 'error', summary: `Erro em ${key}`, detail: value, life: 5000 });
        }
      });
    } else {
      toast.add({ severity: 'error', summary: 'Erro', detail: error.response?.data?.message || error.message || 'Erro ao registrar usuário.', life: 3000 });
    }
  } finally {
    userStore.isLoading = false;
  }
};
</script>

<template>
  <div class="login-wrapper">
    <h1 class="login-title">Agenda Online</h1>
    <div class="container">
      <h1>Registro</h1>
      <Toast />
      <div class="form-group">
        <label for="email">Email</label>
        <InputText id="email" v-model="email" type="email" class="w-full" />
      </div>
      <div class="form-group">
        <label for="password">Senha</label>
        <InputText id="password" v-model="password" type="password" class="w-full" />
      </div>
      <Button label="Registrar" @click="registerUser" :loading="userStore.isLoading" />
      <p class="mt-3">Já tem uma conta? <router-link to="/login">Faça login</router-link></p>
    </div>
  </div>
</template>

<style scoped>
.container {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px;
  margin: 2rem auto;
}

.form-group {
  margin-bottom: 15px;
  width: 100%;
  max-width: 400px;
}

label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

.w-full {
  width: 100%;
}

.mt-3 {
  margin-top: 1rem;
}

.mt-2 {
  margin-top: 0.5rem;
}
</style>