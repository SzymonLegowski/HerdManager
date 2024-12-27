<template>
  <v-navigation-drawer :width="200">
    <v-list-item title="Menedżer stada"></v-list-item>
    <v-divider></v-divider>
    <v-list-item :to="{ path: '/kartalochy' }" link title="Karta lochy">
    </v-list-item>

    <v-list-item :to="{ path: '/wydarzenia' }" link title="Wydarzenia">
    </v-list-item>
  </v-navigation-drawer>
  <v-app-bar title="Wydarzenia"> </v-app-bar>
  <v-data-table
    class="Wydarzenia"
    :headers="headers"
    :items="Wydarzenia"
    item-key="id"
    items-per-page="5"
    :pageText="'{0}-{1} z {2}'"
    items-per-page-text="Elementów na stronę"
  ></v-data-table>
</template>

<script setup>
import { ref, onMounted } from "vue";
import apiClient from "@/plugins/axios";

const Wydarzenia = ref([]);
const error = ref(null);
const headers = 
[
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
    title: "Lochy",
    value: "lochyId",
    sortable: true,
  },
  {
    title: "Mioty",
    value: "miotyId",
    sortable: true,
  }
];  

onMounted(async () => {
  try {
    const response = await apiClient.get("/Wydarzenie");
    Wydarzenia.value = response.data;
  } catch (err) {
    error.value = err;
  }
});
</script>
