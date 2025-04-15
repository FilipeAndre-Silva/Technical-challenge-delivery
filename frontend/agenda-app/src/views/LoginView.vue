<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'primevue/usetoast';
import { useUserStore } from '@/stores/user';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import { onMounted } from 'vue';

const email = ref('');
const password = ref('');
const router = useRouter();
const toast = useToast();
const userStore = useUserStore();

onMounted(() => {
  userStore.checkAuth();
  if (userStore.isAuthenticated) {
    router.push('/dashboard');
  }
});

const loginUser = async () => {
  try {
    await userStore.login({ email: email.value, password: password.value });
    console.log('userStore.isAuthenticated após login:', userStore.isAuthenticated);
    if (userStore.isAuthenticated) {
      toast.add({ severity: 'success', summary: 'Sucesso', detail: 'Login realizado com sucesso!', life: 2000 });
      setTimeout(() => {
        router.push('/dashboard');
        console.log('Redirecionando para /dashboard após 2 segundos');
      }, 2000);
    } else {
      toast.add({ severity: 'error', summary: 'Erro', detail: 'Não foi possível fazer o login. Verifique suas credenciais.', life: 3000 });
    }
  } catch (error: any) {
    // ...
  }
};
</script>

<template>
  <div class="login-wrapper">
    <h1 class="login-title">Agenda Online</h1>
    <div class="container">
      <h1>Login</h1>
      <Toast />
      <div class="form-group">
        <label for="email">Email</label>
        <InputText id="email" v-model="email" type="email" class="w-full" />
      </div>
      <div class="form-group">
        <label for="password">Senha</label>
        <InputText id="password" v-model="password" type="password" class="w-full" />
      </div>
      <Button label="Login" @click="loginUser" :loading="userStore.isLoading" class="w-full" />
      <p class="mt-3">Não tem uma conta? <router-link to="/register">Registre-se</router-link></p>
      <p class="mt-2"><router-link to="/reset-password">Esqueceu sua senha?</router-link></p>
      <Button label="Confirmar Email" class="mt-2 w-full" @click="router.push('/reset-password-confirmation')" />
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