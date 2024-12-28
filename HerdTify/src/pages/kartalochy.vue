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
        item-text="numerLochy"
        item-value="numerLochy"
        v-model="selectedLocha"
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
      :items="combinedItems"
      item-key="id"
      items-per-page="5"
      :pageText="'{0}-{1} z {2}'"
      items-per-page-text="Elementów na stronę"
    ></v-data-table>
  </template>
  
  <script setup>
  import { ref, onMounted } from "vue";
  import apiClient from "@/plugins/axios";
  
  const Lochy = ref([]);
  const numeryLoch = ref([]);
  const WydarzeniaLochy = ref([]);
  const Mioty = ref([]);
  const error = ref(null);
  const selectedLocha = ref(null);
  
  const combinedItems = computed(() => {
  const combined = [];

  Mioty.value.forEach(miot => {
    const combinedItem = {
      id: miot.id,
      urodzoneZywe: miot.urodzoneZywe,
      urodzoneMartwe: miot.urodzoneMartwe,
      przygniecone: miot.przygniecone,
      odsadzone: miot.odsadzone,
      ocena: miot.ocena,
    };

    miot.wydarzeniaMiotuId?.forEach(wydarzenieId => {
      const wydarzenie = WydarzeniaLochy.value.find(w => w.id === wydarzenieId);
      if (wydarzenie) {
        switch (wydarzenie.typWydarzenia) {
          case "Krycie":
            combinedItem.dataKrycia = wydarzenie.dataWydarzenia;
            break;
          case "PrzewidywaneProszenie":
            combinedItem.dataPrzewidywanegoProszenia = wydarzenie.dataWydarzenia;
            break;
          case "Proszenie":
            combinedItem.dataProszenia = wydarzenie.dataWydarzenia;
            break;
          case "Odsadzanie":
            combinedItem.dataOdsadzania = wydarzenie.dataWydarzenia;
            break;
        }
      }
    });

    combined.push(combinedItem);
  });

  return combined;
});

  const headers = [
    {
      title: "Nr miotu",
      value: "id",
    },
    {
      title: "Data",
      align: "center",
      children: [
        { title: "Pokrycia", value: "dataKrycia" },
        { title: "Przew. oproszenia", value: "dataPrzewidywanegoProszenia" },
        { title: "Oproszenia", value: "dataProszenia" },
        { title: "Odsadzenia", value: "dataOdsadzania" }
      ]
    },
    {
      title: "Liczba prosiąt",
      align: "center",
      children: [
        { title: "ur. żywe", value: "urodzoneZywe" },
        { title: "ur. martwe", value: "urodzoneMartwe" },
        { title: "Przygnieconych", value: "przygniecone" },
        { title: "Odsadzonych", value: "odsadzone" }
      ]
    }
  ];

  
  onMounted(async () => {
  try {
    const lochyResponse = await apiClient.get("/Locha");
    console.log("Dane lochy:", lochyResponse.data); // Debugowanie
    Lochy.value = Array.isArray(lochyResponse.data) ? lochyResponse.data : [];
    numeryLoch.value = Lochy.value.map(locha => locha.numerLochy).sort((a, b) => a - b); // Dopasuj klucz do struktury danych
    console.log("Numery loch:", numeryLoch.value);
  } catch (e) {
    console.error("Błąd podczas pobierania danych:", e);
    error.value = e;
  }
});

watch(selectedLocha, async (newValue, oldValue) => {
  console.log("Selected locha zmieniona z:", oldValue, "na:", newValue);
  if (newValue !== null) {
    try {
      const selected = Lochy.value.find(locha => locha.numerLochy === newValue);
    if(selected)
    {
      console.log("Dane lochy:", selected); // Debugowanie
      if (selected.wydarzeniaLochyId) {
      console.log(`WydarzeniaLochyId dla lochy ${newValue}:`);
      selected.wydarzeniaLochyId.forEach(async wydarzenieId => {
        const wydarzenie = await apiClient.get(`/Wydarzenie/${wydarzenieId}`);
        console.log("Wydarzenie:", wydarzenie.data); // Debugowanie
        WydarzeniaLochy.value.push(wydarzenie.data);
      });
      console.log("Wydarzenia lochy:", WydarzeniaLochy.value);
    } else {
      console.log(`Brak Wydarzeń dla lochy ${newValue}`);
    }
      if (selected.miotyId)
      {
        console.log(`MiotyId dla lochy ${newValue}:`);
        selected.miotyId.forEach(async miotId => {
          const miot = await apiClient.get(`/Miot/${miotId}`);
          console.log("Miot:", miot.data); // Debugowanie
          Mioty.value.push(miot.data);
        });
        console.log("Mioty:", Mioty.value);
        console.log("Combined items:", combinedItems.value);
      }
      else {
        console.log(`Brak Miotów dla lochy ${newValue}`);
      }
    }
    } catch (e) {
      console.error("Błąd podczas pobierania danych wybranej lochy:", e);
      error.value = e;
    }
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
  