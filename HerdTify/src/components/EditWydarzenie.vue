<template>
    <v-dialog
      :model-value="editWydarzenieDialog"
      @update:model-value="editWydarzenieDialog"
      max-width="500"
      persistent>
      <LochyGrid v-if="showLochyGrid" :items="numeryLoch" @update:selectedLocha="updateSelectedLochy" class="lochyGridDialog"/>
      <v-card
        prepend-icon="mdi-pencil"
        title="Edytuj wydarzenie"
        :class="{ 'shift-left': showLochyGrid }">
        <v-card-text>
              <v-alert
                v-if="errorMessage"
                type="error"
                variant="tonal"
                v-model="errorAlert"
                style="margin-bottom: 10px;">{{ errorMessage }}</v-alert>
              <v-alert
                v-if="success"
                type="success"
                variant="tonal"
                text="Dodano pomyślnie!"
                style="margin-bottom: 10px;"/>
            <v-autocomplete
              :items="['Krycie', 'Szczepienie']"
              label="Typ"
              auto-select-first
              v-model:="editedWydarzenie.typWydarzenia" 
              variant="outlined"
              :rules="[required]"/>
           <v-text-field
              hint="rrrr-mm-dd"
              label="Data Wydarzenia"
              v-model="editedWydarzenie.dataWydarzenia"
              :rules="[required]"
              variant="outlined"
              style="margin-bottom:5px; margin-top: 5px;"/>
            <v-text-field
              hint="np. Maximus"
              label="Rasa"
              v-model="editedWydarzenie.rasa"
              variant="outlined"/>
            <v-textarea label="Uwagi" v-model="editedWydarzenie.uwagi" variant="outlined" style="width: 100%; margin-top: 10px"></v-textarea>
            <v-row style="margin-left: 0px;">
              <v-btn variant="tonal" color="secondary" text="Dodaj lochy" @click="selectLochy"/>
              <v-btn variant="tonal" color="secondary" @click="wybraneLochyEmpty" style="margin-left: 10px;">{{ editedWydarzenie.numeryLoch }}</v-btn>
            </v-row>
            <div style="margin:20px"></div>
            {{ editedWydarzenie.lochyId }}
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
editWydarzenieDialog: {
    type: Boolean,
    required: true
},
wydarzenie: {
    type: Object,
    required: false
}
});

const numeryLoch = ref([]);
const lochyWolne = ref([]);
const lochyPokryte = ref([]);
const lochyProsne = ref([]);
const lochyKarmiace = ref([]);
const lochy = ref([]);
const showLochyGrid = ref(false);
let success = ref(""); 
let errorMessage = ref("");
let editedWydarzenie = ref(null);
let editedWydarzenieSent = ref({
    id: "",
    typWydarzenia: "",
    uwagi: "",
    rasa: "",
    dataWydarzenia: "",
    lochyId: [],
    miotyId: []
});

const emit = defineEmits(['update:editWydarzenieDialog', 'update-wydarzenie']);

const closeDialog = () => {
emit('update:editWydarzenieDialog', false);
success.value = "";
errorMessage.value = "";
};

const saveDialog = () => {
  for(let lochaNr = 0; lochaNr < editedWydarzenie.value.lochyId.length; lochaNr++)
    {
      for(let locha = 0; locha < lochy.value.length; locha++)
      {
        if(lochy.value[locha].numerLochy === editedWydarzenie.value.lochyId[lochaNr])
        {
          editedWydarzenie.value.numeryLoch.push(lochy.value[locha].numerLochy);
          editedWydarzenieSent.value.lochyId[lochaNr] = lochy.value[locha].id;
        }
      }
    }
  editedToSent();
  apiClient.put('/Wydarzenie/' + editedWydarzenieSent.value.id, editedWydarzenieSent.value)
  .then(response => {
    emit('update-wydarzenie', editedWydarzenie.value);
    success.value = response.data;
    errorMessage.value = "";
  })
  .catch((e) => {
      console.error("Błąd podczas pobierania danych:", e);
      success.value = "";
      errorMessage.value = "Wystąpił błąd podczas zapisywania wydarzenia";
  });
};

const selectLochy = () => {
showLochyGrid.value = !showLochyGrid.value;
};

const selectMioty = () => {
showMiotyGrid.value = !showMiotyGrid.value;
};

const wybraneLochyEmpty = () => {
editedWydarzenie.value.lochyId = [];
editedWydarzenie.value.numeryLoch = [];
};

const wybraneMiotyEmpty = () => {
editedWydarzenie.value.miotyId = [];
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
  let idLochy = lochy.value.find(locha => locha.numerLochy === number)?.id;
  if(!editedWydarzenie.value.numeryLoch.includes(number)){
    editedWydarzenie.value.numeryLoch.push(number);
    editedWydarzenie.value.lochyId.push(idLochy);
  } else
  {
    let nrIdx = editedWydarzenie.value.numeryLoch.indexOf(number);
    let idIdx = editedWydarzenie.value.lochyId.indexOf(idLochy);
    editedWydarzenie.value.numeryLoch.splice(nrIdx, 1);
    editedWydarzenie.value.lochyId.splice(idIdx, 1);
  }
  console.log("wybrane lochy:", editedWydarzenie); //Debugowanie  
};

watch(() => props.wydarzenie,(newWydarzenie) => {
editedWydarzenie.value = { ...newWydarzenie };
},
{ deep: true, immediate: true }
);

const editedToSent = () => {
    editedWydarzenieSent.value.id = editedWydarzenie.value.id;
    editedWydarzenieSent.value.typWydarzenia = editedWydarzenie.value.typWydarzenia;
    editedWydarzenieSent.value.uwagi = editedWydarzenie.value.uwagi;
    editedWydarzenieSent.value.rasa = editedWydarzenie.value.rasa;
    editedWydarzenieSent.value.dataWydarzenia = editedWydarzenie.value.dataWydarzenia;
    editedWydarzenieSent.value.lochyId = editedWydarzenie.value.lochyId;
    editedWydarzenieSent.value.miotyId = editedWydarzenie.value.miotyId;
};

const required = (v) => { return !!v || 'Pole jest wymagane' };

</script>

<style scoped>
.AddButton {
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