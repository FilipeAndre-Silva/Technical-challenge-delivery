<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'primevue/usetoast';
import { useContactStore } from '@/stores/contact';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';

const name = ref('');
const email = ref('');
const phone = ref('');
const router = useRouter();
const toast = useToast();
const contactStore = useContactStore();
const isLoading = ref(false);

const createContact = async () => {
  isLoading.value = true;
  try {
    const contactData = {
      name: name.value,
      email: email.value,
      phone: phone.value,
    };
    await contactStore.createContact(contactData);
    toast.add({ severity: 'success', summary: 'Sucesso', detail: 'Contato criado com sucesso!', life: 3000 });
    router.push('/dashboard');
  } catch (error: any) {
    console.error('Erro ao criar contato:', error);
    if (error.response?.status === 400 && error.response?.data?.errors) {
      // Erros de validação do backend
      Object.entries(error.response.data.errors).forEach(([key, value]) => {
        if (Array.isArray(value)) {
          value.forEach(errorMessage => {
            toast.add({ severity: 'error', summary: `Erro em ${key}`, detail: errorMessage, life: 5000 });
          });
        } else if (typeof value === 'string') {
          toast.add({ severity: 'error', summary: `Erro em ${key}`, detail: value, life: 5000 });
        }
      });
    } else {
      // Outros erros
      toast.add({ severity: 'error', summary: 'Erro', detail: error.message || 'Erro ao criar o contato.', life: 5000 });
    }
  } finally {
    isLoading.value = false;
  }
};
</script>

<template>
  <div class="create-contact-view">
    <h1>Criar Novo Contato</h1>
    <Toast />
    <div class="form-group">
      <label for="name">Nome</label>
      <InputText id="name" v-model="name" type="text" class="w-full" />
    </div>
    <div class="form-group">
      <label for="email">Email</label>
      <InputText id="email" v-model="email" type="email" class="w-full" />
    </div>
    <div class="form-group">
      <label for="phone">Telefone</label>
      <InputText id="phone" v-model="phone" type="text" class="w-full" />
    </div>
    <Button label="Criar Contato" @click="createContact" :loading="isLoading" class="w-full" />
    <Button label="Cancelar" @click="router.go(-1)" class="w-full mt-2 p-button-secondary" />
  </div>
</template>

<style scoped>
.create-contact-view {
  padding: 20px;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

.w-full {
  width: 100%;
}

.mt-2 {
  margin-top: 0.5rem;
}
</style>