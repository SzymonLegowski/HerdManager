<template>
      <v-dialog
        :model-value="addWydarzenieDialog"
        @update:model-value="addWydarzenieDialog"
        max-width="500"
        persistent>
        <LochyGrid 
          v-if="showLochyGrid" 
          :numeryLoch="numeryLoch"
          :lochy="lochy" 
          @update:selectedLocha="updateSelectedLochy" 
          class="lochyGridDialog"/>
        <v-card
          prepend-icon="mdi-plus"
          title="Nowe Wydarzenie"
          :class="{ 'shift-left': showLochyGrid }">
          <v-card-text>
            <v-alert
              v-if="alert"
              type="error"
              variant="tonal"
              style="margin-bottom: 10px;">{{ message }}</v-alert>
            <v-alert
              v-if="success"
              type="success"
              variant="tonal"
              text="Dodano pomyślnie!"
              style="margin-bottom: 10px;"/>
            <v-autocomplete
              :items="['Krycie', 'Odsadzanie']"
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
            <div class="numeryLoch-container"><div class="numeryLoch-container-label">Wybrane lochy</div> {{ noweWydarzenie.lochyId.join(", ") }} </div>
            <v-row style="margin-left: 0px; justify-content: space-between;">
              <v-btn variant="tonal" color="secondary" text="Dodaj lochy" @click="selectLochy"/>
              <v-btn variant="tonal" class="wybraneLochyEmpty-btn" @click="wybraneLochyEmpty">Wyczyść</v-btn>
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

let message = ref(null);
let alert = ref(false);
let success = ref(false);
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
  dataWydarzenia: "Krycie",
  dataWykonania: "",
  dataCzasUtworzenia: "",
  dataCzasModyfikacji: "",
  numeryLoch: [],
  miotyId: []
});

const emit = defineEmits(['update:addWydarzenieDialog', 'save-wydarzenie']);

const closeDialog = () => {
  alert.value = false;
  success.value = false;
  emit('update:addWydarzenieDialog', false);
};
          
const saveDialog = async () => {
  if(noweWydarzenie.value.typWydarzenia === "Odsadzanie")
  {
    noweWydarzenie.value.dataWykonania = noweWydarzenie.value.dataWydarzenia;
    noweWydarzenie.value.lochyId.forEach(lochaId => {
        const znaleziona = lochy.value.find(obj => obj.numerLochy === lochaId);
        console.log(znaleziona.miotyId.join(", "));
    });
    console.log(noweWydarzenie.value.lochyId);
  }
  else
  {
    if(noweWydarzenie.value.dataWykonania === ""){
        noweWydarzenie.value.dataWykonania = noweWydarzenie.value.dataWydarzenia;
      }
    for(let lochaNr = 0; lochaNr < noweWydarzenie.value.lochyId.length; lochaNr++)
    {
      const znaleziony = numeryLoch.value.find(obj => obj.numerLochy === noweWydarzenie.value.lochyId[lochaNr]);
      znaleziony.statusLochy = "Pokryta";
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
    await apiClient.post('/Wydarzenie', noweWydarzenie.value)
      .then((response) => {
        alert.value = false;
        success.value = true;
        mapNoweWydarzenieTemp();
        clearNoweWydarzenie();
        noweWydarzenieTemp.value.id = response.data;
        emit('save-wydarzenie', noweWydarzenieTemp.value);
      })
      .catch((e) => {
        success.value = false;
        console.error(e);
        if(noweWydarzenie.value.lochyId.length === 0) 
        message.value = "Wydarzenie musi zawierać przynajmniej 1 lochę"; 
        else if (noweWydarzenie.value.dataWydarzenia === "")
        message.value = "Nie podano daty wydarzenia";
        else if (e.response.data.e.errors[0].errorMessage !== null)
        message.value = e.response.data.e.errors[0].errorMessage;
        else
        message.value = "Błąd podczas dodawania wydarzenia";
        alert.value = true;
      });
  }
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
        idLochy: locha.id,
        numerLochy: locha.numerLochy,
        statusLochy: locha.status
      }));
    console.log("lochy ",lochy.value)
    console.log("numeryLoch ",numeryLoch.value)
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
  top: 0%;
  left: 70%;
}
.numeryLoch-container{
  padding: 11px 13px;
  margin-bottom:30px;
  border: 1.5px;
  border-radius: 4px;
  height: 50px;
  border-style: solid;
  border-color: #777777;
}
.numeryLoch-container-label{
  position: absolute;
  transform: translate(0, -21px);
  background-color: #222222;
  color:#bbbbbb;
  font-size:12px;
  padding: 0 5px;
}
.wybraneLochyEmpty-btn{
  background-color: rgb(255, 0, 0);
  margin-right:12px ;
}
</style>