import { createApp } from 'vue';
import App from './App.vue';
import router from './router';
import { createPinia } from 'pinia';
import { useUserStore } from '@/stores/user';

import PrimeVue from 'primevue/config';
import Aura from '@primeuix/themes/aura';
import Button from "primevue/button";
import InputText from "primevue/inputtext";

import ToastService from 'primevue/toastservice';
import Toast from 'primevue/toast';

import 'primeicons/primeicons.css';

import './assets/main.css';

const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(router);
app.use(PrimeVue, {
    theme: {
        preset: Aura
    }
});

app.use(ToastService);

app.component('Button', Button);
app.component('InputText', InputText);
app.component('Toast', Toast);

app.mount('#app');