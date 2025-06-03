<template>
      <v-dialog
        :model-value="addWydarzenieDialog"
        @update:model-value="addWydarzenieDialog"
        max-width="500"
        persistent>
        <LochyGrid v-if="showLochyGrid" :items="numeryLoch" @update:selectedLocha="updateSelectedLochy" class="lochyGridDialog"/>
        <v-card
          prepend-icon="mdi-plus"
          title="Nowe Wydarzenie"
          :class="{ 'shift-left': showLochyGrid }">
          <v-card-text>
            <v-autocomplete
              :items="['Krycie', 'Szczepienie']"
              label="Typ"
              auto-select-first
              v-model:="noweWydarzenie.typWydarzenia"
              variant="outlined"
              :rules="[required]"/>
            <v-text-field
              hint="rrrr-mm-dd"
              label="Data Wydarzenia"
              v-model="noweWydarzenie.dataWydarzenia"
              :rules="[required]"
              variant="outlined"
              style="margin-bottom:5px; margin-top: 5px;"/>
            <v-text-field
              hint="np. Maximus"
              label="Rasa kn."
              v-model="noweWydarzenie.rasa"
              variant="outlined"/>
            <v-textarea label="Uwagi" v-model="noweWydarzenie.uwagi" style=" margin-top: 5px; margin-bottom: 10px;" variant="outlined"/>
            <v-row style="margin-left: 0px;">
              <v-btn variant="tonal" color="secondary" text="Dodaj lochy" @click="selectLochy"/>
              <v-btn variant="tonal" color="secondary" @click="wybraneLochyEmpty" style="margin-left: 10px;">{{ noweWydarzenie.lochyId }}</v-btn>
            </v-row>
          </v-card-text>
          <v-divider/>
          <v-card-actions>
            <v-spacer/>
            <v-btn
              text="Zamknij"
              variant="plain"
              @click="closeDialog()"/>
            <v-btn
              color="primary"
              text="Zapisz"
              variant="tonal"
              @click="saveDialog()"/>
          </v-card-actions>
        </v-card>
      </v-dialog>
  </template>

<script setup>
import { onMounted, ref} from 'vue';
import apiClient from "@/plugins/axios";
import LochyGrid from './LochyGrid.vue';

const props = defineProps({
  addWydarzenieDialog: {
    type: Boolean,
    required: true
  }
});

const numeryLoch = ref([]);
const lochyWolne = ref([]);
const lochyPokryte = ref([]);
const lochyProsne = ref([]);
const lochyKarmiace = ref([]);
const lochy = ref([]);
const showLochyGrid = ref(false); 
let noweWydarzenie = ref({
  typWydarzenia: "Krycie",
  uwagi: "",
  dataWydarzenia: "",
  dataWykonania: "",
  lochyId: [],
  miotyId: []
});
let noweWydarzenieTemp = ref({
  id: "",
  typWydarzenia: "",
  uwagi: "",
  dataWydarzenia: "",
  dataWykonania: "",
  dataCzasUtworzenia: "",
  dataCzasModyfikacji: "",
  numeryLoch: [],
  miotyId: []
});

const emit = defineEmits(['update:addWydarzenieDialog', 'save-wydarzenie']);

const closeDialog = () => {
  emit('update:addWydarzenieDialog', false);
};
          
const saveDialog = () => {
  if(noweWydarzenie.value.dataWykonania === ""){
    noweWydarzenie.value.dataWykonania = noweWydarzenie.value.dataWydarzenia;
  }
  for(let lochaNr = 0; lochaNr < noweWydarzenie.value.lochyId.length; lochaNr++)
  {
    for(let locha = 0; locha < lochy.value.length; locha++)
    {
      if(lochy.value[locha].numerLochy === noweWydarzenie.value.lochyId[lochaNr])
      {
        noweWydarzenieTemp.value.numeryLoch.push(lochy.value[locha].numerLochy);
        noweWydarzenie.value.lochyId[lochaNr] = lochy.value[locha].id;
      }
    }
  }
  console.log("noweWydarzenie", noweWydarzenie); //Debugowanie
  apiClient.post('/Wydarzenie', noweWydarzenie.value)
    .then(() => {
      closeDialog();
      mapNoweWydarzenieTemp();
      clearNoweWydarzenie();
    })
    .catch((e) => {
      console.error(e);
    });
  emit('save-wydarzenie', noweWydarzenieTemp.value);
};

const selectLochy = () => {
  showLochyGrid.value = !showLochyGrid.value;
};

const selectMioty = () => {
  showMiotyGrid.value = !showMiotyGrid.value;
};

const wybraneLochyEmpty = () => {
  noweWydarzenie.value.lochyId = [];
};

const wybraneMiotyEmpty = () => {
  noweWydarzenie.value.miotyId = [];
};

onMounted(async () => {
  try {
    const responseA = await apiClient.get(`/Locha/status/0`);
    const responseB = await apiClient.get(`/Locha/status/1`);
    const responseC = await apiClient.get(`/Locha/status/2`);
    const responseD = await apiClient.get(`/Locha/status/3`);
    lochyWolne.value = responseA.data;
    lochyPokryte.value = responseB.data;
    lochyProsne.value = responseC.data;
    lochyKarmiace.value = responseD.data;
    lochy.value = lochyWolne.value.concat(lochyPokryte.value, lochyKarmiace.value, lochyProsne.value);
    numeryLoch.value = lochy.value.map(locha => ({
        numerLochy: locha.numerLochy,
        statusLochy: locha.status
      }));
    console.log("numeryLoch", numeryLoch); //Debugowanie
  } catch (e) {
    console.error(e);
  }
});

const updateSelectedLochy = (number) => {
  if(!noweWydarzenie.value.lochyId.includes(number)){
    noweWydarzenie.value.lochyId.push(number);
  } else
  {
    let index = noweWydarzenie.value.lochyId.indexOf(number);
    noweWydarzenie.value.lochyId.splice(index, 1);
  }
  console.log("wybrane lochy:", noweWydarzenie); //Debugowanie  
};

const clearNoweWydarzenie = () => {
  noweWydarzenie.value.typWydarzenia = "";
  noweWydarzenie.value.uwagi = "";
  noweWydarzenie.value.rasa = "";
  noweWydarzenie.value.dataWydarzenia = "";
  noweWydarzenie.value.dataWykonania = "";
  noweWydarzenie.value.lochyId = [];
  noweWydarzenie.value.miotyId = [];
};

const mapNoweWydarzenieTemp = () => {
  noweWydarzenieTemp.value.typWydarzenia = noweWydarzenie.value.typWydarzenia;
  noweWydarzenieTemp.value.uwagi = noweWydarzenie.value.uwagi;
  noweWydarzenieTemp.value.rasa = noweWydarzenie.value.rasa;
  noweWydarzenieTemp.value.dataWydarzenia = noweWydarzenie.value.dataWydarzenia;
  noweWydarzenieTemp.value.dataWykonania = noweWydarzenie.value.dataWykonania;
};

const required = (v) => { return !!v || 'Pole jest wymagane' };

</script>

<style scoped>
.addButton {
  margin-bottom: 30px;
  
}
.shift-left {
  transform: translateX(-120px);
  transition: all 0.4s ease-in-out;
}

.lochyGridDialog {
  position: absolute;
  top: -5%;
  left: 70%;
}
h5 {
  cursor: pointer; /* Wskazuje, że element jest klikalny */
}
</style>