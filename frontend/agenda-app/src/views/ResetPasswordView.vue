<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'primevue/usetoast';
import { useUserStore } from '@/stores/user';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import api from '@/api';

const email = ref('');
const router = useRouter();
const toast = useToast();
const userStore = useUserStore();

const requestPasswordReset = async () => {
  userStore.isLoading = true;
  try {
    const response = await api.post('/User/reset-password', { email: email.value });

    if (response.status === 200 && response.data?.isSuccess === true) {
      toast.add({ severity: 'success', summary: 'Sucesso', detail: 'Um email para redefinir sua senha foi enviado para você.', life: 5000 });
      if (response.data?.data) {
        toast.add({ severity: 'success', summary: 'Link de Redefinição', detail: response.data.data as string, life: 7000 });
      }
      setTimeout(() => {
        router.push('/login');
        console.log('Redirecionando para /login após 5 segundos');
      }, 5000);
    } else if (response.status === 400) {
      toast.add({ severity: 'warn', summary: 'Aviso', detail: response.data as string || 'Não foi possível enviar o email de redefinição de senha. Verifique o email inserido.', life: 5000 });
    } else {
      toast.add({ severity: 'error', summary: 'Erro', detail: response.data?.message || 'Erro ao solicitar a redefinição de senha.', life: 5000 });
    }
  } catch (error: any) {
    console.error('Erro ao solicitar redefinição de senha:', error);
    if (error.response?.status === 400) {
      toast.add({ severity: 'error', summary: 'Erro', detail: error.response?.data as string || error.message || 'Não foi possível enviar o email de redefinição de senha.', life: 5000 });
    } else {
      toast.add({ severity: 'error', summary: 'Erro', detail: error.response?.data?.message || error.message || 'Erro ao solicitar a redefinição de senha.', life: 5000 });
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
      <h1>Esqueceu sua senha?</h1>
      <Toast />
      <p>Informe seu email para receber um link de redefinição de senha.</p>
      <div class="form-group">
        <label for="email">Email</label>
        <InputText id="email" v-model="email" type="email" class="w-full" />
      </div>
      <Button label="Enviar Link de Redefinição" @click="requestPasswordReset" :loading="userStore.isLoading" />
      <p class="mt-3"><router-link to="/login">Voltar para o Login</router-link></p>
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