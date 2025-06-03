<template>

  <AddWydarzenie :addWydarzenieDialog="addWydarzenieDialog" @update:addWydarzenieDialog="addWydarzenieDialog = $event" @save-wydarzenie="handleSaveWydarzenie" />
  <EditWydarzenie :editWydarzenieDialog="editWydarzenieDialog" :wydarzenie="selectedWydarzenie" @update:editWydarzenieDialog="editWydarzenieDialog = $event" @update-wydarzenie="handleUpdateWydarzenie" />

    <v-navigation-drawer :width="200">
      <v-list-item title="Menedżer stada"></v-list-item>
      <v-divider></v-divider>

      <v-list-item :to="{ path: '/kartalochy' }" link title="Karta lochy"></v-list-item>
      <v-list-item :to="{ path: '/wydarzenia' }" link title="Wydarzenia"></v-list-item>
      <v-list-item :to="{ path: '/stado' }" link title="Stado"></v-list-item>
      <!-- <v-list-item :to="{ path: '/import' }" link title="Importuj dane"></v-list-item> -->
  
    </v-navigation-drawer>
    <v-app-bar title="Wydarzenia">

      <v-btn
          style="min-width: 0; width: 100px; background-color: green; margin-right: 40px; "
          size="small"
          @click="addItem()"
        >Dodaj</v-btn>   
    </v-app-bar>

    <v-data-table
      class="Wydarzenia"
      :headers="headers"
      :items="Wydarzenia"
      item-key="id"
      :pageText="'{0}-{1} z {2}'"
      items-per-page-text="Elementów na stronę" 
      :sort-by="sortBy"
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
import EditWydarzenie from "@/components/EditWydarzenie.vue";

const Wydarzenia = ref([]);
const error = ref(null);
const addWydarzenieDialog = ref(false);
const editWydarzenieDialog = ref(false);
const selectedWydarzenie = ref(null);
const headers = ref(
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
    title: "Uwagi",
    value: "uwagi",
  },
  {
    title: "Data wydarzenia",
    value: "dataWydarzenia",
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
]);  

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
  editWydarzenieDialog.value = true;
  selectedWydarzenie.value = item;
};

const deleteItem = (item) => {
  apiClient.delete("/Wydarzenie/" + item.id);
  Wydarzenia.value = Wydarzenia.value.filter((wydarzenie) => wydarzenie.id !== item.id);
};

const handleSaveWydarzenie = (noweWydarzenie) => {
  const teraz = new Date();
  const padZero = (num) => String(num).padStart(2, '0');
  const sformatowanaData = 
    `${teraz.getFullYear()}-${padZero(teraz.getMonth() + 1)}-${padZero(teraz.getDate())} ` +
    `${padZero(teraz.getHours())}:${padZero(teraz.getMinutes())}:${padZero(teraz.getSeconds())}`;
  noweWydarzenie.dataCzasUtworzenia = sformatowanaData;
  noweWydarzenie.dataCzasModyfikacji = sformatowanaData;
  noweWydarzenie.id = Wydarzenia.value[Wydarzenia.value.length-1].id + 1;
  Wydarzenia.value.push(noweWydarzenie);
};

const handleUpdateWydarzenie = (editedWydarzenie) => {
  const teraz = new Date();
  const padZero = (num) => String(num).padStart(2, '0');
  const sformatowanaData = 
    `${teraz.getFullYear()}-${padZero(teraz.getMonth() + 1)}-${padZero(teraz.getDate())} ` +
    `${padZero(teraz.getHours())}:${padZero(teraz.getMinutes())}:${padZero(teraz.getSeconds())}`;
  editedWydarzenie.dataCzasModyfikacji = sformatowanaData;
  const index = Wydarzenia.value.findIndex((wydarzenie) => wydarzenie.id === editedWydarzenie.id);
  Wydarzenia.value[index] = editedWydarzenie;
}

const sortBy = ref([{ key: 'dataWydarzenia', order: 'desc'}]);
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
