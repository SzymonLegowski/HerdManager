<template>
  
  <LochyGrid v-if="showGrid" :items="numeryLoch" @update:selectedLocha="updateSelectedLocha"/>
  <AddMiot :addMiotDialog="addMiotDialog" :nrLochy="selectedLocha" :krycieId="ostatnieKrycieId" @update:addMiotDialog="addMiotDialog = $event"/>
  

  <v-navigation-drawer :width="200">
    <v-list-item title="Menedżer stada"></v-list-item>
    <v-divider></v-divider>
      
      <v-list-item :to="{ path: '/kartalochy' }" link title="Karta lochy"></v-list-item>
      <v-list-item :to="{ path: '/wydarzenia' }" link title="Wydarzenia"></v-list-item>
      <v-list-item :to="{ path: '/stado' }" link title="Stado"></v-list-item>
      <v-list-item :to="{ path: '/import' }" link title="Importuj dane"></v-list-item>
    
  </v-navigation-drawer>
    
    <v-app-bar title="Karta lochy nr:" class="appBar">
      
      <v-autocomplete
        class="autocompleteNrLochy"
        :items="numeryLoch"
        label="Nr Lochy"
        item-text="numerLochy"
        item-value="numerLochy"
        v-model="selectedLocha"
        width="100"
        ></v-autocomplete>
        
      <v-btn icon size-adjustable class="gridButton" @click="gridButtonClick">
        <v-icon size="large">mdi-grid</v-icon>
      </v-btn>
      
      <v-btn
        style="min-width: 0; width: 100px; background-color: green; margin-right: 40px; "
        size="small"
        @click="addItem()"
      >Dodaj</v-btn>  
    
    </v-app-bar>
    
    <v-data-table
      class="KartaLochy"
      :headers="headers"
      :items="Mioty"
      item-key="id"
      items-per-page="5"
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
    <template v-slot:no-data></template>
   
  </v-data-table>
  
  </template>
  
  <script setup>
  import { ref, onMounted } from "vue";
  import apiClient from "@/plugins/axios";
  import LochyGrid from "@/components/LochyGrid.vue";
  import AddMiot from "@/components/AddMiot.vue";

  const Lochy = ref([]);
  const numeryLoch = ref([]);
  const Mioty = ref([]);
  const error = ref(null);
  const selectedLocha = ref(null);
  const showGrid = ref(false);
  const addMiotDialog = ref(false);
  const ostatnieKrycieId = ref(null);

  const baseHeaders = [
    {
      title: "Miot",
      value: "id",
    },
    {
      title: "Data",
      align: "center",
      children: [
        { title: "Przew. oproszenia", value: "dataPrzewidywanegoProszenia" },
        { title: "Oproszenia", value: "dataProszenia" },
        { title: "Odsadzenia", value: "dataOdsadzenia" }
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
    },
    {title: "Działania", key: "actions", sortable: false, align: "center"}
  ];
  
  const headers = ref(JSON.parse(JSON.stringify(baseHeaders)));

  onMounted(async () => {
    getData();
});

watch(selectedLocha, async (newValue, oldValue) => {
  let ostatniIndeksWydarzenia = 0;
  let najwiekszaLiczbaKrycMiotu = 0;
  Mioty.value = [];
  headers.value = JSON.parse(JSON.stringify(baseHeaders));
  console.log("Selected locha zmieniona z:", oldValue, "na:", newValue); //Debugowanie
  console.log("HeadersValue:", headers); // Debugowanie
  if (newValue !== null) 
  {
    try 
    {
      const selected = Lochy.value.find(locha => locha.numerLochy === newValue);
      if (selected.miotyId)
      {
        console.log(`MiotyId dla lochy ${newValue}:`); //Debugowanie
        for (let indeksMiotu = 0; indeksMiotu < selected.miotyId.length; indeksMiotu++) 
        { 
          const miotId = selected.miotyId[indeksMiotu];
          const miotResponse = await apiClient.get(`/Miot/${miotId}`);
          const miot = miotResponse.data;
          miot.datyKrycia = [];

          for (let indeksWydarzenia = ostatniIndeksWydarzenia; indeksWydarzenia < selected.wydarzeniaLochyId.length; indeksWydarzenia++) 
          {
            const wydarzenieId = selected.wydarzeniaLochyId[indeksWydarzenia];
            const wydarzenieResponse = await apiClient.get(`/Wydarzenie/${wydarzenieId}`);
            const wydarzenie = wydarzenieResponse.data;

            if (new Date(wydarzenie.dataWydarzenia).getTime() < new Date(miot.dataPrzewidywanegoProszenia).getTime()) 
            {
              miot.datyKrycia.push(wydarzenie.dataWydarzenia);
              ostatniIndeksWydarzenia++;
              console.log("Indeks wydarzenia:", indeksWydarzenia);  // Debugowanie
              if (najwiekszaLiczbaKrycMiotu < miot.datyKrycia.length) 
              {
                najwiekszaLiczbaKrycMiotu = miot.datyKrycia.length;
              }
            } 
            else 
            {
              break;
            }
          }
          console.log("wydarzeniaLochyId:", selected.wydarzeniaLochyId); // Debugowanie
          ostatnieKrycieId.value = selected.wydarzeniaLochyId[selected.wydarzeniaLochyId.length - 1];
          console.log("ostatniIndeksWydarzenia:", ostatniIndeksWydarzenia); // Debugowanie
          Mioty.value.push(miot);
          console.log("NajwiekszaLiczbaKrycMiotu:", najwiekszaLiczbaKrycMiotu); // Debugowanie
        }

        for (let newHeader = 0; newHeader < najwiekszaLiczbaKrycMiotu; newHeader++) 
        {
          headers.value[1].children.splice(-3, 0, { title: `Krycia nr ${newHeader + 1}`, value: `datyKrycia[${newHeader}]` });
        }

        if (ostatniIndeksWydarzenia < selected.wydarzeniaLochyId.length) 
        {
          Mioty.value.push({id:Mioty.value.length+1});
          Mioty.value[Mioty.value.length-1].datyKrycia = [];
          if(selected.wydarzeniaLochyId.length - ostatniIndeksWydarzenia > 0)
          {
            console.log("Dodawanie dat krycia dla ostatniego miotu"); // Debugowanie
            for (let newHeader = najwiekszaLiczbaKrycMiotu; newHeader < selected.wydarzeniaLochyId.length - ostatniIndeksWydarzenia; newHeader++) 
            {
              headers.value[1].children.splice(-3, 0, { title: `Krycia nr ${newHeader + 1}`, value: `datyKrycia[${newHeader}]` });
            }
          }
          for (let indeksWydarzenia = ostatniIndeksWydarzenia; indeksWydarzenia < selected.wydarzeniaLochyId.length; indeksWydarzenia++) 
          {
            const wydarzenieId = selected.wydarzeniaLochyId[indeksWydarzenia];
            const wydarzenieResponse = await apiClient.get(`/Wydarzenie/${wydarzenieId}`);
            const wydarzenie = wydarzenieResponse.data;
            
            Mioty.value[Mioty.value.length-1].datyKrycia.push(wydarzenie.dataWydarzenia);

          }
        }

        console.log("Headers:", headers.value[1].children); // Debugowanie
        console.log("BaseHeaders:", baseHeaders); // Debugowanie
        console.log("Mioty:", Mioty.value); // Debugowanie
      }
      else {
        console.log(`Brak Miotów dla lochy ${newValue}`);
      }
    } catch (e) {
      console.error("Błąd podczas pobierania danych wybranej lochy:", e);
      error.value = e;
    }
  }
});

const getData = async () => {
  try {
    const response = await apiClient.get("/Locha");
    console.log("Dane lochy:", response.data); // Debugowanie
    Lochy.value = Array.isArray(response.data) ? response.data : [];
    numeryLoch.value = Lochy.value.filter(locha => locha.status == "Karmiaca" || locha.status == "Wolna" || locha.status == "Pokryta").map(locha => locha.numerLochy).sort((a, b) => a - b); // Dopasuj klucz do struktury danych
    console.log("Numery loch:", numeryLoch.value);
  } catch (e) {
    console.error("Błąd podczas pobierania danych:", e);
    error.value = e;
  }
};

const gridButtonClick = () => {
  console.log("Grid button clicked");
  showGrid.value = !showGrid.value;
};

const updateSelectedLocha = (number) => {
  selectedLocha.value = number;
  showGrid.value = !showGrid.value;
};

const addItem = () => {
  addMiotDialog.value = true;
};

const editItem = (item) => {
  console.log("Edytuj Miot o id:"); //Debugowanie
};

const deleteItem = (item) => {
  console.log("Usuń Miot o id:"); //Debugowanie
  apiClient.delete("/Miot/" + item.id);
  Mioty.value = Mioty.value.filter((Miot) => Miot.id !== Miot.id);
};

const handleSaveMiot = (nowyMiot) => {
  const teraz = new Date();
  const padZero = (num) => String(num).padStart(2, '0');
  const sformatowanaData = 
    `${teraz.getFullYear()}-${padZero(teraz.getMonth() + 1)}-${padZero(teraz.getDate())} ` +
    `${padZero(teraz.getHours())}:${padZero(teraz.getMinutes())}:${padZero(teraz.getSeconds())}`;
  Mioty.value[Mioty.value.length-1].dataCzasUtworzenia = sformatowanaData;
  Mioty.value[Mioty.value.length-1].dataCzasModyfikacji = sformatowanaData;
  Mioty.value[Mioty.value.length-1].urodzoneZywe = nowyMiot.urodzoneZywe;
  Mioty.value[Mioty.value.length-1].urodzoneMartwe = nowyMiot.urodzoneMartwe;
  Mioty.value[Mioty.value.length-1].przygniecone = nowyMiot.przygniecone;
  Mioty.value[Mioty.value.length-1].odsadzone = nowyMiot.odsadzone;
};

</script>
  
  <style lang="scss">
  
  .autocompleteNrLochy {
    min-width: 130px;
    position: absolute;
    transform: translate(160px, 10px);
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
    border-width: 2px;
  }
  td {
    border-style: solid;
    border-color: #1a1a1a;
    border-width: 2px;
  }
.gridButton {
    position: absolute;
    transform: translate(300px, 0px);
  }
  </style>
  