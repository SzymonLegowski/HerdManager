<template>
    <v-dialog
      :model-value="editWydarzenieDialog"
      @update:model-value="editWydarzenieDialog"
      max-width="600"
      persistent
    >
      <LochyGrid v-if="showLochyGrid" :items="numeryLoch" @update:selectedLocha="updateSelectedLochy" class="lochyGridDialog"/>
      <v-card
        prepend-icon="mdi-pencil"
        title="Edytuj wydarzenie"
        :class="{ 'shift-left': showLochyGrid }"
      >
        <v-card-text>
            <v-row dense>
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
                style="margin-bottom: 10px;">{{ success }}</v-alert>
            </v-row>
          <v-row dense>
            <v-autocomplete
                :items="['Krycie', 'Szczepienie']"
                label="Typ"
                auto-select-first
                v-model:="editedWydarzenie.typWydarzenia" 
                variant="outlined"
                :rules="[required]"
                style = "margin-right: 10px;"
              ></v-autocomplete>
            <v-text-field
                hint="rrrr-mm-dd"
                label="Data Wydarzenia"
                v-model="editedWydarzenie.dataWydarzenia"
                variant="outlined"
                :rules="[required]"
                style = "margin-left: 10px;"
              ></v-text-field>
            <v-textarea label="Uwagi" v-model="editedWydarzenie.uwagi" variant="outlined" style="width: 100%; margin-top: 10px"></v-textarea>
            <v-btn variant="tonal" color="secondary" text="Dodaj lochy" @click="selectLochy"></v-btn>
            <v-col>
              <h4 @click="wybraneLochyEmpty" style="margin-top: 1%; margin-left:10px;">Wybrane lochy:{{ editedWydarzenie.numeryLoch }}</h4>
            </v-col>      
          </v-row>
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
  lochy.value = lochyWolne.value.concat(lochyPokryte.value, lochyKarmiace.value, lochyWolne.value, lochyProsne.value);
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
    if(!editedWydarzenie.value.numeryLoch.includes(number)){
        editedWydarzenie.value.numeryLoch.push(number);
        editedWydarzenie.value.lochyId.push(lochy.value.find(locha => locha.numerLochy === number).id);
    }
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