<template>
      <v-dialog
        :model-value="addWydarzenieDialog"
        @update:model-value="addWydarzenieDialog"
        max-width="600"
        persistent
      >
        <LochyGrid v-if="showLochyGrid" :items="numeryLoch" @update:selectedLocha="updateSelectedLochy" class="lochyGridDialog"/>
        <v-card
          prepend-icon="mdi-plus"
          title="Nowe Wydarzenie"
          :class="{ 'shift-left': showLochyGrid }"
        >
          <v-card-text>
            <v-row dense>
              <v-autocomplete
                  :items="['Krycie', 'Szczepienie']"
                  label="Typ"
                  auto-select-first
                  v-model:="noweWydarzenie.typWydarzenia"
                  style = "margin-right: 10px;"
                ></v-autocomplete>
              <v-text-field
                  hint="rrrr-mm-dd"
                  label="Data Wydarzenia*"
                  v-model="noweWydarzenie.dataWydarzenia"
                  required
                  style = "margin-left: 10px;"
                ></v-text-field>
              <v-textarea label="Uwagi" v-model="noweWydarzenie.uwagi" style="width: 100%;"></v-textarea>
            
              <v-btn variant="outlined" text="Wybierz lochy" @click="selectLochy"></v-btn>
              <v-col>
                <h5 @click="wybraneLochyEmpty" style="margin-top: 1%; margin-left: 10px;">Wybrane lochy:{{ noweWydarzenie.lochyId }}</h5>
              </v-col>
            </v-row>
            
            <small class="text-caption text-medium-emphasis">*wymagane</small>
            <v-spacer></v-spacer>
            <small class="text-caption text-medium-emphasis">wydarzenie musi zawierać chociaż 1 lochę</small>
          </v-card-text>
  
          <v-divider></v-divider>
  
          <v-card-actions>
            <v-spacer></v-spacer>
  
            <v-btn
              text="Zamknij"
              variant="plain"
              @click="closeDialog()"
            ></v-btn>
  
            <v-btn
              color="primary"
              text="Zapisz"
              variant="tonal"
              @click="saveDialog()"
            ></v-btn>
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
const lochyKarmiace = ref([]);
const lochy = ref([]);
const showLochyGrid = ref(false); 
let noweWydarzenie = ref({
  typWydarzenia: "",
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
    lochyWolne.value = responseA.data
    lochyPokryte.value = responseB.data
    lochyKarmiace.value = responseC.data
    lochy.value = lochyWolne.value.concat(lochyPokryte.value, lochyKarmiace.value, lochyWolne.value);
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
  noweWydarzenie.value.dataWydarzenia = "";
  noweWydarzenie.value.dataWykonania = "";
  noweWydarzenie.value.lochyId = [];
  noweWydarzenie.value.miotyId = [];
};

const mapNoweWydarzenieTemp = () => {
  noweWydarzenieTemp.value.typWydarzenia = noweWydarzenie.value.typWydarzenia;
  noweWydarzenieTemp.value.uwagi = noweWydarzenie.value.uwagi;
  noweWydarzenieTemp.value.dataWydarzenia = noweWydarzenie.value.dataWydarzenia;
  noweWydarzenieTemp.value.dataWykonania = noweWydarzenie.value.dataWykonania;
};

</script>

<style scoped>
.addButton {
  margin-bottom: 30px;
  
}
.shift-left {
  transform: translateX(-220px);
  transition: transform 0.3s ease-in-out;
}

.lochyGridDialog {
  position: absolute;
  top: 10%;
  left: 70%;
}
h5 {
  cursor: pointer; /* Wskazuje, że element jest klikalny */
}
</style>