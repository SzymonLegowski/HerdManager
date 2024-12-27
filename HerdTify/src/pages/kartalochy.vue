<template>
    <v-navigation-drawer :width="200">
      <v-list-item title="Menedżer stada"></v-list-item>
      <v-divider></v-divider>
      <v-list-item :to="{ path: '/kartalochy' }" link title="Karta lochy"></v-list-item>
      <v-list-item :to="{ path: '/wydarzenia' }" link title="Wydarzenia"></v-list-item>
    </v-navigation-drawer>
    <v-app-bar title="Karta lochy nr:">
      <v-autocomplete
        class="autocompleteNrLochy"
        :items="numeryLoch"
        label="Nr Lochy"
        width="100"
      ></v-autocomplete>
      <v-btn class="AddButton" variant="outlined">
        Dodaj
      </v-btn>
      <v-btn class="DeleteButton" variant="outlined">
        Usuń
      </v-btn>
    </v-app-bar>
    <v-data-table
      class="KartaLochy"
      :headers="headers"
      :items="items"
      item-key="name"
      items-per-page="5"
      :pageText="'{0}-{1} z {2}'"
      items-per-page-text="Elementów na stronę"
    ></v-data-table>
  </template>
  
  <script setup>
  import { ref, onMounted } from "vue";
  import apiClient from "@/plugins/axios";
  
  const Lochy = ref([]);
  const Wydarzenia = ref([]);
  const Mioty = ref([]);
  const numeryLoch = ref([]);
  const error = ref(null);
  
  const headers = [
    {
      title: "Nr miotu",
      class: "Nrmiotu,"
    },
    {
      title: "Data",
      align: "center",
      children: [
        { title: "Pokrycia" },
        { title: "Przew. oproszenia" },
        { title: "Oproszenia" },
        { title: "Odsadzenia" }
      ]
    },
    {
      title: "Liczba prosiąt",
      align: "center",
      children: [
        { title: "ur. żywe" },
        { title: "ur. martwe" },
        { title: "Przygnieconych" },
        { title: "Odsadzonych" }
      ]
    }
  ];
  
  onMounted(async () => {
  try {
    const lochyResponse = await apiClient.get("/Locha");
    console.log("Dane lochy:", lochyResponse.data); // Debugowanie
    Lochy.value = Array.isArray(lochyResponse.data) ? lochyResponse.data : [];
    numeryLoch.value = Lochy.value.map(locha => locha.numerLochy); // Dopasuj klucz do struktury danych
    console.log("Numery loch:", numeryLoch.value);
  } catch (e) {
    console.error("Błąd podczas pobierania danych:", e);
    error.value = e;
  }
});

  </script>
  
  <style>
  .autocompleteNrLochy {
    min-width: 130px;
    position: absolute;
    left: auto;
    transform: translate(160px, 10px);
  }
  .KartaLochy {
    border-color: blue;
  }
  .AddButton {
    transform: translate(-30px, 0px);
  }
  .DeleteButton {
    transform: translate(-10px, 0px);
  }
  th {
    border-style: solid;
    border-color: #1a1a1a;
  }
  </style>
  