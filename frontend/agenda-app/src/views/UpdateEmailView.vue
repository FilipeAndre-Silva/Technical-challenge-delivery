<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'primevue/usetoast';
import { useUserStore } from '@/stores/user';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import { onMounted } from 'vue';
import api from '@/api';

const email = ref('');
const newEmail = ref('');
const router = useRouter();
const toast = useToast();
const userStore = useUserStore();

onMounted(() => {
  userStore.checkAuth();
  if (!userStore.isAuthenticated) {
    router.push('/login');
  }
});

const updateUserEmail = async () => {
  userStore.isLoading = true;
  try {
    const response = await api.post('/User/update-user-data', { email: email.value, newEmail: newEmail.value });

    if (response.status === 200) {
      toast.add({ severity: 'success', summary: 'Sucesso', detail: 'Email atualizado com sucesso!', life: 3000 });
      setTimeout(() => {
        router.push('/dashboard');
      }, 3000);
    } else if (response.status === 400) {
      toast.add({ severity: 'error', summary: 'Erro', detail: response.data?.message || 'Não foi possível atualizar o email. Verifique os dados.', life: 5000 });
    } else {
      toast.add({ severity: 'error', summary: 'Erro', detail: response.data?.message || 'Erro ao atualizar o email.', life: 5000 });
    }
  } catch (error: any) {
    console.error('Erro ao atualizar o email:', error);
    toast.add({ severity: 'error', summary: 'Erro', detail: error.response?.data?.message || error.message || 'Erro ao atualizar o email.', life: 5000 });
  } finally {
    userStore.isLoading = false;
  }
};
</script>

<template>
  <div class="login-wrapper">
    <h1 class="login-title">Atualizar Email</h1>
    <div class="container">
      <h1>Atualizar seu Email</h1>
      <Toast />
      <div class="form-group">
        <label for="email">Email Atual</label>
        <InputText id="email" v-model="email" type="email" class="w-full" />
      </div>
      <div class="form-group">
        <label for="newEmail">Novo Email</label>
        <InputText id="newEmail" v-model="newEmail" type="email" class="w-full" />
      </div>
      <Button label="Atualizar Email" @click="updateUserEmail" :loading="userStore.isLoading" class="w-full" />
      <p class="mt-3"><router-link to="/dashboard">Voltar para o Dashboard</router-link></p>
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
</style>