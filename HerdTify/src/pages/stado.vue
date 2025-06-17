<template>
  <AddLocha 
    :addLochaDialog="addLochaDialog" 
    @update:addLochaDialog="addLochaDialog = $event" 
    @save-locha="handleSaveLocha"/>
  <EditLocha 
    :editLochaDialog="editLochaDialog" 
    :locha="selectedLocha" 
    @update:editLochaDialog="editLochaDialog = $event" 
    @update-locha="handleUpdateLocha"/>
  
  <NavigationDrawer/>   
  <v-app-bar title="Stado" class="appBar">
    <v-btn
        style="min-width: 0; width: 100px; background-color: green; margin-right: 40px; "
        size="small"
        @click="addItem()"
      >Dodaj</v-btn>  
  </v-app-bar>

  <v-data-table
    class="Lochy"
    item-key="id"
    :headers="headers"
    :items="Lochy"
    :pageText="'{0}-{1} z {2}'"
    items-per-page-text="Elementów na stronę"
    :sort-by="sortBy">
  <template v-slot:item.actions="{ item }">
      <v-btn
        class="me-2"
        style="min-width: 0; width: 10px; background-color: #d67804;"
        size="small"
        @click="editItem(item)">
      <v-icon>mdi-pencil</v-icon>
      </v-btn>
      <v-btn
        style="min-width: 0; width: 10px; background-color: red;"
        size="small"
        @click="deleteItem(item)">
        <v-icon>mdi-delete</v-icon>
      </v-btn>
    </template>
  </v-data-table>
</template>
<script setup>
import AddLocha from '@/components/AddLocha.vue';
import EditLocha from '@/components/EditLocha.vue';
import NavigationDraver from '@/components/NavigationDrawer.vue';
import apiClient from '@/plugins/axios';
import { onMounted, ref } from 'vue';
import "@/styles/appBar.scss"

const error = ref(null);
const Lochy = ref([]);
const addLochaDialog = ref(false);
const editLochaDialog = ref(false);
const selectedLocha = ref(null);
const headers = 
[
  {
    title: "Id",
    value: "id",
    sortable: true,
  },
  {
    title: "Numer",
    value: "numerLochy",
    sortable: true,
  },
  {
    title: "Status",
    value: "status",
    sortable: true,
  }, 
  {
    title: "Mioty(id)",
    value: "miotyId",
  },
  {
    title: "Wydarzenia(id)",
    value: "wydarzeniaLochyId",
  },
  {
    title: "Uwagi",
    value: "uwagi",
    sortable: false,
  },
  {
    title: "Data brakowania",
    value: "dataBrakowania",
    sortable: true,
  },
  {
    title: "IndeksProd365Dni",
    value: "indeksProdukcji365Dni",
  },
  {
    title: "Data dodania",
    value: "dataCzasUtworzenia",
    sortable: true,
  },
  {
    title: "Data modyfikacji",
    value: "dataCzasModyfikacji",
    sortable: true,
  },
  {
    title: "Działania", 
    key: "actions", 
    sortable: false, 
    align: "center"}
];  


const addItem = () => {
  addLochaDialog.value = true;
};

const editItem = (item) => {
  console.log("Edytuj lochę o id:"); //Debugowanie
  selectedLocha.value = item;
  editLochaDialog.value = true;
};

const deleteItem = async (item) => {
  console.log("Usuń lochę o id:"); //Debugowanie
  
  try {
    await apiClient.delete(`/Locha/${item.id}`);
    Lochy.value = Lochy.value.filter((locha) => locha.id !== item.id);
  } catch (error) {
    console.error("Błąd podczas usuwania lochy:", error);
  }
};

const handleSaveLocha = (locha) => {
  const teraz = new Date();
  const padZero = (num) => String(num).padStart(2, '0');
  const sformatowanaData = 
    `${teraz.getFullYear()}-${padZero(teraz.getMonth() + 1)}-${padZero(teraz.getDate())} ` +
    `${padZero(teraz.getHours())}:${padZero(teraz.getMinutes())}:${padZero(teraz.getSeconds())}`;
  locha.dataCzasUtworzenia = sformatowanaData;
  locha.dataCzasModyfikacji = sformatowanaData;
  locha.indeksProdukcji365Dni = 0;
  Lochy.value.push(locha);
};

const handleUpdateLocha = (locha) => {
  const teraz = new Date();
  const padZero = (num) => String(num).padStart(2, '0');
  const sformatowanaData = 
    `${teraz.getFullYear()}-${padZero(teraz.getMonth() + 1)}-${padZero(teraz.getDate())} ` +
    `${padZero(teraz.getHours())}:${padZero(teraz.getMinutes())}:${padZero(teraz.getSeconds())}`;
  locha.dataCzasModyfikacji = sformatowanaData;
  selectedLocha.value.status = locha.status;
  selectedLocha.value.numerLochy = locha.numerLochy;
  selectedLocha.value.dataCzasModyfikacji = locha.dataCzasModyfikacji;
  selectedLocha.value.uwagi = locha.uwagi;
  selectedLocha.value.dataBrakowania = locha.dataBrakowania;
}

onMounted(async () => {
    try {
        const response = await apiClient.get("/Locha");
        Lochy.value = response.data;
    } catch (err) {
        error.value = err;
    }
})

const sortBy = ref([{ key: 'id', order: 'desc'}]);
</script>