<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useUserStore } from '@/stores/user';
import { useContactStore } from '@/stores/contact';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Checkbox from 'primevue/checkbox';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import { useToast } from 'primevue/usetoast';
import { jwtDecode } from 'jwt-decode';
import type { Contact } from '@/interfaces/Contact';

const router = useRouter();
const userStore = useUserStore();
const contactStore = useContactStore();
const toast = useToast();
const userEmail = ref('');
const searchName = ref('');
const isPaged = ref(false);
const pageNumber = ref(1);
const pageSize = ref(10);
const contacts = ref<Contact[]>([]);
const isFetching = ref(false);

onMounted(async () => {
  userStore.checkAuth();
  if (!userStore.isAuthenticated) {
    router.push('/login');
    toast.add({ severity: 'warn', summary: 'Aviso', detail: 'Você precisa estar logado para acessar o dashboard.', life: 3000 });
  } else {
    const token = localStorage.getItem('authToken');
    if (token) {
      try {
        const decodedToken: any = jwtDecode(token);
        userEmail.value = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || 'Email não encontrado';
        await fetchContacts();
      } catch (error) {
        console.error('Erro ao decodificar o token:', error);
        userEmail.value = 'Erro ao obter email';
      }
    } else {
      userEmail.value = 'Token não encontrado';
    }
  }
});

const fetchContacts = async () => {
  if (isFetching.value) {
    return;
  }

  isFetching.value = true;
  try {
    const page = Number(pageNumber.value);
    const size = Number(pageSize.value);

    if (isNaN(page) || isNaN(size)) {
      toast.add({ severity: 'warn', summary: 'Aviso', detail: 'Por favor, insira valores numéricos para página e tamanho.', life: 3000 });
      return;
    }

    await contactStore.fetchContacts({
      SearchName: searchName.value,
      IsPaged: isPaged.value,
      PageNumber: page,
      PageSize: size,
    });
    contacts.value = contactStore.contacts;
  } catch (error: any) {
    console.error('Erro ao buscar contatos:', error);
    toast.add({ severity: 'error', summary: 'Erro', detail: error.message || 'Erro ao buscar contatos.', life: 5000 });
  } finally {
    isFetching.value = false;
  }
};

const resetPagination = () => {
  pageNumber.value = 1;
  pageSize.value = 10;
};

const handlePagedChange = () => {
  fetchContacts();
  if (!isPaged.value) {
    resetPagination();
  }
};

const deleteContact = async (id: string) => {
  if (confirm('Tem certeza que deseja deletar este contato?')) {
    try {
      await contactStore.deleteContact(id);
      toast.add({ severity: 'success', summary: 'Sucesso', detail: 'Contato deletado com sucesso!', life: 3000 });
      await fetchContacts();
    } catch (error: any) {
      console.error('Erro ao deletar contato:', error);
      toast.add({ severity: 'error', summary: 'Erro', detail: error.message || 'Erro ao deletar o contato.', life: 5000 });
    }
  }
};

const editContact = (id: string) => {
  router.push(`/contacts/edit/${id}`);
};

const logout = async () => {
  userStore.logout();
  toast.add({ severity: 'success', summary: 'Sucesso', detail: 'Logout realizado com sucesso!', life: 3000 });
  router.push('/login');
};

const goToCreateContact = () => {
  router.push('/contacts/create');
};

const goToUpdateEmail = () => {
  router.push('/update-email');
};
</script>

<template>
  <div class="p-6 bg-gray-100 min-h-screen">
    <h1 class="text-3xl font-bold text-blue-700 mb-8">Dashboard</h1>
    <Toast />
    <div class="bg-white rounded-lg shadow-md p-6 mb-8">
      <p class="text-lg text-gray-700 mb-4">Bem-vindo ao seu painel, <span class="font-semibold">{{ userEmail }}</span>!</p>

      <div class="mb-6">
        <h2 class="text-2xl font-semibold text-gray-800 mb-4">Lista de Contatos</h2>
        <div class="flex flex-wrap gap-4 items-end mb-4">
          <div class="flex-grow">
            <label for="searchName" class="block text-gray-700 text-sm font-bold mb-2">Buscar por Nome:</label>
            <InputText
              id="searchName"
              v-model="searchName"
              @input="fetchContacts"
              placeholder="Buscar por Nome"
              class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            />
          </div>

          <div class="flex items-center">
            <Checkbox
              id="isPaged"
              v-model="isPaged"
              @change="handlePagedChange"
              :true-value="true"
              :false-value="false"
              class="mr-2"
            />
            <label for="isPaged" class="text-gray-700 text-sm font-medium">Paginado</label>
          </div>

          <div v-if="isPaged" class="flex-grow">
            <label for="pageNumber" class="block text-gray-700 text-sm font-bold mb-2">Página:</label>
            <InputNumber
              id="pageNumber"
              v-model.number="pageNumber"
              @change="fetchContacts"
              class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
              :min="1"
              placeholder="Página"
            />
          </div>

          <div v-if="isPaged" class="flex-grow">
            <label for="pageSize" class="block text-gray-700 text-sm font-bold mb-2">Tamanho da Página:</label>
            <InputNumber
              id="pageSize"
              v-model.number="pageSize"
              @change="fetchContacts"
              class="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
              :min="1"
              placeholder="Tamanho"
            />
          </div>
          <div class="mt-2">
             <Button label="Atualizar" icon="pi pi-refresh" @click="fetchContacts" class="p-button-info" />
          </div>
        </div>

        <DataTable
          v-if="contacts.length > 0"
          :value="contacts"
          responsiveLayout="scroll"
          class="shadow-md rounded-lg overflow-hidden"
        >
          <Column field="name" header="Nome" class="font-semibold"></Column>
          <Column field="email" header="Email"></Column>
          <Column field="phone" header="Telefone"></Column>
          <Column header="Ações">
            <template #body="{ data }">
              <div class="flex gap-2">
                <Button icon="pi pi-pencil" class="p-button-warning" @click="editContact(data.id)" />
                <Button icon="pi pi-trash" class="p-button-danger" @click="deleteContact(data.id)" />
              </div>
            </template>
          </Column>
        </DataTable>
        <div v-else class="mt-6 text-gray-500 text-center">
          Nenhum contato encontrado.
        </div>
      </div>
      <div class="flex flex-wrap gap-4 justify-center mt-8">
        <Button label="Alterar Email" icon="pi pi-envelope" @click="goToUpdateEmail" class="p-button-warning" />
        <Button label="Novo Contato" icon="pi pi-user-plus" @click="goToCreateContact" class="p-button-success" />
        <Button label="Sair" icon="pi pi-sign-out" @click="logout" class="p-button-danger" />
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Estilos podem ser adicionados aqui se necessário */
.dashboard-buttons {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  justify-content: center;
  margin-top: 20px;
}
</style>
