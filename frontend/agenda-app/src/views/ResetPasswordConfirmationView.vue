<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useToast } from 'primevue/usetoast';
import { useUserStore } from '@/stores/user';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import api from '@/api';

const route = useRoute();
const router = useRouter();
const toast = useToast();
const userStore = useUserStore();

const email = ref('');
const token = ref('');
const newPassword = ref('');

const confirmPasswordReset = async () => {
  userStore.isLoading = true;
  try {
    const response = await api.post('/User/reset-password-confirmation', {
      email: email.value,
      token: token.value,
      newPassword: newPassword.value,
    });

    if (response.status === 200 && response.data?.isSuccess === true) {
      toast.add({ severity: 'success', summary: 'Sucesso', detail: response.data.data as string || 'Senha redefinida com sucesso!', life: 5000 });
      setTimeout(() => {
        router.push('/login');
        console.log('Redirecionando para /login após 5 segundos');
      }, 5000);
    } else if (response.status === 400 && response.data?.errors) {
      Object.entries(response.data.errors).forEach(([key, value]) => {
        (value as string[]).forEach(errorMessage => {
          toast.add({ severity: 'error', summary: `Erro em ${key}`, detail: errorMessage, life: 5000 });
        });
      });
    } else if (response.status === 400) {
      toast.add({ severity: 'warn', summary: 'Aviso', detail: response.data as string || 'Não foi possível redefinir a senha. Verifique os dados.', life: 5000 });
    } else {
      toast.add({ severity: 'error', summary: 'Erro', detail: response.data?.message || 'Erro ao redefinir a senha.', life: 5000 });
    }
  } catch (error: any) {
    console.error('Erro ao redefinir a senha:', error);
    if (error.response?.status === 400 && error.response?.data?.errors) {
      Object.entries(error.response.data.errors).forEach(([key, value]) => {
        (value as string[]).forEach(errorMessage => {
          toast.add({ severity: 'error', summary: `Erro em ${key}`, detail: errorMessage, life: 5000 });
        });
      });
    } else if (error.response?.status === 400) {
      toast.add({ severity: 'error', summary: 'Erro', detail: error.response?.data as string || error.message || 'Não foi possível redefinir a senha.', life: 5000 });
    } else {
      toast.add({ severity: 'error', summary: 'Erro', detail: error.response?.data?.message || error.message || 'Erro ao redefinir a senha.', life: 5000 });
    }
  } finally {
    userStore.isLoading = false;
  }
};

onMounted(() => {
  if (route.query?.token) {
    token.value = route.query.token as string;
  }
});
</script>

<template>
  <div class="login-wrapper">
    <h1 class="login-title">Agenda Online</h1>
    <div class="container">
      <h1>Redefinir Senha</h1>
      <Toast />
      <p>Informe seu email, token e a nova senha.</p>
      <div class="form-group">
        <label for="email">Email</label>
        <InputText id="email" v-model="email" type="email" class="w-full" />
      </div>
      <div class="form-group">
        <label for="token">Token</label>
        <InputText id="token" v-model="token" type="text" class="w-full" />
      </div>
      <div class="form-group">
        <label for="newPassword">Nova Senha</label>
        <InputText id="newPassword" v-model="newPassword" type="password" class="w-full" />
      </div>
      <Button label="Redefinir Senha" @click="confirmPasswordReset" :loading="userStore.isLoading" class="w-full" />
      <p class="mt-3"><router-link to="/login">Voltar para o Login</router-link></p>
    </div>
  </div>
</template>

<style scoped>
.login-wrapper {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh; 
  background-color: #f8f9fa; 
}

.login-title {
  margin-bottom: 2rem;
  font-size: 2.5rem;
  color: #343a40;
}

.container {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px;
  background-color: #fff;
}

h1 {
  margin-bottom: 1.5rem;
  color: #343a40;
}

p {
  color: #6c757d; 
  margin-bottom: 1rem;
  text-align: center;
}

.form-group {
  margin-bottom: 1.5rem;
  width: 100%;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: bold;
  color: #495057; 
}

.w-full {
  width: 100%;
}

.mt-3 {
  margin-top: 1.5rem;
}

.mt-2 {
  margin-top: 1rem;
}

a {
  color: #007bff;
  text-decoration: none;
}

a:hover {
  text-decoration: underline;
}

</style>