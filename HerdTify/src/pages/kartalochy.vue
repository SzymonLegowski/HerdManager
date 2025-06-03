<template>
  
  <LochyGrid v-if="showGrid" :items="numeryLoch" @update:selectedLocha="updateSelectedLocha" @quickAdd:newLochaId="quickAddLocha" style="z-index: 1;"/>
  <AddMiot :addMiotDialog="addMiotDialog" :idLochy="idLochy" :krycieId="ostatnieKrycieId" @add:addMiotDialog="addMiotDialog = $event" @save-miot="handleSaveMiot"/>
  <EditMiot :editMiotDialog="editMiotDialog" :miot="selectedMiot" @update:editMiotDialog="editMiotDialog = $event" @save-miot="handleUpdateMiot"/>
  <v-alert
  type="error"
  variant="tonal"
  v-model="alert"
  close-label="Close Alert"
  closable>Nie znaleziono wolnego krycia aby powiązać je z miotem</v-alert>
 
  
  <v-navigation-drawer :width="200">
    <v-list-item title="Menedżer stada"></v-list-item>
    <v-divider></v-divider>
      
      <v-list-item :to="{ path: '/kartalochy' }" link title="Karta lochy"></v-list-item>
      <v-list-item :to="{ path: '/wydarzenia' }" link title="Wydarzenia"></v-list-item>
      <v-list-item :to="{ path: '/stado' }" link title="Stado"></v-list-item>
      <!-- <v-list-item :to="{ path: '/import' }" link title="Importuj dane"></v-list-item> -->
    
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
        style="min-width: 0; width: 150px; background-color: blue; margin-right: 40px; "
        size="small"
        @click="exportToPdf()"
      >Eksportuj do pdf</v-btn>
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

    <template
      v-for="(header, index) in headers[1].children.filter(h => h.value.startsWith('datyKrycia'))"
      :key="header.value"
       #[`item.${header.value}`]="{ item }">
      <div v-if="item.datyKrycia?.[index]">
        <div>{{ item.datyKrycia[index].data }}</div>
        <div style="color: gray;">{{ item.datyKrycia[index].uwagi }}</div>
      </div>
    </template>

    <template v-slot:item.actions="{ item }">
      <v-btn
        v-if="item.dataPrzewidywanegoProszenia"
        class="me-2"
        style="min-width: 0; width: 10px; background-color: #d67804;"
        size="small"
        @click="editItem(item)">
      <v-icon>mdi-pencil</v-icon>
      </v-btn>
      <v-btn
        v-if="item.dataPrzewidywanegoProszenia"
        style="min-width: 0; width: 10px; background-color: red;"
        size="small"
        @click="deleteItem(item)">
        <v-icon>mdi-delete</v-icon>
      </v-btn>
      <v-btn
        v-if="!item.dataPrzewidywanegoProszenia"
        style="min-width: 0; width: 10px; background-color: green;"
        size="small"
        @click="addItem()">
        <v-icon>mdi-plus</v-icon>
      </v-btn>
    </template>
    <template v-slot:no-data></template>
    
  </v-data-table>
  
  </template>
  
  <script setup>
  import { ref, onMounted, watch } from "vue";
  import { useMiotyStore } from "@/stores/miotyStore";
  import apiClient from "@/plugins/axios";
  import LochyGrid from "@/components/LochyGrid.vue";
  import AddMiot from "@/components/AddMiot.vue";
  import EditMiot from "@/components/EditMiot.vue";

  let pobraneLochy = ref([]);
  let Lochy = ref([]);
  let numeryLoch = ref([]);
  let Mioty = ref([]);
  let error = ref(null);
  let selectedLocha = ref(null);
  let selectedMiot = ref(null);
  let showGrid = ref(false);
  let addMiotDialog = ref(false);
  let editMiotDialog = ref(false);
  let ostatnieKrycieId = ref(null);
  let idLochy = ref(null);
  let miotyStore = useMiotyStore();
  let alert = ref(null);

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
    {
      title: "Ocena prosiąt urodzonych (1-5)", value: "ocena",
    },
    {title: "Działania", key: "actions", sortable: false, align: "center"}
  ];
  
  const headers = ref(JSON.parse(JSON.stringify(baseHeaders)));

  const getData = async () => {
  try {
    const response = await apiClient.get("/Locha");
    pobraneLochy.value = Array.isArray(response.data) ? response.data : [];
    Lochy.value = pobraneLochy.value
      .filter(locha => 
        locha.status == "Karmiaca" || 
        locha.status == "Luzna" || 
        locha.status == "Pokryta" ||
        locha.status == "Prosna"
      );
      numeryLoch.value = Lochy.value
      .map(locha => ({
        idLochy: locha.id,
        numerLochy: locha.numerLochy,
        statusLochy: locha.status
      }))
      .sort((a, b) => a.numerLochy - b.numerLochy); // Dopasuj klucz do struktury danych
  } catch (e) {
    console.error("Błąd podczas pobierania danych:", e);
    error.value = e;
  }
};

  onMounted(async () => {
    getData();
});

const loadMioty = async (newValue) => {
  let ostatniIndeksWydarzenia = 0;
  let najwiekszaLiczbaKrycMiotu = 0;
  let selected;
  ostatnieKrycieId.value = null;
  Mioty.value = [];
  headers.value = JSON.parse(JSON.stringify(baseHeaders));
  if (newValue !== null) 
  {
    try 
    {
      selected = Lochy.value.find(locha => locha.numerLochy === newValue);
      idLochy.value = selected.id;
      let response = await apiClient.get("locha/" + idLochy.value);
      selected = response.data;
        for (let indeksMiotu = 0; indeksMiotu < selected.miotyId.length; indeksMiotu++) 
        {
          let miotId = selected.miotyId[indeksMiotu];
          let miotResponse = await apiClient.get(`/Miot/${miotId}`);
          let miot = miotResponse.data;
          miot.datyKrycia = [];
          miot.nr = indeksMiotu + 1;

          for (let indeksWydarzenia = ostatniIndeksWydarzenia; indeksWydarzenia < selected.wydarzeniaLochyId.length; indeksWydarzenia++) 
          {
            let wydarzenieId = selected.wydarzeniaLochyId[indeksWydarzenia];
            let wydarzenieResponse = await apiClient.get(`/Wydarzenie/${wydarzenieId}`);
            let wydarzenie = wydarzenieResponse.data;

            if (new Date(wydarzenie.dataWydarzenia).getTime() < new Date(miot.dataPrzewidywanegoProszenia).getTime()) 
            {
              // miot.datyKrycia.push(`${wydarzenie.dataWydarzenia}`);
              console.log("headers", headers.value[1].children); //Debugowanie
              miot.datyKrycia.push({data: wydarzenie.dataWydarzenia, uwagi: wydarzenie.uwagi});
              console.log("miot", miot); //Debugowanie
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
        for (let newHeader = 0; newHeader < najwiekszaLiczbaKrycMiotu; newHeader++) 
        {
          headers.value[1].children.splice(-3, 0, { title: `Krycia nr ${newHeader + 1}`, value: `datyKrycia[${newHeader}]` });
        }

        if (ostatniIndeksWydarzenia < selected.wydarzeniaLochyId.length) 
        {
          ostatnieKrycieId.value = selected.wydarzeniaLochyId[selected.wydarzeniaLochyId.length - 1];
          Mioty.value.push({nr:Mioty.value.length+1});
          Mioty.value[Mioty.value.length-1].datyKrycia = [];
          if(selected.wydarzeniaLochyId.length - ostatniIndeksWydarzenia > 0)
          {
            for (let newHeader = najwiekszaLiczbaKrycMiotu; newHeader < selected.wydarzeniaLochyId.length - ostatniIndeksWydarzenia; newHeader++) 
            {
              headers.value[1].children.splice(-3, 0, { title: `Krycia nr ${newHeader + 1}`, value: `datyKrycia[${newHeader}]` });
            }
          }
          for (let indeksWydarzenia = ostatniIndeksWydarzenia; indeksWydarzenia < selected.wydarzeniaLochyId.length; indeksWydarzenia++) 
          {
            let wydarzenieId = selected.wydarzeniaLochyId[indeksWydarzenia];
            let wydarzenieResponse = await apiClient.get(`/Wydarzenie/${wydarzenieId}`);
            let wydarzenie = wydarzenieResponse.data;
            Mioty.value[Mioty.value.length-1].datyKrycia.push({data: wydarzenie.dataWydarzenia, uwagi: wydarzenie.uwagi});

          }
        }
      }catch (e) {
        console.error("Błąd podczas pobierania danych wybranej lochy:", e);
        error.value = e;
      }
    }
  };

watch(selectedLocha, async (newValue, oldValue) => {
  loadMioty(newValue);
});

const gridButtonClick = () => {
  showGrid.value = !showGrid.value;
};

const updateSelectedLocha = (number) => {
  selectedLocha.value = number;
  showGrid.value = !showGrid.value;
};

const quickAddLocha = async (newLochaId) => {
  let newLocha = await apiClient.get('/Locha/' + newLochaId);
  Lochy.value.push(newLocha.data);
  numeryLoch.value.push({idLochy: newLocha.data.id,
                         numerLochy: newLocha.data.numerLochy,
                         statusLochy: newLocha.data.status});
};

const addItem = () => {
  if(ostatnieKrycieId.value != null){
    addMiotDialog.value = true;
  }
  else{
    alert.value = true;
  }
};

const editItem = (item) => {
  editMiotDialog.value = true;
  selectedMiot.value = item;
};

const deleteItem = async (item) => {
  await apiClient.delete("/Miot/" + item.id);
  await getData();
  loadMioty(selectedLocha.value); 
};

const exportToPdf = () => {
  miotyStore.setMiotyStore(Mioty, selectedLocha);
  window.open('/exportpdf', '_blank')
};

const handleSaveMiot = async (newMiotId) => {
  // loadMioty(selectedLocha.value);
  let miot = await apiClient.get("/Miot/" + newMiotId);
  console.log("Mioty1", Mioty.value[1]);
  console.log("nowy miot", miot.data);
  console.log("Mioty", Mioty.value[2]);
  Object.assign(Mioty.value[Mioty.value.length-1], miot.data)
  console.log("Mioty2", Mioty.value[2]);
  numeryLoch.value.forEach(obj => {
      if( obj.numerLochy === selectedLocha.value){
        obj.statusLochy = "Prosna";
    }});
};

const handleUpdateMiot = async (editedMiot) => {
  let editedMiotIndex = Mioty.value.findIndex(Miot => Miot.id === editedMiot.id)
  Mioty.value[editedMiotIndex] = editedMiot;
  const indexLochy = numeryLoch.value.findIndex(
    obj => obj.numerLochy === selectedLocha.value
  );
  if (editedMiot.dataProszenia !== null && editedMiot.dataOdsadzenia === null) {
    numeryLoch.value[indexLochy].statusLochy = "Karmiaca";
  } else if (editedMiot.dataOdsadzenia !== null) {
    numeryLoch.value[indexLochy].statusLochy = "Luzna";
  } 
   
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
  