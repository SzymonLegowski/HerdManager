<template>
     <v-navigation-drawer :width="200">
    
    <v-list-item title="Menedżer stada"></v-list-item>
      
    <v-divider></v-divider>
      
      <v-list-item :to="{ path: '/kartalochy' }" link title="Karta lochy"></v-list-item>
      <v-list-item :to="{ path: '/wydarzenia' }" link title="Wydarzenia"></v-list-item>
      <v-list-item :to="{ path: '/stado' }" link title="Stado"></v-list-item>
    
  </v-navigation-drawer>

  <v-app-bar title="Stado">
    <v-btn
        style="min-width: 0; width: 100px; background-color: green; margin-right: 40px; "
        size="small"
        @click="addItem()"
      >Nowa</v-btn>  
  </v-app-bar>

  <v-data-table
    class="Lochy"
    item-key="id"
    :headers="headers"
    :items="Lochy"
    :pageText="'{0}-{1} z {2}'"
    items-per-page-text="Elementów na stronę"
  >
  <template v-slot:item.actions="{ item }">
      <v-btn
        class="me-2"
        style="min-width: 0; width: 10px; background-color: #d67804;"
        size="small"
        @click="editItem(item)"
      >
        <v-icon>mdi-pencil</v-icon>
      </v-btn>
      <v-btn
        style="min-width: 0; width: 10px; background-color: red;"
        size="small"
        @click="deleteItem(item)"
      >
        <v-icon>mdi-delete</v-icon>
      </v-btn>
    </template>
  </v-data-table>
</template>
<script setup>
import apiClient from '@/plugins/axios';
import { onMounted, ref } from 'vue';

const error = ref(null);
const Lochy = ref([]);
const addLochaDialog = ref(false);
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
    title: "Mioty(id)",
    value: "miotyId",
  },
  {
    title: "Wydarzenia(id)",
    value: "wydarzeniaLochyId",
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
  console.log("Edytuj wydarzenie o id:"); //Debugowanie
};

const deleteItem = (item) => {
  console.log("Usuń wydarzenie o id:"); //Debugowanie
  apiClient.delete("/Wydarzenie/" + item.id);
  Wydarzenia.value = Wydarzenia.value.filter((wydarzenie) => wydarzenie.id !== item.id);
};

onMounted(async () => {
    try {
        const response = await apiClient.get("/Locha");
        Lochy.value = response.data;
    } catch (err) {
        error.value = err;
    }
})

</script>