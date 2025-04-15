<template>
    <div class="p-6 bg-gray-100 min-h-screen">
      <h1 class="text-3xl font-bold text-blue-700 mb-8">Editar Contato</h1>
      <Toast />
  
      <Card v-if="loading" class="shadow-lg">
        <template #content>
          <div class="animate-pulse space-y-4">
            <div class="h-10 bg-gray-300 rounded w-full"></div>
            <div class="h-10 bg-gray-300 rounded w-3/4"></div>
            <div class="h-10 bg-gray-300 rounded w-1/2"></div>
            <div class="h-10 bg-gray-300 rounded w-2/3"></div>
          </div>
        </template>
      </Card>
  
      <Card v-else-if="contact" class="shadow-lg">
        <template #content>
          <form @submit.prevent="handleUpdateContact" class="space-y-6">
            <div>
              <label for="name" class="block text-gray-700 text-sm font-bold mb-2">Nome:</label>
              <InputText
                id="name"
                v-model="contact.name"
                type="text"
                class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                placeholder="Nome"
              />
              <span v-if="v$.name.$error" class="text-red-500 text-xs italic">{{ v$.name.$errors[0].$message }}</span>
            </div>
  
            <div>
              <label for="email" class="block text-gray-700 text-sm font-bold mb-2">Email:</label>
              <InputText
                id="email"
                v-model="contact.email"
                type="email"
                class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                placeholder="Email"
              />
              <span v-if="v$.email.$error" class="text-red-500 text-xs italic">{{ v$.email.$errors[0].$message }}</span>
            </div>
  
            <div>
              <label for="phone" class="block text-gray-700 text-sm font-bold mb-2">Telefone:</label>
              <InputText
                id="phone"
                v-model="contact.phone"
                type="text"
                class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                placeholder="Telefone"
              />
              <span v-if="v$.phone.$error" class="text-red-500 text-xs italic">{{ v$.phone.$errors[0].$message }}</span>
            </div>
  
            <Button
              type="submit"
              class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
              :disabled="loading"
            >
              <span v-if="loading">Atualizando...</span>
              <span v-else>Salvar Alterações</span>
            </Button>
            <Button
              type="button"
              @click="cancelEdit"
              class="ml-2 bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
            >
              Cancelar
            </Button>
          </form>
        </template>
      </Card>
      <Card v-else>
        <template #content>
          <p class="text-gray-500 py-6 text-center">Contato não encontrado ou ID inválido.</p>
        </template>
      </Card>
    </div>
  </template>
  
  <script setup lang="ts">
  import { ref, onMounted } from 'vue';
  import { useRouter, useRoute } from 'vue-router';
  import { useContactStore } from '@/stores/contact';
  import { useToast } from 'primevue/usetoast';
  import Button from 'primevue/button';
  import InputText from 'primevue/inputtext';
  import Card from 'primevue/card';
  import { useVuelidate } from '@vuelidate/core';
  import { required, email, helpers } from '@vuelidate/validators';
  import type { Contact } from '@/interfaces/Contact';
  
  const router = useRouter();
  const route = useRoute();
  const contactStore = useContactStore();
  const toast = useToast();
  
  const contactId = String(route.params.id);
  const contact = ref<Contact>({ id: '', name: '', email: '', phone: '' }); // Adicione o 'id'
  const loading = ref(false);
  
  const rules = {
    name: { required },
    email: { required, email },
    phone: { required, helpers: { withMessage: 'Telefone é obrigatório e deve ter o formato correto.' } },
  };
  
  const v$ = useVuelidate(rules, contact.value);
  
  onMounted(async () => {
    if (contactId) {
      loading.value = true;
      try {
        const fetchedContact = await contactStore.getContactById(contactId);
        if (fetchedContact) {
          contact.value = fetchedContact;
        }
      } catch (error: any) {
        toast.add({
          severity: 'error',
          summary: 'Erro',
          detail: error.message || 'Erro ao buscar contato.',
          life: 5000,
        });
        router.push('/dashboard');
      } finally {
        loading.value = false;
      }
    } else {
      toast.add({
        severity: 'error',
        summary: 'Erro',
        detail: 'ID do contato inválido.',
        life: 5000,
      });
      router.push('/dashboard');
    }
  });
  
  const handleUpdateContact = async () => {
    const isFormValid = await v$.value.$validate();
    if (!isFormValid) {
      toast.add({
        severity: 'warn',
        summary: 'Aviso',
        detail: 'Por favor, preencha o formulário corretamente.',
        life: 3000,
      });
      return;
    }
  
    loading.value = true;
    try {
      await contactStore.updateContact(contactId, contact.value);
      toast.add({
        severity: 'success',
        summary: 'Sucesso',
        detail: 'Contato atualizado com sucesso!',
        life: 3000,
      });
      router.push('/dashboard');
    } catch (error: any) {
      toast.add({
        severity: 'error',
        summary: 'Erro',
        detail: error.message || 'Erro ao atualizar contato.',
        life: 5000,
      });
      console.error('Erro ao atualizar contato:', error);
    } finally {
      loading.value = false;
    }
  };
  
  const cancelEdit = () => {
    router.push('/dashboard');
  };
  </script>
  
  <style scoped>
  </style>
  