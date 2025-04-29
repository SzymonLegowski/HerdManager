<template>
  
  <LochyGrid v-if="showGrid" :items="numeryLoch" @update:selectedLocha="updateSelectedLocha" @quickAdd:newLochaId="quickAddLocha"/>
  <AddMiot :addMiotDialog="addMiotDialog" :idLochy="idLochy" :krycieId="ostatnieKrycieId" @update:addMiotDialog="addMiotDialog = $event"/>
  <EditMiot :editMiotDialog="editMiotDialog" :miot="selectedMiot" @update:editMiotDialog="editMiotDialog = $event" />
  

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
        :items="numeryLoch.map(locha => locha.numerLochy)"
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
        style="min-width: 0; width: 100px; background-color: blue; margin-right: 40px; "
        size="small"
        @click="exportToPdf()"
      >Eksportuj</v-btn>

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
  import { useMiotyStore } from "@/stores/miotyStore";
  import apiClient from "@/plugins/axios";
  import LochyGrid from "@/components/LochyGrid.vue";
  import AddMiot from "@/components/AddMiot.vue";
  import EditMiot from "@/components/EditMiot.vue";


  const pobraneLochy = ref([]);
  const Lochy = ref([]);
  const numeryLoch = ref([]);
  const Mioty = ref([]);
  const error = ref(null);
  const selectedLocha = ref(null);
  const selectedMiot = ref(null);
  const showGrid = ref(false);
  const addMiotDialog = ref(false);
  const editMiotDialog = ref(false);
  const ostatnieKrycieId = ref(null);
  const idLochy = ref(null);
  const miotyStore = useMiotyStore();

  const baseHeaders = [
    {
      title: "Miot",
      value: "nr",
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

  const getData = async () => {
  try {
    const response = await apiClient.get("/Locha");
    console.log("Dane lochy:", response.data); // Debugowanie
    pobraneLochy.value = Array.isArray(response.data) ? response.data : [];
    Lochy.value = pobraneLochy.value
      .filter(locha => 
        locha.status == "Karmiaca" || 
        locha.status == "Wolna" || 
        locha.status == "Pokryta"
      );
      numeryLoch.value = Lochy.value
      .map(locha => ({
        idLochy: locha.id,
        numerLochy: locha.numerLochy,
        statusLochy: locha.status
      }))
      .sort((a, b) => a.numerLochy - b.numerLochy); // Dopasuj klucz do struktury danych
    console.log("Dane lochy2 ", Lochy.value); //Debugowanie
    console.log("Numery loch:", numeryLoch.value); // Debugowanie
  } catch (e) {
    console.error("Błąd podczas pobierania danych:", e);
    error.value = e;
  }
};
  onMounted(async () => {
    getData();
});

watch(selectedLocha, async (newValue, oldValue) => {
  let ostatniIndeksWydarzenia = 0;
  let najwiekszaLiczbaKrycMiotu = 0;
  Mioty.value = [];
  headers.value = JSON.parse(JSON.stringify(baseHeaders));
  if (newValue !== null) 
  {
    try 
    {
      const selected = Lochy.value.find(locha => locha.numerLochy === newValue);
      idLochy.value = selected.id;
        for (let indeksMiotu = 0; indeksMiotu < selected.miotyId.length; indeksMiotu++) 
        { 
          const miotId = selected.miotyId[indeksMiotu];
          const miotResponse = await apiClient.get(`/Miot/${miotId}`);
          const miot = miotResponse.data;
          miot.datyKrycia = [];
          miot.nr = indeksMiotu + 1;

          for (let indeksWydarzenia = ostatniIndeksWydarzenia; indeksWydarzenia < selected.wydarzeniaLochyId.length; indeksWydarzenia++) 
          {
            const wydarzenieId = selected.wydarzeniaLochyId[indeksWydarzenia];
            const wydarzenieResponse = await apiClient.get(`/Wydarzenie/${wydarzenieId}`);
            const wydarzenie = wydarzenieResponse.data;

            if (new Date(wydarzenie.dataWydarzenia).getTime() < new Date(miot.dataPrzewidywanegoProszenia).getTime()) 
            {
              miot.datyKrycia.push(wydarzenie.dataWydarzenia);
              ostatniIndeksWydarzenia++;
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
          Mioty.value.push(miot);
        }
        ostatnieKrycieId.value = selected.wydarzeniaLochyId[selected.wydarzeniaLochyId.length - 1];
        for (let newHeader = 0; newHeader < najwiekszaLiczbaKrycMiotu; newHeader++) 
        {
          headers.value[1].children.splice(-3, 0, { title: `Krycia nr ${newHeader + 1}`, value: `datyKrycia[${newHeader}]` });
        }

        if (ostatniIndeksWydarzenia < selected.wydarzeniaLochyId.length) 
        {
          Mioty.value.push({nr:Mioty.value.length+1});
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
    } catch (e) {
      console.error("Błąd podczas pobierania danych wybranej lochy:", e);
      error.value = e;
    }
  }
});



const gridButtonClick = () => {
  console.log("Grid button clicked");
  showGrid.value = !showGrid.value;
};

const updateSelectedLocha = (number) => {
  selectedLocha.value = number;
  showGrid.value = !showGrid.value;
};

const quickAddLocha = async (newLochaId) => {
  console.log("quickAddNewLocha", newLochaId)
  let newLocha = await apiClient.get('/Locha/' + newLochaId);
  Lochy.value.push(newLocha.data);
  numeryLoch.value.push({idLochy: newLocha.data.id,
                         numerLochy: newLocha.data.numerLochy,
                         statusLochy: newLocha.data.status});
  console.log(Lochy.value,"QuickAdd1");
  console.log(numeryLoch.value, "QuickAdd2");
};

const addItem = () => {
  addMiotDialog.value = true;
};

const editItem = (item) => {
  editMiotDialog.value = true;
  selectedMiot.value = item;
};

const deleteItem = (item) => {
  apiClient.delete("/Miot/" + item.id);
  Mioty.value.splice(Mioty.value.indexOf(item), 1);
};

const exportToPdf = () => {
  miotyStore.setMiotyStore(Mioty, selectedLocha);
  window.open('/exportpdf', '_blank')
  console.log("Mioty:", miotyStore.mioty); //Debugowanie
  console.log("NrLochyMiotStore", miotyStore.nrLochy);
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
  