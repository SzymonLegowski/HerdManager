<template>

<AddWydarzenie 
:addWydarzenieDialog="addWydarzenieDialog" 
@update:addWydarzenieDialog="addWydarzenieDialog = $event"/>

  <v-navigation-drawer :width="200">
    <v-list-item title="Menedżer stada"></v-list-item>
    <v-divider></v-divider>
    <v-list-item :to="{ path: '/kartalochy' }" link title="Karta lochy"></v-list-item>

    <v-list-item :to="{ path: '/wydarzenia' }" link title="Wydarzenia"></v-list-item>
  </v-navigation-drawer>

  <v-app-bar title="Wydarzenia">
    <v-btn
        style="min-width: 0; width: 150px; background-color: green; margin-right: 40px; "
        size="small"
        @click="addItem()"
      >dodaj nowe</v-btn>  
  </v-app-bar>

  <v-data-table
    class="Wydarzenia"
    :headers="headers"
    :items="Wydarzenia"
    item-key="id"
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
import { ref, onMounted } from "vue";
import apiClient from "@/plugins/axios";
import AddWydarzenie from "@/components/AddWydarzenie.vue";

const Wydarzenia = ref([]);
const error = ref(null);
const addWydarzenieDialog = ref(false);
const headers = 
[
  {
    title: "Id",
    value: "id",
    sortable: true,
  },
  {
    title: "Typ",
    value: "typWydarzenia",
    sortable: true,
  },
  {
    title: "Data wydarzenia",
    value: "dataWydarzenia",
    sortable: true,
  },
  {
    title: "Data wykonania",
    value: "dataWykonania",
    sortable: true,
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
    title: "Lochy(numery)",
    value: "numeryLoch",
    sortable: true,
  },
  {
    title: "Mioty(id)",
    value: "miotyId",
    sortable: true,
  },
  {
    title: "Działania", 
    key: "actions", 
    sortable: false, 
    align: "center"}
];  

onMounted(async () => {
  try {
    const response = await apiClient.get("/Wydarzenie");
    Wydarzenia.value = response.data;
    
    for (let indeksWydarzenia = 0; indeksWydarzenia < Wydarzenia.value.length; indeksWydarzenia++)
    {
      const numeryLochWydarzenia = [];
      Wydarzenia.value[indeksWydarzenia].numeryLoch = [];
      
      for (let indeksLochy = 0; indeksLochy < Wydarzenia.value[indeksWydarzenia].lochyId.length; indeksLochy++)
      {
        const responseLocha = await apiClient.get("/Locha/" + Wydarzenia.value[indeksWydarzenia].lochyId[indeksLochy]);
          numeryLochWydarzenia.push(responseLocha.data.numerLochy);
      }
      
      Wydarzenia.value[indeksWydarzenia].numeryLoch = numeryLochWydarzenia;
    }

  } catch (err) {
    error.value = err;
  }
});


const addItem = () => {
  addWydarzenieDialog.value = true;
};

const editItem = (item) => {
  console.log("Edytuj wydarzenie o id:"); //Debugowanie
};

const deleteItem = (item) => {
  console.log("Usuń wydarzenie o id:"); //Debugowanie
  apiClient.delete("/Wydarzenie/" + item.id);
  Wydarzenia.value = Wydarzenia.value.filter((wydarzenie) => wydarzenie.id !== item.id);
};

</script>
<style lang="scss">

th {
    border-style: solid;
    border-color: #1a1a1a;
    border-width: 2px;
  }
  td {
    border-style: solid;
    border-color: #1a1a1a;
    border-width: 2px;
  }
</style>
